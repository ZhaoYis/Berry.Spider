$(function () {
    var l = abp.localization.getResource('Admin');

    var dataTable = $('#ServNodesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(berry.spider.biz.servMachine.getList),
            columnDefs: [
                {
                    title: l('SMI:MachineCode'),
                    data: "machineCode"
                },
                {
                    title: l('SMI:MachineName'),
                    data: "machineName"
                },
                {
                    title: l('SMI:MachineIpAddr'),
                    data: "machineIpAddr"
                },
                {
                    title: l('SMI:GroupCode'),
                    data: "groupCode"
                },
                {
                    title: l('SMI:Status'),
                    data: "status",
                    render: function (data) {
                        return l('Enum:SMI:MachineStatus.' + data);
                    }
                },
                {
                    title: l('SMI:LastOnlineTime'),
                    data: "lastOnlineTime",
                    render: function (data) {
                        return luxon.DateTime.fromISO(data, {
                            locale: abp.localization.currentCulture.name
                        }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                }
            ]
        })
    );
});
