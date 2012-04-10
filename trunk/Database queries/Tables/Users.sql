USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 04/10/2012 15:38:25 ******/
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
	[deadline] [datetime] NULL,
	[hourWarning] [bit] NOT NULL,
	[5minutesWarning] [bit] NOT NULL,
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

