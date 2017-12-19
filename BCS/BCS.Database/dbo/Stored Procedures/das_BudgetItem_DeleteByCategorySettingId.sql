-- =============================================
-- Author:		BauNM
-- Create date: 2016-05-20
-- Description:	Delete budget item by category setting id
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetItem_DeleteByCategorySettingId]
	@UserId int,
	@CategorySettingId int
AS
BEGIN
	BEGIN TRY
		UPDATE das_BudgetItem SET DeletedFlg = 1 WHERE CategorySettingId = @CategorySettingId;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_BudgetItem_DeleteByCategorySettingId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetItem', 'UPD', @SessionID, @AddlInfo
	END CATCH
END
