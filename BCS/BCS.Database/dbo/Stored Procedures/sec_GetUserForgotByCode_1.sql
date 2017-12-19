
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		sec_GetUserForgotByCode
***************************************************************************
*/

create  procedure [dbo].[sec_GetUserForgotByCode]

	@ForgotCode		    uniqueidentifier
as

	begin try

		select * from dbo.acc_Users where ForgotCode = @ForgotCode
				
	end try

	begin catch

	

	end catch