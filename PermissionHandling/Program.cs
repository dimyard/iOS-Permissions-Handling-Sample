using AppiumPermissionsHandler.Core.Driver;
using AppiumPermissionsHandler.Permissions.Handlers;

namespace AppiumPermissionsHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("=== Appium Permissions Handler - Демонстрация работы ==");
            Console.WriteLine("======================================================");
            
            try 
            {
                AppiumPreset preset = new AppiumPreset() { DeviceName = "iPhone 15 Pro Max", PlatformVersion = "17.5" };
                var driverWrapper = new AppiumDriverWrapper(preset);
            	var handlingResult = new PermissionHandler(driverWrapper).HandlePermissionIfPresent();
                Console.WriteLine(handlingResult.Found ? $"Разрешение [{handlingResult.PermissionType}] успешно обработано" : "Разрешение не было найдено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }
    }
}