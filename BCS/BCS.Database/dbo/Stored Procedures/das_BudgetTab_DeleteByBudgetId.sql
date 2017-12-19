-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-26
-- Description:	Delete budget tab and item by budget id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetTab_DeleteByBudgetId]
	@UserId INT,
	@BudgetId INT,
	@Result BIT OUTPUT
AS
BEGIN
	BEGIN TRY

		-- set flag deleted to budget item
		UPDATE das_BudgetItem SET DeletedFlg = 1 WHERE BudgetTabId IN (SELECT BudgetTabId FROM das_BudgetTab WHERE BudgetId = @BudgetId)

		-- set flag deleted to budget tab
		UPDATE das_BudgetTab SET DeletedFlg = 1 WHERE BudgetId = @BudgetId;

		-- set flag and return
		SET @Result = CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetTab_DeleteByBudgetId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetTab', 'UPD', @SessionID, @AddlInfo
	END CATCH
END
