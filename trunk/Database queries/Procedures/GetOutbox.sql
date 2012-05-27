USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[GetOutbox]    Script Date: 05/28/2012 15:25:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[GetOutbox]
as 
begin

exec GetUsersRunningOutOfTime;

select id,[from], [to],[text],isProcessed,ts from [Messages]
where LogicalFolderId = (select Id from LogicalFolders where Name = 'Outbox')
order by ts desc;

update [Messages]
set LogicalFolderId = (select Id from LogicalFolders where Name = 'Send')
where LogicalFolderId = (select Id from LogicalFolders where Name = 'Outbox');

end


GO


