
// Ping
function DemoPing() {
    $.ajax({
        url: "/Home/Ping",
        data: { 'name': $('#computer-name').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            $('#ping-result').html("ping: " + data);
            if (data != -1) {
                $("#ping-result").attr('class', 'ping-result-style-online');
            } else {
                $("#ping-result").attr('class', 'ping-result-style-offline');
            }
        },
        error: function () {
            $('#ping-result').html("ERROR!");
            $("#ping-result").attr('class', 'ping-result-style-offline');
        }
    });
};

// Shutdown
function DemoShutdown() {
    $.ajax({
        url: "/Home/Shutdown",
        data: { 'name': $('#computer-name').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            if (data == "True") {
                $('#shutdown-result').html("SENT!");
                $("#shutdown-result").attr('class', 'ping-result-style-online');
            } else {
                $('#shutdown-result').html("NOT SENT!!");
                $("#shutdown-result").attr('class', 'ping-result-style-offline');
            }
        },
        error: function () {
            $('#shutdown-result').html("ERROR!");
            $("#shutdown-result").attr('class', 'ping-result-style-offline');
        }
    });
};

// Wake On Lan
function DemoWol() {
    $.ajax({
        url: "/Home/Wol",
        data: { 'mac': $('#computer-mac').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            if (data == "True") {
                $('#wol-result').html("SENT!");
                $("#wol-result").attr('class', 'ping-result-style-online');
            } else {
                $('#wol-result').html("NOT SENT!!");
                $("#wol-result").attr('class', 'ping-result-style-offline');
            }
        },
        error: function () {
            $('#wol-result').html("ERROR!");
            $("#wol-result").attr('class', 'ping-result-style-offline');
        }
    });
};
