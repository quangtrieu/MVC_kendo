﻿CREATE TABLE [dbo].[das_Budgets]
(
	[BudgetId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[BudgetName] VARCHAR(255) NOT NULL, 
    [RestCode] VARCHAR(255) NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL,
	[BudgetLengthType] INT NULL,
	[BudgetLengthStart] DATE NULL,
	[FiscalYearStartOn] DATE NULL,
	[BudgetLength] INT NULL,
	[ActualNumbersFlg] BIT NULL,
	[VarianceFlg] BIT NULL,
	[InputMethodId] INT NULL,
    [Sales] DECIMAL(18, 2) NULL,
	[SalesPercent] DECIMAL(8,2) NULL,
	[COGS] DECIMAL(18, 2) NULL,
	[COGSPercent] DECIMAL(8, 2) NULL,
	[GrossProfit] DECIMAL(18, 2) NULL,
	[GrossProfitPercent] DECIMAL(8, 2) NULL,
	[PayrollExpenses] DECIMAL(18, 2) NULL,
	[PayrollExpensesPercent] DECIMAL(8, 2) NULL,
	[OperatingProfit] DECIMAL(18, 2) NULL,
	[OperatingProfitPercent] DECIMAL(8, 2) NULL,
	[PrimeCost] DECIMAL(18, 2) NULL,
	[PrimeCostPercent] DECIMAL(8, 2) NULL,
	[OperatingExpenses] DECIMAL(18, 2) NULL,
	[OperatingExpensesPercent] DECIMAL(8, 2) NULL,
	[NetProfitLoss] DECIMAL(18, 2) NULL,
	[NetProfitLossPercent] DECIMAL(8, 2) NULL,
	[BreakEvenPoint] DECIMAL(18, 2) NULL,
	[BreakEvenPointPercent] DECIMAL(8, 2) NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)