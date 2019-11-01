using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

class Program
{
    /********** Main(string[]) **********
     * Main method that will run when myWeatherApp.exe is ran.. or 
     * when the user uses dotnet run.
     */
    static void Main(string[] args)
    {
        // check if no argument is passsed in (args.length == 0)
        if (args.Length == 0)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Invalid number of arguments passed in. Please enter "
                        + Environment.NewLine + "a city name as an argument when running the program.");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        else if (lettersOnly(String.Join("", args)))
        {
            // check if argument passed is more than one word
            if (args.Length > 1)
            {
                // Join the arguments sperated by a space and start the program
                runWeatherApp(String.Join(" ", args));
            }
            else
            {
                runWeatherApp(args[0]);
            }
        }
        else
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Invalid argument passed in. " + String.Join(" ", args)
                        + Environment.NewLine + "is not a valid argument when running the program.");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
    }

    /********** runWeatherApp(string) **********
     * This is the main program method to call to retrieve weather information for
     * the passed in string city.
     */
    public static void runWeatherApp(string city)
    {
        // ADD your open cage and dark sky API keys to these variables
        string openCage = "YOUR_KEY";
        string darkSky = "YOUR_KEY";
        // ***********************************************************************
        using (var client = new HttpClient())
        {
            // set the base url address to get geo cords of passed in city
            client.BaseAddress = new Uri("https://api.opencagedata.com/geocode/v1/");
            // get the response message for the city desired and using the correct API key
            HttpResponseMessage geoResponse = client.GetAsync("json?q=" + city + "+usa&key=" + openCage
                                        + "&pretty=1&no_annotations=1").Result;
            // status code of response
            int status = (int)geoResponse.StatusCode;
            // run this task is the response code is valid
            if (status < 299 && status >= 200)
            {
                string geoResult = geoResponse.Content.ReadAsStringAsync().Result;
                GeoLocation cords = new GeoLocation();
                WeatherInfo details = new WeatherInfo();
                cords = JsonConvert.DeserializeObject<GeoLocation>(geoResult);
                bool validCity = false; // used to track if geo locations returned are not for cities
                string locationName; // will be used to store the name of the city found
                string weatherResult;
                double lat; // latitude
                double lon; // longitude
                // check all results returned by opencage API for the requested city name
                foreach (Result e in cords.Results)
                {
                    // only operate on results that are of type city AND contain the name of that city
                    if (e.Components["_type"].Equals("city") && e.Components.ContainsKey("city"))
                    {
                        validCity = true; // we have at least 1 valid city in the result
                        // retrieve latitude, longitude from the current result
                        lat = e.Geometry.Lat;
                        lon = e.Geometry.Lng;
                        // make a request to DarkSky API using hte geo coordinates
                        HttpResponseMessage weatherResponse = client.GetAsync("https://api.darksky.net/forecast/" + darkSky + "/" + lat + ","
                                                                             + lon + "?exclude=minutely,hourly,flags,alerts").Result;
                        // set the current location name
                        locationName = e.Components["city"] + ", " + e.Components["state_code"];
                        weatherResult = weatherResponse.Content.ReadAsStringAsync().Result;
                        details = JsonConvert.DeserializeObject<WeatherInfo>(weatherResult);
                        // display weather information about the current location
                        Console.WriteLine("####################" + Environment.NewLine + locationName);
                        displayCurrentWeather(details);
                        Console.WriteLine("--------------------");
                        // display next 3 days information for the current location (exluding today)
                        Console.WriteLine("Next 3 days forecast:");
                        for (int i = 1; i <= 3; i++)
                        {
                            displayFutureForecast(details.Daily.Data[i]);
                        }
                        Console.WriteLine("**********************************************" + Environment.NewLine);
                    }
                }
                // this will check if the request to opencage did not return a city in the result
                if (!validCity)
                {
                    Console.WriteLine("The city requested is not a valid US city."
                                    + Environment.NewLine + "Please try again");
                }
            }
            // DEAL WITH RESPONSE CODES
            // Should not get 300 response code according to open cage API documentation
            // But, will have it just in case
            else if (status >= 300 && status < 399)
            {
                Console.WriteLine("Reached a status code = " + status);
                Console.WriteLine("Please try again");

            }
            else if (status >= 400 && status < 499)
            {
                // URL not found
                urlNotFound(status);
            }
            else if (status >= 500 && status < 599)
            {
                // Server Down
                serverDown(status);
            }
        }
    }

    /********** displayCurrentWeather(WeatherInfo) **********
     * This method will display the current weather information from the
     * passed in WetherInfo object.
     * Information will be displayed to the console as follows:
     * #######################
     * Day Of Week MM/DD/YY
     * Summary of Today:
     * Current Temp: °F
     * High: °F
     * Low: °F
     * Cloud Cover: %
     * (if any possible precipitation)
     * Chance of (Precip Type): %
     * This weeks Summary:
     *
     */
    public static void displayCurrentWeather(WeatherInfo cur)
    {
        var localDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(cur.Currently.Time).DateTime.ToLocalTime().ToString();
        var index = localDateTimeOffset.IndexOf(" ");
        var date = localDateTimeOffset.Substring(0, index);
        Console.WriteLine("####################");
        Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds(cur.Currently.Time).LocalDateTime.DayOfWeek + " " + date);
        Console.WriteLine("Summary of Today: " + cur.Currently.Summary);
        Console.WriteLine("Current Temp: " + (int)cur.Currently.Temperature + "°F");
        Console.WriteLine("High: " + (int)cur.Daily.Data[0].TemperatureHigh + "°F");
        Console.WriteLine("Low: " + (int)cur.Daily.Data[0].TemperatureLow + "°F");
        Console.WriteLine("Cloud Cover: " + cur.Currently.CloudCover * 100 + "%");
        if (cur.Currently.PrecipProbability > 0)
        {
            Console.WriteLine("Chance of " + cur.Currently.PrecipType + ": " + (int)(cur.Currently.PrecipProbability * 100) + "%");
        }
        Console.WriteLine("This Week's Summary: " + cur.Daily.Summary);
    }

    /********** displayCurrentWeather(Datum) **********
     * This method will display the weather information from a passed in
     * Datum object from WeatherInfo.Daily.
     * Information will be displayed to the console as follow:
     * ------------------------
     * Day of Week MM/DD/YY
     * High: °F
     * Low: °F
     * (if precipitation probability > 0)
     * Chance of (Precip Type): %
     *
     */
    public static void displayFutureForecast(Datum datum)
    {
        var localDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(datum.Time).DateTime.ToLocalTime().ToString();
        var index = localDateTimeOffset.IndexOf(" ");
        var date = localDateTimeOffset.Substring(0, index);
        Console.WriteLine("-------------------");
        Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds(datum.Time).LocalDateTime.DayOfWeek + " " + date);
        Console.WriteLine(datum.Summary);
        Console.WriteLine("High: " + Convert.ToInt32(datum.TemperatureHigh) + "°F");
        Console.WriteLine("Low: " + Convert.ToInt32(datum.TemperatureLow) + "°F");
        if (datum.PrecipProbability > 0)
        {
            Console.WriteLine("Chances of " + datum.PrecipType + ": "
                            + (int)(datum.PrecipProbability * 100) + "%");
        }
    }

    /********** serverDown(int) **********
     * This method is used to handle a 5xx HttpResponseMessage.
     */
    public static void serverDown(int status)
    {
        Console.WriteLine("Reached status code: " + status);
        Console.WriteLine("This action is: Not allowed\nPlease try again");
    }

    /********** urlNotFound(int) **********
    * This method is used to handle a 4xx HttpResponseMessage
    */
    public static void urlNotFound(int status)
    {
        Console.WriteLine("Reached status code: " + status);
        Console.WriteLine("This action is: Not allowed\nPlease try again");
    }

    /********** lettersOnly(string) **********
    * This method is used to check if the string passed in only contains
    * letters.
    * Returns: True is all chars are letters, False otherwise.
    */
    public static bool lettersOnly(string toCheck)
    {
        foreach (char e in toCheck)
        {
            if (!Char.IsLetter(e))
                return false;
        }
        return true;
    }
}
