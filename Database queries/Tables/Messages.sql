USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[Inbox]    Script Date: 05/28/2012 14:53:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- drop table [Messages]

CREATE TABLE [dbo].[Messages](
	[id] [uniqueidentifier] NOT NULL primary key default newid(),
	[from] [nvarchar](max),
	[to] [nvarchar](max),
	[text] [nvarchar](max) NULL,
	[isProcessed] [bit] NULL,
	[TS] [datetime] NULL,
	LogicalFolderId tinyint references LogicalFolders(Id)
) ON [PRIMARY]

GO


