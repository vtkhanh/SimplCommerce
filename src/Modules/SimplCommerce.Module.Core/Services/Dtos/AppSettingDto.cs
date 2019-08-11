using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Services.Dtos
{
    public class AppSettingDto
    {
        private static AppSettingDto s_nullObject = new AppSettingDto(string.Empty, string.Empty, string.Empty, string.Empty);

        public AppSettingDto(string key, string value, string module, string description)
        {
            Key = key;
            Value = value;
            Module = module;
            Description = description;
        }

        public string Key { get; }

        public string Value { get; }

        public string Module { get; }

        public string Description { get; set; }

        public static AppSettingDto Create(AppSetting appSetting) => 
            appSetting is null ? s_nullObject : new AppSettingDto(appSetting.Key, appSetting.Value, appSetting.Module, appSetting.Description);
    }
}
