$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url:'/admin/product/getall'},
        "columns": [
            { data: 'nameOfProduct', "width": "15%"},
            { data: 'displayProductNr', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            { data: 'kindOfProduct.typeOfProduct', "width": "15%" },
            { data: 'description', "width": "25%" },
            {
                data: 'id',
                "render": function(data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pen-fill"></i>Edituj</a>
                    <a class="btn btn-danger mx-2"> <i class="bi bi-trash">Smaž</i></a>                    
                    </div>`
                },
                "width": "20%"
            }
        ]
    });
}
