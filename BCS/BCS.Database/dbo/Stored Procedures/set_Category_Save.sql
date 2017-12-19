-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-08
-- Description:	Add or Edit category
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_Save]
	@UserId int,
	@XML nvarchar(max),
	@CategoryId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @CategoryId = 0
		BEGIN
			INSERT INTO set_Category(CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, ParentCategoryId, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, ParentCategoryId, 0, GETDATE(), @UserId, GETDATE(), @UserId
			FROM OPENXML(@XMLID, '/Category', 2) 
			WITH(
				CategoryId INT,
				CategoryName VARCHAR(255),
				SortOrder INT,
				IsSelected BIT,
				IsPrimeCost BIT,
				IsTaxCost BIT,
				IsPercentage BIT,
				ParentCategoryId INT
			) X;

			SET @CategoryId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE set_Category
			SET
				CategoryName = X.CategoryName,
				SortOrder = X.SortOrder,
				IsSelected = X.IsSelected,
				IsPrimeCost = X.IsPrimeCost,
				IsTaxCost = X.IsTaxCost,
				IsPercentage = X.IsPercentage,
				ParentCategoryId = X.ParentCategoryId,
				DeletedFlg = X.DeletedFlg,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/Category', 2) 
			WITH(
				CategoryId INT,
				CategoryName VARCHAR(255),
				SortOrder INT,
				IsSelected BIT,
				IsPrimeCost BIT,
				IsTaxCost BIT,
				IsPercentage BIT,
				ParentCategoryId INT,
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X
			INNER JOIN set_Category Y ON Y.CategoryId = X.CategoryId;
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
		SET @ErrorMsg					= 'set_Category_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'UPD', @SessionID, @AddlInfo
	END CATCH
END