using MarketHours.Models;
using MarketHours.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace MarketHours
{

    /// <summary>
    /// The Entry Point of the application
    /// </summary>
    class Program
    {

        static int timeout = 0;

        /// <summary>
        /// Entry Point - currently expecting no parameters
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //Create Instance of the HourService which will handle all IO
            var service = HourService.CreateNew();

            //Varialbe to hold time number in 24hr format
            int _utcTime;
            int _localTime;

            //This loop could be better setup with cancellation or something better than CTRL-C to exit
            bool success = true;
            while (success)
            {
                Console.Clear();

                //Set the time variable to the hour-minute of current UTC time
                _utcTime = DateTime.UtcNow.TimeOfDay.Hours * 100 + DateTime.UtcNow.TimeOfDay.Minutes;
                _localTime = DateTime.Now.TimeOfDay.Hours * 100 + DateTime.Now.TimeOfDay.Minutes;


                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Welcome to the Market Open Project");
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Current UTC Hour is {_utcTime.ToString("0000")}");
                Console.WriteLine($"Current Local Hour is {_localTime.ToString("0000")}");

                Console.WriteLine("");


                //Call the service to get a current list of open markets based on UtcTime
                var openMarkets = service.OpenMarkets(_utcTime).OrderBy(x => x.Name);
                
                //Call method to display to the screen
                DisplayOpen(openMarkets, _utcTime);


                //Set the timeout and sleep for a minute.
                timeout = (60 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond;
                Thread.Sleep(timeout);

            }
        }


        /// <summary>
        /// Method to Display/Format market items to the screen
        /// </summary>
        /// <param name="markets"></param>
        /// <param name="currentUTC"></param>
        static void DisplayOpen(IEnumerable<Market> markets, int currentUTC)
        {


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0, -10} {1, -10} {2, -50} {3, -20}", "Open", "Close", "Name", "City");  //Composite Formatting
            Console.WriteLine(new string('-', 100));

            //Loop through each item in the markets collection
            foreach (Market m in markets)
            {
                //If the market is within 1 hour of closing then display that line in red, otherwise 
                if (currentUTC >= m.MarketCloseUTC -100 )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }


                //Using Composite Formatting to display the market item
                Console.WriteLine("{0, -10} {1, -10} {2, -50} {3, -20}", m.MarketOpenUTC.ToString("0000"), m.MarketCloseUTC.ToString("0000"), m.Name, m.City);
            }


        }

    }
}
