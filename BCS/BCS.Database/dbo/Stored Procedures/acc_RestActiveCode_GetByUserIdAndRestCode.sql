-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-01
-- Description:	Get by user id and rest code
-- =============================================
CREATE PROCEDURE [dbo].[acc_RestActiveCode_GetByUserIdAndRestCode]
	@UserId INT,
	@RestCode VARCHAR(255)
AS
BEGIN
	BEGIN TRY

		SELECT * FROM acc_RestActiveCode
		WHERE UserId = @UserId AND RestCode = @RestCode AND DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_RestActiveCode_GetByUserIdAndRestCode: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @RestCode
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_RestActiveCode', 'GET', @SessionID, @AddlInfo
	END CATCH
END