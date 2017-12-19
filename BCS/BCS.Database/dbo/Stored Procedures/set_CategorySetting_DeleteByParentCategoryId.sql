-- =============================================
-- Author:		TrieuLQ
-- Create date: 2016-08-05
-- Description:	delete category setting by budget id
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_DeleteByParentCategoryId]
	@UserId int,
	@BudgetId int,
	@ParentCategoryId int,
	@Result bit out
AS
BEGIN
	BEGIN TRY
		SET @Result = 0;

		-- 1. delete all budget item by category setting
		DELETE FROM das_BudgetItem WHERE CategorySettingId IN (SELECT CategorySettingId FROM set_CategorySetting WHERE BudgetId = @BudgetId AND ParentCategoryId = @ParentCategoryId AND CreatedUserId = @UserId)

		-- 2. delete all category setting by parent category id
		DELETE FROM set_CategorySetting  WHERE BudgetId = @BudgetId AND ParentCategoryId = @ParentCategoryId AND CreatedUserId = @UserId;

		-- 3. return result
		IF @@ROWCOUNT > 0
			SET @Result = 1;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_DeleteByParentCategoryId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'UPD', @SessionID, @AddlInfo
	END CATCH
END