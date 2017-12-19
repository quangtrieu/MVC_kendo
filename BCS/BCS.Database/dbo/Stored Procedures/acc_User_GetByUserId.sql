-- =============================================
-- Author:		TrieuLQ
-- Create date: 2016-08-23
-- Description:	Get user by token
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_GetByUserId]
	@UserId	varchar(255)
AS
BEGIN
	BEGIN TRY
		SELECT A.* FROM acc_Users A
		WHERE A.UserId = @UserId AND DeletedFlg = 0
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_User_GetByUserId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @UserId
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'GET', @SessionID, @AddlInfo
	END CATCH
END