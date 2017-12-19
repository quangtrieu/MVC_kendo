-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-11
-- Description:	Get category setting by id
-- =============================================
CREATE PROCEDURE [dbo].[set_CategorySetting_GetById]
	@CategorySettingId int
AS
BEGIN
	BEGIN TRY
		SELECT * FROM set_CategorySetting WHERE CategorySettingId = @CategorySettingId AND DeletedFlg = 0;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_CategorySetting_GetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@CategorySettingId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_CategorySetting', 'GET', @SessionID, @AddlInfo
	END CATCH
END