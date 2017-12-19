-- =============================================
-- Author:		BauNM
-- Create date: 2016-05-04
-- Description:	Add or Edit budget item
-- =============================================
CREATE PROCEDURE [dbo].[das_BudgetItem_Save]
	@UserId int,
	@XML nvarchar(max),
	@BudgetItemId int out
AS
BEGIN
	BEGIN TRY
		SET		@XML	= dbo.ufn_Replace_XmlChars(@XML);
		DECLARE	@XMLID	INT;
		EXEC sp_xml_preparedocument	@XMLID OUT, @XML;

		IF @BudgetItemId = 0
		BEGIN
			INSERT INTO das_BudgetItem(BudgetTabId, CategorySettingId, BudgetItemRow, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT BudgetTabId, CategorySettingId, BudgetItemRow, DeletedFlg, GETDATE(), CreatedUserId, GETDATE(), UpdatedUserId
			FROM OPENXML(@XMLID, '/BudgetItem', 2) 
			WITH(
				BudgetTabId INT,
				CategorySettingId INT,
				BudgetItemRow VARCHAR(MAX),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X;

			SET @BudgetItemId = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE das_BudgetItem
			SET
				BudgetTabId = X.BudgetTabId,
				CategorySettingId = X.CategorySettingId,
				BudgetItemRow = X.BudgetItemRow,
				DeletedFlg = X.DeletedFlg,
				CreatedDate = X.CreatedDate,
				CreatedUserId = X.CreatedUserId,
				UpdatedDate = GETDATE(),
				UpdatedUserId = @UserId
			FROM OPENXML(@XMLID, '/BudgetItem', 2) 
			WITH(
				BudgetItemId INT,
				BudgetTabId INT,
				CategorySettingId INT,
				BudgetItemRow VARCHAR(MAX),
				DeletedFlg BIT,
				CreatedDate DATETIME,
				CreatedUserId INT,
				UpdatedDate DATETIME,
				UpdatedUserId INT
			) X
			INNER JOIN das_BudgetItem Y ON Y.BudgetItemId = X.BudgetItemId;
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
		SET @ErrorMsg					= 'das_BudgetItem_Save: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11)) + ':' + @XML
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'das_BudgetItem', 'ADD', @SessionID, @AddlInfo
	END CATCH
END
