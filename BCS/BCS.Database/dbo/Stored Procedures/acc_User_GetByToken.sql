-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-30
-- Description:	Get user by token
-- =============================================
CREATE PROCEDURE [dbo].[acc_User_GetByToken]
	@TokenId	varchar(255)
AS
BEGIN
	BEGIN TRY
		SELECT A.* FROM acc_Users A INNER JOIN acc_RestActiveCode B ON B.UserId = A.UserId AND B.TokenId = @TokenId;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_User_GetByToken: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @TokenId
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_Users', 'GET', @SessionID, @AddlInfo
	END CATCH
END