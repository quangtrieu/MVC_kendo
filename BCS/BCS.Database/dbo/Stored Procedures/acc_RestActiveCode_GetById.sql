/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get User sec_Get_UserByEmail
***************************************************************************
*/
CREATE PROCEDURE [dbo].[acc_RestActiveCode_GetById]
	@RestActiveCodeId	INT
AS
SELECT	p.*
FROM	dbo.acc_RestActiveCode p (NOLOCK)
WHERE	p.RestActiveCodeId = @RestActiveCodeId