CREATE TABLE [dbo].[utl_Error_Log] (
    [ErrorID]     INT           IDENTITY (1, 1) NOT NULL,
    [ErrorNum]    INT           NULL,
    [ErrorMsg]    VARCHAR (200) NULL,
    [ProcName]    VARCHAR (50)  NULL,
    [TableName]   VARCHAR (50)  NULL,
    [ActionType]  VARCHAR (3)   NULL,
    [SessionID]   BIGINT        NULL,
    [AddlInfo]    VARCHAR (MAX) NULL,
    [CreatedDate] DATETIME      CONSTRAINT [DF_utl_Error_Log_CreatedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_utl_Error_Log] PRIMARY KEY CLUSTERED ([ErrorID] ASC) WITH (FILLFACTOR = 70)
);
