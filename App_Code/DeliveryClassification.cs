using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// This static class returns how much it will cost to use a certain amount of trucks
/// </summary>
public static class DeliveryClassification
{
    private const string NUMBEROFTRUCKS = "numberOfTrucks";
    private const string DELIVERYFEE = "deliveryFee";

    //Dictionary holding the column as the Keys and then rest of the column as a list<string>
    private static Dictionary<string, List<string>> deliveryClassificationData = new Dictionary<string, List<string>>();
    //A list of the header names just used for reference
    private static List<string> colHeaderNames = new List<string>();

    /// <summary>
    /// Add a new element in the master dictionary where the KEY will be the parameter passed by the user.
    /// </summary>
    /// <param name="colName">Name of Column from CSV</param>
    private static void createDictionary(string colName)
    {
        deliveryClassificationData.Add(colName, new List<string>());
    }

    /// <summary>
    /// Importing all the data from the csv file
    /// </summary>
    public static void importDeliveryClassificationCSV()
    {
        //Line count will be used to see how many rows we have processed
        int lineCount = 0;
        int indexCount = 0;
        //Open the file reader and file stream
        FileStream fs = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/deliveryClassification.csv"));
        StreamReader sr = new StreamReader(fs);
        string text;
        while ((text = sr.ReadLine()) != null)
        {
            //break string up into seperate words possibly in array
            string[] colNames = text.Split(',');
            //If are reading the first Line which is a header
            if (lineCount == 0)
            {
                foreach (string name in colNames)
                {
                    //add the name of the column to the master dictionary
                    createDictionary(name);
                    //Add the names of the header to a seperate List just for referencing.
                    colHeaderNames.Add(name);
                }
            }
            //If the first line has already been read
            else
            {
                foreach (string name in colNames)
                {
                    string colheader = colHeaderNames[indexCount];
                    deliveryClassificationData[colheader].Add(name);
                    //Since index count is used to count the number of columns we are moving across
                    //once it reaches the edge we need to tell it to reset back to zero to count the next
                    //line.
                    if (indexCount >= deliveryClassificationData.Count - 1)
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
    /// depending on the number of trucks being sent will change the delivery fee.
    /// </summary>
    /// <param name="numberOfTrucks">Number of trucks being sent</param>
    /// <returns>Price for the number of trucks</returns>
    public static decimal getPrice(int numberOfTrucks)
    {
        //Find which index numberOfTrucks belongs too in the list
        int indexOfNumberOfTruckHeader = colHeaderNames.FindIndex(x => x.Contains(NUMBEROFTRUCKS));
        int indexOfDeliveryFeeHeader = colHeaderNames.FindIndex(x => x.Contains(DELIVERYFEE));
        //get the key with the header index
        string keyTrucks = deliveryClassificationData.Keys.ElementAt(indexOfNumberOfTruckHeader);
        string keyDeliveryFee = deliveryClassificationData.Keys.ElementAt(indexOfDeliveryFeeHeader);
        List<string> tempNumberOfTrucks = deliveryClassificationData[keyTrucks];
        List<List<int>> newTempNumberOfTrucks = new List<List<int>>();
        //For each string in the list see what the value is
        foreach(string s in tempNumberOfTrucks)
        {
            int number;
            //If the string is a number add it to the list
            bool pass = int.TryParse(s, out number);
            if (pass)
            {
                newTempNumberOfTrucks.Add(new List<int> { number } );
            }
            //If the string has other characters in it, eliminate them and create a list of the numbers
            else
            {
                List<int> tempNumbers = new List<int>();
                foreach (char c in s)
                {
                    //If the first character is a number add it to the list
                    int numbers;
                    bool passs = int.TryParse(c.ToString(), out numbers);
                    if (passs)
                    {
                        tempNumbers.Add(numbers);
                    }
                }
                for(int i = tempNumbers[0]+1;i< tempNumbers[1]; i++)
                {
                    tempNumbers.Add(i);
                }
                newTempNumberOfTrucks.Add(tempNumbers);
            }
        }
        int indexOfDeliveryFee = newTempNumberOfTrucks.FindIndex(c => c.Contains(numberOfTrucks));
        string price = deliveryClassificationData[keyDeliveryFee][indexOfDeliveryFee];
        return int.Parse(price);
    }

    /// <summary>
    /// Ensures that the data that is loaded in is the latest data.
    /// </summary>
    public static void cleanData()
    {
        if (deliveryClassificationData.Count > 0)
        {
            deliveryClassificationData.Clear();
        }

    }
}