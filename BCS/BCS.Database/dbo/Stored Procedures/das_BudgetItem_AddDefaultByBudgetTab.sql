-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-22
-- Description:	Add default value to budget item by budget tab
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetItem_AddDefaultByBudgetTab]
	@UserId INT,
	@BudgetId INT,
	@BudgetTabId INT,
	@BudgetItemRow VARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

		DECLARE @RowCount INT = 0;
		SELECT @RowCount = COUNT(*) FROM das_BudgetItem WHERE BudgetTabId = @BudgetTabId;

		IF @RowCount = 0
		BEGIN
			INSERT INTO das_BudgetItem(BudgetTabId, CategorySettingId, BudgetItemRow, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT @BudgetTabId, A.CategorySettingId, @BudgetItemRow, 0, GETDATE(), @UserId, GETDATE(), @UserId FROM set_CategorySetting A
			INNER JOIN set_CategorySetting B ON B.BudgetId = @BudgetId AND B.ParentCategoryId = 0 AND B.DeletedFlg = 0 AND B.CategorySettingId = A.ParentCategoryId
			WHERE A.DeletedFlg = 0
			ORDER BY A.ParentCategoryId, A.SortOrder;
		END

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetItem_AddDefaultByBudgetTab: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetItem', 'ADD', @SessionID, @AddlInfo
	END CATCH
END