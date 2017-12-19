-- =============================================
-- Author:		BauNM
-- Create date: 2016-08-31
-- Description:	Check duplicate user name
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_CheckDuplicateUserName]
	@UserId			INT,
	@UserName		VARCHAR(255),
	@Result			BIT OUTPUT
AS
BEGIN
	BEGIN TRY

		SELECT
			@Result = CASE WHEN COUNT(UserId) > 0 THEN 1 ELSE 0 END
		FROM acc_Users
		WHERE UserId <> @UserId AND UserName = @UserName AND DeletedFlg = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_User_GetByRoleId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @UserId
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'GET', @SessionID, @AddlInfo
	END CATCH
END
