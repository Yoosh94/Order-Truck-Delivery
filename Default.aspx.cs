using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;



public partial class _Default : System.Web.UI.Page
{

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //This needs to be in Pre-Render because these need to be loaded before the pages loads, otherwise the page will throw an error
        //saying the rangeValidator is missing attributes
        rngDate.MinimumValue = DateTime.Now.Date.ToString("dd-MM-yyyy");
        rngDate.MaximumValue = DateTime.Now.Date.AddYears(90).ToString("dd-MM-yyyy");
        rngDate.ErrorMessage = "Please pick a day after " + rngDate.MinimumValue.ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Clean all the data just in case there is something in it.
        publicHolidays.cleanData();
        PostCode.cleanData();
        DeliveryClassification.cleanData();
        //Run a function which will get all the data from a csv. 
        publicHolidays.importPublicHolidayCSV();
        DeliveryClassification.importDeliveryClassificationCSV();
        HourlyRates2.createList();
        PostCode.importPostCodeCSV();
    }

    /// <summary>
    /// When the User clicks the submit button on the UI.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void submitButton_Click(object sender, EventArgs e)
    {
        string billingUserAddress = "";
        //Get all the information from the UI and save it in assigned variables
        string firstName = ((TextBox)form1.FindControl(firstNameTextBox.ClientID)).Text;
        string lastName = ((TextBox)form1.FindControl(lastNameTextBox.ClientID)).Text;
        string oPostCode = ((TextBox)form1.FindControl(originPostCodeTexBox.ClientID)).Text;
        string dPostCode = ((TextBox)form1.FindControl(destinationPostCodeTextBox.ClientID)).Text;
        string deliveryDate = ((TextBox)form1.FindControl(datepicker.ClientID)).Text;
        string deliveryTime = ((TextBox)form1.FindControl(timepicker.ClientID)).Text;
        string weight = ((TextBox)form1.FindControl(deliveryWeightTextBox.ClientID)).Text;
        string originUserAddress = ((TextBox)form1.FindControl(originAddressTextBox.ClientID)).Text + " " + oPostCode;
        string destinationUserAddress = ((TextBox)form1.FindControl(destinationAddressTextBox.ClientID)).Text + " " + dPostCode;
        //If checkbox is checked, delivery address will become the billing address
        if (billingCheckBox.Checked)
        {
             billingUserAddress = ((TextBox)form1.FindControl(destinationAddressTextBox.ClientID)).Text + " " + dPostCode;
        }
        else
        {
            billingUserAddress = ((TextBox)form1.FindControl(billingAddressTextBox.ClientID)).Text + " " + dPostCode;

        }
        //Get Distance and Duration from Google API
        string[] googleMapsData = GetDrivingInfo(originUserAddress, destinationUserAddress,deliveryTime);
        //Create a Payment Object only if Google Maps was able to calculate a distance.
        if(googleMapsData != null)
        {
            Payment payment = new Payment(oPostCode, dPostCode, deliveryDate, deliveryTime, weight, googleMapsData[0], googleMapsData[1], billingUserAddress, originUserAddress, destinationUserAddress, firstName, lastName);
            //Calculate all the neccassary data
            payment.getAllData();
            //Calculate the final fee
            payment.calculateFinalFee();
            //Create a pdf recipt which holds all the relevant information
            createPDF(payment);
        }
        else
        {
            //Display an error
            errorLbl.Text = "Unable to find location. Please enter correct addresses.";
            errorLbl.ControlStyle.ForeColor = System.Drawing.Color.Red;
        }
             
    }

    /// <summary>
    /// When the user clicks the checkbox to indicate the delivery address is the same as the billing address
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Unnamed_CheckedChanged(object sender, EventArgs e)
    {
        //if the checkbox is checked hide the billing address fields.
        if (((CheckBox)sender).Checked)
        {
            billingAddressLbl.Visible = false;
        }
        
    }

    //http://www.webthingsconsidered.com/2013/08/09/adventures-in-json-parsing-with-c/
    /// <summary>
    /// A web request is made to the google API with parameters ( origin, destination,arrival time) and the response is parsed from a JSON object to a string[]
    /// </summary>
    /// <param name="origin">Origin address specified by the user</param>
    /// <param name="destination">Destination address specified by the user</param>
    /// <param name="endTime">Delivery time specified by the user</param>
    /// <returns></returns>
    public static string[] GetDrivingInfo(string origin, string destination,string endTime)
    {
        //Initialise local variables
        string distanceInMeters = null;
        string durationInSeconds = null;

        //Calculate the delivery time in seconds to parse into the web request
        double seconds = getArrivalTimeInSeconds(endTime);
        string s = Convert.ToString(seconds);

        //Pass request to google api with orgin and destination details
        HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create("http://maps.googleapis.com/maps/api/distancematrix/json?origins="
            + origin + " Australia&destinations=" + destination
            + " Australia&mode=Car&language=us-en&sensor=false&components=country:AUS&arrival_time=" + s);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (var streamReader = new StreamReader(response.GetResponseStream()))
        {
            var Json = streamReader.ReadToEnd();
            // If a response was given and destination was found
            if (!string.IsNullOrEmpty(Json))
            {
                // Parse JSON into dynamic object
                JObject results = JObject.Parse(Json);

                // Process each route
                foreach (var result in results["rows"])
                {
                    foreach (var c in result["elements"])
                    {
                        JToken status = c["status"];
                        
                        if(status.ToString() == "OK") {
                            JToken duration = c["duration"];
                            JToken distance = c["distance"];
                            durationInSeconds = (string)(duration).Last;
                            distanceInMeters = (string)(distance).Last;
                        }
                        else
                        {
                            return null;
                        }
                        
                    }
                }
            }
            else
            {
                //return null;
            }
        }
        //save data into an arry and return it
        string[] values = new string[2] { distanceInMeters, durationInSeconds };
        return values;
    }

    /// <summary>
    /// Convert a date (as a string) to the amount of seconds
    /// </summary>
    /// <param name="time">this is a time as a string</param>
    /// <returns></returns>
    private static double getArrivalTimeInSeconds(string time)
    {
        //create a UTC time which is used by google API
        DateTime utc = new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc);

        //Create the time provided by the user
        DateTime endTime = DateTime.Parse(time);

        //Get the time difference between the two times
        TimeSpan diff = endTime.Subtract(utc);

        //Calculate and return the seconds
        double seconds = diff.TotalSeconds;
        return seconds;

    }

    /// <summary>
    /// Once all the calculations are complete, create a PDF will all the information
    /// </summary>
    /// <param name="payment">Payment Object which holds all the data</param>
    private void createPDF(Payment payment)
    {
        //Get current Date and time to add as a time stamp for the recipt
        DateTime nowTime = DateTime.Now;
        string dateForInvoiceName = nowTime.ToString("d-MMM HHmm tt");
        //Create the Document object that will be modified
        var doc1 = new Document(PageSize.A4,50,50,25,25);
        HTMLWorker htmlWorker = new HTMLWorker(doc1);
        var output = new MemoryStream();
        PdfWriter.GetInstance(doc1, new FileStream(HttpContext.Current.Server.MapPath("~/App_Data/") + "Invoice "+dateForInvoiceName+".pdf",FileMode.Create));
        //var writer = PdfWriter.GetInstance(doc1, output);
        doc1.Open();

        var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
        var subTitleFone = FontFactory.GetFont("Arial", 14, Font.BOLD);
        var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
        var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
        var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);

        string contents = File.ReadAllText(Server.MapPath("~/HTMLTemplates/HtmlPage.html"));
        contents = contents.Replace("[ORDERID]", doc1.ID.ToString().ToUpper());
        contents = contents.Replace("[TOTALPRICE]", payment.FinalFee.ToString("c")); 
        contents = contents.Replace("[ORDERDATE]", nowTime.ToString("d-MMM yyyy HH:mm tt"));
        contents = contents.Replace("[DELIVERYDATE]", payment.DeliveryDate +" " + payment.DeliveryTime);
        contents = contents.Replace("[INSURANCEFEE]", payment.InsuranceCharge.ToString("c"));
        contents = contents.Replace("[DELIVERYFEE]", payment.DeliveryFee.ToString("c"));
        contents = contents.Replace("[FERRYFEE]", payment.FerryRate.ToString("c"));
        contents = contents.Replace("[DRIVERFEE]", payment.DriverSalary.ToString("c"));
        contents = contents.Replace("[GST]", payment.GST.ToString("c"));
        contents = contents.Replace("[DELIVERYADDRESS]", payment.DestinationAddress.ToUpper());
        contents = contents.Replace("[ORIGINADDRESS]", payment.OriginAddress.ToUpper());
        contents = contents.Replace("[FIRSTNAME]", payment.FirstName.ToUpper());
        contents = contents.Replace("[LASTNAME]", payment.LastName.ToUpper());
        contents = contents.Replace("[BILLINGADDRESS]", payment.BillingAddress.ToUpper());
        contents = contents.Replace("[NUMBEROFTRUCKS]",payment.NumberOfTrucks.ToString());
        contents = contents.Replace("[ORIGINSTARTDATE]", payment.DeliveryStartDateTime.ToString("d-MMM yyyy HH: mm tt"));

        var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(contents), null);
        foreach(var htmlElements in parsedHtmlElements)
        {
            doc1.Add(htmlElements as IElement);
        }
        doc1.Close();
        System.Diagnostics.Process.Start(HttpContext.Current.Server.MapPath("~/App_Data/Invoice "+dateForInvoiceName+".pdf"));    
    }

    /// <summary>
    /// If checked hide the billing address as it will be the same as the Delivery Address
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void billingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (billingCheckBox.Checked)
        {
            billingAddressLbl.Visible = false;
            billingAddressTextBox.Visible = false;
        }
        else
        {
            billingAddressLbl.Visible = true;
            billingAddressTextBox.Visible = true;
        }
    }
}