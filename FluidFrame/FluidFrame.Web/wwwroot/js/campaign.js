var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess")
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed")
        }
        else {
            if (url.includes("approved")) {
                loadDataTable("approved")
            }
            else {
                loadDataTable("all")
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Campaign/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "15%" },
            { "data": "phone", "width": "15%" },
            { "data": "applicationUser.email", "width": "25%" },
            { "data": "campaignStatus", "width": "10%" },
            { "data": "campaignTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Campaign/Details?campaignId=${data}"
                            class="btn btn-warning mx-2"> <i class="bi bi-pencil-square"></i></a>
                        </div>
                        `
                },
                "width": "10%"
            },
        ]
    });
}