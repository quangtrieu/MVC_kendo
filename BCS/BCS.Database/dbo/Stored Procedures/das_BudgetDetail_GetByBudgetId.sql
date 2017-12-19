-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-20
-- Description:	Get budget detail by budget id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetDetail_GetByBudgetId]
	@BudgetId INT,
	@ParentCategoryName VARCHAR(255),
	@UserId INT
AS
BEGIN
	BEGIN TRY
		-- get tab number by budget id
		DECLARE @TabNumber VARCHAR(1000) = '';
		SELECT @TabNumber = dbo.ufn_GetTabNumberByBudgetId(@BudgetId, @UserId);

		-- get parent category id by name
		DECLARE @ParentCategoryId INT = 0;
		SELECT @ParentCategoryId = CategorySettingId FROM set_CategorySetting WHERE BudgetId = @BudgetId AND ParentCategoryId = 0 AND CategoryName = @ParentCategoryName AND DeletedFlg = 0;

		-- get all data category by tab name
		DECLARE @tempTable TABLE(CategorySettingId INT, CategoryName VARCHAR(255), TabIndex INT, TabName VARCHAR(255));
		INSERT INTO @tempTable
		SELECT A.CategorySettingId, A.CategoryName, B.Id, B.Data FROM set_CategorySetting A, dbo.ufn_Split(@TabNumber, ';') B
		WHERE A.ParentCategoryId = @ParentCategoryId AND A.DeletedFlg = 0 AND LEN(B.Data) > 0;

		-- get budget detail by budget id
		SELECT A.*, B.BudgetDetailId, B.BudgetId, B.ColumnName, B.ColumnValue FROM @tempTable A
		LEFT JOIN das_BudgetDetails B ON B.CategorySettingId = A.CategorySettingId AND B.BudgetId = @BudgetId AND B.TabIndex = A.TabIndex AND B.DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetDetail_GetByBudgetId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetDetails', 'GET', @SessionID, @AddlInfo
	END CATCH
END