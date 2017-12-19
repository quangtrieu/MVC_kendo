-- =============================================
-- Author:		BauNM
-- Create date: 2016-09-20
-- Description:	Get user by role id
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_GetSSPMember]
	@UserId				INT,
	@SystemId			INT,
	@RoleId				INT
AS
BEGIN
	BEGIN TRY
		SELECT
			A.*,
			dbo.func_GetRestCodeByUserId(A.UserId) AS RestCode
		FROM acc_Users A
		WHERE A.DeletedFlg = 0 AND A.SystemId = @SystemId AND A.RoleId = @RoleId;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_User_GetSSPMember: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @UserId
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'GET', @SessionID, @AddlInfo
	END CATCH
END
