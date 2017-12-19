CREATE TABLE [dbo].[set_CategorySetting]
(
	[CategorySettingId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [BudgetId] INT NULL,
	[ParentCategoryId] INT NULL,
	[SalesCategoryRefId] INT NULL, 
    [CategoryName] VARCHAR(255) NULL,
	[SortOrder] INT NULL, 
    [IsSelected] BIT NULL, 
    [IsPrimeCost] BIT NULL, 
    [IsTaxCost] BIT NULL,
    [IsPercentage] BIT NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
