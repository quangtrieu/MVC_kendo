CREATE TABLE [dbo].[das_BudgetTab]
(
	[BudgetTabId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [BudgetId] INT NULL, 
    [TabIndex] INT NULL, 
    [TabName] VARCHAR(50) NULL,
	[AnnualSales] DECIMAL(15,2),
	[HeaderColumn] VARCHAR(MAX) NULL,
	[TargetColumn] VARCHAR(MAX) NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
