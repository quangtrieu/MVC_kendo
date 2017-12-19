-- =============================================
-- Author:		BauNM
-- Create date: 2016-07-04
-- Description:	Get default section by user id
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_GetDefaultSectionByUser]
	@UserId INT,
	@DataCode VARCHAR(50)
AS
BEGIN
	BEGIN TRY
		-- insert section default if not exists
		INSERT INTO set_Category(CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, ParentCategoryId, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT A.DataText, A.SortOrder, 0, 0, 0, 0, 0, 0, GETDATE(), @UserId, GETDATE(), @UserId FROM common_Data A
			LEFT JOIN (
				SELECT * FROM set_Category WHERE CreatedUserId = @UserId AND DeletedFlg = 0 AND (ParentCategoryId IS NULL OR ParentCategoryId = 0)
			) B ON B.CategoryName = A.DataText
			WHERE A.DataCode = @DataCode AND A.DeletedFlg = 0 AND B.CategoryId IS NULL
		;

		-- Get section default by user id
		SELECT * FROM set_Category WHERE CreatedUserId = @UserId AND DeletedFlg = 0 AND (ParentCategoryId IS NULL OR ParentCategoryId = 0)
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_GetDefaultSectionByUser: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'GET', @SessionID, @AddlInfo
	END CATCH
END

