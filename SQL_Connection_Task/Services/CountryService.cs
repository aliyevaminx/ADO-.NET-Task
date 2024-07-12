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
    internal static class CountryService
    {
        public static void ShowAllCountries()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.Default))
            {
                sqlConnection.Open();

                var selectCommand = new SqlCommand("SELECT * FROM Countries", sqlConnection);

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

        public static void AddCountry()
        {

            Messages.InputMessage("Country Name");
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
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                        {
                            Messages.AlreadyExistsMessage(countryName);
                            return;
                        }

                        Messages.InputMessage("Country Area");
                        string inputArea = Console.ReadLine();
                        decimal area;
                        bool isTrueFormat = decimal.TryParse(inputArea, out area);
                        if (isTrueFormat)
                        {
                            var sqlCommand = new SqlCommand("INSERT INTO Countries VALUES(@name, @area)", sqlConnection);
                            sqlCommand.Parameters.AddWithValue("@name", countryName);
                            sqlCommand.Parameters.AddWithValue("@area", area);

                            var affectedRows = sqlCommand.ExecuteNonQuery();
                            if (affectedRows > 0)
                                Messages.SuccessAddMessage("Country", countryName);
                            else
                                Messages.ErrorOccuredMessage();

                        }
                        else
                            Messages.InvalidInputMessage("Area");
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Country Name");
        }

        public static void UpdateCountry()
        {
            ShowAllCountries();

            Messages.InputMessage("Country Name");
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
                        int id = (int)selectCommand.ExecuteScalar();
                        if (id > 0)
                        {
                        AskWantToChangeNameSection: Messages.AskForWantToChangeMessage("country name");
                            var choiceForName = Console.ReadLine();
                            if (choiceForName.isValidChoice())
                            {
                                string newCountryName = string.Empty;

                                if (choiceForName.Equals("y"))
                                {
                                EnterNewName: Messages.InputMessage("New name");
                                    newCountryName = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newCountryName))
                                    {
                                        var existNameCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", sqlConnection);
                                        existNameCommand.Parameters.AddWithValue("@name", newCountryName);
                                        existNameCommand.Parameters.AddWithValue("@id", id);

                                        int existCountryId = Convert.ToInt32(existNameCommand.ExecuteScalar());
                                        if (existCountryId > 0)
                                        {
                                            Messages.AlreadyExistsMessage("Country");
                                            goto AskWantToChangeNameSection;
                                        }
                                    }
                                    else
                                    {
                                        Messages.InvalidInputMessage("New Name");
                                        goto EnterNewName;
                                    }
                                }


                            AskWantToChangeAreaSection: Messages.AskForWantToChangeMessage("country area");
                                var choiceForArea = Console.ReadLine();
                                decimal newArea = default;
                                if (choiceForArea.isValidChoice())
                                {
                                    if (choiceForArea.Equals("y"))
                                    {
                                    EnterNewArea: Messages.InputMessage("New Area");
                                        var newAreaInput = Console.ReadLine();
                                        bool isTrueFormat = decimal.TryParse(newAreaInput, out newArea);

                                        if (!isTrueFormat)
                                        {
                                            Messages.InvalidInputMessage("New Area");
                                            goto EnterNewArea;
                                        }
                                    }
                                }
                                else
                                {
                                    Messages.InvalidInputMessage("Choice");
                                    goto AskWantToChangeAreaSection;
                                }

                                if (newCountryName != string.Empty || newArea != default)
                                {
                                    string update = "UPDATE Countries SET ";

                                    if (newCountryName != string.Empty)
                                        update += "Name=@name";
                                    if (newArea != default)
                                    {
                                        if (newCountryName != string.Empty)
                                            update += ",";
                                        update += "Area=@area";
                                    }

                                    update += " WHERE Id=@id";

                                    SqlCommand updateCommand = new(update, sqlConnection);

                                    if (newCountryName != string.Empty)
                                        updateCommand.Parameters.AddWithValue("@name", newCountryName);
                                    if (newArea != default)
                                        updateCommand.Parameters.AddWithValue("@area", newArea);
                                    updateCommand.Parameters.AddWithValue("@id", id);

                                    int affectedRows = updateCommand.ExecuteNonQuery();
                                    if (affectedRows > 0)
                                        Messages.SuccessUpdateMessage("Country", countryName);
                                    else
                                        Messages.ErrorOccuredMessage();
                                }

                            }
                            else
                                Messages.InvalidInputMessage("Choice");
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
                Messages.InvalidInputMessage("Country Name");
        }

        public static void DeleteCountry()
        {
            ShowAllCountries();

            Messages.InputMessage("Country Name");
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
                            SqlCommand deleteCities = new SqlCommand("DELETE Cities WHERE CountryId=@countryId", sqlConnection);
                            deleteCities.Parameters.AddWithValue("@countryId", countryId);

                            int affectedRows = deleteCities.ExecuteNonQuery();
                            if (affectedRows >= 0)
                            {
                                SqlCommand deleteCommand = new SqlCommand("DELETE Countries WHERE Id=@id", sqlConnection);
                                deleteCommand.Parameters.AddWithValue("@id", countryId);
                                
                                var affectedCountryRow = deleteCommand.ExecuteNonQuery();
                                if (affectedCountryRow > 0)
                                    Messages.SuccessDeleteMessage("Country", countryName);
                            }
                            else
                                Messages.ErrorOccuredMessage();
                        }
                        else
                            Messages.NotFoundMessage("Country",countryName);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Country Name");
        }

        public static void GetDetailsOfCountry()
        {
            ShowAllCountries();

            Messages.InputMessage("Country name");
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
                Messages.InvalidInputMessage("Country name");
        }
    }
}
