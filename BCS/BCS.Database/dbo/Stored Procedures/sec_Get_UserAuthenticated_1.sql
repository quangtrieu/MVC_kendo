/*
***************************************************************************
	-- Author:			HiepNV
	-- Description:		Get Authenticated
***************************************************************************
*/
CREATE procedure [dbo].[sec_Get_UserAuthenticated]
	@UserName				varchar(128),
	@PassWord				varchar(128),
	@Result					bit out
as
begin try
	select	u.*
	from	dbo.acc_Users u (nolock)
	where	DeletedFlg = 0 AND 
		--check Active = 1 AND
		(u.UserName = @UserName or u.Email = @UserName) AND
		u.[PassWord] = @PassWord

	if(@@ROWCOUNT > 0)
		select @Result = 1
	else
		select @Result = 0
end try

begin catch
end catch