-- =============================================
-- Author:		BauNM
-- Create date: 2016-05-04
-- Description:	Get budget tab by budget tab id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetTab_GetById]
	@BudgetTabId int
AS
BEGIN
	BEGIN TRY

		-- get budget item by budget item id
		SELECT * FROM das_BudgetTab WHERE BudgetTabId = @BudgetTabId AND DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetTab_GetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@BudgetTabId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetTab', 'GET', @SessionID, @AddlInfo
	END CATCH
END
