USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[PhoneNUmbers]    Script Date: 04/06/2012 17:06:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PhoneNumbers](
	[PhoneId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[PhoneCompany] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PhoneId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PhoneNUmbers]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO

