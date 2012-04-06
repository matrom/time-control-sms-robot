USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[messageLog]    Script Date: 04/06/2012 17:06:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[messageLog](
	[id] [uniqueidentifier] NOT NULL,
	[from] [int] NOT NULL,
	[to] [int] NOT NULL,
	[text] [nvarchar](max) NULL,
	[isProcessed] [bit] NULL,
	[TS] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[messageLog]  WITH CHECK ADD FOREIGN KEY([from])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[messageLog]  WITH CHECK ADD FOREIGN KEY([to])
REFERENCES [dbo].[Users] ([UserId])
GO

