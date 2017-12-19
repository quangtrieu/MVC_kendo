-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-01
-- Description:	Add or Edit help setting
-- =============================================
CREATE PROCEDURE [dbo].[set_HelpSetting_Save]
	@UserId				INT,
	@XML				NVARCHAR(MAX),
	@HelpSettingId		INT OUT
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF(@HelpSettingId) = 0
		BEGIN
			INSERT INTO set_HelpSetting(UserId, HelpSettingDataId, IsHidden, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT X.UserId, X.HelpSettingDataId, X.IsHidden, X.DeletedFlg, X.CreatedDate, X.CreatedUserId, X.UpdatedDate, X.UpdatedUserId
			FROM OPENXML(@XMLID, '/HelpSetting', 2) 
			WITH(
				HelpSettingId		INT,
				UserId				INT,
				HelpSettingDataId	INT,
				IsHidden			BIT,
				DeletedFlg			BIT,
				CreatedDate			DATETIME,
				CreatedUserId		INT,
				UpdatedDate			DATETIME,
				UpdatedUserId		INT
			) X;
			SET @HelpSettingId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE set_HelpSetting
			SET
				UserId				= X.UserId,
				HelpSettingDataId	= X.HelpSettingDataId,
				IsHidden			= X.IsHidden,
				DeletedFlg			= X.DeletedFlg,
				UpdatedDate			= X.UpdatedDate,
				UpdatedUserId		= X.UpdatedUserId
			FROM OPENXML(@XMLID, '/HelpSetting', 2) 
			WITH(
				HelpSettingId		INT,
				UserId				INT,
				HelpSettingDataId	INT,
				IsHidden			BIT,
				DeletedFlg			BIT,
				CreatedDate			DATETIME,
				CreatedUserId		INT,
				UpdatedDate			DATETIME,
				UpdatedUserId		INT
			) X
			INNER JOIN set_HelpSetting Y ON X.HelpSettingId = Y.HelpSettingId;
		END

		EXEC sp_xml_removedocument @XMLID
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_HelpSetting_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_HelpSetting', 'UPD', @SessionID, @AddlInfo
	END CATCH
END

