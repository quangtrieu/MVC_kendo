-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-06
-- Description:	Get category by parent category id
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_GetByParentId]
	@UserId INT,
	@ParentCategoryId INT
AS
BEGIN
	BEGIN TRY
		-- Get childen category by parent category id
		SELECT * FROM set_Category
		WHERE ParentCategoryId = @ParentCategoryId AND CreatedUserId = @UserId AND DeletedFlg = 0
		ORDER BY SortOrder;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_GetByParentId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'GET', @SessionID, @AddlInfo
	END CATCH
END
