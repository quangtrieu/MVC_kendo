-- =============================================
-- Author:		TRIEULQ
-- Create date: 2016-09-01
-- Description:	Clone budget by id
-- =============================================
CREATE PROCEDURE [dbo].[das_Budget_CloneNewBudgetById]
	@UserId INT,
	@BudgetId INT,
	@BudgetName VARCHAR(255),
	@NewBugetId int OUTPUT

AS
BEGIN
	BEGIN TRY
	-- Temp table
		DECLARE @tmpCategorySetting TABLE(
			Current_CategorySettingId INT,
			New_CategorySettingId INT
		);
		-- 1. Call insert new budget by id
		INSERT INTO das_Budgets(BudgetName,
			RestCode, StartDate, EndDate, BudgetLengthType, BudgetLengthStart, FiscalYearStartOn, BudgetLength, ActualNumbersFlg, VarianceFlg, InputMethodId, Sales, SalesPercent, COGS, COGSPercent,
			GrossProfit, GrossProfitPercent, PayrollExpenses, PayrollExpensesPercent, OperatingProfit, OperatingProfitPercent, PrimeCost, PrimeCostPercent, OperatingExpenses, OperatingExpensesPercent, 
			NetProfitLoss, NetProfitLossPercent, BreakEvenPoint, BreakEvenPointPercent, DeletedFlg, 
			CreatedUserId, CreatedDate, UpdatedUserId, UpdatedDate)
		SELECT 
			@BudgetName,
			RestCode, StartDate, EndDate, BudgetLengthType, BudgetLengthStart, FiscalYearStartOn, BudgetLength, ActualNumbersFlg, VarianceFlg, InputMethodId, Sales, SalesPercent, COGS, COGSPercent,
			GrossProfit, GrossProfitPercent, PayrollExpenses, PayrollExpensesPercent, OperatingProfit, OperatingProfitPercent, PrimeCost, PrimeCostPercent, OperatingExpenses, OperatingExpensesPercent, 
			NetProfitLoss, NetProfitLossPercent, BreakEvenPoint, BreakEvenPointPercent, DeletedFlg, 
			@UserId, GETDATE(), @UserId, GETDATE()
		FROM das_Budgets
		WHERE BudgetId = @BudgetId AND DeletedFlg = 0;

		-- 2. Get new budget id
		--DECLARE @NewBugetId INT = 0;
		SET @NewBugetId = @@IDENTITY;

		-- 3. Loop parent category setting id
		BEGIN
			DECLARE @ParentCategoryId INT;
			-------------------------------------------------------
			DECLARE @Cursor CURSOR  
			SET @Cursor = CURSOR FAST_FORWARD
			FOR 
		 		SELECT CategorySettingId FROM set_CategorySetting where BudgetId =	@BudgetId AND ParentCategoryId = 0 AND DeletedFlg = 0; 

			OPEN @Cursor
			FETCH NEXT FROM @Cursor INTO @ParentCategoryId
			WHILE @@FETCH_STATUS = 0
			BEGIN
				-- 3.1. Copy this category setting ParentCategoryId to new budget
				INSERT INTO set_CategorySetting (BudgetId, ParentCategoryId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
				SELECT
					@NewBugetId, ParentCategoryId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, DeletedFlg, GETDATE(), @UserId, GETDATE(), @UserId
				FROM set_CategorySetting
				WHERE CategorySettingId = @ParentCategoryId AND DeletedFlg = 0;

				-- 3.2. Get new ParentCategoryId
				DECLARE @NewParentCategoryId INT = @@IDENTITY;

				-- 3.3. Loop all children category by current parent category id
				BEGIN
					DECLARE @ChildrenCategorySettingId INT;
					---------------------------------------
					DECLARE @CursorChildrenCategory CURSOR  
					SET @CursorChildrenCategory = CURSOR FAST_FORWARD
					FOR 
		    			SELECT CategorySettingId FROM set_CategorySetting where BudgetId =	@BudgetId AND ParentCategoryId = @ParentCategoryId AND DeletedFlg = 0;

					OPEN @CursorChildrenCategory
					FETCH NEXT FROM @CursorChildrenCategory INTO @ChildrenCategorySettingId
					WHILE @@FETCH_STATUS = 0
					BEGIN
							-- 3.3.2. Copy insert children category setting
							INSERT INTO set_CategorySetting (BudgetId, ParentCategoryId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
							SELECT
								@NewBugetId, @NewParentCategoryId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, DeletedFlg, GETDATE(), @UserId, GETDATE(), @UserId
							FROM set_CategorySetting
							WHERE CategorySettingId = @ChildrenCategorySettingId AND DeletedFlg = 0;

							-- 3.3.3 Get new CategorySettingId children
							DECLARE @NewChildrenCategorySettingId INT = @@IDENTITY;

							-- 3.3.4 Insert CategorySettingId old and CategorySettingId new in temp table
							INSERT INTO @tmpCategorySetting(Current_CategorySettingId, New_CategorySettingId) VALUES (@ChildrenCategorySettingId, @NewChildrenCategorySettingId);

						FETCH NEXT FROM @CursorChildrenCategory INTO @ChildrenCategorySettingId
					END
					CLOSE @CursorChildrenCategory
					DEALLOCATE @CursorChildrenCategory
				END
				FETCH NEXT FROM @Cursor INTO @ParentCategoryId
			END
			CLOSE @Cursor
			DEALLOCATE @Cursor
		END
		-- 4. Loop all tab by budget
		DECLARE @BudgetTabId INT;
		-------------------------------------------------------
		DECLARE @CursorBudgetTab CURSOR  
		SET @CursorBudgetTab = CURSOR FAST_FORWARD
		FOR 
		 	SELECT BudgetTabId FROM das_BudgetTab WHERE BudgetId = @BudgetId AND DeletedFlg = 0;

		OPEN @CursorBudgetTab
		FETCH NEXT FROM @CursorBudgetTab INTO @BudgetTabId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- 4.1. Copy this budget tab to new budget by budgettab id
			INSERT INTO das_BudgetTab (BudgetId, TabIndex, TabName, HeaderColumn, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId, AnnualSales, TargetColumn)
			SELECT
				@NewBugetId, TabIndex, TabName, HeaderColumn, DeletedFlg, GETDATE(), @UserId, GETDATE(), @UserId, AnnualSales, TargetColumn
			FROM das_BudgetTab
			WHERE BudgetTabId = @BudgetTabId AND DeletedFlg = 0;

			-- 4.2. Get new budget tab id
			DECLARE @NewBudgetTabId INT = @@IDENTITY;

			-- 4.3 Loop budget item by cureent budget tab id
			INSERT INTO das_BudgetItem (BudgetTabId, CategorySettingId, BudgetItemRow, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT 
				@NewBudgetTabId, B.New_CategorySettingId, A.BudgetItemRow, A.DeletedFlg, GETDATE(), @UserId, GETDATE(), @UserId
			FROM das_BudgetItem A
			INNER JOIN @tmpCategorySetting B ON A.CategorySettingId = B.Current_CategorySettingId
			WHERE A.BudgetTabId = @BudgetTabId AND A.DeletedFlg = 0;

			FETCH NEXT FROM @CursorBudgetTab INTO @BudgetTabId
		END
		CLOSE @CursorBudgetTab
		DEALLOCATE @CursorBudgetTab
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_CloneBudgetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_Budgets', 'CLONE', @SessionID, @AddlInfo
	END CATCH
END