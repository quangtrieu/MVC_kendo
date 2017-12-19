CREATE TABLE [dbo].[das_BudgetItem]
(
	[BudgetItemId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[BudgetTabId] INT NOT NULL,
	[CategorySettingId] INT NOT NULL,
	[BudgetItemRow] VARCHAR(MAX),
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
