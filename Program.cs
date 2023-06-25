using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace LogReader
{
    class Program
    {
        private MySqlConnection connection;
        private string logFile;
        private string jsonFilePath;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        public void Run()
        {
            Console.WriteLine("Добро пожаловать!");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Регистрация");
            Console.WriteLine("2. Вход в систему");
            Console.WriteLine("3. Экспорт данных в формате JSON");

            Console.Write("Введите номер действия: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    ExportToJson();
                    break;
                default:
                    Console.WriteLine("Неверный ввод.");
                    break;
            }
        }

        public void RegisterUser()
        {
            // Ваш код для регистрации пользователя
            Console.WriteLine("Метод регистрации пользователя.");
        }

        public void Login()
        {
            // Ваш код для входа в систему
            Console.WriteLine("Метод входа в систему.");
        }

        public void ExportToJson()
        {
            Console.WriteLine("Метод экспорта данных в формате JSON.");

            // Установите путь к файлу лога
            logFile = @"C:\Users\Logika\source\repos\ConsoleApp5\access.log";

            // Установите путь к файлу JSON
            jsonFilePath = @"C:\Data\exported_data.json";

            // Создайте список для хранения данных
            List<LogData> logDataList = new List<LogData>();

            // Чтение и обработка файла лога
            using (StreamReader reader = new StreamReader(logFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] logData = line.Split(',');

                    // Добавьте логовые данные в список
                    logDataList.Add(new LogData
                    {
                        Id = int.Parse(logData[0].Trim()),
                        Ip = logData[1].Trim(),
                        DateTime = DateTime.Parse(logData[2].Trim()),
                        Method = logData[3].Trim(),
                        URL = logData[4].Trim()
                    });
                }
            }

            // Преобразование списка в JSON и сохранение в файл
            string jsonData = JsonConvert.SerializeObject(logDataList, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);

            Console.WriteLine("Данные экспортированы в файл JSON: " + jsonFilePath);
        }

        // Другие методы (HashPassword, ValidateUser) остаются без изменений
    }

    // Дополнительный класс для хранения логовых данных
    class LogData
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public DateTime DateTime { get; set; }
        public string Method { get; set; }
        public string URL { get; set; }
    }
}
