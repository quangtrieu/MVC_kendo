-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-01
-- Description:	Update default value to rest code by user id
-- =============================================
CREATE PROCEDURE [dbo].[acc_RestActiveCode_UpdateDefaultRestCodeByUser]
	@UserId INT,
	@RestCode VARCHAR(255),
	@Result BIT OUTPUT
AS
BEGIN
	BEGIN TRY
		-- Update default to rest code
		UPDATE acc_RestActiveCode
		SET
			IsDefault = (CASE WHEN RestCode = @RestCode THEN 1 ELSE 0 END)
		WHERE UserId = @UserId;

		IF @@ROWCOUNT > 0
			SET @Result = 1;
		ELSE
			SET @Result = 0;

	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'acc_RestActiveCode_UpdateDefaultRestCodeByUser: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @RestCode
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_RestActiveCode', 'UPD', @SessionID, @AddlInfo
	END CATCH
END