-- =============================================
-- Author:		BauNM
-- Create date: 2016-07-04
-- Description:	Add category default by user id
-- =============================================
CREATE PROCEDURE [dbo].[set_Category_AddCategoryDefaultByUser]
	@UserId INT,
	@ParentCategoryId INT,
	@IsPrimeCost BIT,
	@IsTaxCost BIT,
	@DataCode VARCHAR(50)
AS
BEGIN
	BEGIN TRY
		DECLARE @IsPercentage BIT = 1,
				@DeletedFlg BIT = 0,
				@CreatedDate DATETIME = GETDATE();

		-- insert section default if not exists
		INSERT INTO set_Category(CategoryName, SortOrder, IsSelected, IsPrimeCost, IsTaxCost, IsPercentage, ParentCategoryId, DeletedFlg, CreatedDate, CreatedUserId, UpdatedDate, UpdatedUserId)
			SELECT A.DataText, A.SortOrder, 0, @IsPrimeCost, @IsTaxCost, @IsPercentage, @ParentCategoryId, @DeletedFlg, @CreatedDate, @UserId, @CreatedDate, @UserId FROM common_Data A
			LEFT JOIN (
				SELECT * FROM set_Category WHERE CreatedUserId = @UserId AND ParentCategoryId = @ParentCategoryId
			) B ON B.CategoryName = A.DataText
			WHERE A.DataCode = @DataCode AND A.DeletedFlg = 0 AND B.CategoryId IS NULL
		;
	END TRY
	BEGIN CATCH
		DECLARE	@ErrorNum				INT,
				@ErrorMsg				VARCHAR(200),
				@ErrorProc				VARCHAR(50),
				@SessionID				INT,
				@AddlInfo				VARCHAR(MAX)

		SET @ErrorNum					= error_number()
		SET @ErrorMsg					= 'set_Category_AddCategoryDefaultByUser: ' + error_message()
		SET @ErrorProc					= error_procedure()
		SET @AddlInfo					= CAST(@UserId AS VARCHAR(11))
		EXEC utl_Insert_ErrorLog @ErrorNum, @ErrorMsg, @ErrorProc, 'set_Category', 'ADD', @SessionID, @AddlInfo
	END CATCH
END


