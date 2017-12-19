-- =============================================
-- Author:		BauNM
-- Create date: 2016-09-20
-- Description:	Get rest code by user id
-- =============================================
CREATE FUNCTION [dbo].[func_GetRestCodeByUserId]
(
	@UserId INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @return VARCHAR(MAX) = '';
	SELECT @return = @return + RestCode + '; ' FROM acc_RestActiveCode WHERE UserId = @UserId;
	IF @@ROWCOUNT > 0
		SET @return = SUBSTRING(@return, 0, LEN(@return));

	RETURN @return
END
