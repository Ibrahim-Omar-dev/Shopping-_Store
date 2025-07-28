$(document).ready(function () {
    $('#myTable').DataTable({
        ajax: {
            url: '/Admin/Company/GetAll', // Adjust controller route if needed
            dataSrc: 'data'
        },
        columns: [
            { data: 'name' },
            { data: 'streetAddress' },
            { data: 'city' },
            { data: 'state' },
            { data: 'postalCode' },
            { data: 'phoneNumber' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <a href="/Admin/Company/UpSert/${data}" class="btn btn-sm btn-primary">Edit</a>
                        <a onclick=Delete('/Admin/Company/Delete/${data}') class="btn btn-sm btn-danger">Delete</a>
                    `;
                },
                orderable: false
            }
        ]
    });
});
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'This action cannot be undone!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        $('#myTable').DataTable().ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
