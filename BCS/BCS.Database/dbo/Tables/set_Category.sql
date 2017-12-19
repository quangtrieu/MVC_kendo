CREATE TABLE [dbo].[set_Category]
(
	[CategoryId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [CategoryName] VARCHAR(255) NULL, 
    [SortOrder] INT NULL, 
    [IsSelected] BIT NULL, 
    [IsPrimeCost] BIT NULL, 
    [IsTaxCost] BIT NULL, 
    [IsPercentage] BIT NULL, 
    [ParentCategoryId] INT NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
