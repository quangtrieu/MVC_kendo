-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-15
-- Description:	Get category setting by parent category name
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_GetByParentName]
	@UserId INT,
	@BudgetId INT,
	@CategoryName VARCHAR(255)
AS
BEGIN
	BEGIN TRY
		-- get parent category id by name
		DECLARE @ParentCategorySettingId INT = 0;
		SELECT @ParentCategorySettingId = CategorySettingId FROM set_CategorySetting WHERE BudgetId = @BudgetId AND ParentCategoryId = 0 AND DeletedFlg = 0 AND CategoryName = @CategoryName;

		SELECT * FROM set_CategorySetting
		WHERE BudgetId = @BudgetId AND DeletedFlg = 0 AND (ParentCategoryId = @ParentCategorySettingId OR CategorySettingId = @ParentCategorySettingId);

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_GetByParentName: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'GET', @SessionID, @AddlInfo
	END CATCH
END
