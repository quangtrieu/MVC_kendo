
/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		ssp_IsExistEmailOfUser
***************************************************************************
*/

CREATE  procedure [dbo].[sec_IsExistEmailOfUser]

	@Email				varchar(128),
	@Result				bit out
as

	begin try

		declare @Total int = 0

		select u.*	from	dbo.acc_Users u (nolock)
					where	upper(u.Email)	= upper(@Email) 

		set @Total = (select count(u.UserID)
						from	dbo.acc_Users u (nolock)
						where	upper(u.Email)	= upper(@Email))

		if(@Total > 0)
			select @Result = 1 -- ton tai Email tuong ung voi  Restaurant
		else
			select @Result = 0 -- khong ton tai Email tuong ung voi Restaurant

				
	end try

	begin catch

	end catch