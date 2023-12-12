$(function () {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/signalr-hubs/spider/app-notify")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveMessageAsync", function (res) {
        $('#MessageList').append('<li><strong><i class="fas fa-long-arrow-alt-right"></i> ' + res.message + '</strong></li>');
    });

    connection.on("ReceiveSystemMessageAsync", function (res) {
        $('#MessageList').append('<li><strong>系统消息：<i class="fas fa-long-arrow-alt-right"></i> ' + res.message + '</strong></li>');
    });

    connection.start().then(function () {
        console.log("conn to signalR..")
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $('#SendMessageButton').click(function (e) {
        e.preventDefault();

        var targetUserName = $('#TargetUser').val();
        var message = $('#Message').val();
        $('#Message').val('');

        var push = {
            Code: 1000,//发送系统消息 --> NotifyMessageCode.PUSH_SYSTEM_MESSAGE
            Data: message,
            Message: message
        };
        connection.invoke("SendToAllAsync", push)
            .then(function () {
                $('#MessageList')
                    .append('<li><i class="fas fa-long-arrow-alt-left"></i> ' + abp.currentUser.userName + ': ' + message + '</li>');
            })
            .catch(function (err) {
                return console.error(err.toString());
            });
    });
});