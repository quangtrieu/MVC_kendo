-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-01
-- Description:	Get help setting by user id and help setting data id
-- =============================================
CREATE PROCEDURE [dbo].[set_HelpSetting_GetByUserIdAndHelpSettingDataId]
	@UserId INT,
	@HelpSettingDataId INT
AS
BEGIN
	BEGIN TRY

		SELECT * FROM set_HelpSetting WHERE UserId = @UserId AND HelpSettingDataId = @HelpSettingDataId;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_HelpSetting_GetByUserIdAndHelpSettingDataId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + CAST(@HelpSettingDataId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_HelpSetting', 'GET', @SessionID, @AddlInfo
	END CATCH
END