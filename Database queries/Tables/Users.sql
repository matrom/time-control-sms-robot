USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 05/28/2012 16:51:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[Surname] [nvarchar](100) NULL,
	[login] [nvarchar](100) NULL,
	[passwd] [nvarchar](100) NULL,
	[deadline] [datetime2](7) NULL,
	[hourWarning] [bit] NOT NULL,
	[5minutesWarning] [bit] NOT NULL,
	[OutOfTime] [bit] NOT NULL,
	[ServiceNotes] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [hourWarning]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [5minutesWarning]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [OutOfTime]
GO


