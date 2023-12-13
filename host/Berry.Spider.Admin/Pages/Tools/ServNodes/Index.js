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
                    title: l('MachineCode'),
                    data: "machineCode"
                },
                {
                    title: l('MachineName'),
                    data: "machineName"
                },
                {
                    title: l('MachineIpAddr'),
                    data: "machineIpAddr"
                },
                {
                    title: l('MachineMacAddr'),
                    data: "machineMacAddr"
                },
                {
                    title: l('Status'),
                    data: "status"
                },
                {
                    title: l('LastOnlineTime'), data: "lastOnlineTime",
                    render: function (data) {
                        return luxon
                            .DateTime
                            .fromISO(data, {
                                locale: abp.localization.currentCulture.name
                            }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                }
            ]
        })
    );
});
