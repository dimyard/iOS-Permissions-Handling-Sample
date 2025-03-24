using AppiumPermissionsHandler.Permissions.Models;

namespace AppiumPermissionsHandler.Permissions
{
    /// <summary>
    /// Репозиторий предопределенных системных запросов разрешений
    /// </summary>
    public static class KnownPermissions
    {
        /// <summary>
        /// Запрос разрешения на использование камеры
        /// </summary>
        public static PermissionRequest Camera => new PermissionRequest
        {
            Type = "Camera",
            KeywordsToIdentify = new[] 
            { 
                "camera", "камера", "фото", "снимки", "съемк", 
                "доступ к камере", "разрешить доступ",
                "would like to access the camera", 
                "wants to use your camera",
                "would like to access your camera"
            },
            AllowButtonKeywords = new[] 
            { 
                "allow", "разрешить", "ок", "ok", "да" 
            },
            DenyButtonKeywords = new[] 
            { 
                "deny", "запретить", "отклонить", "нет", "no", "отмена", "cancel"
            }
        };
        
        /// <summary>
        /// Запрос разрешения на использование камеры
        /// </summary>
        public static PermissionRequest Files => new PermissionRequest
        {
            Type = "Camera",
            KeywordsToIdentify = new[] 
            { 
                "медиатеке", "фото и видео"
            },
            AllowButtonKeywords = new[] 
            { 
                "Разрешить полный доступ",  
            },
            DenyButtonKeywords = new[] 
            { 
                "Не разрешать"
            }
        };
        
        /// <summary>
        /// Запрос разрешения на отправку уведомлений
        /// </summary>
        public static PermissionRequest Notifications => new PermissionRequest
        {
            Type = "Notifications",
            KeywordsToIdentify = new[] 
            { 
                "notification", "уведомлен", "сообщен", 
                "присылать", "отправлять", "push",
                "would like to send you notifications",
                "wants to send you notifications", 
                "permission to send you notifications"
            },
            AllowButtonKeywords = new[] 
            { 
                "allow", "разрешить", "ок", "ok", "да" 
            },
            DenyButtonKeywords = new[] 
            { 
                "deny", "запретить", "отклонить", "нет", "no", "отмена", "cancel"
            }
        };
        
        /// <summary>
        /// Получает все предопределенные запросы разрешений
        /// </summary>
        public static IEnumerable<PermissionRequest> GetAll()
        {
            yield return Files;
            yield return Camera;
            yield return Notifications;
            // Добавляйте новые разрешения здесь по мере необходимости
        }
        
        /// <summary>
        /// Пытается определить тип разрешения по тексту
        /// </summary>
        /// <param name="text">Текст для анализа</param>
        /// <returns>Распознанный запрос разрешения или null, если не удалось распознать</returns>
        public static PermissionRequest IdentifyFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
                
            foreach (var permission in GetAll())
            {
                if (permission.MatchesText(text))
                {
                    return permission;
                }
            }
            
            return null;
        }
    }
}