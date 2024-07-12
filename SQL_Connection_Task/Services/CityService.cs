using SQL_Connection_Task.Constants;
using SQL_Connection_Task.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Connection_Task.Services
{
    internal static class CityService
    {
        public static void ShowAllCities()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
            {
                sqlConnection.Open();

                var selectCommand = new SqlCommand("SELECT * FROM Cities", sqlConnection);

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = Convert.ToString(reader["Name"]);
                        decimal area = Convert.ToDecimal(reader["Area"]);
                        Messages.ShowAllMessage("Name", name);
                        Messages.ShowAllMessage("Area", area.ToString());
                        Console.WriteLine();

                    }
                }
            }
        }

        public static void GetAllCitiesOfCountry()
        {
            CountryService.ShowAllCountries();

            Messages.InputMessage("country name to show its cities");
            string countryName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
                {
                    sqlConnection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", sqlConnection);
                    selectCommand.Parameters.AddWithValue("@name", countryName);

                    try
                    {
                        int countryId = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (countryId > 0)
                        {
                            var selectCityCommand = new SqlCommand("SELECT * FROM Cities WHERE CountryId=@countryId", sqlConnection);
                            selectCityCommand.Parameters.AddWithValue("@countryId", countryId);

                            using (var reader = selectCityCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string name = Convert.ToString(reader["Name"]);
                                    decimal area = Convert.ToDecimal(reader["Area"]);
                                    Messages.ShowAllMessage("Name", name);
                                    Messages.ShowAllMessage("Area", area.ToString());
                                    Console.WriteLine();
                                }
                            }
                        }
                        else
                            Messages.NotFoundMessage("Country", countryName);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.NotFoundMessage("Country", countryName);
        }

        public static void AddCity()
        {
            CountryService.ShowAllCountries();

            Messages.InputMessage("Country name to add city");
            string countryName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
                {
                    sqlConnection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", sqlConnection);
                    selectCommand.Parameters.AddWithValue("@name", countryName);
                    try
                    {
                        int countryId = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (countryId > 0)
                        {
                            Messages.InputMessage("City name");
                            string cityName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(cityName))
                            {
                                var selectCityCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", sqlConnection);
                                selectCityCommand.Parameters.AddWithValue("@name", cityName);

                                int cityId = Convert.ToInt32(selectCityCommand.ExecuteScalar());
                                if (cityId > 0)
                                {
                                    Messages.AlreadyExistsMessage(cityName);
                                    return;
                                }

                                Messages.InputMessage("City area");
                                var cityAreaInput = Console.ReadLine();
                                decimal cityArea;
                                bool isTrueFormat = decimal.TryParse(cityAreaInput, out cityArea);
                                if (isTrueFormat)
                                {

                                    var countryAreaCommand = new SqlCommand("SELECT Area FROM Countries WHERE Id=@id", sqlConnection);
                                    countryAreaCommand.Parameters.AddWithValue("@id", countryId);
                                    var countryArea = countryAreaCommand.ExecuteScalar();
                                    decimal convertCountryArea = countryArea != DBNull.Value ? Convert.ToDecimal(countryArea) : 0;

                                    var sumOfCitiesFromCountryCommand = new SqlCommand("Select SUM(Area) AS SumOfCityArea FROM Cities WHERE CountryId=@countryId", sqlConnection);
                                    sumOfCitiesFromCountryCommand.Parameters.AddWithValue("@countryId", countryId);
                                    var sumOfCitiesArea = sumOfCitiesFromCountryCommand.ExecuteScalar();
                                    decimal ConvertSumOfCitiesArea = sumOfCitiesArea != DBNull.Value ? Convert.ToDecimal(sumOfCitiesArea) : 0;

                                    if (convertCountryArea.isEnoughArea(cityArea, ConvertSumOfCitiesArea))
                                    {

                                        var sqlCityInsertCommand = new SqlCommand("INSERT INTO Cities VALUES(@name, @area, @countryId)", sqlConnection);
                                        sqlCityInsertCommand.Parameters.AddWithValue("@name", cityName);
                                        sqlCityInsertCommand.Parameters.AddWithValue("@area", cityArea);
                                        sqlCityInsertCommand.Parameters.AddWithValue("@countryId", countryId);

                                        var affectedRows = sqlCityInsertCommand.ExecuteNonQuery();
                                        if (affectedRows > 0)
                                            Messages.SuccessAddMessage("City", cityName);
                                        else
                                            Messages.ErrorOccuredMessage();
                                    }
                                    else
                                        Messages.IsNotEnoughAreaMessage(countryName);
                                }
                                else
                                    Messages.InvalidInputMessage("City Area");
                            }
                        }
                        else
                            Messages.NotFoundMessage("Country", countryName);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Country name");
        }

        public static void UpdateCity()
        {
            ShowAllCities();

            Messages.InputMessage("city name to edit");
            string cityName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
                {
                    sqlConnection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", sqlConnection);
                    selectCommand.Parameters.AddWithValue("@name", cityName);
                    try
                    {
                        int cityId = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (cityId > 0)
                        {
                        AskWantToChangeCityName: Messages.AskForWantToChangeMessage("city name");
                            var choiceForName = Console.ReadLine();
                            if (choiceForName.isValidChoice())
                            {
                                string newCityName = string.Empty;

                                if (choiceForName.Equals("y"))
                                {
                                EnterNewCityName: Messages.InputMessage("new city name");
                                    newCityName = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newCityName))
                                    {
                                        var existCityNameCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", sqlConnection);
                                        existCityNameCommand.Parameters.AddWithValue("@name", newCityName);

                                        int existCityId = Convert.ToInt32(existCityNameCommand.ExecuteScalar());
                                        if (existCityId > 0)
                                        {
                                            Messages.AlreadyExistsMessage("city");
                                            goto AskWantToChangeCityName;
                                        }
                                    }
                                    else
                                    {
                                        Messages.InvalidInputMessage("new city name");
                                        goto EnterNewCityName;
                                    }
                                }


                            AskWantToChangeCityArea: Messages.AskForWantToChangeMessage("city area");
                                var choiceForArea = Console.ReadLine();
                                decimal newArea = default;
                                if (choiceForArea.isValidChoice())
                                {
                                    if (choiceForArea.Equals("y"))
                                    {
                                    EnterNewCityArea: Messages.InputMessage("New Area");
                                        var newAreaInput = Console.ReadLine();
                                        bool isTrueFormat = decimal.TryParse(newAreaInput, out newArea);

                                        if (!isTrueFormat)
                                        {
                                            Messages.InvalidInputMessage("New Area");
                                            goto EnterNewCityArea;
                                        }
                                    }
                                }
                                else
                                {
                                    Messages.InvalidInputMessage("Choice");
                                    goto AskWantToChangeCityArea;
                                }

                            AskWantToChangeCountry: Messages.AskForWantToChangeMessage("country of city");
                                var choiceForCountry = Console.ReadLine();
                                string newCountryOfCity = string.Empty;
                                int countryId = default;
                                if (choiceForCountry.isValidChoice())
                                {
                                    if (choiceForCountry.Equals("y"))
                                    {
                                    EnterNewCountryNameForCity: Messages.InputMessage("new country name");
                                        newCountryOfCity = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newCountryOfCity))
                                        {
                                            var selectCountryCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", sqlConnection);
                                            selectCountryCommand.Parameters.AddWithValue("@name", newCountryOfCity);

                                            countryId = Convert.ToInt32(selectCountryCommand.ExecuteScalar());
                                            if (!(countryId > 0))
                                            {
                                                Messages.NotFoundMessage("Country", newCountryOfCity);
                                                goto AskWantToChangeCountry;
                                            }
                                        }
                                        else
                                        {
                                            Messages.InvalidInputMessage("country name");
                                            goto EnterNewCountryNameForCity;
                                        }
                                    }
                                }

                                if (newCityName != string.Empty || newArea != default || newCountryOfCity != string.Empty)
                                {
                                    string update = "UPDATE Citites SET ";
                                    if (newCityName != string.Empty)
                                        update += "Name=@name";
                                    if (newArea != default)
                                    {
                                        if (newCityName != string.Empty)
                                            update += ", ";
                                        update += "Area=@area";
                                    }
                                    if (newCountryOfCity != string.Empty)
                                    {
                                        if (newCityName != string.Empty || newArea != default)
                                            update += ", ";
                                        update += "CountryId=@countryId";
                                    }

                                    update += " WHERE Id=@id";


                                    var countryAreaCommand = new SqlCommand("SELECT Area FROM Countries WHERE Id=@id", sqlConnection);
                                    countryAreaCommand.Parameters.AddWithValue("@id", countryId);
                                    var countryArea = countryAreaCommand.ExecuteScalar();
                                    decimal convertCountryArea = countryArea != DBNull.Value ? Convert.ToDecimal(countryArea) : 0;

                                    var sumOfCitiesFromCountryCommand = new SqlCommand("Select SUM(Area) AS SumOfCityArea FROM Cities WHERE CountryId=@countryId", sqlConnection);
                                    sumOfCitiesFromCountryCommand.Parameters.AddWithValue("@countryId", countryId);
                                    var sumOfCitiesArea = sumOfCitiesFromCountryCommand.ExecuteScalar();
                                    decimal ConvertSumOfCitiesArea = sumOfCitiesArea != DBNull.Value ? Convert.ToDecimal(sumOfCitiesArea) : 0;

                                    if (convertCountryArea.isEnoughArea(newArea, ConvertSumOfCitiesArea))
                                    {
                                        SqlCommand updateCommand = new SqlCommand(update, sqlConnection);

                                        if (newCityName != string.Empty)
                                            updateCommand.Parameters.AddWithValue("@name", newCityName);
                                        if (newArea != default)
                                            updateCommand.Parameters.AddWithValue("@area", newArea);
                                        if (newCountryOfCity != string.Empty)
                                            updateCommand.Parameters.AddWithValue("@countryId", countryId);
                                        updateCommand.Parameters.AddWithValue("@id", cityId);

                                        int affectedRows = updateCommand.ExecuteNonQuery();
                                        if (affectedRows > 0)
                                            Messages.SuccessUpdateMessage("City", cityName);
                                        else
                                            Messages.ErrorOccuredMessage();
                                    }
                                    else
                                        Messages.IsNotEnoughAreaMessage("country");
                                }
                            }
                            else
                                Messages.NotFoundMessage("City", cityName);
                        }
                        else
                            Messages.NotFoundMessage("City", cityName);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }

        public static void DeleteCity()
        {
            ShowAllCities();

            Messages.InputMessage("City Name");
            string cityName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cityName))
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
                {
                    sqlConnection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", sqlConnection);
                    selectCommand.Parameters.AddWithValue("@name", cityName);

                    try
                    {
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                        {
                            SqlCommand deleteCommand = new SqlCommand("DELETE Cities WHERE Id=@id", sqlConnection);
                            deleteCommand.Parameters.AddWithValue("@id", id);

                            int affectedRows = deleteCommand.ExecuteNonQuery();
                            if (affectedRows > 0)
                                Messages.SuccessDeleteMessage("City", cityName);
                            else
                                Messages.ErrorOccuredMessage();
                        }
                        else
                            Messages.NotFoundMessage("City", cityName);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City Name");
        }

        public static void GetDetailsOfCity()
        {
            ShowAllCities();

            Messages.InputMessage("City name");
            string cityName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
                {
                    sqlConnection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", sqlConnection);
                    selectCommand.Parameters.AddWithValue("@name", cityName);

                    try
                    {
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                Messages.GetDetailsOfCountry("Name", Convert.ToString(reader["Name"]));
                                Messages.GetDetailsOfCountry(" Area", Convert.ToString(reader["Area"]));
                                Console.WriteLine();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }
    }
}
