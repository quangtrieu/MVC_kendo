-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-31
-- Description:	Get rest active code by user id
-- =============================================
CREATE PROCEDURE [dbo].[acc_RestActiveCode_GetByUserId]
	@UserId INT
AS
BEGIN
	BEGIN TRY
		SELECT A.* FROM acc_RestActiveCode A WHERE A.UserId = @UserId;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_RestActiveCode_GetByUserId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_RestActiveCode', 'GET', @SessionID, @AddlInfo
	END CATCH
END
