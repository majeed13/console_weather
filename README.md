Read Me

Description:
This is a console application that can be executed from the command terminal. It is built using C#/.NET
to make http requests to 2 different APIs in order to display weather information about a user specified
city in the United States. The city name must be passed to the program as an argument at the same time
execution is called. In the occurrence of multiple cities with the same name, weather information about all
the cities that share that name will be displayed.
Typing ‘myWeatherApp.exe Seattle’ into the command terminal when inside the correct
directory will result in the following information to be displayed:

####################
Seattle, WA
####################

Saturday 10/19/2019

Summary of Today: Overcast

Current Temp: 50°F

High: 52°F

Low: 43°F

Cloud Cover: 89%

This Week's Summary: Rain today through Tuesday, with high temperatures rising to 63°F on Friday.

--------------------

Next 3 days forecast:

-------------------

Sunday 10/20/2019

Light rain in the morning and afternoon.

High: 53°F

Low: 48°F

Chances of rain: 83%

-------------------

Monday 10/21/2019

Light rain throughout the day.

High: 55°F

Low: 50°F

Chances of rain: 86%

-------------------

Tuesday 10/22/2019

Mostly cloudy throughout the day.

High: 57°F

Low: 44°F

Chances of rain: 53%

**********************************************

HOW TO USE:
Make sure you obtain an API key from opencagedata.com and darksky.net and place the keys into the correct variables in the code.

Build the app from source code:

• Navigate to the directory where weather_app_builder.bat, Program.cs, GeoLocation.cs,
WeatherInfo.cs and myWeatherApp.csproj are stored in the command terminal. Make sure they
are all in the same directory if they are not.

• Type ‘weather_app_builder.bat’ and press Enter.

• The script will try to build myWeatherApp.exe using dotnet build. (Make sure you have .NET
installed on your machine)

• After build is successful, the script will navigate you to
{currentDirectory}\bin\Debug\netcoreapp3.0

• Then you will be able to type ‘myWeatehrApp.exe city name’ to run the console
application.
