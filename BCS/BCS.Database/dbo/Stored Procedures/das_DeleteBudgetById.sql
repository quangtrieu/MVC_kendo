-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-30
-- Description:	Delete budget by id
-- =============================================
CREATE PROCEDURE [dbo].[das_DeleteBudgetById]
	@BudgetId int,
	@UserId int
AS
BEGIN
	BEGIN TRY
		DELETE das_Budgets WHERE BudgetId = @BudgetId AND CreatedUserId = @UserId;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_DeleteBudgetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_Budgets', 'DEL', @SessionID, @AddlInfo
	END CATCH
END