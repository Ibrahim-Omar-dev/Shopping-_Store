$(document).ready(function () {
    if ($('#myTable').length) {
        loadDataTable();
    }
});

function loadDataTable() {
    var dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/admin/product/getall"
        },
        "columns": [
            { "data": "title", "width": "25%" },
            { "data": "isbn", "width": "15%" },
            { "data": "author", "width": "10%" },
            { "data": "price", "width": "10%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id"
                , "render": function (data) {
                    return `<div class="text-center">
                                <a href="/admin/product/upsert?id=${data}" class='btn btn-primary mx-2 text-white' style='cursor:pointer; width:100px;'>
                                    <i class="bi bi-pencil"></i> Edit
                                </a>
                                <a onclick="Delete('/admin/product/delete/${data}')" class='btn btn-danger text-white' style='cursor:pointer; width:100px;'>
                                   <i class="bi bi-trash"></i> Delete
                                 </a>

                            </div>`;
                }
                , "width": "25%"
            }

        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        Swal.fire(
                            'Deleted!',
                            data.message,
                            'success'
                        );
                        $('#myTable').DataTable().ajax.reload(); // if using DataTable
                    } else {
                        Swal.fire(
                            'Error!',
                            data.message,
                            'error'
                        );
                    }
                }
            });
        }
    });
}
