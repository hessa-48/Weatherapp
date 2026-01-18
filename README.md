# Weatherapp


A simple ASP.NET Core MVC web application for viewing and saving weather forecasts.

---

## Technologies Used

- **Backend:** C# (ASP.NET Core MVC)
- **Frontend:** Razor Views, HTML, CSS, JavaScript
- **Database:** SQL Server (with stored procedures)
- **External API:** OpenWeatherMap API for weather data

---

## Features

1. **User Login:** Users can log in to access and save their weather results.  
2. **Get Weather:** Users can enter a city name and view current weather data (humidity, min/max temperature).  
3. **Save Weather:** Users can save the weather result to their account.  
4. **Weather History:** Users can view and edit their saved weather results in a table.  

---

## Project Structure

**WeatherApp**
- **Controllers:** AccountController.cs, WeatherController.cs, WeatherHistoryController.cs  
- **Models:** User.cs, WeatherResult.cs  
- **Services:** DatabaseService.cs  
- **Views:** Account/, Weather/, WeatherHistory/  
- **Database:** WeatherAppDB.sql  
- **appsettings.json**

## Demo Video

You can view a demo of the application in action [here](https://drive.google.com/drive/folders/1r3kH8af0rLyjHPjOFqnn1VtamX1xJ9Jv?usp=drive_link).

