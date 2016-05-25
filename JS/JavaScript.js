//Global variables
//var SECRET_API_KEY = "AIzaSyDCwFjPXUsxtEPKCbrVCLYDnSkMS56KWQs";
//If user clicks the check box which states "Is billing address the same as destination address", this function
//will hide the billing address field and just use the destination address as the billing address. Also if the
//user unclicks the check box the field will appear again.
function hideBillingAddress() {
    var checkBox = document.getElementById("billingCheckBox");
    var billing = document.getElementById("billingAddressLbl");
    var billingTxtBox = document.getElementById("billingAddressTextBox");
    if (checkBox.checked) {
        billing.className = 'hidden';
        billingTxtBox.className = 'hidden';
    }
    else {
        billing.className = '';
        billingTxtBox.className = '';
    }

}

//function checkAddresses(origin,originPC,destination,destinationPC) {

//    //if both the fields have data is it 
//    if (origin.value == "" || destination.value == "" || originPC.value == "" || destinationPC.value == "") {
//        //do nothing
//    }
//    else {
//        //USE JS googleAP https://developers.google.com/maps/documentation/javascript/distancematrix#display_of_a_google_map
//        var InitialOrigin = origin.value + " " + originPC.value;
//        var finalOrigin = destination.value + " " + destinationPC.value;
//        //origin.value = origin.value +" "+ originPC.value;
//        //destination = destination.value +" " + destinationPC.value;
//        createAPIrequest(InitialOrigin, finalOrigin);
//    }

//}
//function createAPIrequest(origin,destination) {
//    var service = new google.maps.DistanceMatrixService();
//    service.getDistanceMatrix(
//      {
//          origins: [origin],
//          destinations: [destination],
//          travelMode: google.maps.TravelMode.DRIVING,
          
//          //transitOptions: TransitOptions,
//          //drivingOptions: [trafficModel: google.maps.TrafficModel.BEST_GUESS],,
//          //unitSystem: UnitSystem,
//          //avoidHighways: Boolean,
//          //avoidTolls: Boolean,
//      }, checkAPI);
//}

//function checkAPI(response, status) {
//    if (status == google.maps.DistanceMatrixStatus.OK) {
//        console.log("success");
        
//        var origins = response.originAddresses;
//        var destinations = response.destinationAddresses;

//        for (var i = 0; i < origins.length; i++) {
//            var results = response.rows[i].elements;
//            for (var j = 0; j < results.length; j++) {
//                var element = results[j];
//                var distance = element.distance.value;
//                var duration = element.duration.value;
//                var from = origins[i];
//                var to = destinations[j];
//            }
//        }
//    }
//    //This function is a script on the main page.
//    addDistance(distance, duration);
//    //loadJsonData(distance, duration);
//}

//function loadJsonData(distance,duration) {
//    var MapMatrix = JSON.stringify(
//        {
//            "Distance": distance,
//            "Duration": duration
//        });
//    try {
//        $.ajax({
//            type: "POST",
//            url: "localhost",
//            cache: false,
//            data: MapMatrix,
//            dataType: "json",
//            success: getSuccess,
//            error: getFail
//        });
//    } catch (e) {
//        alert(e);
//    }
//    function getSuccess(data, textStatus, jqXHR) {
//        alert(data.Response);
//    };
//    function getFail(jqXHR, textStatus, errorThrown) {
//        alert(jqXHR.status);
//    };
//};

//// Create the XHR object.
//function createCORSRequest(method, url) {
//    var xhr = new XMLHttpRequest();
//    if ("withCredentials" in xhr) {
//        // XHR for Chrome/Firefox/Opera/Safari.
//        console.log("chrome");
//        xhr.open(method, url, true);
//    } else if (typeof XDomainRequest != "undefined") {
//        // XDomainRequest for IE.
//        console.log("IE");
//        xhr = new XDomainRequest();
//        xhr.open(method, url);
//    } else {
//        console.log("NA");
//        // CORS not supported.
//        xhr = null;
//    }
    
//    return xhr;
//}

//function createGoogleMapAPIrequest(origin, destination) {
    
//    var xhr = createCORSRequest('GET',"https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=driving&language=en-EN&key=" + SECRET_API_KEY);
//    xhr.setRequestHeader("Access-Control-Allow-Origin", "https://maps.google.com.au");
//    xhr.setRequestHeader("Content-Type", "application/json");
//    var headers = xhr.getAllResponseHeaders().toLowerCase();
//    console.log(headers);
    
//    if (!xhr) {
//        alert('CORS not supported');
//        return;
//    }
//    xhr.onload = function () {
//        var jsonObt = JSON.parse(xhr.responseText);
//        console.log("success");
//        console.log(jsonObt);
//    };
//    xhr.onerror = function () {
//        alert('Woops, there was an error making the request.');
//    };
//    xhr.onloadstart = function () {
//        console.log("onloadstart");
//    };
//    xhr.onloadend = function () {
//        console.log("onloadend");
//    }
//    xhr.onabort = function () {
//        console.log("onabort");
//    }
//    xhr.onprogress = function () {
//        console.log("onprogress");
//    }
//    xhr.send();
//}
    //var xhttp = new XMLHttpRequest();
    //xhttp.setRequestHeader
    //xhttp.onreadystatechange = function () {
    //    if (xhttp.readyState == 4 && xhttp.status == 200) {
    //        var jsonOjt = JSON.parse(xhttp.responseText);
    //    }
    //};
    
    //xhttp.open("GET", "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=driving&language=en-EN&key=" + SECRET_API_KEY, true);
    //xhttp.send();
window.addEventListener('load', function () {
    if (document.getElementById('map-canvas')) {
        google.load("maps", "3", {
            callback: function () {
                new google.maps.Map(document.getElementById('map-canvas'), {
                    center: new google.maps.LatLng(0, 0),
                    zoom: 3
                });
            }
        });
    }
}, false);