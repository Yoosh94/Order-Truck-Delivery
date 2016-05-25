using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Payment class holds all the information for a particular instance of a payment.
/// </summary>
public class Payment
{
    #region declare Variables
    public const int TASMANIAFERRYFEE = 200;
    const decimal INSURANCEMULTIPLIER = 0.1M;
    const int HOURSTILLBREAK = 6;

    private string _firstName;
    public string FirstName
    {
        get { return _firstName; }
    }
    private string _lastName;
    public string LastName
    {
        get { return _lastName; }
    }
    private string _originPostCode;
    private string _destinationPostCode;
    private string _originAddress;
    public string OriginAddress
    {
        get { return _originAddress; }
    }
    private string _destinationAddress;
    public string DestinationAddress
    {
        get { return _destinationAddress; }
    }
    private string _deliveryDate;
    public string DeliveryDate
    {
        get { return _deliveryDate; }
    }
    private string _deliveryTime;
    public string DeliveryTime
    {
        get { return _deliveryTime; }
    }
    private string _weight;
    private string _distance;
    private string _duration;
    private bool _goingInterstate;
    private bool _goingToTasmania;
    public bool GoingToTasmania
    {
        get { return _goingToTasmania; }
    }
    private decimal _deliveryFee;
    public decimal DeliveryFee
    {
        get { return _deliveryFee; }
    }
    private decimal _insuranceCharge;
    public decimal InsuranceCharge
    {
        get { return _insuranceCharge; }
    }
    private string _billingAddress;
    public string BillingAddress
    {
        get { return _billingAddress; }
    }
    private int _numberOfTrucks;
    public int NumberOfTrucks
    {
        get { return _numberOfTrucks; }
    }
    private DateTime _deliveryDateTime;
    private DateTime _deliveryStartDateTime;
    public DateTime DeliveryStartDateTime
    {
        get { return _deliveryStartDateTime; }
    }
    private DateTime _currentDateTime;
    private TimeSpan _totalDuration;
    private double _driverSalary;
    public double DriverSalary
    {
        get { return _driverSalary; }
    }
    private decimal finalFee = 0;
    public decimal FinalFee
    {
        get { return finalFee; }
    }
    private decimal _gst;
    public decimal GST
    {
        get { return _gst; }
    }
    private int _ferryRate = 0;
    public int FerryRate
    {
        get { return _ferryRate; }
    }
    #endregion

    /// <summary>
    /// Class contructor
    /// </summary>
    /// <param name="originPostCode">The Origin Postcode from where the delivery will begin</param>
    /// <param name="destinationPostCode">The destination postcode where the delivery will end</param>
    /// <param name="deliveryDate">The date the delivery is planned to arrive</param>
    /// <param name="deliveryTime">The time the delivery is planned to arrive</param>
    /// <param name="weight">The approx weight of the deliver</param>
    /// <param name="distance">The distance the truck(s) have to travel to get from the origin to the destination</param>
    /// <param name="duration">The total duration the truck(s) have to travel to get from the origin to the destionation</param>
    public Payment(string originPostCode, string destinationPostCode,string deliveryDate,string deliveryTime,string weight,string distance,string duration,string billingAddress,string originAddress,string destinationAddress,string firstName,string lastName)
    {
        _originPostCode = originPostCode;
        _destinationPostCode = destinationPostCode;
        _deliveryDate = deliveryDate;
        _deliveryTime = deliveryTime;
        _weight = weight;
        _distance = distance;
        _duration = duration;
        _billingAddress = billingAddress;
        _originAddress = originAddress;
        _destinationAddress = destinationAddress;
        _firstName = firstName;
        _lastName = lastName;
    }

    /// <summary>
    /// Get all the data by applying business rules
    /// </summary>
    public void getAllData()
    {
        //Check if the trucks have to go interstate
        _goingInterstate = PostCode.oPostCodeVSdPostCode(_originPostCode, _destinationPostCode);
        //Check if the trucks have to go to tasmania
        _goingToTasmania = PostCode.checkIfTasmania(_originPostCode, _destinationPostCode);
        //If the trucks go to Tasmania, add a fee of $200
        if (_goingToTasmania)
        {
            _ferryRate = TASMANIAFERRYFEE;
        }
        //Calculate the number of trucks required by using the weight
        _numberOfTrucks = calculateNumberOfTrucks();
        //Calculate delivery Fee. This will only be added if the trucks are going interstate
        _deliveryFee = calculateDeliveryFee();
        //Calculate the insurance for the total amount of trucks
        _insuranceCharge = calculateInsuranceCharge();
        //Create a Delivery DateTIme object by combing the Date and Time string given by the user
        _deliveryDateTime = calculateDeliveryTimeDateTimeObject();
        //Calculate the start date taking into consideration break times by the truck drivers
        _deliveryStartDateTime = calculateStartTime(_deliveryDateTime);
        //Calculate all the prices
        calculateDriverPay();
    }

    /// <summary>
    /// Calculates the final prices for the delivery
    /// </summary>
    /// <returns>The final price as a decimal</returns>
    public decimal calculateFinalFee()
    {
        //If the truck is going interstate we add the delivery Fee
        if (_goingInterstate)
        {
            finalFee = finalFee + _deliveryFee;
        }
        //If the truck is going to Tasmania and using the Ferry and charge per truck is applied
        if (_goingToTasmania)
        {
            finalFee = finalFee + (TASMANIAFERRYFEE * _numberOfTrucks);
        }
        //add insurance charge
        finalFee = finalFee + _insuranceCharge;
        //add the drivers salary
        finalFee = finalFee + Convert.ToDecimal(_driverSalary);
        //calculate and add the GST
        _gst = finalFee / 10;
        finalFee = finalFee + _gst;
        //return the Final Price
        return finalFee;
    }

