USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[GetUsersRunningOutOfTime]    Script Date: 05/28/2012 15:25:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetUsersRunningOutOfTime]
as 
begin
select us.UserId, FirstName, Surname, isnull(PhoneNumber, 'Not available') as PhoneNumber, case when deadline < GETDATE() then 'OutOfTime' 
when DATEDIFF(MINUTE, GETDATE(), deadline) <=5 and [5minutesWarning]=0 then '5minutes' when DATEDIFF(MINUTE, GETDATE(), deadline) <=60 and hourWarning=0 then 'hour' end as warningType, DATEDIFF(minute, getdate(), deadline) as minutesLeft
into #UsersRunningOutOfTime
from Users us
left join PhoneNUmbers pn
on us.UserId = pn.UserId
where ((DATEDIFF(MINUTE, GETDATE(), deadline) <=60 and hourWarning=0)
or (DATEDIFF(MINUTE, GETDATE(), deadline) <=5 and [5minutesWarning]=0)
or deadline < GETDATE())
and OutOfTime = 0
and isnull(us.serviceNotes, '') != 'Bank';

update Users
set hourWarning = case when warningType='hour' or warningType='5minutes' then 1 else hourWarning end,
[5minutesWarning] = case when warningType='5minutes' then 1 else [5minutesWarning] end,
OutOfTime = case when warningType='OutOfTime' then 1 else OutOfTime end
from Users us
inner join #UsersRunningOutOfTime urot
on us.UserId = urot.UserId;

--Select FirstName, Surname, PhoneNumber, warningType, minutesLeft from #UsersRunningOutOfTime;
Insert into [Messages] ([from], [to],[text],ts, LogicalFolderId)
select pn.PhoneNumber, urot.PhoneNumber, case 
when minutesLeft <=0 then 'Ваше время вышло'
when (minutesLeft <10 or minutesLeft >20) and minutesLeft%10 = 1 then 'Внимание: на Вашем счету осталась ' + cast(minutesLeft as varchar) + ' минута!'
when (minutesLeft <10 or minutesLeft >20) and minutesLeft%10 in (2, 3, 4) then 'Внимание: на Вашем счету остались ' + cast(minutesLeft as varchar) + ' минуты!'
else 'Внимание: на Вашем счету осталось ' + cast(minutesLeft as varchar) + ' минут!' end, GETDATE(), (select Id from LogicalFolders where Name = 'Outbox') from #UsersRunningOutOfTime urot, Users us, PhoneNumbers pn where isnull(us.serviceNotes, '') = 'Bank' and pn.UserId = us.UserId
end
GO


