
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		[sec_ResetPassword]
***************************************************************************
*/

create  procedure [dbo].[sec_ResetPassword]
	@UserId				bigint,
	@PassWord		    varchar(128)
as

	begin try
		update  dbo.acc_Users  set [PassWord] = @PassWord where UserID = @UserId
				
	end try

	begin catch


	end catch