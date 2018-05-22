SET IDENTITY_INSERT [dbo].[Core_AppSetting] ON ;

DELETE [dbo].[Core_AppSetting] WHERE Id = 11;

INSERT [dbo].[Core_AppSetting] ([Id], [Key], [Value], [IsVisibleInCommonSettingPage], [Module], [Description]) 
VALUES (11, N'Cost.CurrencyConversionRate', N'21.09', 1, N'Catalog', 'KRW/VND');

DELETE [dbo].[Core_AppSetting] WHERE Id = 12;
INSERT [dbo].[Core_AppSetting] ([Id], [Key], [Value], [IsVisibleInCommonSettingPage], [Module], [Description]) 
VALUES (12, N'Cost.FeePerWeightUnit', N'2', 1, N'Catalog', 'KRW per gam');


DELETE [dbo].[Core_AppSetting] WHERE Id = 13;
INSERT [dbo].[Core_AppSetting] ([Id], [Key], [Value], [IsVisibleInCommonSettingPage], [Module], [Description]) 
VALUES (13, N'Cost.FeeOfPicker', N'5', 1, N'Catalog', '%');

SET IDENTITY_INSERT [dbo].[Core_AppSetting] OFF;
GO