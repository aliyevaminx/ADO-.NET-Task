using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SQL_Connection_Task.Constants
{
    internal static class Messages
    {
        public static void InvalidInputMessage(string message) => Console.WriteLine($"{message} is invalid");
        public static void InputMessage(string message) => Console.WriteLine($"Enter {message}");
        public static void SuccessAddMessage(string title, string message) => Console.WriteLine($"{title} - {message} added succesfully");
        public static void SuccessUpdateMessage(string title, string message) => Console.WriteLine($"{title} - {message} updated succesfully");
        public static void SuccessDeleteMessage(string title, string message) => Console.WriteLine($"{title} - {message} deleted succesfully");
        public static void ErrorOccuredMessage() => Console.WriteLine("Error has occured");
        public static void AlreadyExistsMessage(string message) => Console.WriteLine($"{message} already exists");
        public static void ShowAllMessage(string title, string value) => Console.Write($"{title} - {value}; ");
        public static void NotFoundMessage(string title, string value) => Console.WriteLine($"{title} - {value} not found");
        public static void AskForWantToChangeMessage(string title) => Console.WriteLine($"Do you want to change {title}? y(Yes) or n(No)");
        public static void GetDetailsOfCountry(string title, string value) => Console.Write($"{title} - {value}");
        public static void IsNotEnoughAreaMessage(string title) => Console.WriteLine($"There is not enough area in {title}");
    }
}
