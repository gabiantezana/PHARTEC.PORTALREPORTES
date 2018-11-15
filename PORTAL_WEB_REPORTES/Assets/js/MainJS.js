
function notificacion(tipo, texto) {
    if (texto) {
        toastr.options = {
            closeButton: true,
            progressBar: true
        };
        tipo == 0 ? toastr.success(texto) : toastr.error(texto);
    }
}