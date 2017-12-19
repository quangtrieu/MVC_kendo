-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-22
-- Description:	Get budget item by budget tab
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetItem_GetByBudgetTabId]
	@UserId INT,
	@BudgetId INT,
	@BudgetTabId INT,
	@BudgetItemRow VARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		-- Get category is not exists data in budget item id
		INSERT INTO das_BudgetItem(BudgetTabId, CategorySettingId, BudgetItemRow, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT @BudgetTabId, A.CategorySettingId, @BudgetItemRow, 0, GETDATE(), @UserId, GETDATE(), @UserId FROM set_CategorySetting A
			INNER JOIN set_CategorySetting B ON B.BudgetId = @BudgetId AND B.ParentCategoryId = 0 AND B.DeletedFlg = 0 AND B.CategorySettingId = A.ParentCategoryId
			LEFT JOIN das_BudgetItem C ON C.BudgetTabId = @BudgetTabId AND C.DeletedFlg = 0 AND C.CategorySettingId = A.CategorySettingId
			WHERE A.DeletedFlg = 0 AND C.BudgetItemId IS NULL;

		SELECT A.CategoryName, A.SalesCategoryRefId, B.CategorySettingId AS ParentCategoryId, B.CategoryName AS ParentCategoryName, A.IsPercentage, A.IsPrimeCost, A.IsTaxCost, A.SortOrder, C.* FROM set_CategorySetting A
		INNER JOIN set_CategorySetting B ON B.BudgetId = @BudgetId AND B.ParentCategoryId = 0 AND B.DeletedFlg = 0 AND B.CategorySettingId = A.ParentCategoryId
		LEFT JOIN das_BudgetItem C ON C.BudgetTabId = @BudgetTabId AND C.DeletedFlg = 0 AND C.CategorySettingId = A.CategorySettingId
		WHERE A.DeletedFlg = 0
		ORDER BY A.ParentCategoryId, A.SortOrder;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetItem_GetByBudgetTabId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetTab', 'GET', @SessionID, @AddlInfo
	END CATCH
END
