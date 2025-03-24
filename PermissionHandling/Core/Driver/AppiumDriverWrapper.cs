using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace AppiumPermissionsHandler.Core.Driver
{
    /// <summary>
    /// Обертка для работы с Appium Driver для iOS
    /// </summary>
    public class AppiumDriverWrapper
    {
        private readonly AppiumDriver _driver;
        
        private readonly Uri _appiumServerUri = new("http://127.0.0.1:4723/");
        
        /// <summary>
        /// Возвращает внутренний экземпляр Appium Driver
        /// </summary>
        public AppiumDriver Driver => _driver;
        
		/// <summary>
		/// Создает новый экземпляр обертки для Appium Driver с указанным набором параметров
		/// </summary>
		/// <param name="preset">Набор параметров для Appium</param>
		public AppiumDriverWrapper(AppiumPreset preset)
		{
	    	var appiumOptions = new AppiumOptions();
	    	appiumOptions.PlatformName = preset.PlatformName;
	    	appiumOptions.DeviceName = preset.DeviceName;
	    	appiumOptions.AutomationName = preset.AutomationName;
	    	appiumOptions.PlatformVersion = preset.PlatformVersion;

    		// Добавляем все дополнительные параметры
	    	foreach (var option in preset.AdditionalOptions)
			{
	        	appiumOptions.AddAdditionalAppiumOption(option.Key, option.Value);
			}

    		_driver = new IOSDriver(preset.ServerUri, appiumOptions,
	        TimeSpan.FromSeconds(preset.ConnectionTimeoutSeconds));
		}
        
        /// <summary>
        /// Проверяет наличие элемента на экране
        /// </summary>
        /// <param name="by">Локатор элемента</param>
        /// <param name="timeoutMs">Таймаут ожидания в миллисекундах</param>
        /// <returns>True, если элемент присутствует</returns>
        public bool IsElementPresent(By by, int timeoutMs = 1000)
        {
            try
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(timeoutMs);
                return _driver.FindElements(by).Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
            }
        }
        
        /// <summary>
        /// Делает скриншот экрана устройства и возвращает путь к сохраненному файлу
        /// </summary>
        /// <param name="directory">Директория для сохранения</param>
        /// <returns>Путь к файлу скриншота</returns>
        public string TakeScreenshot(string directory)
        {
            var screenshot = _driver.GetScreenshot();
            
            // Создаем директорию, если не существует
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
                
            string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = Path.Combine(directory, fileName);
            
            screenshot.SaveAsFile(filePath);
            return filePath;
        }
        
        /// <summary>
        /// Делает скриншот экрана устройства и возвращает его в виде массива байтов
        /// </summary>
        /// <returns>Массив байтов изображения</returns>
        public byte[] TakeScreenshotAsBytes()
        {
            var screenshot = _driver.GetScreenshot();
            return screenshot.AsByteArray;
        }
        
        /// <summary>
        /// Нажимает на элемент по локатору
        /// </summary>
        /// <param name="by">Локатор элемента</param>
        /// <returns>True, если нажатие выполнено успешно</returns>
        public bool TapElement(By by)
        {
            try
            {
                var element = _driver.FindElement(by);
                element.Click();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Проверяет наличие системных кнопок разрешений/диалогов для iOS
        /// </summary>
        /// <returns>True, если обнаружены системные кнопки разрешений</returns>
        public bool HasSystemPermissionButtons()
        {
            try
            {
                var alertButtons = ((IOSDriver)_driver).ExecuteScript("mobile: alert", new Dictionary<string, object> { { "action", "getButtons" } });
                return alertButtons != null && ((IList<object>)alertButtons).Count > 0;
            }
            catch
            {
                // Если нет алерта, будет исключение
                return false;
            }
        }
    }
}