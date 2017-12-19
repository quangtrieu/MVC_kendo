-- =============================================
-- Author:		BauNM
-- Create date: 2016-04-20
-- Description:	Get tab number by budget id
-- =============================================
CREATE FUNCTION [dbo].[ufn_GetTabNumberByBudgetId]
(
	@BudgetId int,
	@UserId int
)
RETURNS VARCHAR(1000)
AS
BEGIN
	DECLARE @Result VARCHAR(1000) = '';

	-- get budget by id
	DECLARE @BudgetType INT = 0, @BudgetLength INT = 0, @BudgetLengthStart date, @FiscalYearStartOn date;
	SELECT
		@BudgetType = BudgetLengthType,
		@BudgetLengthStart = BudgetLengthStart,
		@FiscalYearStartOn = FiscalYearStartOn,
		@BudgetLength = BudgetLength	
	FROM das_Budgets WHERE BudgetId = @BudgetId;

	IF @BudgetType = 1 AND MONTH(@FiscalYearStartOn) = 1
	BEGIN
		SET @Result = CAST(YEAR(@BudgetLengthStart) AS VARCHAR(4));
		IF @BudgetLength >= 106			--2 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 1 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 107			--3 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 2 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 108			--4 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 3 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 109			--5 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 4 AS VARCHAR(4));
		END
	END
	ELSE IF @BudgetType = 1 AND MONTH(@FiscalYearStartOn) <> 1
	BEGIN
		SET @Result = CAST(YEAR(@BudgetLengthStart) AS VARCHAR(4)) + ' - ' + CAST(YEAR(@BudgetLengthStart) + 1 AS VARCHAR(4));
		IF @BudgetLength >= 106			--2 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 1 AS VARCHAR(4)) + ' - ' + CAST(YEAR(@BudgetLengthStart) + 2 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 107			--3 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 2 AS VARCHAR(4)) + ' - ' + CAST(YEAR(@BudgetLengthStart) + 3 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 108			--4 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 3 AS VARCHAR(4)) + ' - ' + CAST(YEAR(@BudgetLengthStart) + 4 AS VARCHAR(4));
		END 
		IF @BudgetLength >= 109			--5 year						
		BEGIN
			SET @Result = @Result + ';' + CAST(YEAR(@BudgetLengthStart) + 4 AS VARCHAR(4)) + ' - ' + CAST(YEAR(@BudgetLengthStart) + 5 AS VARCHAR(4));
		END
	END
	ELSE
	BEGIN
		SET @Result = 'Period budget - ' + CAST(YEAR(@BudgetLengthStart) AS VARCHAR(4))
		IF @BudgetLength >= 214			--2 year						
		BEGIN
			SET @Result = @Result + ';Period budget - ' + CAST(YEAR(@BudgetLengthStart) + 1 AS VARCHAR(4));
		END
		IF @BudgetLength >= 215			--3 year						
		BEGIN
			SET @Result = @Result + ';Period budget - ' + CAST(YEAR(@BudgetLengthStart) + 2 AS VARCHAR(4));
		END
		IF @BudgetLength >= 216			--4 year						
		BEGIN
			SET @Result = @Result + ';Period budget - ' + CAST(YEAR(@BudgetLengthStart) + 3 AS VARCHAR(4));
		END
		IF @BudgetLength >= 217			--5 year
		BEGIN
			SET @Result = @Result + ';Period budget - ' + CAST(YEAR(@BudgetLengthStart) + 4 AS VARCHAR(4));
		END

		IF @BudgetLength = 218			--52 periods
		BEGIN
			SET @Result = 'Period budget - ' + CAST(YEAR(@BudgetLengthStart) AS VARCHAR(4))
		END
	END


	RETURN @Result
END
