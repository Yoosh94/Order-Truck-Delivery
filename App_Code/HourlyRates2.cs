using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
/// <summary>
/// This class encapsualtes the hourly rates of the truck drivers
/// </summary>
public static class HourlyRates2
{
    //Constants which relates to the column headers in the CSV file. This way creates a tight couple with the file which is not the best practice
    //As long as the headers in the csv files do not change and the morning and evening rate pattern does not change this code will work.
    private const int MORNING = 0;
    private const int EVENING = 1;

    private const int WEEKDAYSTARTTIME = 1;
    private const int WEEKDAYENDTIME = 2;
    private const int WEEKDAYRATE = 3;

    private const int WEEKENDSTARTTIME = 4;
    private const int WEEKENDENDTIME = 5;
    private const int WEEKENDRATE = 6;

    private const int PUBLICHOLIDAYSTARTTIME = 7;
    private const int PUBLICHOLIDAYENDTIME = 8;
    private const int PUBLICHOLIDAYRATE = 9;

    //list of string which hold all the data of the csv file
    private static List<string[]> rowsInFile;

    /// <summary>
    /// Will call a function which will populate the list
    /// </summary>
    public static void createList()
    {
        rowsInFile = readCSV();
    }

    /// <summary>
    /// private class which reads all the data from the csv files
    /// </summary>
    /// <returns>A list of string arrays which holds all the data.</returns>
    private static List<string[]> readCSV()
    {
        var stuff = from line in File.ReadAllLines(HttpContext.Current.Server.MapPath("~/App_Data/hourlyRates.csv")) 
                    let data = line.Split(',')
                    select data;
        return stuff.ToList();
    }


    /// <summary>
    /// Calculate the hourly rate of the truck driver by first determining what day of the week it is, and then checking what time of the day it is.
    /// This function will get called once for every hour the truck has to travel.
    /// </summary>
    /// <param name="currenttime">DateTime object with the current time to check the payrate for</param>
    /// <param name="state">WHich state the truck driver is currently in</param>
    /// <returns>The hourly rate to add to the grand total</returns>
    public static int getHourlyRate(DateTime currenttime,string state)
    {
        //variable to hold the payrate per hour
        int rate = 0;
        //find the two rows which contain the state hourly rates
        var bothRates = from l in rowsInFile
                        where l.Contains(state)
                        select l;
        //Auto assign datetype to weekday
        string dateType = "WEEKDAY";
        //Determine if date is a public holiday
        bool isItaPublicHoliday = publicHolidays.isItAPublicHoliday(state, currenttime);
        //Determine if the date is a weekend
        bool isItaWeekend = CheckWeekend(currenttime);
        //Simple if statement to assign a string to a variable
        if (isItaPublicHoliday)
        {
            dateType = "PUBLIC HOLIDAY";
        }
        if(!isItaPublicHoliday && isItaWeekend)
        {
            dateType = "WEEKEND";
        }
        //Switch statement which runs a function depending on what kind of day it is. Chose switch statment instead of IF statements, because the switch statement will break once a condition is
        //matched and will not check the rest. Also I have placed Weekday on the top because that will be the most used date which will lessen server processing by a minimal amount.
        switch (dateType)
        {
            case "WEEKDAY":
                //These temp TimeSpan objects hold the start and end time for morning and evening for a particlar state.
                TimeSpan tempMorningStart = Convert.ToDateTime(bothRates.ElementAt(MORNING)[WEEKDAYSTARTTIME]).TimeOfDay;
                TimeSpan tempMorningEnd = Convert.ToDateTime(bothRates.ElementAt(MORNING)[WEEKDAYENDTIME]).TimeOfDay;
                TimeSpan tempEveningStart = Convert.ToDateTime(bothRates.ElementAt(EVENING)[WEEKDAYSTARTTIME]).TimeOfDay;
                TimeSpan tempEveningEnd = Convert.ToDateTime(bothRates.ElementAt(EVENING)[WEEKDAYENDTIME]).TimeOfDay;
               

                //If the current time is between the morning section find the rate. (inclusive of the morning startTime)
                if(currenttime.TimeOfDay >= tempMorningStart && currenttime.TimeOfDay < tempMorningEnd)
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(MORNING)[WEEKDAYRATE]);
                }
                //If it is not the morning rate, it is default the evening rate
                else
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(EVENING)[WEEKDAYRATE]);
                }
                break;
            case "WEEKEND":
                //These temp TimeSpan objects hold the start and end time for morning and evening for a particlar state.
                TimeSpan tempMorningStartWE = Convert.ToDateTime(bothRates.ElementAt(MORNING)[WEEKENDSTARTTIME]).TimeOfDay;
                TimeSpan tempMorningEndWE = Convert.ToDateTime(bothRates.ElementAt(MORNING)[WEEKENDENDTIME]).TimeOfDay;
                TimeSpan tempEveningStartWE = Convert.ToDateTime(bothRates.ElementAt(EVENING)[WEEKENDSTARTTIME]).TimeOfDay;
                TimeSpan tempEveningEndWE = Convert.ToDateTime(bothRates.ElementAt(EVENING)[WEEKENDENDTIME]).TimeOfDay;

                //If the time is between the morning section find the rate. (inclusive of the morning startTime)
                if (currenttime.TimeOfDay >= tempMorningStartWE && currenttime.TimeOfDay < tempMorningEndWE)
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(MORNING)[WEEKENDRATE]);
                }
                //If the time is between the evening section (inclsuive of evening starttime)
                else
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(EVENING)[WEEKENDRATE]);
                }
                break;
            case "PUBLIC HOLIDAY":
                //These temp TimeSpan objects hold the start and end time for morning and evening for a particlar state.
                TimeSpan tempMorningStartPH = Convert.ToDateTime(bothRates.ElementAt(MORNING)[PUBLICHOLIDAYSTARTTIME]).TimeOfDay;
                TimeSpan tempMorningEndPH = Convert.ToDateTime(bothRates.ElementAt(MORNING)[PUBLICHOLIDAYENDTIME]).TimeOfDay;
                TimeSpan tempEveningStartPH = Convert.ToDateTime(bothRates.ElementAt(EVENING)[PUBLICHOLIDAYSTARTTIME]).TimeOfDay;
                TimeSpan tempEveningEndPH = Convert.ToDateTime(bothRates.ElementAt(EVENING)[PUBLICHOLIDAYENDTIME]).TimeOfDay;

                //If the time is between the morning section find the rate. (inclusive of the morning startTime)
                if (currenttime.TimeOfDay >= tempMorningStartPH && currenttime.TimeOfDay < tempMorningEndPH)
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(MORNING)[PUBLICHOLIDAYRATE]);
                }
                //If the time is between the evening section (inclsuive of evening starttime)
                else 
                {
                    rate = Convert.ToInt32(bothRates.ElementAt(EVENING)[PUBLICHOLIDAYRATE]);
                }
                break;
        }
        //return the rate found
        return rate;
    }

    /// <summary>
    /// Check if the DateTime object is on the weekend
    /// </summary>
    /// <param name="currentTime">Current date in a DateTime object</param>
    /// <returns></returns>
    private static bool CheckWeekend(DateTime currentTime)
    {
        if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
        {
            return true;
        }
        return false;
    }
}