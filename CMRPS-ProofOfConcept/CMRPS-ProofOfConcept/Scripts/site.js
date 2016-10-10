
// Ping
function DemoPing() {
    $('#ping-result').html("Pinging...");
    $('#ping-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/Ping",
        data: { 'name': $('#computer-name').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            
            if (data !== -1) {
                $('#ping-result').html("ping: " + data);
                $("#ping-result").attr('class', 'ping-result-style-online');
            } else {
                $('#ping-result').html("UNREACHABLE!");
                $("#ping-result").attr('class', 'ping-result-style-offline');
            }
        },
        error: function () {
            $('#ping-result').html("ERROR!");
            $("#ping-result").attr('class', 'ping-result-style-offline');
        }
    });
}

// Shutdown
function DemoShutdown() {
    $('#shutdown-result').html("Sending...");
    $('#shutdown-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/Shutdown",
        data: { 'name': $('#computer-name').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            if (data === "True") {
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
}

// Wake On Lan
function DemoWol() {
    $('#wol-result').html("Sending...");
    $('#wol-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/Wol",
        data: { 'mac': $('#computer-mac').val() },
        datatype: "text",
        type: "POST",
        success: function (data) {
            if (data === "True") {
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
}
