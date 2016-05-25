using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// This static public holiday class holds all the information regarding the public holidays that are held in Australia
/// </summary>
public static class publicHolidays
{
    //Dictionary where the key is the column headers and the list is all the colomn data.
    private static Dictionary<string, List<string>> publicHolidayData = new Dictionary<string, List<string>>();
    //A list which holds all the column Headers.
    private static List<string> colHeaderNames = new List<string>();

    /// <summary>
    /// Ensures that the data that is loaded in is the latest data.
    /// </summary>
    public static void cleanData()
    {
        if (publicHolidayData.Count > 0)
        {
            publicHolidayData.Clear();
        }

    }

    /// <summary>
    /// Read a csv file and save all the data
    /// </summary>
    public static void importPublicHolidayCSV()
    {
        //Line count will be used to see how many rows we have processed
        int lineCount = 0;
        int indexCount = 0;
        //Open a file stream to read all the data. 
        FileStream fs = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/publicHolidays.csv"));
        StreamReader sr = new StreamReader(fs);
        string text;
        while ((text = sr.ReadLine()) != null)
        {
            //break string up into seperate words possibly in array
            string[] colNames = text.Split(',');
            //If are reading the first Line which is a header
            if(lineCount == 0)
            {
                foreach (string name in colNames)
                {
                    //add the name of the column to the master dictionary
                    createDictionary(name);
                    //Add the names of the header to a seperate List just for referencing.
                    if (colHeaderNames.Contains(name))
                    {

                    }
                    else
                    {
                        colHeaderNames.Add(name);
                    }
                    
                }
            }
            //If the first line has already been read.
            else
            {
                //for each word in the string array which holds all the row data.
                foreach(string name in colNames)
                {
                    string colheader = colHeaderNames[indexCount];
                    publicHolidayData[colheader].Add(name);
                    //Since index count is used to count the number of columns we are moving across
                    //once it reaches the edge we need to tell it to reset back to zero to count the next
                    //line.
                    if (indexCount >= publicHolidayData.Count - 1)
                    {
                        indexCount = 0;
                    }
                    else
                    {
                        indexCount++;
                    }
                }
            } 
            //increase the line count. This will increment to specifiy which row we are working on with index 0.         
            lineCount++;
        }
        sr.Close();
    }

    /// <summary>
    /// Add an entry the the master dictionary. The key is the parameter string being passed to the function
    /// </summary>
    /// <param name="colName">Name of Column from CSV</param>
    private static void createDictionary(string colName)
    {
        publicHolidayData.Add(colName, new List<string>());
    }

    /// <summary>
    /// Checks if a data has a public holiday in a specific date
    /// </summary>
    /// <param name="state">Name of the State which needs to be checked for Public Holiday</param>
    /// <param name="start">The date which needs to be checked for Public Holiday</param>
    /// <returns></returns>
    public static bool isItAPublicHoliday(string state, DateTime start)
    {
        //Initialise varialbe as false
        bool isItAPublicHoliday = false;
        //Get the index of the header list where the header equals "Data"
        int indexOfHolidayDateHeader = colHeaderNames.FindIndex(x => x.Contains("date"));
        //Get the index of the header list where the header equals the state
        int indexOfHolidayStateHeader = colHeaderNames.FindIndex(x => x.Contains(state));
        //This will get me the key for all the dates and all the data for the particular state.
        string keyDate = publicHolidayData.Keys.ElementAt(indexOfHolidayDateHeader);
        string keyState = publicHolidayData.Keys.ElementAt(indexOfHolidayStateHeader);
        //Create a temp list of all the dates and state data.
        List<string> tempDate = publicHolidayData[keyDate];
        List<string> tempState = publicHolidayData[keyState];

        //If the date exists
        if (tempDate.Contains(start.ToString("dd-MMM")))
        {
            //Check if the state celebrates the public holiday
            int indexOfPHdate = tempDate.FindIndex(c => c == start.ToString("dd-MMM"));
            string holiday = tempState[indexOfPHdate];
            //If state does celebrate the public holiday assign the variable
            if(holiday == "YES")
            {
                isItAPublicHoliday = true;
            }
        }
        //return the variable. The variable will not have changed after initilising it if the date was not found
        return isItAPublicHoliday;
    }
}