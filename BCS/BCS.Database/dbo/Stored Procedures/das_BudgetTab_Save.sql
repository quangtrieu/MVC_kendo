-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-22
-- Description:	Add or Edit budget tab
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetTab_Save]
	@UserId int,
	@XML nvarchar(max),
	@BudgetTabId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @BudgetTabId = 0
		BEGIN
			INSERT INTO das_BudgetTab(BudgetId, TabIndex, TabName, AnnualSales, HeaderColumn, TargetColumn, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT BudgetId, TabIndex, TabName, AnnualSales, HeaderColumn, TargetColumn, DeletedFlg, GETDATE(), CreatedUserId, GETDATE(), UpdatedUserId
			FROM OPENXML(@XMLID, '/BudgetTab', 2) 
			WITH(
				BudgetId INT,
				TabIndex INT,
				TabName VARCHAR(50),
				AnnualSales DECIMAL(15,2),
				HeaderColumn VARCHAR(MAX),
				TargetColumn VARCHAR(MAX),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X;

			SET @BudgetTabId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE das_BudgetTab
			SET
				BudgetId = X.BudgetId,
				TabIndex = X.TabIndex,
				TabName = X.TabName,
				AnnualSales = X.AnnualSales,
				HeaderColumn = X.HeaderColumn,
				TargetColumn = X.TargetColumn,
				DeletedFlg = X.DeletedFlg,
				CreatedDate = X.CreatedDate,
				CreatedUserId = X.CreatedUserId,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/BudgetTab', 2) 
			WITH(
				BudgetTabId INT,
				BudgetId INT,
				TabIndex INT,
				TabName VARCHAR(50),
				AnnualSales DECIMAL(15,2),
				HeaderColumn VARCHAR(MAX),
				TargetColumn VARCHAR(MAX),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X
			INNER JOIN das_BudgetTab Y ON Y.BudgetTabId = X.BudgetTabId;
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
		SET @ErrorMsg					= 'das_BudgetTab_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetTab', 'UPD', @SessionID, @AddlInfo
	END CATCH
END