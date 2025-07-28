//$(document).ready(function () {
//    if ($('#myTable').length) {
//        loadDataTable();
//    }
//});

//function loadDataTable() {
//    var dataTable = $('#myTable').DataTable({
//        "ajax": {
//            "url": "/admin/user/getall", // ✅ تأكد من هذا المسار
//            "type": "GET",
//            "datatype": "json"
//        },
//        "columns": [
//            { "data": "name", "width": "15%" },
//            { "data": "email", "width": "15%" },
//            { "data": "phoneNumber", "width": "15%" },
//            { "data": "company.name", "width": "15%" },
//            { "data": "role", "width": "15%" },
//            {
//                "data": "id",
//                "render": function (data) {
//                    return `
//                        <div class="text-center">
//                            <a href="/admin/user/upsert?id=${data}" class='btn btn-primary mx-2 text-white' style='cursor:pointer; width:100px;'>
//                                <i class="bi bi-pencil"></i> Edit
//                            </a>
//                            <a onclick="Delete('/admin/user/delete/${data}')" class='btn btn-danger text-white' style='cursor:pointer; width:100px;'>
//                                <i class="bi bi-trash-fill"></i> Delete
//                            </a>
//                        </div>`;
//                },
//                "width": "25%"
//            }
//        ]
//    });
//}
