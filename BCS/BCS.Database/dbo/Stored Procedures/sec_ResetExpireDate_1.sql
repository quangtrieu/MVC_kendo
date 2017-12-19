
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		ssp_IsExistEmailOfUser
***************************************************************************
*/

create  procedure [dbo].[sec_ResetExpireDate]

	@UserID				bigint,
	@ForgotExpired		DateTime,
	@ForgotCode		    uniqueidentifier
as

	begin try

		update dbo.acc_Users  set
					ForgotExpired	= @ForgotExpired,
					ForgotCode		= @ForgotCode
				where UserID = @UserID
				
	end try

	begin catch


	end catch