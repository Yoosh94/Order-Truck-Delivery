using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using TopicPublisher;
using TopicSubscriber;



/// <summary>
/// Summary description for DeliveryBookingProcessor
/// </summary>
public static class DeliveryBookingProcessor
{
    private static string XMLDOCPATH = HttpContext.Current.Server.MapPath("~/test-xml.xml");
    private static IConnection connection;
    private static IConnectionFactory connectionFactory;
    private static ISession session;
    const string TOPIC_NAME_SENDING_ACTION = "sendingAction";
    private const string BROKER = "tcp://localhost:61616";
    const string CLIENT_ID = "ass3.ClientID";
    private static Subscriber subscriber;
    const string CONSUMER_ID = "ass3.subscriber";

    public static void createXML()
    {
        
        //Create an xml document which passes actions to complete
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("Action");
        xmlDoc.AppendChild(rootNode);

        //Create A search action
        XmlNode searchNode = xmlDoc.CreateElement("Search");
        rootNode.AppendChild(searchNode);

        //Create a category which will represent the column in a database
        XmlNode categoryNode = xmlDoc.CreateElement("Category");
        XmlNode valueNode = xmlDoc.CreateElement("Value");
        searchNode.AppendChild(categoryNode);
        searchNode.AppendChild(valueNode);

        //Create a add action
        //XmlNode addNode = xmlDoc.CreateElement("Add");
        //rootNode.AppendChild(addNode);
        
        xmlDoc.Save(XMLDOCPATH);
        
    }

    public static void SearchFor(string thingToSearchFor, string valueToSearchFor)
    {
        XmlDocument xmlDoc = openXMLDoc();
        XmlNode searchNode = xmlDoc.SelectSingleNode("//Action/Search");
        searchNode["Category"].InnerText = thingToSearchFor;
        searchNode["Value"].InnerText = valueToSearchFor;
        xmlDoc.Save(XMLDOCPATH);
    }

    /// <summary>
    /// Opens a XmlDocument and converts it to a string
    /// </summary>
    /// <returns>String representation of Xml Document</returns>
    public static string ConvertToString()
    {
        return openXMLDoc().OuterXml;
    }

    /// <summary>
    /// Open a new Instance of XmlDocument and return it
    /// </summary>
    /// <returns>XmlDocument</returns>
    private static XmlDocument openXMLDoc()
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(XMLDOCPATH);
        return xDoc;
    }

    public static void sendFromDeliveryToCRM(string messageToSend)
    {
        //Create a new conneciton object, specifying who the message broker is and supplying our identifier
        connectionFactory = new ConnectionFactory(BROKER, CLIENT_ID);
        connection = connectionFactory.CreateConnection();
        connection.Start();

        //Create a new session on our newly setup connection
        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

        using (var publisher = new Publisher(session, TOPIC_NAME_SENDING_ACTION))
        {
            publisher.SendMessage(messageToSend);
        }
    }
}