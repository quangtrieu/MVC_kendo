-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-11
-- Description:	Get category setting by parent category id
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_GetByParentCategoryId]
	@UserId INT,
	@BudgetId INT,
	@ParentCategoryId INT
AS
BEGIN
	BEGIN TRY

		SELECT * FROM set_CategorySetting
		WHERE BudgetId = @BudgetId AND ParentCategoryId = @ParentCategoryId AND DeletedFlg = 0
		ORDER BY SortOrder ASC;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_GetByParentCategoryId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'GET', @SessionID, @AddlInfo
	END CATCH
END
