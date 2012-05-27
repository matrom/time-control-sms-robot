USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[ProcessMessage]    Script Date: 05/28/2012 15:22:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[ProcessMessage]
	@id uniqueidentifier,
	@from nvarchar(max),
	@to nvarchar(max),
	@text nvarchar(max),
	@isprocessed bit,
	@ts datetime
as 
begin
-- Find ID of sender and receiver
declare @senderId int;
set @senderId = 2;
declare @receiverId int;
set @receiverId = 1;


merge [Messages] as target
using (select isnull(@id, NEWID ()) as id, numbersFrom.UserId as [from], numbersTo.UserId as [to], @text as [text], @isprocessed as isProcessed, isnull(@ts, getdate()) as ts from TimeControlServer..PhoneNUmbers numbersFrom
inner join TimeControlServer..PhoneNUmbers numbersTo
on numbersFrom.PhoneNumber = isnull(@from, '')
and numbersTo.PhoneNumber = isnull(@to, '')) as source
on target.id = source.id
WHEN MATCHED THEN 
UPDATE SET [from] = source.[from],
[to] = source.[to],
[text] = source.[text],
isProcessed = source.[isProcessed],
ts = source.ts
WHEN NOT MATCHED THEN	
INSERT (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
VALUES (source.id, source.[from], source.[to], source.[text], source.isProcessed, isnull(source.ts, GETDATE()), (select Id from LogicalFolders where Name = 'Inbox'));


/*Insert into [Messages] (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
values (@id, @senderId, @receiverId, @text, 1, isnull(@ts, GETDATE()), (select Id from LogicalFolders where Name = 'Inbox'));*/

set @text = 'ECHO: ' + @text;

Insert into [Messages] (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
values (@id, @senderId, @receiverId, @text, 0, isnull(@ts, GETDATE()), (select Id from LogicalFolders where Name = 'Outbox'));

end


GO


