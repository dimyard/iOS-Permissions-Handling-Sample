using Tesseract;

namespace AppiumPermissionsHandler.Core.OCR
{
    /// <summary>
    /// Сервис распознавания текста с использованием Tesseract OCR
    /// </summary>
    public class TesseractTextRecognizer : IDisposable
    {
        private readonly TesseractEngine _engine;
        private readonly string _dataPath;
        
        /// <summary>
        /// Создает новый экземпляр распознавателя текста с настройками по умолчанию
        /// </summary>
        /// <param name="dataPath">Путь к данным Tesseract (tessdata)</param>
        /// <param name="language">Язык распознавания (например, "eng" или "eng+rus")</param>
        public TesseractTextRecognizer(string language = "rus+eng")
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
    
            if (!Directory.Exists(_dataPath))
                throw new DirectoryNotFoundException($"Tesseract data directory not found: {_dataPath}");
        
            _engine = new TesseractEngine(_dataPath, language, EngineMode.Default);
        }
        
        /// <summary>
        /// Распознает текст с изображения
        /// </summary>
        /// <param name="imagePath">Путь к файлу изображения</param>
        /// <returns>Распознанный текст</returns>
        public string RecognizeText(string imagePath)
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = _engine.Process(img))
                {
                    return page.GetText().Trim();
                }
            }
        }
        
        /// <summary>
        /// Асинхронно распознает текст с изображения
        /// </summary>
        /// <param name="imagePath">Путь к файлу изображения</param>
        /// <returns>Задача с распознанным текстом</returns>
        public Task<string> RecognizeTextAsync(string imagePath)
        {
            return Task.Run(() => RecognizeText(imagePath));
        }
        
        /// <summary>
        /// Распознает текст из массива байтов изображения
        /// </summary>
        /// <param name="imageBytes">Байты изображения</param>
        /// <returns>Распознанный текст</returns>
        public string RecognizeTextFromBytes(byte[] imageBytes)
        {
            // Сохраняем во временный файл, чтобы использовать Pix.LoadFromFile
            string tempFile = Path.Combine(Path.GetTempPath(), $"ocr_temp_{Guid.NewGuid()}.png");
            
            try
            {
                File.WriteAllBytes(tempFile, imageBytes);
                return RecognizeText(tempFile);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    try { File.Delete(tempFile); } catch { /* игнорируем ошибки при удалении */ }
                }
            }
        }
        
        /// <summary>
        /// Асинхронно распознает текст из массива байтов изображения
        /// </summary>
        /// <param name="imageBytes">Байты изображения</param>
        /// <returns>Задача с распознанным текстом</returns>
        public Task<string> RecognizeTextFromBytesAsync(byte[] imageBytes)
        {
            return Task.Run(() => RecognizeTextFromBytes(imageBytes));
        }
        
        /// <summary>
        /// Освобождает ресурсы, используемые Tesseract
        /// </summary>
        public void Dispose()
        {
            _engine?.Dispose();
        }
    }
}