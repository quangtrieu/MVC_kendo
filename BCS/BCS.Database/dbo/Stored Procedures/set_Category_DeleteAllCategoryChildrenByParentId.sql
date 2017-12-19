-- =============================================
-- Author:		TrieuLQ
-- Create date: 2016-08-04
-- Description:	Delte all category children default by parentCategoryId
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_DeleteAllCategoryChildrenByParentId]
	@ParentCategoryId INT,
	@UserId INT
AS
BEGIN
	BEGIN TRY

		-- delete all category default by user id
		DELETE FROM set_Category WHERE ParentCategoryId = @ParentCategoryId and CreatedUserId = @UserId

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_DeleteAllCategoryChildrenByParentId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'UPD', @SessionID, @AddlInfo
	END CATCH
END