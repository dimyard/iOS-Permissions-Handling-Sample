using AppiumPermissionsHandler.Core.Driver;
using AppiumPermissionsHandler.Core.OCR;
using OpenQA.Selenium;

namespace AppiumPermissionsHandler.Permissions.Handlers
{
    /// <summary>
    /// Обработчик запросов разрешений, объединяющий логику обнаружения и управления системными диалогами
    /// </summary>
    public class PermissionHandler
    {
        private readonly AppiumDriverWrapper _driver;
        private readonly TesseractTextRecognizer _textRecognizer;
        private readonly string _screenshotDir;
        
        /// <summary>
        /// Создает новый экземпляр обработчика разрешений
        /// </summary>
        /// <param name="driver">Обертка драйвера Appium</param>
        /// <param name="textRecognizer">Сервис распознавания текста</param>
        /// <param name="screenshotDir">Директория для сохранения временных скриншотов</param>
        public PermissionHandler(
            AppiumDriverWrapper driver, 
            string screenshotDir = null)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _textRecognizer = new TesseractTextRecognizer();
            _screenshotDir = screenshotDir ?? Path.Combine(Path.GetTempPath(), "AppiumPermissions");
            
            // Создаем директорию для скриншотов, если она не существует
            if (!Directory.Exists(_screenshotDir))
                Directory.CreateDirectory(_screenshotDir);
        }
        
        /// <summary>
        /// Проверяет наличие запроса разрешения на экране и обрабатывает его при обнаружении
        /// </summary>
        /// <param name="action">Действие для выполнения с запросом (Allow/Deny)</param>
        /// <returns>Информацию о найденном и обработанном разрешении или null, если разрешение не обнаружено</returns>
        public PermissionHandlerResult HandlePermissionIfPresent(PermissionAction action = PermissionAction.Allow, bool saveScreenshot = false)
        {
            if (!_driver.HasSystemPermissionButtons())
                return PermissionHandlerResult.NotFound();
        
            // Получаем скриншот как массив байтов
            var screenshotBytes = _driver.TakeScreenshotAsBytes();
            string screenshotPath = null;
    
            // Сохраняем скриншот только если это запрошено
            if (saveScreenshot)
            {
                string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                screenshotPath = Path.Combine(_screenshotDir, fileName);
                File.WriteAllBytes(screenshotPath, screenshotBytes);
            }
    
            // Распознаем текст напрямую из байтов
            var recognizedText = _textRecognizer.RecognizeTextFromBytes(screenshotBytes);

            
            // Определяем тип разрешения по тексту
            var permission = KnownPermissions.IdentifyFromText(recognizedText);
            
            if (permission == null)
                return PermissionHandlerResult.UnknownPermission(recognizedText);
                
            // Выполняем действие в зависимости от выбранного типа
            bool actionPerformed = false;
            
            if (action == PermissionAction.Allow)
            {
                actionPerformed = TapPermissionButton(permission.AllowButtonKeywords);
            }
            else
            {
                actionPerformed = TapPermissionButton(permission.DenyButtonKeywords);
            }
            
            return new PermissionHandlerResult
            {
                Found = true,
                Handled = actionPerformed,
                PermissionType = permission.Type,
                RecognizedText = recognizedText,
                ScreenshotPath = screenshotPath
            };
        }
        
        /// <summary>
        /// Пытается нажать на кнопку разрешения на основе массива ключевых слов
        /// </summary>
        /// <param name="buttonKeywords">Ключевые слова для поиска кнопки</param>
        /// <returns>True, если кнопка была найдена и нажата</returns>
        private bool TapPermissionButton(string[] buttonKeywords)
        {
            try
            {
                var alertButtons = (IList<object>)_driver.Driver.ExecuteScript("mobile: alert", new Dictionary<string, object> { { "action", "getButtons" } });
        
                if (alertButtons == null || alertButtons.Count == 0)
                    return false;
            
                // Ищем кнопку по ключевым словам
                foreach (string button in alertButtons)
                {
                    foreach (var keyword in buttonKeywords)
                    {
                        if (button.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            // Нажимаем на найденную кнопку
                            _driver.Driver.ExecuteScript("mobile: alert", new Dictionary<string, object> 
                            { 
                                { "action", "accept" },
                                { "buttonLabel", button }
                            });
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки
            }
    
            // Пробуем обычный метод через XPath, если предыдущий не сработал
            foreach (var keyword in buttonKeywords)
            {
                if (_driver.TapElement(By.XPath($"//XCUIElementTypeButton[contains(@name, '{keyword}')]")))
                    return true;
            }
    
            return false;
        }
    }
    
    /// <summary>
    /// Типы действий с разрешениями
    /// </summary>
    public enum PermissionAction
    {
        Allow,
        Deny
    }
    
    /// <summary>
    /// Результат обработки запроса разрешения
    /// </summary>
    public class PermissionHandlerResult
    {
        /// <summary>
        /// Флаг обнаружения запроса разрешения
        /// </summary>
        public bool Found { get; set; }
        
        /// <summary>
        /// Флаг успешной обработки запроса разрешения
        /// </summary>
        public bool Handled { get; set; }
        
        /// <summary>
        /// Тип обнаруженного разрешения
        /// </summary>
        public string PermissionType { get; set; }
        
        /// <summary>
        /// Распознанный текст с экрана
        /// </summary>
        public string RecognizedText { get; set; }
        
        /// <summary>
        /// Путь к сохраненному скриншоту
        /// </summary>
        public string ScreenshotPath { get; set; }
        
        /// <summary>
        /// Создает результат для случая, когда разрешение не найдено
        /// </summary>
        public static PermissionHandlerResult NotFound() => new PermissionHandlerResult
        {
            Found = false,
            Handled = false,
            PermissionType = null,
            RecognizedText = null
        };
        
        /// <summary>
        /// Создает результат для случая, когда найден неизвестный запрос разрешения
        /// </summary>
        public static PermissionHandlerResult UnknownPermission(string recognizedText) => new PermissionHandlerResult
        {
            Found = true,
            Handled = false,
            PermissionType = "Unknown",
            RecognizedText = recognizedText
        };
    }
}