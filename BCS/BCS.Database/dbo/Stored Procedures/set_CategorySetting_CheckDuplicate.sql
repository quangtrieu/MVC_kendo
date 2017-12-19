-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-12
-- Description:	check duplicate category setting
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_CheckDuplicate]
	@UserId int,
	@BudgetId int,
	@CategorySettingId int,
	@ParentCategoryId int,
	@CategoryName varchar(255),
	@Result bit out
AS
BEGIN
	BEGIN TRY
		IF @CategorySettingId = 0
		BEGIN
			SELECT
				@Result = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END)
			FROM set_CategorySetting
			WHERE BudgetId = @BudgetId AND ParentCategoryId = @ParentCategoryId AND CategoryName = @CategoryName AND DeletedFlg = 0;
		END
		ELSE
		BEGIN
			SELECT
				@Result = (CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END)
			FROM set_CategorySetting
			WHERE CategorySettingId <> @CategorySettingId AND BudgetId = @BudgetId AND ParentCategoryId = @ParentCategoryId AND CategoryName = @CategoryName AND DeletedFlg = 0;
		END
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_CheckDuplicate: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'GET', @SessionID, @AddlInfo
	END CATCH
END