/*
Scripts to create a database schema for MSSQL, to store the state in the database
*/

/***================================= BatchJob ========================================***/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [batch].[Job](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[CreateDate] [datetime] NOT NULL,
	[LastRun] [datetime] NOT NULL,
 CONSTRAINT [PK_BatchJob_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [batch].[Job] ADD  CONSTRAINT [DF_BatchJob_CreateDate]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO

/***================================= BatchStep ========================================***/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [batch].[Step](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StepName] [nvarchar](100) NOT NULL,
	[StepIndex] [bigint] NOT NULL,
	[NumberOfItemsProcessed] [int] NOT NULL,
	[ExceptionMsg] [nvarchar](500) NULL,
	[ExceptionDetails] [nvarchar](1500) NULL,
	[Skipped] [bit] NOT NULL CONSTRAINT [DF_BatchStep_Done]  DEFAULT ((0)),
	[RunDate] [datetime] NOT NULL CONSTRAINT [DF_BatchStep_RunDate]  DEFAULT (sysutcdatetime()),
	[BatchJobID] [bigint] NOT NULL,
 CONSTRAINT [PK_BatchStep] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [batch].[Step]  WITH CHECK ADD  CONSTRAINT [FK_BatchStep_BatchJob] FOREIGN KEY([BatchJobID])
REFERENCES [batch].[Job] ([ID])
GO

ALTER TABLE [batch].[Step] CHECK CONSTRAINT [FK_BatchStep_BatchJob]
GO


CREATE NONCLUSTERED INDEX [IX_StepName] ON [batch].[Step]
(
	[StepName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO