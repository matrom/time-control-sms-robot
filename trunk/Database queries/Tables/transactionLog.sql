USE [TimeControlServer]
GO

/****** Object:  Table [dbo].[transactionLog]    Script Date: 04/06/2012 17:06:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[transactionLog](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[Sender] [int] NOT NULL,
	[Receiver] [int] NOT NULL,
	[amountOfSeconds] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[transactionLog]  WITH CHECK ADD FOREIGN KEY([Receiver])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[transactionLog]  WITH CHECK ADD FOREIGN KEY([Sender])
REFERENCES [dbo].[Users] ([UserId])
GO

