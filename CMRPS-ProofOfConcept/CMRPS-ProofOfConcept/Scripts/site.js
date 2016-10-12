
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
function Shutdown_WMI() {
    $('#shutdown-result').html("Sending...");
    $('#shutdown-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/ShutdownWMI",
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

function Shutdown_CMD() {
    $('#shutdown-result').html("Sending...");
    $('#shutdown-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/ShutdownCMD",
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

function Shutdown_Cancel() {
    $('#shutdown-result').html("Sending...");
    $('#shutdown-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/ShutdownCancel",
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
function Wol_Packet() {
    $('#wol-result').html("Sending...");
    $('#wol-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/WolPacket",
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

function Wol_Winwake() {
    $('#wol-result').html("Sending...");
    $('#wol-result').attr("class", 'ping-result-style-unknown');
    $.ajax({
        url: "/Home/WolCMD",
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

function FillComputer(name) {
    if (name == 'cb12') {
        $("#computer-name").val("cb12")
        $("#computer-mac").val("00:25:64:51:AB:A3")
    }
    if (name == 'lv1') {
        $("#computer-name").val("lv1")
        $("#computer-mac").val("D4:BE:D9:6A:5D:0F")
    }
}