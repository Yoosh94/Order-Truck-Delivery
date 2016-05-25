<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Theme="Theme1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
     
<head runat="server">
    <title>CRM System</title>
</head>
<body>
    <div id="title"> 
           <h1>CRM</h1>
    </div>
    <form id="form1" runat="server">
        <div id="mainDiv" class="mainDiv">
        
            <div id="Origin">
                <div id="originInfo">
                <br />
                <asp:label ID="firstNamelbl" Text="*First Name:        " runat="server"/>

                <asp:TextBox ID="firstNameTextBox" runat="server" CssClass="texboxInput" />
                <asp:RegularExpressionValidator runat="server" id="regexFirstName" controltovalidate="firstNameTextBox" display="Dynamic" validationexpression="^[a-zA-Z''-'\s]{1,20}$" errormessage="Incorrect Name" />
                <asp:RequiredFieldValidator runat="server" id="reqName" controltovalidate="firstNameTextBox" display="Dynamic" errormessage="Please enter your name!" />
                <br />
                <br />

                <asp:label ID="lastNamelbl" Text="*Last Name: " runat="server"/>
                <asp:TextBox ID="lastNameTextBox" runat="server" CssClass="texboxInput"/>
                <asp:RegularExpressionValidator runat="server" id="regexLastName" controltovalidate="lastNameTextBox" display="Dynamic" validationexpression="^[a-zA-Z''-'\s]{1,20}$" errormessage="Please enter your Last name" />
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" controltovalidate="lastNameTextBox" display="Dynamic" errormessage="Please enter your name!" />
                <br />
                <br />
                <asp:label ID="originAddress" Text="*Origin Address:        " runat="server"/>
                <asp:TextBox ID="originAddressTextBox"  runat="server" CssClass="texboxInput"/>
                <asp:RequiredFieldValidator runat="server" id="originAddressValidation" controltovalidate="originAddressTextBox" display="Dynamic" errormessage="This field can't be empty." />
                 <br />   
                <br />
        
                <asp:Label ID="originPostCodeLbl" runat="server" Text="*Origin Postcode:   " />
                <asp:TextBox ID="originPostCodeTexBox" runat="server" CssClass="texboxInput"/>
                <asp:RegularExpressionValidator runat="server" id="OriginRegexPostCode" display="Dynamic" controltovalidate="originPostCodeTexBox" validationexpression="^(\d{3,4})$" errormessage="Incorrect Postcode" />
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator2" display="Dynamic" controltovalidate="originPostCodeTexBox" errormessage="Please enter the Postcode" /> 
                <br />
                <br />
             </div>
        </div>
        
        <div id="destination">
            <div id="destinationInfo">
                <br />
                <asp:label ID="destinationAddress" Text="*Destination Address: " runat="server"/>
                <asp:TextBox ID="destinationAddressTextBox" runat="server" CssClass="texboxInput"/>
                <asp:RequiredFieldValidator runat="server" id="destinationAddressValidation" controltovalidate="destinationAddressTextBox" display="Dynamic" errormessage="This field can't be empty." />
                <br />
                <br />
                <asp:Label ID="destinationPostCodeLbl" runat="server" Text="*Destination Postcode: " />
                <asp:TextBox ID="destinationPostCodeTextBox" runat="server" CssClass="texboxInput"/>
                <asp:RegularExpressionValidator runat="server" id="destinationRegexPostCode" display="Dynamic" controltovalidate="destinationPostCodeTextBox" validationexpression="^(\d{3,4})$" errormessage="Incorrect Postcode" />
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator3" display="Dynamic" controltovalidate="destinationPostCodeTextBox" errormessage="Please enter the Postcode" />
                <br />
                <br />
                <!-- have a check box to ask if the billing address is the same as the destination address. If the value of the checkbox is true we should hide that bit. -->
                <asp:CheckBox runat="server" name="sameAddress" ID="billingCheckBox" OnCheckedChanged="billingCheckBox_CheckedChanged" AutoPostBack="true"/>Billing Address is the same as Destination Address
                <br />
                <asp:label ID="billingAddressLbl" Text="Billing Address: " runat="server"/>
                <asp:TextBox ID="billingAddressTextBox" runat="server" CssClass="texboxInput"/>
                <br />
                <br />
                <asp:Label ID="phoneNumberLbl" runat="server" Text="*Phone Number: " />
                <asp:TextBox ID="phoneNumberTextBox" runat="server" CssClass="texboxInput"/>
