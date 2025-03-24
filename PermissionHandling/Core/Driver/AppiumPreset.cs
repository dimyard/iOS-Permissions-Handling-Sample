namespace AppiumPermissionsHandler.Core.Driver
{
    /// <summary>
    /// Хранит предустановленные параметры конфигурации Appium
    /// </summary>
    public class AppiumPreset
    {
        /// <summary>
        /// Название платформы (iOS, Android)
        /// </summary>
        public string PlatformName { get; set; } = "iOS";

        /// <summary>
        /// Название устройства
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Имя автоматизации
        /// </summary>
        public string AutomationName { get; set; } = "XCUITest";

        /// <summary>
        /// Версия платформы
        /// </summary>
        public string PlatformVersion { get; set; }

        /// <summary>
        /// URI сервера Appium
        /// </summary>
        public Uri ServerUri { get; set; } = new Uri("http://0.0.0.0:4723/");

        /// <summary>
        /// Таймаут соединения с драйвером в секундах
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// Дополнительные параметры Appium
        /// </summary>
        public Dictionary<string, object> AdditionalOptions { get; set; } = new Dictionary<string, object>
        {
            { "autoAcceptAlerts", false }
        };

        /// <summary>
        /// Создает набор стандартных параметров для iPhone 15 Pro Max
        /// </summary>
        public static AppiumPreset IPhone15ProMax => new AppiumPreset
        {
            DeviceName = "iPhone 15 Pro Max",
            PlatformVersion = "17.5"
        };
    }
}