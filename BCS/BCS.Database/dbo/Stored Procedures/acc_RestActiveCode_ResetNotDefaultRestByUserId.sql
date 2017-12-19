-- =============================================
-- Author:		BauNM
-- Create date: 2016-09-19
-- Description: Reset not link rest by user id
-- =============================================
CREATE PROCEDURE [dbo].[acc_RestActiveCode_ResetNotDefaultRestByUserId]
	@UserId INT,
	@Result BIT OUTPUT
AS
BEGIN
	BEGIN TRY
		-- Update default to rest code
		UPDATE acc_RestActiveCode
		SET
			IsDefault = 0
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
		SET @ErrorMsg					= 'acc_RestActiveCode_ResetNotDefaultRestByUserId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'acc_RestActiveCode', 'UPD', @SessionID, @AddlInfo
	END CATCH
END
