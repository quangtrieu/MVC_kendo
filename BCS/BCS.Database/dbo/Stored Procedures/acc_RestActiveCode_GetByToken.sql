/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get User sec_Get_UserByEmail
***************************************************************************
*/
CREATE PROCEDURE [dbo].[acc_RestActiveCode_GetByToken]
	@TokenId	VARCHAR(255)
AS
SELECT	p.*
FROM	dbo.acc_RestActiveCode p (NOLOCK)
WHERE	p.TokenId = @TokenId