-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-28
-- Description:	Add or Edit user
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_Save]
	@XML nvarchar(max),
	@UserId int,
	@NewUserId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @NewUserId = 0
		BEGIN
			INSERT INTO acc_Users(Email, UserName, [Password], SystemId, RoleId, Active, FullName, Phone, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT X.Email, X.UserName, X.[Password], X.SystemId, X.RoleId, X.Active, X.FullName, X.Phone, X.DeletedFlg, GETDATE(), @UserId, GETDATE(), @UserId
			FROM OPENXML(@XMLID, '/User', 2) 
			WITH(
				UserId			INT,
				Email			VARCHAR(255),
				UserName		VARCHAR(255),
				[Password]		VARCHAR(255),
				SystemId		INT,
				RoleId			INT,
				Active			BIT,
				FullName		VARCHAR(255),
				Phone			VARCHAR(30),
				DeletedFlg		BIT
			) X;

			SET @NewUserId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE acc_Users
			SET
				Email = X.Email,
				UserName = X.UserName,
				[Password] = X.[Password],
				SystemId = X.SystemId,
				RoleId = X.RoleId,
				Active = X.Active,
				FullName = X.FullName,
				Phone = X.Phone,
				DeletedFlg = X.DeletedFlg,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/User', 2) 
			WITH(
				UserId			INT,
				Email			VARCHAR(255),
				UserName		VARCHAR(255),
				[Password]		VARCHAR(255),
				SystemId		INT,
				RoleId			INT,
				Active			BIT,
				FullName		VARCHAR(255),
				Phone			VARCHAR(30),
				DeletedFlg		BIT
			) X
			INNER JOIN acc_Users Y ON Y.UserId = X.UserId;
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
		SET @ErrorMsg					= 'acc_User_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'ADD', @SessionID, @AddlInfo
	END CATCH
END