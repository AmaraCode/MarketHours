using MarketHours.Models;
using MarketHours.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

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


            Console.WriteLine("Press ESC to Exit");

            var taskKeys = new Task(ReadKeys);
            var taskProcessFiles = new Task(CheckMarkets);

            taskKeys.Start();
            taskProcessFiles.Start();

            var tasks = new[] { taskKeys };
            Task.WaitAll(tasks);


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Thank you for playing....");
        }


        public static void ReadKeys()
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();

            while (!Console.KeyAvailable && key.Key != ConsoleKey.Escape)
            {

                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        //Console.WriteLine("UpArrow was pressed");
                        break;
                    case ConsoleKey.DownArrow:
                        //Console.WriteLine("DownArrow was pressed");
                        break;

                    case ConsoleKey.RightArrow:
                        //Console.WriteLine("RightArrow was pressed");
                        break;

                    case ConsoleKey.LeftArrow:
                        Console.WriteLine("LeftArrow was pressed");
                        break;

                    case ConsoleKey.Escape:
                        break;

                    default:
                        if (Console.CapsLock && Console.NumberLock)
                        {
                            Console.WriteLine(key.KeyChar);
                        }
                        break;
                }
            }
        }


        public static void CheckMarkets()
        {

            //Create Instance of the HourService which will handle all IO
            var service = HourService.CreateNew();

            //Varialbe to hold time number in 24hr format
            int _utcTime;
            int _localTime;

            //This loop could be better setup with cancellation or something better than CTRL-C to exit
            bool success = true;


            while (success == true)
            {
                Console.Clear();


                //Set the time variable to the hour-minute of current UTC time
                _utcTime = DateTime.UtcNow.TimeOfDay.Hours * 100 + DateTime.UtcNow.TimeOfDay.Minutes;
                //_utcTime = 0230;
                _localTime = DateTime.Now.TimeOfDay.Hours * 100 + DateTime.Now.TimeOfDay.Minutes;


                Console.CursorVisible = false;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Welcome to the Market Open Project");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Current UTC Hour is {_utcTime.ToString("0000")}");
                Console.WriteLine($"Current Local Hour is {_localTime.ToString("0000")}");

                Console.WriteLine("");


                //Call the service to get a current list of open markets based on UtcTime
                var openMarkets = service.OpenMarkets(_utcTime).OrderBy(x => x.Name).ToList();

                //Call method to display to the screen
                DisplayOpen(openMarkets, _utcTime);




                Console.CursorTop = 0;

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
        static void DisplayOpen(IList<Market> markets, int currentUTC)
        {
            //first check to see if there are any open markets
            if (markets.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("");
                System.Console.WriteLine("ALL MARKETS ARE CURRENTLY CLOSED");
                System.Console.WriteLine("");


            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0, -10} {1, -10} {2, -50} {3, -20}", "Open", "Close", "Name", "City");  //Composite Formatting
                Console.WriteLine(new string('-', 100));

                //Loop through each item in the markets collection
                foreach (Market m in markets)
                {
                    //If the market is within 1 hour of closing then display that line in red, otherwise 
                    if (Math.Abs(currentUTC - m.MarketCloseUTC) < 100)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (Math.Abs(currentUTC - m.MarketOpenUTC) < 100)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }



                    //Using Composite Formatting to display the market item
                    Console.WriteLine("{0, -10} {1, -10} {2, -50} {3, -20}", m.MarketOpenUTC.ToString("0000"), m.MarketCloseUTC.ToString("0000"), m.Name, m.City);

                }

            }

            if (markets.Count > 0)
            {
                //display footer
                Console.WriteLine(new string('-', 100));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Indicates market open first hour");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Indicates market open last hour");
                Console.ForegroundColor = ConsoleColor.White;
            }




        }

    }
}
