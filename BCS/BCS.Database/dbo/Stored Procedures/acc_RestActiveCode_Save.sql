-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-28
-- Description:	Add or Edit rest active code
-- =============================================
CREATE PROCEDURE [dbo].[acc_RestActiveCode_Save]
	@UserId				INT,
	@XML				NVARCHAR(MAX),
	@RestActiveCodeId	INT OUT
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF(@RestActiveCodeId) = 0
		BEGIN
			INSERT INTO acc_RestActiveCode(UserId, RestCode, RestName, TokenId, IsDefault, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT X.UserId, X.RestCode, X.RestName, X.TokenId, X.IsDefault, X.DeletedFlg, X.CreatedDate, X.CreatedUserId, X.UpdatedDate, X.UpdatedUserId
			FROM OPENXML(@XMLID, '/RestActiveCode', 2) 
			WITH(
				RestActiveCodeId	INT,
				UserId				INT,
				RestCode			VARCHAR(255),
				RestName			VARCHAR(255),
				TokenId				VARCHAR(255),
				IsDefault			BIT,
				DeletedFlg			BIT,
				CreatedDate			DATETIME,
				CreatedUserId		INT,
				UpdatedDate			DATETIME,
				UpdatedUserId		INT
			) X;
			SET @RestActiveCodeId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE acc_RestActiveCode
			SET
				UserId			= X.UserId,
				RestCode		= X.RestCode,
				RestName		= X.RestName,
				TokenId			= X.TokenId,
				IsDefault		= X.IsDefault,
				DeletedFlg		= X.DeletedFlg,
				UpdatedDate		= X.UpdatedDate,
				UpdatedUserId	= X.UpdatedUserId
			FROM OPENXML(@XMLID, '/RestActiveCode', 2) 
			WITH(
				RestActiveCodeId	INT,
				UserId				INT,
				RestCode			VARCHAR(255),
				RestName			VARCHAR(255),
				TokenId				VARCHAR(255),
				IsDefault			BIT,
				DeletedFlg			BIT,
				CreatedDate			DATETIME,
				CreatedUserId		INT,
				UpdatedDate			DATETIME,
				UpdatedUserId		INT
			) X
			INNER JOIN acc_RestActiveCode Y ON X.RestActiveCodeId = Y.RestActiveCodeId;
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
		SET @ErrorMsg					= 'acc_RestActiveCode_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_RestActiveCode', 'UPD', @SessionID, @AddlInfo
	END CATCH
END
