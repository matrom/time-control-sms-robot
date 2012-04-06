USE [TimeControlServer]
GO

/****** Object:  StoredProcedure [dbo].[LogMessage]    Script Date: 04/06/2012 17:07:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[LogMessage]
	@id uniqueidentifier,
	@from nvarchar(max),
	@to nvarchar(max),
	@text nvarchar(max),
	@isprocessed bit,
	@ts datetime
as 
begin
merge messageLog as target
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
INSERT (id,[from], [to],[text],isProcessed,ts)
VALUES (source.id, source.[from], source.[to], source.[text], source.isProcessed, source.ts);
end
GO

