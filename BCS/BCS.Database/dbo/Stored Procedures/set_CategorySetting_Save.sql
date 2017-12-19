-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-11
-- Description:	Add or Edit category setting
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_Save]
	@UserId int,
	@XML nvarchar(max),
	@CategorySettingId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @CategorySettingId = 0
		BEGIN
			INSERT INTO set_CategorySetting(BudgetId, ParentCategoryId, SalesCategoryRefId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT BudgetId, ParentCategoryId, SalesCategoryRefId, CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, 0, GETDATE(), @UserId, GETDATE(), @UserId
			FROM OPENXML(@XMLID, '/CategorySetting', 2) 
			WITH(
				BudgetId INT,
				ParentCategoryId INT,
				SalesCategoryRefId INT,
				CategoryName VARCHAR(255),
				SortOrder INT,
				IsSelected BIT,
				IsPrimeCost BIT,
				IsTaxCost BIT,
				IsPercentage BIT
			) X;

			SET @CategorySettingId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE set_CategorySetting
			SET
				BudgetId = X.BudgetId,
				ParentCategoryId = X.ParentCategoryId,
				SalesCategoryRefId = X.SalesCategoryRefId,
				CategoryName = X.CategoryName,
				SortOrder = X.SortOrder,
				IsSelected = X.IsSelected,
				IsPrimeCost = X.IsPrimeCost,
				IsTaxCost = X.IsTaxCost,
				IsPercentage = X.IsPercentage,
				DeletedFlg = X.DeletedFlg,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/CategorySetting', 2) 
			WITH(
				CategorySettingId INT,
				BudgetId INT,
				ParentCategoryId INT,
				SalesCategoryRefId INT,
				CategoryName VARCHAR(255),
				SortOrder INT,
				IsSelected BIT,
				IsPrimeCost BIT,
				IsTaxCost BIT,
				IsPercentage BIT,
				DeletedFlg BIT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X
			INNER JOIN set_CategorySetting Y ON Y.CategorySettingId = X.CategorySettingId;
		END

		EXEC sp_xml_removedocument @XMLID
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'UPD', @SessionID, @AddlInfo
	END CATCH
END