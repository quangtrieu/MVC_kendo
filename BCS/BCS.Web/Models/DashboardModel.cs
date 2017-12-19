using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BCS.Web.Models
{
    public class DashBoardModel
    {
        public int CountNotification { get; set; }
        public List<DashBoradNotification> ListNotifications { get; set; }
        public List<WeatherDetails> ListWeathers { get; set; }
        public DateTime CurrentDate { get; set; }
        public string CurrentCity { get; set; }
        public string State { get; set; }
        public string ImageMain { get; set; }

        public string CreatedUserId { get; set; }
        public string RestCode { get; set; }
        public string BudgetType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public IList<BudgetModel> BudgetList { get; set; }
    }
    public class DashBoradNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int StatusId { get; set; }
    }

    public class WeatherDetails
    {
        public string Weather { get; set; }
        public string WeatherIcon { get; set; }
        public string WeatherDay { get; set; }
        public string Temperature { get; set; }
        public string MaxTemperature { get; set; }
        public string MinTemperature { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string Humidity { get; set; }
        public string TimeZone { get; set; }
 

        public static async Task<List<WeatherDetails>> GetWeather(string location)
        {
           // const string appId = "PLACE-YOUR-APP-ID-HERE";
            const string appId = "2e74e6061687e69815ed732d9a5fa7e8";
            string url = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&type=accurate&mode=xml&units=metric&cnt=7&appid={1}",
                location, appId);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(url);
                    if (!(response.Contains("message") && response.Contains("cod")))
                    {
                        XElement xEl = XElement.Load(new System.IO.StringReader(response));
                        return GetWeatherInfo(xEl);
                    }
                    else
                    {
                        return new List<WeatherDetails>();
                    }
                }
                catch (HttpRequestException)
                {
                    return new List<WeatherDetails>();
                }
            }
        }


        private static List<WeatherDetails> GetWeatherInfo(XElement xEl)
        {
            IEnumerable<WeatherDetails> w = xEl.Descendants("time").Select((el) =>
            {
                var timezoneToString = string.Format("{0:dd MMM yyyy}", DateTime.Parse(el.Attribute("day").Value, new CultureInfo("en-CA")));
                var xElement = el.Element("humidity");
                return xElement != null ? new WeatherDetails
                {
                    TimeZone = timezoneToString,
                    Humidity = xElement.Attribute("value").Value + "%",
                    MaxTemperature = el.Element("temperature").Attribute("max").Value + "°",
                    MinTemperature = el.Element("temperature").Attribute("min").Value + "°",
                    Temperature = el.Element("temperature").Attribute("day").Value + "°",
                    Weather = el.Element("symbol").Attribute("name").Value,
                    WeatherDay = DayOfTheWeek(el),
                    WeatherIcon = WeatherIconPath(el),
                    WindDirection = el.Element("windDirection").Attribute("name").Value,
                    WindSpeed = el.Element("windSpeed").Attribute("mps").Value + "mps"
                } : null;
            });

            return w.ToList();
        }


        private static string DayOfTheWeek(XElement el)
        {
            DayOfWeek dW = Convert.ToDateTime(el.Attribute("day").Value).DayOfWeek;
            return dW.ToString();
        }

        private static string WeatherIconPath(XElement el)
        {
            var r = el.Element("symbol");
            if (r != null)
            {
                string symbolVar = r.Attribute("var").Value;
                string symbolNumber = r.Attribute("number").Value;
                string dayOrNight = symbolVar.ElementAt(2).ToString(); // d or n
                return String.Format("WeatherIcons/{0}{1}.png", symbolNumber, dayOrNight);
            }
            return "";
        }

    }
}