using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MarketHours.Models
{


    public class Market
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Zone { get; set; }
        public object ZoneOffset { get; set; }
        public string DST { get; set; }
        public string LocalOpen { get; set; }
        public string LocalClose { get; set; }
        public string LocalLunch { get; set; }
        public string UTCOpen { get; set; }
        public string UTCClose { get; set; }
        public string UTCLunch { get; set; }

        [JsonIgnore]
        public int MarketOpenUTC
        {
            get
            {
                try
                {
                    return Convert.ToInt32(UTCOpen);
                }
                catch
                {
                    return default(int);
                }

            }
        }

        [JsonIgnore]
        public int MarketCloseUTC
        {

            get
            {
                try
                {
                    return Convert.ToInt32(UTCClose);
                }
                catch
                {
                    return default(int);
                }


            }
        }


    }

}
