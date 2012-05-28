USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[ProcessMessage]    Script Date: 05/30/2012 01:37:12 ******/
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
	@ts datetime,
	@folder nvarchar(20) = 'Inbox'
as 
begin

merge [Messages] as target
using (select isnull(@id, NEWID ()) as id, @from as [from], @to as [to], @text as [text], @isprocessed as isProcessed, isnull(@ts, getdate()) as ts, @folder as [folder]) as source
on target.id = source.id
WHEN MATCHED THEN 
UPDATE SET [from] = source.[from],
[to] = source.[to],
[text] = source.[text],
isProcessed = source.[isProcessed],
ts = source.ts,
LogicalFolderId = (select Id from LogicalFolders where Name = source.[folder])
WHEN NOT MATCHED THEN	
INSERT (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
VALUES (source.id, source.[from], source.[to], source.[text], source.isProcessed, isnull(source.ts, GETDATE()), (select Id from LogicalFolders where Name = source.[folder]));


/*Insert into [Messages] (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
values (@id, @senderId, @receiverId, @text, 1, isnull(@ts, GETDATE()), (select Id from LogicalFolders where Name = 'Inbox'));*/
if @folder = 'Inbox'
begin
set @text = 'ECHO: ' + @text;

Insert into [Messages] (id,[from], [to],[text],isProcessed,ts, LogicalFolderId)
values (NEWID(), @to, @from, @text, 0, isnull(@ts, GETDATE()), (select Id from LogicalFolders where Name = 'Outbox'));
end
end




GO

