-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-05
-- Description:	Get category by user id
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_GetByUserId]
	@UserId INT
AS
BEGIN
	BEGIN TRY
		-- Get all category by user id
		SELECT * FROM set_Category
		WHERE CreatedUserId = @UserId AND DeletedFlg = 0
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_GetByUserId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'GET', @SessionID, @AddlInfo
	END CATCH
END
