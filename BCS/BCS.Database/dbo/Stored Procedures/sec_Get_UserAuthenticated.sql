
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get Authenticated
***************************************************************************
*/

create procedure [dbo].[sec_Get_UserAuthenticated]

	@UserName				varchar(128),
	@PassWord				varchar(128),
	@Result					bit out
as

	begin try
	
		declare @Total int = 0

		select	u.*
		from	dbo.acc_Users u (nolock)
		where	upper(u.UserName)	= upper(@UserName) 
			and upper(u.[PassWord]) = upper(@PassWord)

		set @Total = (select	count(u.UserID)
						from	dbo.acc_Users u (nolock)
						where	upper(u.UserName)	= upper(@UserName) 
							and upper(u.[PassWord]) = upper(@PassWord)
					)
		if(@Total >0)
			select @Result = 1
		else
			select @Result = 0
				
	end try

	begin catch
	end catch