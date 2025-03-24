

namespace AppiumPermissionsHandler.Permissions.Models
{
    /// <summary>
    /// Представляет запрос разрешения системы, который может появиться в приложении
    /// </summary>
    public class PermissionRequest
    {
        /// <summary>
        /// Уникальный идентификатор типа разрешения (например, "Camera", "Location")
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Ключевые слова для идентификации этого типа разрешения из текста
        /// </summary>
        public string[] KeywordsToIdentify { get; set; }
        
        /// <summary>
        /// Ключевые слова для идентификации кнопки подтверждения разрешения
        /// </summary>
        public string[] AllowButtonKeywords { get; set; }
        
        /// <summary>
        /// Ключевые слова для идентификации кнопки отклонения разрешения
        /// </summary>
        public string[] DenyButtonKeywords { get; set; }
        
        /// <summary>
        /// Пороговое значение для распознавания (0-1), определяет необходимую уверенность 
        /// в правильности распознавания текста разрешения
        /// </summary>
        public double RecognitionThreshold { get; set; } = 0.7;
        
        /// <summary>
        /// Проверяет, содержит ли текст ключевые слова для идентификации этого разрешения
        /// </summary>
        /// <param name="text">Текст для проверки</param>
        /// <returns>True, если текст содержит ключевые слова для идентификации</returns>
        public bool MatchesText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
                
            foreach (var keyword in KeywordsToIdentify)
            {
                if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}