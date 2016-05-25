using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


/// <summary>
/// Summary description for all the postcodes
/// </summary>
public static class PostCode
{
    //Dictionary holding the column as the Keys and then rest of the column as a list<string>
    private static Dictionary<string, List<string>> postCodeData = new Dictionary<string, List<string>>();
    //A list of the header names just used for reference
    private static List<string> colHeaderNames = new List<string>();

    /// <summary>
    /// Ensures that the data that is loaded in is the latest data.
    /// </summary>
    public static void cleanData()
    {
        postCodeData.Clear();
    }

    #region importing CSV
    /// <summary>
    /// Add a new element in the master dictionary where the KEY will be the parameter passed by the user.
    /// </summary>
    /// <param name="colName">Name of Column from CSV</param>
    private static void createDictionary(string colName)
    {
        postCodeData.Add(colName, new List<string>());
    }

    /// <summary>
    /// Importing all the data from the csv file
    /// </summary>
    public static void importPostCodeCSV()
    {
        //Line count will be used to see how many rows we have processed
        int lineCount = 0;
        int indexCount = 0;
        //Open the file reader and file stream
        FileStream fs = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/postCodes.csv"));
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
                    postCodeData[colheader].Add(name);
                    //Since index count is used to count the number of columns we are moving across
                    //once it reaches the edge we need to tell it to reset back to zero to count the next
                    //line.
                    if (indexCount >= postCodeData.Count - 1)
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
    /// Check if the truck will have to travel to Tasmania. Since Tasmania will not be part of the middle states, 
    /// only the origin and destination post code need to be checked to see if they are part of Tasmania
    /// </summary>
    /// <param name="origin">Origin Address PostCode</param>
    /// <param name="destination">Destination Postcode</param>
    /// <returns></returns>
    public static bool checkIfTasmania(string origin, string destination)
    {
        //Init variables
        string OriginState = "";
        string DestinationState = "";
        bool goingToTas = false;


        //if "Pcode" exists in the header reference file
        bool hasPC = colHeaderNames.Any(pc => pc == "Pcode");
        bool hasState = colHeaderNames.Any(s => s == "State");

        if (hasPC && hasState)
        {
            //Find which index postcode belongs too in the list
            int indexOfPCHeader = colHeaderNames.FindIndex(x => x.Contains("code"));
            //Get the Key value for the particlar Header
            string keyPC = postCodeData.Keys.ElementAt(indexOfPCHeader);
            //Create a temp list with all the data from the key
            List<string> tempPC = postCodeData[keyPC];
            int indexOfOriginPC = tempPC.FindIndex(c => c == origin);
            int indexOfDestinationPC = tempPC.FindIndex(c => c == destination);
            //Now check if this matches the state headers.
            if (indexOfDestinationPC >= 0 && indexOfOriginPC >= 0)
            {
                //Find which index state belongs too in the list
                int indexOfStateHeader = colHeaderNames.FindIndex(x => x.Contains("State"));
                //This will get me the key that I am looking for
                string keyState = postCodeData.Keys.ElementAt(indexOfStateHeader);
                List<string> tempState = postCodeData[keyState];
                //Now i need to check if two states are the same
                OriginState = tempState[indexOfOriginPC];
                DestinationState = tempState[indexOfDestinationPC];
            }
            else
            {
                //If the postcodes given did not match give an error.
            }
        }
        else
        {
            //Display Error that "postCode Header could not be found"
        }
        //if the origin in the same and not empty
        if (OriginState == "TAS" || DestinationState == "TAS")
        {
            goingToTas = true;
        }
        //return the variable
        return goingToTas;
    }

    /// <summary>
    /// Check if the trucks will have to travel interstate by checking the PostCodes
    /// </summary>
    /// <param name="origin">Origin Postcode</param>
    /// <param name="destination">Destination Postcode</param>
    /// <returns>A boolean. True if interstate travel is required and false if it is not required.</returns>
    public static bool oPostCodeVSdPostCode(string origin, string destination)
    {
        string OriginState = "";
        string DestinationState = "";
        bool goingInterstate;

        //if "Pcode" exists in the header file
        bool hasPC = colHeaderNames.Any(pc => pc == "Pcode");
        bool hasState = colHeaderNames.Any(s => s == "State");

        if (hasPC && hasState)
        {
            //Find which index postcode belongs too in the list
            int indexOfPCHeader = colHeaderNames.FindIndex(x => x.Contains("code"));
            //This will get me the key that I am looking for
            string keyPC = postCodeData.Keys.ElementAt(indexOfPCHeader);
            List<string> tempPC = postCodeData[keyPC];
            int indexOfOriginPC = tempPC.FindIndex(c => c == origin);
            int indexOfDestinationPC = tempPC.FindIndex(c => c == destination);
            //Now check if this matches the state headers.
            if (indexOfDestinationPC >= 0 && indexOfOriginPC >= 0)
            {
                //Find which index state belongs too in the list
                int indexOfStateHeader = colHeaderNames.FindIndex(x => x.Contains("State"));
                //This will get me the key that I am looking for
                string keyState = postCodeData.Keys.ElementAt(indexOfStateHeader);
                List<string> tempState = postCodeData[keyState];
                //Now i need to check if two states are the same
                OriginState = tempState[indexOfOriginPC];
                DestinationState = tempState[indexOfDestinationPC];
            }
            else
            {
                //If the postcodes given did not match give an error.
            }
        }
        else
        {
            //Display Error that "postCode Header could not be found"
        }

        //if the origin in the same and not empty
        if (OriginState == DestinationState && OriginState != "")
        {
            goingInterstate = false;
        }
        else
        {
            goingInterstate = true;
        }
        //return variable
        return goingInterstate;
    }

    #endregion

    /// <summary>
    /// Find the states where origin and destination fall on.
    /// </summary>
    /// <param name="origin">Origin postcode</param>
    /// <param name="destination">Destination Postcode</param>
    /// <returns>An array of states where the truck will travel</returns>
    public static string[] findStates(string origin, string destination)
    {
        string OriginState = "";
        string DestinationState = "";
        //Find which index postcode belongs too in the list
        int indexOfPCHeader = colHeaderNames.FindIndex(x => x.Contains("code"));
        //This will get me the key that I am looking for
        string keyPC = postCodeData.Keys.ElementAt(indexOfPCHeader);
        List<string> tempPC = postCodeData[keyPC];
        int indexOfOriginPC = tempPC.FindIndex(c => c == origin);
        int indexOfDestinationPC = tempPC.FindIndex(c => c == destination);
        //Now check if this matches the state headers.
        if (indexOfDestinationPC >= 0 && indexOfOriginPC >= 0)
        {
            //Find which index state belongs too in the list
            int indexOfStateHeader = colHeaderNames.FindIndex(x => x.Contains("State"));
            //This will get me the key that I am looking for
            string keyState = postCodeData.Keys.ElementAt(indexOfStateHeader);
            List<string> tempState = postCodeData[keyState];
            //Now i need to check if two states are the same
            OriginState = tempState[indexOfOriginPC];
            DestinationState = tempState[indexOfDestinationPC];
        }
        else
        {
            //If the postcodes given did not match give an error.

        }
        string[] states = new string[2] { OriginState, DestinationState };
        return states;
    }

    /// <summary>
    /// Calulates the number of states the truck will have to travel through to get from origin to destinatino
    /// </summary>
    /// <param name="theStates">Array which holds the origin and destination state as a string</param>
    /// <returns>Number of States the truck will have to pass</returns>
    public static int CalculateNumberOfStates(string[] theStates)
    {
        int numberOfStates = -1;
        //Check the Origin State
        switch (theStates[0])
        {
            //If origin is in VIC
            case "VIC":
                switch (theStates[1])
                {
                    case "VIC":
                        numberOfStates = 1;
                        break;
                    case "TAS":
                    case "NSW":
                    case "ACT":
                    case "SA":
                        numberOfStates = 2;
                        break;
                    case "QLD":
                    case "NT":
                    case "WA":
                        numberOfStates= 3;
                        break;
                }
                break;
            //If origin is in TAS
            case "TAS":
                switch (theStates[1])
                {
                    case "TAS":
                        numberOfStates = 1;
                        break;
                    case "VIC":
                        numberOfStates= 2;
                        break;                
                    case "NSW":
                    case "ACT":
                    case "SA":
                        numberOfStates = 3;
                        break;
                    case "QLD":
                    case "NT":
                    case "WA":
                        numberOfStates = 4;
                        break;
                }
                break;
            //If Origin is NSW or ACT
            case "NSW":
            case "ACT":
                switch (theStates[1])
                {
                    case "NSW":
                    case "ACT":
                        numberOfStates = 1;
                        break;
                    case "VIC":
                    case "QLD":
                    case "SA":
                        numberOfStates = 2;
                        break;
                    case "TAS":
                    case "NT":
                    case "WA":
                        numberOfStates = 3;
                        break; 
                }
                break;
            //If origin is in QueensLand
            case "QLD":
                switch (theStates[1])
                {
                    case "QLD":
                        numberOfStates = 1;
                        break;
                    case "NT":
                    case "NSW":
                    case "ACT":
                        numberOfStates = 2;
                        break;
                    case "VIC":
                    case "SA":
                    case "WA":
                        numberOfStates = 3;
                        break;
                    case "TAS":
                        numberOfStates = 4;
                        break;
                }
                break;

            //if origin is SA
            case "SA":
                switch (theStates[1])
                {
                    case "SA":
                        numberOfStates = 1;
                        break;
                    case "NT":
                    case "NSW":
                    case "ACT":
                    case "VIC":
                    case "WA":
                        numberOfStates = 2;
                        break;
                    case "QLD":
                    case "TAS":
                        numberOfStates = 3;
                        break;
                }
                break;

            //If origin is in NT
            case "NT":
                switch (theStates[1])
                {
                    case "NT":
                        numberOfStates = 1;
                        break;
                    case "QLD":
                    case "SA":
                    case "WA":
                        numberOfStates = 2;
                        break;
                    case "NSW":
                    case "ACT":
                    case "VIC":
                        numberOfStates = 3;
                        break;
                    case "TAS":
                        numberOfStates = 4; ;
                        break;
                }
                break;

            //if origin is in wA
            case "WA":
                switch (theStates[1])
                {
                    case "WA":
                        numberOfStates = 1;
                        break;
                    case "NT":
                    case "SA":
                        numberOfStates = 2;
                        break;
                    case "NSW":
                    case "ACT":
                    case "VIC":
                    case "QLD":
                        numberOfStates = 3;
                        break;
                    case "TAS":
                        numberOfStates = 4;
                        break;
                }
                break;
        }
        return numberOfStates;
    }

    /// <summary>
    /// Find the states that fall between the origin and destination state.
    /// </summary>
    /// <param name="theStates">Array of origin and destination state</param>
    /// <returns>A list of other states that fall between the origin and destination</returns>
    public static List<string> findStatesInBetween(string[] theStates)
    {
        //Init a list
        List<string> betweenStates = new List<string>();
        //theStates[0] is the origin state
        switch (theStates[0])
        {
            //If origin is in VIC
            case "VIC":
                switch (theStates[1])
                {
                    case "QLD":
                        betweenStates.Add("NSW");
                        break;
                    case "NT":
                    case "WA":
                        betweenStates.Add("SA");
                        break;
                }
                break;
            //If origin is in TAS
            case "TAS":
                switch (theStates[1])
                {
                    case "NSW":
                    case "ACT":
                    case "SA":
                        betweenStates.Add("VIC");
                        break;
                    case "QLD":
                        betweenStates.Add("VIC");
                        betweenStates.Add("NSW");
                        break;
                    case "NT":
                        betweenStates.Add("VIC");
                        betweenStates.Add("SA");
                        break;
                    case "WA":
                        betweenStates.Add("VIC");
                        betweenStates.Add("SA");
                        break;
                }
                break;
            //If Origin is NSW or ACT
            case "NSW":
            case "ACT":
                switch (theStates[1])
                {
                    case "TAS":
                        betweenStates.Add("VIC");
                        break;
                    case "NT":
                    case "WA":
                        betweenStates.Add("SA");
                        break;
                }
                break;
            //If origin is in QueensLand
            case "QLD":
                switch (theStates[1])
                {

                    case "VIC":
                    case "SA":
                        betweenStates.Add("NSW");
                        break;
                    
                    case "WA":
                        betweenStates.Add("NT");
                        break;
                    case "TAS":
                        betweenStates.Add("NSW");
                        betweenStates.Add("VIC");
                        break;
                }
                break;

            //if origin is SA
            case "SA":
                switch (theStates[1])
                {
                    case "QLD":
                        betweenStates.Add("NSW");
                        break;
                    case "TAS":
                        betweenStates.Add("VIC");
                        break;
                }
                break;

            //If origin is in NT
            case "NT":
                switch (theStates[1])
                {
                    case "NSW":
                    case "ACT":
                        betweenStates.Add("SA");
                        break;
                    case "VIC":
                        
                        betweenStates.Add("SA");
                        break;
                    case "TAS":
                        betweenStates.Add("SA");
                        betweenStates.Add("VIC");
                        break;
                }
                break;

            //if origin is in wA
            case "WA":
                switch (theStates[1])
                {

                    case "NSW":
                    case "ACT":
                    case "VIC":
                        betweenStates.Add("SA");
                        break; 
                    case "QLD":
                        betweenStates.Add("NT");

                        break;
                    case "TAS":
                        betweenStates.Add("SA");
                        betweenStates.Add("VIC");
                        break;
                }
                break;
        }
        return betweenStates;
    }
}