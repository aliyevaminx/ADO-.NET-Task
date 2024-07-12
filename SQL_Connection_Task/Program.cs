using SQL_Connection_Task.Constants;
using SQL_Connection_Task.Services;

namespace AdoNet {
    public static class  Program 
    {
        public static void Main(string[] args) {

            while (true)
            {
                showMenu();

                string selectedChoice = Console.ReadLine();
                int choice;
                bool isTrueChoice = int.TryParse(selectedChoice, out choice);

                if (isTrueChoice)
                {
                    switch (choice)
                    {
                        case (int)Options.Exit:
                            return;
                        case (int)Options.AllCountries:
                            CountryService.ShowAllCountries();  
                            break;
                        case (int)Options.AddCountry:
                            CountryService.AddCountry();
                            break;
                        case (int)Options.UpdateCountry:
                            CountryService.UpdateCountry();
                            break;
                        case (int)Options.DeleteCountry:
                            CountryService.DeleteCountry();
                            break;
                        case (int)Options.DetailsOfCountry:
                            CountryService.GetDetailsOfCountry();
                            break;
                        case (int)Options.AllCities:
                            CityService.ShowAllCities();
                            break;
                        case (int)Options.AllCitiesOfCountry:
                            CityService.GetAllCitiesOfCountry();
                            break;
                        case (int)Options.AddCity:
                            CityService.AddCity();
                            break;
                        case (int)Options.UpdateCity:
                            CityService.UpdateCity();
                            break;
                        case (int)Options.DeleteCity:
                            CityService.DeleteCity();
                            break;
                        case (int)Options.DetailsOfCity:
                            CityService.GetDetailsOfCity();
                            break;
                        default:
                            Messages.InvalidInputMessage("Choice");
                            break;
                    }
                }
                else
                {
                    Messages.InvalidInputMessage("Choice");
                }
            }
        }

        public static void showMenu() {
            Console.WriteLine("~~~~~~~~ MENU ~~~~~~~~");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. All Countries");
            Console.WriteLine("2. Add Country");
            Console.WriteLine("3. Update Country");
            Console.WriteLine("4. Delete Country");
            Console.WriteLine("5. Details of Country");
            Console.WriteLine("6. All Cities");
            Console.WriteLine("7. All Cities of Country");
            Console.WriteLine("8. Add City");
            Console.WriteLine("9. Update City");
            Console.WriteLine("10. Delete City");
            Console.WriteLine("11. Details of City");
        }
    }
}