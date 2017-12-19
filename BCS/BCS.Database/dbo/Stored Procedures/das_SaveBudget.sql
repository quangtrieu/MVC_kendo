-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-31
-- Description:	Add or Edit budget
-- =============================================
CREATE PROCEDURE [dbo].[das_SaveBudget]
	@UserId int,
	@XML nvarchar(max),
	@BudgetId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @BudgetId = 0
		BEGIN
			INSERT INTO das_Budgets(BudgetName, RestCode,
				StartDate, EndDate, BudgetLengthType, BudgetLengthStart, FiscalYearStartOn, BudgetLength, ActualNumbersFlg, VarianceFlg, InputMethodId,
				Sales, SalesPercent, COGS, COGSPercent, GrossProfit, GrossProfitPercent, PayrollExpenses, PayrollExpensesPercent, OperatingProfit, OperatingProfitPercent, PrimeCost, PrimeCostPercent,
				OperatingExpenses, OperatingExpensesPercent, NetProfitLoss, NetProfitLossPercent, BreakEvenPoint, BreakEvenPointPercent,
				DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT BudgetName, RestCode,
				StartDate, EndDate, BudgetLengthType, BudgetLengthStart, FiscalYearStartOn, BudgetLength, ActualNumbersFlg, VarianceFlg, InputMethodId,
				Sales, SalesPercent, COGS, COGSPercent, GrossProfit, GrossProfitPercent, PayrollExpenses, PayrollExpensesPercent, OperatingProfit, OperatingProfitPercent, PrimeCost, PrimeCostPercent,
				OperatingExpenses, OperatingExpensesPercent, NetProfitLoss, NetProfitLossPercent, BreakEvenPoint, BreakEvenPointPercent,
				DeletedFlg, GETDATE(), CreatedUserId, GETDATE(), UpdatedUserId
			FROM OPENXML(@XMLID, '/Budget', 2) 
			WITH(
				BudgetId INT,
				BudgetName VARCHAR(255),
				RestCode VARCHAR(255),
				StartDate DATE,
				EndDate DATE,
				BudgetLengthType INT,
				BudgetLengthStart DATE,
				FiscalYearStartOn DATE,
				BudgetLength INT,
				ActualNumbersFlg BIT,
				VarianceFlg BIT,
				InputMethodId INT,
				Sales DECIMAL(18, 2),
				SalesPercent DECIMAL(5,2),
				COGS DECIMAL(18, 2),
				COGSPercent DECIMAL(5, 2),
				GrossProfit DECIMAL(18, 2),
				GrossProfitPercent DECIMAL(5, 2),
				PayrollExpenses DECIMAL(18, 2),
				PayrollExpensesPercent DECIMAL(5, 2),
				OperatingProfit DECIMAL(18, 2),
				OperatingProfitPercent DECIMAL(5, 2),
				PrimeCost DECIMAL(18, 2),
				PrimeCostPercent DECIMAL(5, 2),
				OperatingExpenses DECIMAL(18, 2),
				OperatingExpensesPercent DECIMAL(5, 2),
				NetProfitLoss DECIMAL(18, 2),
				NetProfitLossPercent DECIMAL(5, 2),
				BreakEvenPoint DECIMAL(18, 2),
				BreakEvenPointPercent DECIMAL(5, 2),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X;

			SET @BudgetId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE das_Budgets
			SET
				BudgetName = X.BudgetName,
				RestCode = X.RestCode,
				StartDate = X.StartDate,
				EndDate = X.EndDate,
				BudgetLengthType = X.BudgetLengthType,
				BudgetLengthStart = X.BudgetLengthStart,
				FiscalYearStartOn = X.FiscalYearStartOn,
				BudgetLength = X.BudgetLength,
				ActualNumbersFlg = X.ActualNumbersFlg,
				VarianceFlg = X.VarianceFlg,
				InputMethodId = X.InputMethodId,
				Sales = X.Sales,
				SalesPercent = X.SalesPercent,
				COGS = X.COGS,
				COGSPercent = X.COGSPercent,
				GrossProfit = X.GrossProfit,
				GrossProfitPercent = X.GrossProfitPercent,
				PayrollExpenses = X.PayrollExpenses,
				PayrollExpensesPercent = X.PayrollExpensesPercent,
				OperatingProfit = X.OperatingProfit,
				OperatingProfitPercent = X.OperatingProfitPercent,
				PrimeCost = X.PrimeCost,
				PrimeCostPercent = X.PrimeCostPercent,
				OperatingExpenses = X.OperatingExpenses,
				OperatingExpensesPercent = X.OperatingExpensesPercent,
				NetProfitLoss = X.NetProfitLoss,
				NetProfitLossPercent = X.NetProfitLossPercent,
				BreakEvenPoint = X.BreakEvenPoint,
				BreakEvenPointPercent = X.BreakEvenPointPercent,
				DeletedFlg = X.DeletedFlg,
				CreatedDate = X.CreatedDate,
				CreatedUserId = X.CreatedUserId,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/Budget', 2) 
			WITH(
				BudgetId INT,
				BudgetName VARCHAR(255),
				RestCode VARCHAR(255),
				StartDate DATE,
				EndDate DATE,
				BudgetLengthType INT,
				BudgetLengthStart DATE,
				FiscalYearStartOn DATE,
				BudgetLength INT,
				ActualNumbersFlg BIT,
				VarianceFlg BIT,
				InputMethodId INT,
				Sales DECIMAL(18, 2),
				SalesPercent DECIMAL(5,2),
				COGS DECIMAL(18, 2),
				COGSPercent DECIMAL(5, 2),
				GrossProfit DECIMAL(18, 2),
				GrossProfitPercent DECIMAL(5, 2),
				PayrollExpenses DECIMAL(18, 2),
				PayrollExpensesPercent DECIMAL(5, 2),
				OperatingProfit DECIMAL(18, 2),
				OperatingProfitPercent DECIMAL(5, 2),
				PrimeCost DECIMAL(18, 2),
				PrimeCostPercent DECIMAL(5, 2),
				OperatingExpenses DECIMAL(18, 2),
				OperatingExpensesPercent DECIMAL(5, 2),
				NetProfitLoss DECIMAL(18, 2),
				NetProfitLossPercent DECIMAL(5, 2),
				BreakEvenPoint DECIMAL(18, 2),
				BreakEvenPointPercent DECIMAL(5, 2),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X
			INNER JOIN das_Budgets Y ON Y.BudgetId = X.BudgetId;
		END

		EXEC sp_xml_removedocument @XMLID
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_SaveBudget: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_Budget', 'ADD', @SessionID, @AddlInfo
	END CATCH
END