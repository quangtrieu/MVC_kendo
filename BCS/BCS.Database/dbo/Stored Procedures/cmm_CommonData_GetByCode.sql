-- =============================================
-- Author:		TrieuLQ
-- Create date: 2016-04-12
-- Description:	Get common_Data by DataCode
-- =============================================

CREATE PROCEDURE [dbo].[cmm_CommonData_GetByCode]
	@DataCode	varchar(50)
AS
BEGIN
	BEGIN TRY
		SELECT * FROM common_Data WHERE DataCode = @DataCode AND DeletedFlg = 0;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'cmm_CommonData_GetByCode: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= @DataCode
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'common_Data', 'GET', @SessionID, @AddlInfo
	END CATCH
END