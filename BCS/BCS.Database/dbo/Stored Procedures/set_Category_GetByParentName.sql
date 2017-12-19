-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-15
-- Description:	Get category by parent category name
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_GetByParentName]
	@UserId INT,
	@CategoryName VARCHAR(255)
AS
BEGIN
	BEGIN TRY
		-- Get parent category id by name.
		DECLARE @ParentCategoryId INT = 0;
		SELECT @ParentCategoryId = CategoryId FROM set_Category WHERE CreatedUserId = @UserId AND CategoryName = @CategoryName AND ParentCategoryId = 0 AND DeletedFlg = 0;

		-- Get all category by parent and this category
		SELECT * FROM set_Category
		WHERE (ParentCategoryId = @ParentCategoryId OR CategoryId = @ParentCategoryId) AND CreatedUserId = @UserId AND DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_GetByParentName: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @CategoryName
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'GET', @SessionID, @AddlInfo
	END CATCH
END