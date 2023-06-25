using MySql.Data.MySqlClient;
using Newtonsoft.Json;


namespace LogReader
{
    public class LogData
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public DateTime DateTime { get; set; }
        public string Method { get; set; }
        public string URL { get; set; }
    }

    class Program
    {
        private MySqlConnection connection;
        private string logFile;
        private string jsonFilePath;
        private string connectionString = "Server=sql7.freesqldatabase.com;Database=sql7627201;Uid=sql7627201;Pwd=Zwg79E7SlF;";

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
            Console.WriteLine("4. Просмотр данных в БД");
            Console.WriteLine("5. Просмотр данных в БД по айпи");

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
                case "4":
                    ViewDataInDatabase();
                    break;
                case "5":
                    Console.WriteLine("Укажите IP, по которому хотите вывести лог");
                    string ipAddress = Console.ReadLine();
                    ViewDataByIP(ipAddress);
                    break;
                default:
                    Console.WriteLine("Неверный ввод.");
                    break;
            }
        }

        public void RegisterUser()
        {
            try
            {
                Console.WriteLine("Метод регистрации пользователя.");

                Console.Write("Введите имя пользователя: ");
                string username = Console.ReadLine();

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string checkUserQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    using (MySqlCommand checkUserCommand = new MySqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@username", username);
                        int existingUserCount = Convert.ToInt32(checkUserCommand.ExecuteScalar());

                        if (existingUserCount > 0)
                        {
                            Console.WriteLine("Пользователь с таким именем уже существует.");
                            return;
                        }
                    }


                    string insertUserQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    using (MySqlCommand insertUserCommand = new MySqlCommand(insertUserQuery, connection))
                    {
                        insertUserCommand.Parameters.AddWithValue("@username", username);
                        insertUserCommand.Parameters.AddWithValue("@password", password);
                        insertUserCommand.ExecuteNonQuery();

                        Console.WriteLine("Пользователь успешно зарегистрирован.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при регистрации пользователя: " + ex.Message);
            }
        }


        public void Login()
        {
            Console.WriteLine("Метод входа в систему.");

            Console.Write("Введите имя пользователя: ");
            string username = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (MySqlCommand checkUserCommand = new MySqlCommand(checkUserQuery, connection))
                {
                    checkUserCommand.Parameters.AddWithValue("@username", username);
                    int existingUserCount = Convert.ToInt32(checkUserCommand.ExecuteScalar());

                    if (existingUserCount == 0)
                    {
                        Console.WriteLine("Пользователь с указанным именем не найден.");
                        return;
                    }
                }

                string getPasswordQuery = "SELECT password FROM users WHERE username = @username";
                using (MySqlCommand getPasswordCommand = new MySqlCommand(getPasswordQuery, connection))
                {
                    getPasswordCommand.Parameters.AddWithValue("@username", username);
                    string storedPassword = getPasswordCommand.ExecuteScalar()?.ToString();

                    if (password == storedPassword)
                    {
                        Console.WriteLine("Вход выполнен успешно.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный пароль.");
                    }
                }
            }
        }




        public void ExportToJson()
        {
            try
            {
                Console.WriteLine("Метод экспорта данных в формате JSON.");

                logFile = @"C:\Users\Logika\source\repos\ConsoleApp5\access.log";
                jsonFilePath = @"C:\Data\exported_data.json";

                List<LogData> logDataList = new List<LogData>();

                using (StreamReader reader = new StreamReader(logFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] logData = line.Split(',');

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

                string directoryPath = Path.GetDirectoryName(jsonFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string jsonData = JsonConvert.SerializeObject(logDataList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, jsonData);

                Console.WriteLine("Данные экспортированы в файл JSON: " + jsonFilePath);

                Console.WriteLine("Содержимое файла JSON:");
                Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при экспорте данных в JSON: " + ex.Message);
            }
        }

        public void ViewDataInDatabase()
        {
            try
            {
                Console.WriteLine("Метод просмотра данных в БД.");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM access_log";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ip = reader.GetString("Ip");
                                string Method = reader.GetString("Method");
                                string URL = reader.GetString("URL");
                                string Data_Time = reader.GetString("Data_Time");

                                Console.WriteLine($"ip:{ip}, Data_Time:{Data_Time},Method:{Method},URL:{URL} ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при просмотре данных в БД: " + ex.Message);
            }
        }

        public void ViewDataByIP(string ipAddress)
        {
            try
            {
                Console.WriteLine("Метод просмотра данных в БД с фильтром по IP.");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM access_log WHERE Ip = @IpAddress";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IpAddress", ipAddress);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ip = reader.GetString("Ip");
                                string method = reader.GetString("Method");
                                string URL = reader.GetString("URL");
                                string dateTime = reader.GetString("Data_Time");

                                Console.WriteLine($"IP: {ip}, Data_Time: {dateTime}, Method: {method}, URL: {URL}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при просмотре данных в БД с фильтром по IP: " + ex.Message);
            }
        }
    }
}
