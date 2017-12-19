
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get User sec_Get_UserByEmail
***************************************************************************
*/

CREATE procedure [dbo].[sec_Get_UserByUserName]
	@UserName				varchar(128)
as
	
	begin try

		select	*
		from	dbo.acc_Users p (nolock)
		where	upper(p.UserName) = upper(@UserName) 
				
	end try

	begin catch
	end catch