    /// <summary>
    /// Checks how many states the trucks will have to drive though and calculate an average time per truck per state
    /// Then calculates the pay rate for the truck drivers
    /// </summary>
    private void calculateDriverPay()
    {
        //Create a list and init
        List<string> statesInBetween = new List<string>();
        //Get the origin and destination postcodes
        string[] states = PostCode.findStates(_originPostCode,_destinationPostCode);
        // Get the number of states the trucks need to drive though
        int numberOfStates = PostCode.CalculateNumberOfStates(states);
        // Get the average distance the trucks will travel through each state
        double distanceForEachState = ((Convert.ToDouble(_distance))/1000)/ numberOfStates;
        //Create a TimeSpan object for the durtaion for each state.
        TimeSpan durationPerState = TimeSpan.FromTicks(_totalDuration.Ticks / numberOfStates);
        double hourPerState = durationPerState.TotalHours;
        //If the truck is going intersate find all the states that will come in between.
        if (numberOfStates > 1)
        {
            statesInBetween = PostCode.findStatesInBetween(states);
        }

        //Create a list with all the states that are involved
        List<string> allStatesInvoled = new List<string>();
        allStatesInvoled.Add(states[0]);
        foreach(string state in statesInBetween)
        {
            allStatesInvoled.Add(state);
        }
        allStatesInvoled.Add(states[1]);
        //If the first and last ones are the same remove the last one. This will only occur if the travel is not interstate.
        if(allStatesInvoled[0] == allStatesInvoled[allStatesInvoled.Count-1])
        {
            allStatesInvoled.RemoveAt(allStatesInvoled.Count-1);
        }
        //check the start time and match it to the origin state using the HourlyRates class.
        //Check if the durationPerstate will still make it fall between the same time frame.
        long referenceTicks = _deliveryStartDateTime.Ticks;
        DateTime referenceDateTime = new DateTime(referenceTicks);
        //init a variable which checks how many hours the trucks have travelled
        int numberOfHoursTravelled = 0;
        //For every state the truck is passing through
        foreach(string currentState in allStatesInvoled)
        {  
            //For each hour in the current state 
            for (int i = 0; i < hourPerState; i++)
            {
                //Since break times have been added in the start time
                //here we see if they have travelled 6 hours and then we skip 1 hour of pay.
                if (numberOfHoursTravelled == HOURSTILLBREAK)
                {
                    numberOfHoursTravelled = 0;
                    referenceDateTime = referenceDateTime.AddHours(1);               
                }
                else
                {
                    //Calculate if the Reference date and time(hour) is a weekend/WeekDay/Public Holiday and return the rate per hour
                    int ratePerHour = HourlyRates2.getHourlyRate(referenceDateTime, currentState);
                    _driverSalary = _driverSalary + ratePerHour;
                    //Add one hour to the reference DateTime
                    referenceDateTime = referenceDateTime.AddHours(1);
                    numberOfHoursTravelled++;
                }      
            }    
        }
        //Multiple the salary with the number of trucks being sent
        _driverSalary = _driverSalary * _numberOfTrucks;
    }

    /// <summary>
    /// Calculates the start time of the Journey
    /// </summary>
    /// <param name="endTime">The end time specified by the user.</param>
    /// <returns>DateTime object with the start Time</returns>
    private DateTime calculateStartTime(DateTime endTime)
    {
        //Calculate duration (currently in seconds)
        //If driver has driving more than or equal to 6 hours he has to have 1 hour break/sleep

        //Total duration of the trip calcualted by google API
        _totalDuration = TimeSpan.FromSeconds(Convert.ToDouble(_duration));
        //Add a certain amount of hours depending on how long the travel is. 
        int hoursToAdd = _totalDuration.Hours / HOURSTILLBREAK;
        //Add the amount of hours to the total duration
        TimeSpan tempHours = TimeSpan.FromHours(Convert.ToDouble(hoursToAdd));
        TimeSpan totalDurationWithRests = tempHours + _totalDuration;
        //Find the end time by subtracting the end time by the total duration
        long timeTaken = (endTime.Ticks - totalDurationWithRests.Ticks);
        DateTime startTime = new DateTime(timeTaken);
        _currentDateTime = new DateTime(timeTaken);
        //return the dateTime object
        return startTime;
    }

    /// <summary>
    /// Converts the delivery Date string and delivery time string to a DateTime OBject
    /// </summary>
    /// <returns></returns>
    private DateTime calculateDeliveryTimeDateTimeObject()
    {
        //Create a temporary Date time object which has the correct date but incorrect time
        DateTime oldDeliveryDate = Convert.ToDateTime(_deliveryDate);
        //Parse the time string to create a TimeSpan Object
        TimeSpan ts = TimeSpan.Parse(_deliveryTime);
        //Add the time span object to the old DateTime variable which will create a new Object.
        DateTime newDateTime = oldDeliveryDate.Add(ts);
        return newDateTime;
    }

    //This will calcualte the number of trucks required
    private int calculateNumberOfTrucks()
    {
        decimal weight = Convert.ToDecimal(_weight);
        return  Convert.ToInt32(Math.Ceiling((weight / 5000)));

    }

    //This will calculate the insurance for all the trucks in total
    private decimal calculateInsuranceCharge()
    {
        return (((Convert.ToDecimal(_distance)/1000) * INSURANCEMULTIPLIER)*_numberOfTrucks);
    }

    //This will calculate the price for delivering Interstate
    private decimal calculateDeliveryFee()
    {
        //This should only be use if the bus is not going interstate  
        return DeliveryClassification.getPrice(_numberOfTrucks);
    }
    
}