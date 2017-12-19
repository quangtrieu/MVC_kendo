CREATE TABLE [dbo].[acc_Users]
(
    [UserId]          INT PRIMARY KEY  IDENTITY (1, 1) NOT NULL,
    [Email]           VARCHAR (255)    NOT NULL,
    [UserName]        VARCHAR (255)    NOT NULL,
    [Password]        VARCHAR (255)    NOT NULL,
    [SystemId]        INT              NOT NULL,
    [RoleId]          INT              NULL,
    [Active]          BIT              NULL,
    [FullName]        NVARCHAR (255)   NULL,
    [Phone]           VARCHAR (30)     NULL,
    [LastedDateLogin] DATETIME         NULL,
    [DeletedFlg]      BIT              NULL,
    [ForgotCode]      UNIQUEIDENTIFIER NULL,
    [ForgotExpired]   DATETIME         NULL,
    [CreatedDate]     DATETIME         NULL,
    [CreatedUserId]   INT              NULL,
    [UpdatedDate]     DATETIME         NULL,
    [UpdatedUserId]   INT              NULL
)
