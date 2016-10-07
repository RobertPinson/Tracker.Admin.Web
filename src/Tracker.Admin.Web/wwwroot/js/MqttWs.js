
if (typeof Tracker === "undefined") {
    Tracker = {};
}

Tracker.MqttWsClient = (function () {

    var client = new Paho.MQTT.Client("m21.cloudmqtt.com", 35480, "admin");

    client.onConnectionLost = onConnectionLost;
    client.onMessageArrived = onMessageArrived;

    var id;
    var personUrl;
    var Init = function (locationId, getPersonUrl) {
        console.log("init id: " + locationId + " url:" + getPersonUrl);

        personUrl = getPersonUrl;
        id = locationId;
        client.connect({
            useSSL: true,
            userName: "regcjdre",
            password: "sjOPZR3T1ehQ",
            onSuccess: onConnect,
            onFailure: onFailure
        });
    };

    var getPerson = function (cardId, callBack) {
        $.ajax({
            type: 'GET',
            url: personUrl,
            data: { cardId: cardId },
            dataType: 'json',
            cache: false,
            success: function (response) {
                callBack(response);
            },
            error: function(response) {
                alert(response);
            }
        });
    };

    function onConnect() {
        console.log("onConnect");
        client.subscribe("location/" + id + "/movement");
    }

    function onFailure() {
        console.log("onFailure");
    }

    // called when the client loses its connection
    function onConnectionLost(responseObject) {
        if (responseObject.errorCode !== 0) {
            console.log("onConnectionLost:" + responseObject.errorMessage);
            setTimeout(function () { client.connect() }, 5000);
        }
    }

    // called when a message arrives
    function onMessageArrived(message) {
        console.log("onMessageArrived:" + message.payloadString);

        var data;
        try {
            data = JSON.parse(message.payloadString);
        } catch (e) {

            console.log("Error parsing JSON MQTT payload : " + e.message);

            return false;
        }

        if (data.InLocation === 0) {
            console.log("Not in location removing from list card Id: " + data.CardId);
            //remove from list
            $("#" + data.CardId).remove();

            return true;
        }

        //Get person to add to list
        getPerson(data.CardId, function(person) {

            console.log("Get Person Call back");
            console.log("Swipe time: " + data.SwipeTime);

            var swipeDate = new Date(data.SwipeTime + 'UTC');

            console.log("Local time: " + swipeDate.toString());

            var formatedDate = ("0" + swipeDate.getDate()).slice(-2) +
                "/" +
                ("0" + swipeDate.getMonth()).slice(-2) +
                "/" +
                swipeDate.getFullYear() +
                " " +
                swipeDate.getHours() +
                ":" +
                swipeDate.getMinutes() +
                ":" +
                ("0" + swipeDate.getSeconds()).slice(-2);

            var $newRow = $('<tr id="' + data.CardId + '">  ' +
                    '<td><img src="/Person/GetAvatar?id='+ person.id +'"alt="Person Image" class="img-circle" width="70" height="70" /></td>'+
                    '<td><span>'+ person.fullName +'</span></td>'+
                    '<td><span>' + formatedDate + '</span></td>' +
                '</tr>').hide();

            $("#people-table").append($newRow);
            $newRow.fadeIn(2000);

        });
        return true;
    }

    return { Init: Init };
})();