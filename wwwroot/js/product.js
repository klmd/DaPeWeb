var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'nameOfProduct', "width": "10%" },
            { data: 'displayProductNr', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            { data: 'kindOfProduct.typeOfProduct', "width": "10%" },
            { data: 'description', "width": "25%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-outline-primary mx-2"> <i class="bi bi-pen-fill"></i>Edituj</a>
                    <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash"></i>Smaž</a>
                     </div>`
                },
                "width": "35%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Určitě?',
        text: "Tato akce nelze vrátit!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Ano, smaž záznam!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function(data) {
                    dataTable.ajax.reload();
                    toaster.SUCCESS(data.message);
                }
            });
        }
    });
}