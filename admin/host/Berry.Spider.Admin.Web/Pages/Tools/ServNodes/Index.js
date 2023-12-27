$(function () {
    var l = abp.localization.getResource('Admin');
    var deployAppNodeModal = new abp.ModalManager({
        viewUrl: '/Tools/ServNodes/DeployAppNodeModal'
    });
    deployAppNodeModal.onResult(function () {
        abp.log.debug(arguments)
        abp.log.debug("代理节点" + arguments[1].responseText.currentAgentBizNo + "部署成功");
    });

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
                    title: l('SMI:Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('SMI:DeployAppNode'),
                                visible: abp.auth.isGranted('Admin.Tools.ServNodes.DeployAppNode'),
                                iconClass: "fas fa-building",
                                action: function (data) {
                                    deployAppNodeModal.open({
                                        agentBizNo: data.record.bizNo
                                    });
                                }
                            },
                            {
                                text: l('SMI:RestartAllAppNode'),
                                visible: abp.auth.isGranted('Admin.Tools.ServNodes.RestartAllAppNode'),
                                confirmMessage: function (data) {
                                    return "Are you sure to restart all node?" + data.record.machineCode;
                                },
                                iconClass: "fas fa-trash-restore",
                                action: function (data) {
                                    berry.spider.biz.servMachine
                                        .restartAllAppNode(data.record.bizNo)
                                        .then(function () {
                                            abp.notify.info("Successfully restarted!");
                                            data.table.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
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
                        if (data === 10) {
                            return '<i class="fa fa-dot-circle text-success">' + l('Enum:SMI:MachineStatus.' + data) + '</i>';
                        } else if (data === 20) {
                            return '<i class="fa fa-dot-circle-o text-danger">' + l('Enum:SMI:MachineStatus.' + data) + '</i>';
                        }
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
