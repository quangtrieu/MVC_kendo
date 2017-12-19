CREATE TABLE [dbo].[set_HelpSetting]
(
	[HelpSettingId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [UserId] INT NULL,
	[HelpSettingDataId] INT NULL,
	[IsHidden] BIT NULL,
	[DeletedFlg] BIT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedUserId] INT NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedUserId] INT NULL
)
