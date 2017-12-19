CREATE TABLE [dbo].[common_Data]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[DataType] VARCHAR(255) NOT NULL,
	[DataCode] VARCHAR(50) NOT NULL,
	[DataValue] INT NOT NULL,
	[DataText] VARCHAR(1000) NULL,
	[Description] VARCHAR(1000) NULL,
	[SortOrder] INT NULL,
	[DeletedFlg] BIT NULL
)
