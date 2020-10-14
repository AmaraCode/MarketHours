using MarketHours.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace MarketHours.Services
{

    /// <summary>
    /// Service class to handle DataSet
    /// </summary>
    public class HourService
    {

        private IList<Market> _markets;

        /// <summary>
        /// Constructor to setup the object
        /// </summary>
        public HourService()
        {
            _markets = new List<Market>();
            LoadMarketData();
        }


        /// <summary>
        /// Factory for easy creation 
        /// </summary>
        /// <returns></returns>
        public static HourService CreateNew()
        {
            return new HourService();
        }



        /// <summary>
        /// Method to read the json file into the list collection.
        /// Note:  Assumes the json file is in the root folder of the app.
        /// </summary>
        private void LoadMarketData()
        {

            try
            {
                //setup the file variable with path and filename
                string file = Environment.CurrentDirectory + "\\Data\\WorldMarketHours.json";
                Debug.WriteLine($"Market DataFile: {file}");

                //Using AmaraCodes class to handle the FileIO and reading the collection from the json file.
                AmaraCode.FileIO fileIO = new AmaraCode.FileIO();
                //_markets = fileIO.GetCollection<IList<Market>>(file);
                _markets = fileIO.GetCollectionJSON<IList<Market>>(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

        }


        /// <summary>
        /// Method to simply return all markets.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Market> GetAllMarkets()
        {
            return _markets;
        }



        /// <summary>
        /// Method to return all open markets based on current UTC time and Market Close time.
        /// Method will handle time on its own if not passed in.  Based on system clock.
        /// </summary>
        /// <param name="UtcTime"></param>
        /// <returns></returns>
        public IList<Market> OpenMarkets(int UtcTime = 0)
        {


            //TODO: modify this method to utilize the Offset 
            //the Offset was recently updated to be accurate 
            //The current code does not take into account the accurate
            //UTC time.  U.S. markets show closed for the first hour
            //in my timezone when they are actually open.  This cascades to 
            //all markets because of the Offset not being used.

            //create list to return
            List<Market> openMarkets = new List<Market>();


            //first determine if markets are open.
            if (MarketIsClosed())
            {
                return openMarkets;  //yea it is empty
            }




            //if UtcTime was not passed in then read the time from the system
            if (UtcTime == 0)
            {
                UtcTime = DateTime.UtcNow.TimeOfDay.Hours * 100 + DateTime.UtcNow.TimeOfDay.Minutes;
            }


            //Loop through all the markets to figure out which are open. 
            //We chose this over a fat LINQ statement which would be harder to read
            foreach (Market mkt in _markets)
            {

                //There may be markets that overlap the 24 hour mark due to using UTC
                //so in this cas the logic changes
                if (mkt.MarketOpenUTC > mkt.MarketCloseUTC)
                {
                    if ((UtcTime >= mkt.MarketOpenUTC && UtcTime <= 2359) || UtcTime < mkt.MarketCloseUTC)
                    {
                        openMarkets.Add(mkt);
                    }
                }
                else
                {
                    //The rest of the markets that do not overlap 24hrs mark work simply.
                    if (UtcTime >= mkt.MarketOpenUTC && UtcTime < mkt.MarketCloseUTC)
                    {
                        openMarkets.Add(mkt);
                    }
                }

            }

            return openMarkets;
        }


        /// <summary>
        /// Forex markets are closed from Friday GMT (2200) - Sunday GMT (2200)
        /// </summary>
        /// <returns></returns>
        private bool MarketIsClosed()
        {
            bool closed = false;

            //Set the time variable to the hour-minute of current UTC time
            int utcTime = DateTime.UtcNow.TimeOfDay.Hours * 100 + DateTime.UtcNow.TimeOfDay.Minutes;
            DayOfWeek dayOfWeek = DateTime.UtcNow.DayOfWeek;

            if ((dayOfWeek == DayOfWeek.Friday && utcTime >= 2200) ||
                 (dayOfWeek == DayOfWeek.Saturday) ||
                 (dayOfWeek == DayOfWeek.Sunday && utcTime <= 2200))
            {
                closed = true;
            }

            return closed;
        }

    }
}
