CREATE TABLE [dbo].[acc_RestActiveCode]
(
	[RestActiveCodeId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [UserId] INT NOT NULL, 
    [RestCode] VARCHAR(255) NULL,
	[RestName] VARCHAR(255) NULL, 
    [TokenId] VARCHAR(255) NULL, 
    [IsDefault] BIT NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
