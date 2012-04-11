USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[GetUsersRunningOutOfTime]    Script Date: 04/11/2012 17:54:48 ******/
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
or (deadline < GETDATE() and OutOfTime=0))
and OutOfTime = 0;

update Users
set hourWarning = case when warningType='hour' or warningType='5minutes' then 1 else hourWarning end,
[5minutesWarning] = case when warningType='5minutes' then 1 else [5minutesWarning] end,
OutOfTime = case when warningType='OutOfTime' then 1 else OutOfTime end
from Users us
inner join #UsersRunningOutOfTime urot
on us.UserId = urot.UserId;

Select FirstName, Surname, PhoneNumber, warningType, minutesLeft from #UsersRunningOutOfTime;
end



GO

