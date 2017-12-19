CREATE procedure [dbo].[utl_Insert_ErrorLog]
	@ErrorCd		int, 
	@ErrorMsg		varchar(200), 
	@ProcName		varchar(50), 
	@TableName		varchar(50), 
	@ActionType		varchar(3),
	@SessionID		bigint, 
	@AddlInfo		varchar(max)
as
BEGIN
	insert into utl_Error_Log
			(ErrorNum,
			ErrorMsg,
			ProcName,
			TableName,
			ActionType,
			SessionID,
			AddlInfo,
			CreatedDate)
	values
			(@ErrorCd,
			@ErrorMsg,
			@ProcName,
			@TableName,
			@ActionType,
			@SessionID,
			@AddlInfo,
			getdate())
END