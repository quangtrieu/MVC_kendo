-- =============================================
-- Author:		BauNM
-- Create date: 2016-06-09
-- Description:	Get user by rest code
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_GetByRestCode]
	@UserId		int,
	@RestCode	varchar(max)
AS
BEGIN
	BEGIN TRY
		SELECT * FROM acc_Users A
		INNER JOIN (
			SELECT DISTINCT UserId FROM acc_RestActiveCode X
			INNER JOIN dbo.ufn_Split(@RestCode, ',') Y ON X.RestCode = Y.Data
		) B ON A.UserId = B.UserId
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_User_GetByRestCode: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @UserId
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'GET', @SessionID, @AddlInfo
	END CATCH
END