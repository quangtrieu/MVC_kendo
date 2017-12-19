
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get User sec_Get_UserByEmail
***************************************************************************
*/

create procedure [dbo].[sec_Get_UserByEmail]

	@Email				varchar(128)
as
	
	begin try

		select	*
		from	dbo.acc_Users p (nolock)
		where	upper(p.Email) = upper(@Email) 
				
	end try

	begin catch
	end catch