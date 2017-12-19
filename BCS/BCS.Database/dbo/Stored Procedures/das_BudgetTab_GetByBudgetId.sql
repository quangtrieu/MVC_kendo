-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-22
-- Description:	Get budget tab by budget id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetTab_GetByBudgetId]
	@BudgetId INT,
	@UserId INT
AS
BEGIN
	BEGIN TRY

		-- get budget tab by budget id
		SELECT * FROM das_BudgetTab WHERE BudgetId = @BudgetId AND DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetTab_GetByBudgetId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetTab', 'GET', @SessionID, @AddlInfo
	END CATCH
END