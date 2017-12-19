-- =============================================
-- Author:		BauNM
-- Create date: 2016-05-04
-- Description:	Get budget item by budget item id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetItem_GetById]
	@BudgetItemId int
AS
BEGIN
	BEGIN TRY

		-- get budget item by budget item id
		SELECT * FROM das_BudgetItem WHERE BudgetItemId = @BudgetItemId AND DeletedFlg = 0;

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
		SET @AddlInfo					= CAST(@BudgetItemId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetItem', 'GET', @SessionID, @AddlInfo
	END CATCH
END
