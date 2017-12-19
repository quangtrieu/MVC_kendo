-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-30
-- Description:	Get budget by id
-- =============================================
CREATE PROCEDURE [dbo].[das_GetBudgetById]
	@BudgetId int
AS
BEGIN
	BEGIN TRY
		SELECT * FROM das_Budgets WHERE BudgetId = @BudgetId AND DeletedFlg = 0;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_GetBudgetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@BudgetId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_Budgets', 'GET', @SessionID, @AddlInfo
	END CATCH
END