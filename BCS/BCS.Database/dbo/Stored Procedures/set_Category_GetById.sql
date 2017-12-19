-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-05
-- Description:	Get category by id
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_GetById]
	@CategoryId int
AS
BEGIN
	BEGIN TRY
		SELECT * FROM set_Category WHERE CategoryId = @CategoryId AND DeletedFlg = 0;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_GetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@CategoryId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'GET', @SessionID, @AddlInfo
	END CATCH
END