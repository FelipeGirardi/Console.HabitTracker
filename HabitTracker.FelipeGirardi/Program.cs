using HabitTracker.FelipeGirardi.Helpers;
using HabitTracker.FelipeGirardi.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace habit_tracker;

class Program
{
    static string connectionString = @"Data Source=habit-Tracker.db";
    static ValidatorService validatorService = new ValidatorService();
    static void Main(string[] args)
    {
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS habits (
                                       Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                       Date TEXT,
                                       Quantity INTEGER,
                                       Measure TEXT
                                       )";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        }

        GetUserInput();
    }

    static void GetUserInput()
    {
        Console.Clear();

        bool closeApp = false;
        while (closeApp == false)
        {
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("\nType 0 to close the application.");
            Console.WriteLine("Type 1 to view all records.");
            Console.WriteLine("Type 2 to insert record.");
            Console.WriteLine("Type 3 to delete record.");
            Console.WriteLine("Type 4 to update record.");
            Console.WriteLine("--------------------------------------\n");

            string? command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("\nGoodbye!\n");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAllRecords();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                    Console.WriteLine("\nInvalid command. Please type a number from 0 to 4.\n");
                    break;
            }
        }
    }

    private static void GetAllRecords()
    {
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = @"SELECT * FROM habits";

            List<Habit> tableData = new();

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows) {
                while(reader.Read()) {
                    tableData.Add(
                        new Habit
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2),
                            Measure = reader.GetString(3)
                        }
                    );
                }
            } else
            {
                Console.WriteLine("No rows found.");
            }

            connection.Close();

            Console.Clear();
            Console.WriteLine("--------------------------------------\n");

            foreach(var data in tableData)
            {
                Console.WriteLine($"{data.Id} - {data.Date.ToString("dd-MMM-yyyy")} - Quantity: {data.Quantity} {data.Measure}");
            }

            Console.WriteLine("--------------------------------------\n");
        }
    }

    private static void Insert()
    {
        string date = GetDateInput();

        int quantity = GetNumberInput("\n\nPlease insert number of your choice (no decimals allowed)\n");

        string measure = GetStringInput("\n\nPlease insert unit of measure of your choice (no numbers allowed)");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"INSERT INTO habits(date, quantity, measure) VALUES ('{date}', '{quantity}', '{measure}')";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    internal static string GetDateInput()
    {
        Console.WriteLine("\n\nPlease insert the date in the format dd-mm-yy. Type 0 to return to the main menu.\n");
        string? dateInput = Console.ReadLine();

        if (dateInput == "0") GetUserInput();
        if (dateInput == null) GetDateInput();

        while(!validatorService.IsDateValid(dateInput ?? ""))
        {
            Console.WriteLine("\n\nInvalid date (Format: dd-mm-yy). Type 0 to return to the main menu or try again:\n");
            dateInput = Console.ReadLine();
        }

        return dateInput ?? "";
    }

    internal static int GetNumberInput(string message)
    {
        Console.WriteLine(message);
        string? numberInput = Console.ReadLine();

        if (numberInput == "0") GetUserInput();
        if (numberInput == null) GetNumberInput("");

        while (!validatorService.IsNumberValid(numberInput ?? ""))
        {
            Console.WriteLine("\n\nInvalid number. Try again.\n");
            numberInput = Console.ReadLine();
        }

        int finalInput = Convert.ToInt32(numberInput);

        return finalInput;
    }

    internal static string GetStringInput(string message)
    {
        Console.WriteLine(message);
        string? stringInput = Console.ReadLine();

        if (stringInput == "0") GetUserInput();

        while (!validatorService.IsStringValid(stringInput ?? ""))
        {
            Console.WriteLine("\n\nInvalid measure. Try again.\n");
            stringInput = Console.ReadLine();
        }

        return stringInput ?? "";
    }

    private static void Delete()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete. Type 0 to return to the main menu.\n");
    
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"DELETE FROM habits WHERE Id = '{recordId}'";
            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.");
                GetUserInput();
            }

            connection.Close();
        }

        Console.WriteLine($"\n\nRecord with Id {recordId} was deleted.");

    }

    private static void Update()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to update. Type 0 to return to the main menu.\n");
    
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM habits WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"Record with Id {recordId} does not exist.");
                connection.Close();
                GetUserInput();
            }

            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease insert number of your choice (no decimals allowed)\n");
            string measure = GetStringInput("\n\nPlease insert unit of measure of your choice (no numbers allowed)");

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE habits SET Date = '{date}', Quantity='{quantity}', Measure='{measure}' WHERE Id = '{recordId}'";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        }

        Console.WriteLine($"\n\nRecord with Id {recordId} was updated.");
    }
}