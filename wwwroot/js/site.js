// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function loadDetail(id) {
    fetch('/AdminEcatalog/Detail/' + id)
        .then(res => res.text())
        .then(html => {
            document.getElementById('modalContent').innerHTML = html;
        });
}
