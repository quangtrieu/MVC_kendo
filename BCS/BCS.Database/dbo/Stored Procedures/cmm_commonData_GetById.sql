
-- =============================================
-- Author:		TRIEULQ
-- Create date: 2016-04-12
-- Description:	Get common_Data by id
-- =============================================
CREATE PROCEDURE [dbo].[cmm_commonData_GetById]
	@Id int
AS
BEGIN
	BEGIN TRY
		SELECT * FROM common_Data WHERE Id = @Id AND DeletedFlg = 0;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'cmm_commonData_GetById: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@Id AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'common_Data', 'GET', @SessionID, @AddlInfo
	END CATCH
END