<%--        http://regexlib.com/REDetails.aspx?regexp_id=1787&AspxAutoDetectCookieSupport=1--%>
                <asp:RegularExpressionValidator runat="server" ID="regexPhoneNumber" display="Dynamic" ControlToValidate="phoneNumberTextBox" ValidationExpression="^\({0,1}((0|\+61)(2|4|3|7|8)){0,1}\){0,1}(\ |-){0,1}[0-9]{2}(\ |-){0,1}[0-9]{2}(\ |-){0,1}[0-9]{1}(\ |-){0,1}[0-9]{3}$" errormessage="Incorrect" />
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator4" display="Dynamic" controltovalidate="phoneNumberTextBox" errormessage="Please enter your name!" />
                <br />
                <br />
                <asp:Label ID="deliveryDataLbl" runat="server" Text="*Delivery Date: " />
                <asp:TextBox ID="datepicker" runat="server" CssClass="texboxInput"/>
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator5" display="Dynamic" controltovalidate="datepicker" errormessage="Please Enter a Delivery Date" />
                <asp:RangeValidator runat="server" id="rngDate" controltovalidate="datepicker" type="Date" />
                <br />
                <br />
                <asp:Label ID="deliveryTimeLbl" runat="server" Text="*Delivery Time: " />
                <asp:TextBox ID="timepicker" runat="server" CssClass="texboxInput"/>
                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator6" display="Dynamic" controltovalidate="timepicker" errormessage="Please Enter a Delivery Time" />
                <br />
                <br />
           
        <asp:Label ID="deliveryWeightLbl" Text="*Delivery Weight (Kg) :" runat="server"/>
        <asp:TextBox ID="deliveryWeightTextBox" runat="server" CssClass="texboxInput" />
        <asp:RegularExpressionValidator runat="server" ID="regexWeight" display="Dynamic" ControlToValidate="deliveryWeightTextBox" ValidationExpression="^\d+$" ErrorMessage="Numeric numbers only (0-9)" />
        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator7" display="Dynamic" controltovalidate="deliveryWeightTextBox" errormessage="Please enter a weight" />
                <asp:RangeValidator runat="server" ID="weightVal" Display="Dynamic" Type="Integer" ControlToValidate="deliveryWeightTextBox" MinimumValue="1000" MaximumValue="50000" ErrorMessage="Weight must be between 1000 - 50000 Kgs"/>
        <br /><br />
                <asp:Label runat="server" Text="(*) Denotes a required Field." id="errorLbl"/>
         </div>
        </div>
        <asp:Button ID="submitButton" runat="server" OnClick="submitButton_Click" Text="Submit"/>
        <br />
    </div>
        
    </form>
    <div id="map-canvas"></div>
</body>
    <script src="JS/jquery.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.11.4/i18n/jquery-ui-i18n.js" type="text/jquery"></script>
    <script src="https://www.google.com/jsapi?.js"></script>
    <link rel="stylesheet" type="text/css" href="CSS/jquery-ui.css" />
    <link rel="stylesheet" type="text/css" href="CSS/jquery.timepicker.css"/>
    <script src="JS/jquery-ui.js"></script>
    <script src="JS/jquery.timepicker.js"></script>
    <script type="text/javascript" src="<%= ResolveUrl ("JS/jqueryFile.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl ("JS/javascript.js") %>"></script>
</html>


<%--Get all the CSV files are create a dictionary. The KEY will be the coloumn name and the value will be a list of all the values--%>