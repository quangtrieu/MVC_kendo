﻿-- =============================================
-- Author:		BauNM
-- Create date: 2016-03-30
-- Description:	Get budget by user id
-- =============================================
CREATE PROCEDURE [dbo].[das_GetBudgetByUserId]
	@UserId int
AS
BEGIN
	BEGIN TRY
		SELECT A.*, ISNULL(B.FullName, 'Empty') AS CreatedUserName, ISNULL(C.FullName, 'Empty') AS UpdatedUserName, D.DataText AS CommonDataText, D.DataCode AS CommonDataCode
		FROM (
			-- Get all budget by created user id
			SELECT *, 1 AS EditFlg FROM das_Budgets
			WHERE CreatedUserId = @UserId AND DeletedFlg = 0
		UNION ALL
			-- Get all budget by rest code of by user id
			SELECT *, 0 AS EditFlg FROM das_Budgets
			WHERE CreatedUserId <> @UserId AND DeletedFlg = 0 AND RestCode IN (SELECT RestCode FROM acc_RestActiveCode WHERE UserId = @UserId)
		) A
		LEFT JOIN acc_Users B ON B.UserId = A.CreatedUserId
		LEFT JOIN acc_Users C ON C.UserId = A.UpdatedUserId
		LEFT JOIN (SELECT * FROM common_Data WHERE DataCode = 'BCS006' OR DataCode = 'BCS007') D ON D.DataValue = A.BudgetLength
		ORDER BY A.CreatedDate DESC;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'das_GetBudgetByUserId: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_Budgets', 'GET', @SessionID, @AddlInfo
	END CATCH
END