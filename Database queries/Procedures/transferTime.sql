create procedure transferTime
	@phoneNumber nvarchar(50),
	@sendelLogin nvarchar(100),
	@senderPasswd nvarchar(100),
	@receiverLogin nvarchar(100),
	@receiverNumber nvarchar(50),
	@minutesAmount bigint
as
begin
-- @minutesAmount всегда не null
-- если указаны @sendelLogin и @senderPasswd, используем их для идентификации отправителя.
-- если не указаны, используем @phoneNumber
-- если указан @receiverLogin, используем его для идентификации получателя.
-- если не указан, используем @receiverNumber

declare @senderId int;
declare @receiverId int;
declare @senderPasswordSpecimen nvarchar(100);
declare @resultMessage nvarchar(100);
declare @performTransaction smallint;
-- 0 - authentication started
-- 1 - sender is identified
-- 2 - receiver is identified, authentication complete
-- -1 - authentication failed (incorrect password or user name)
set @performTransaction = 0;

if @minutesAmount is null
begin
	set @performTransaction = -1;
	set @resultMessage = 'Time sum not specified';
end

if @minutesAmount < 0
begin
	set @performTransaction = -1;
	set @resultMessage = 'Hack attempt detected. Incident reported to the police';
	
	Insert into [Messages] ([from], [to],[text],ts, LogicalFolderId)
	select pnBank.PhoneNumber, pnPolice.PhoneNumber, 
	'Hack attempt detected from phone number ' + @phoneNumber,
	GETDATE(), (select Id from LogicalFolders where Name = 'Outbox') from Users usBank, Users usPolice, PhoneNumbers pnBank, PhoneNumbers pnPolice 
	where isnull(usBank.serviceNotes, '') = 'Bank' and pnBank.UserId = usBank.UserId
	and isnull(usPolice.serviceNotes, '') = 'Police' and pnPolice.UserId = usPolice.UserId
end

if @sendelLogin is not null
begin
	select @senderId = UserId, @senderPasswordSpecimen = passwd from Users
	where login = @sendelLogin;
	if @senderId is not null
	begin
		if ISNULL(@senderPasswordSpecimen, '') != ISNULL(@senderPasswd, '')
		begin
			set @resultMessage = 'Incorrect password';
			set @performTransaction = -1;
		end
		else 
			set @performTransaction = 1;
	end
	else 
	begin
		set @resultMessage = 'Sender: Invalid user ID';
		set @performTransaction = -1;
	end
end
if @phoneNumber is not null and @performTransaction = 0
begin
	select @senderId = UserId from PhoneNumbers
	where PhoneNumber = @phoneNumber;
	if @senderId is not null
		set @performTransaction = 1
	else
	begin
		set @resultMessage = 'Phone number is not registered in database';
		set @performTransaction = -1;
	end
end
if @receiverLogin is not null and @performTransaction = 1
begin
	select @receiverId = UserId from Users
	where login = @receiverLogin;
	if @receiverId is not null
		set @performTransaction = 2
	else
	begin
		set @resultMessage = 'Receiver: Invalid user ID';
		set @performTransaction = -1;
	end
end
if @receiverNumber is not null and @performTransaction = 1
begin
	select @receiverId = UserId from PhoneNumbers
	where PhoneNumber = @receiverNumber;
	if @receiverId is not null
		set @performTransaction = 2
	else
	begin
		set @resultMessage = 'Receiver phone number is not registered in database';
		set @performTransaction = -1;
	end
end

if @performTransaction = 2
begin
	update Users
	set deadline = DATEADD(MINUTE, @minutesAmount*-1, deadline)
	where UserId = @senderId and isnull(ServiceNotes, '') != 'Bank';
	update Users
	set deadline = DATEADD(MINUTE, @minutesAmount, deadline)
	where UserId = @receiverId and isnull(ServiceNotes, '') != 'Bank';
	set @resultMessage = 'Transaction successfull';
end

select @resultMessage as ResultMessage;
end