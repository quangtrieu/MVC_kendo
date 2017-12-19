
/*
***************************************************************************
	-- Author:			TrieuLQ
	-- Create date: 2016-08-25
	-- Description:		BCS_IsExistEmailOfUserId
***************************************************************************
*/

CREATE  PROCEDURE [dbo].[sec_IsExistEmailOfUserId]

	@UserId				INT,
	@Email				VARCHAR(128),
	@Result				BIT OUT
AS

	BEGIN TRY

		SELECT u.*	FROM	dbo.acc_Users u (NOLOCK)
					WHERE	UserId != @UserId  AND u.Email = @Email
					
		if(@@ROWCOUNT > 0)
			SELECT @Result = 1 
		else
			SELECT @Result = 0 

				
	END TRY

	BEGIN CATCH

	END CATCH