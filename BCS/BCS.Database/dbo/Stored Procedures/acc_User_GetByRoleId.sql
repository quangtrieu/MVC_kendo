-- =============================================
-- Author:		BauNM
-- Create date: 2016-08-31
-- Description:	Get user by role id
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_GetByRoleId]
	@UserId				INT,
	@SystemId			INT,
	@RoleId				INT,
	@SystemAdminUserId	INT
AS
BEGIN
	BEGIN TRY
		SELECT * FROM acc_Users WHERE DeletedFlg = 0 AND UserId <> @SystemAdminUserId AND SystemId = @SystemId AND RoleId = @RoleId;
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
