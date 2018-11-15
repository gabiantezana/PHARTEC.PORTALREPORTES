$(function () {

    //CAPTURA DE EVENTO SUBMIT DE FORMULARIO PRINCIPAL (PETICIÓN AJAX)
    $("#frm-ccosto").submit(function () {
        var form = $(this);

        if (form.validate()) {
            form.ajaxSubmit({
                dataType: 'JSON',
                type: 'POST',
                url: form.attr('action'),
                success: function (r) {
                    if (r.respuesta) {
                        window.location.href = '@(Url.Content("~/"))' + r.redirect;
                    } else {
                        notificacion(1, r.error);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    notificacion(1, errorThrown);
                }
            });
        }

        return false;
    });

    //Variables globales
    var dialog, form,
        nivel_des = $("#niv_des"),
        niv_pri = $("#niv_pri"),
        niv_user = $("#combobox"),
        niv_user_name = $("#combobox option:selected").text();

    //Eliminar una fila de niveles de aprobación
    $(document).on('click', 'button.removebutton', function () {

        if (confirm('¿Está seguro de eliminar este registro?')) {
            var id;
            $($(this).closest('tr')).each(function () {
                id = $(this).find(".id").html();
            });

            $.ajax({
                type: 'POST',
                traditional: true,
                url: '/centrocosto/DataNiveles',
                data: { deleteId: id },
                success: function (data) {
                    $('#cc-niveles').html(data);
                    $(this).closest('tr').remove();
                }
            });
        }

        return false;
    });

    // INICIO EVENTOS DEL DIALOG PARA ACTUALIZAR LA PRIORIDAD ======>
    var dialogPriority;
    dialogPriority = $("#priority-form").dialog({
        autoOpen: false,
        height: 350,
        width: 280,
        modal: true,
        dialogClass: "no-close",
        buttons: {
            "Actualizar": updateOrder,
            "Cancelar": function () {
                dialogPriority.dialog("close");
            }
        },
        close: function () {
            form[0].reset();
        }
    });

    //Actualizar las prioridades desde el dialog hacia el servidor
    function updateOrder() {
        var valid = true, counter = 1;

        var prioridades = [];
        $('#sortable li').each(function () {
            prioridades[counter] = $(this).attr('id') + "|" + counter;
            counter++;
        });

        $.ajax({
            type: 'POST',
            traditional: true,
            url: '/centrocosto/DataNiveles',
            data: { prioridades: prioridades },
            success: function (data) {
                $('#cc-niveles').html(data);
            }
        });

        dialogPriority.dialog("close");
        return valid;
    }

    //Capturar la acción del botón "Modificar prioridad"
    $("#update-priority").button().on("click", function () {
        dialogPriority.dialog("open");
        showLevels();
    });

    //Establecer Sortable list en elementos del dialog
    $("#sortable").sortable();
    $("#sortable").disableSelection();

    //Mostrar los niveles de aprobación asignados al centro de costo en el dialog
    function showLevels() {

        $('#sortable').empty();
        var cList = $('#sortable');

        $('#tbNiveles tr').each(function () {

            var id = $(this).find(".id").html();
            var description = $(this).find(".des").html();

            if (id != null) {
                var li = $('<li/>')
                    .addClass('ui-state-default')
                    .attr('id', id);
                var aaa = $('<span/>')
                    .addClass('ui-icon ui-icon-arrowthick-2-n-s')
                    .appendTo(li);

                li.text(description)
                    .appendTo(cList);
            }
        });
    }
    /////   <====== FIN EVENTOS DIALOG PARA ACTUALIZAR LA PRIORIDAD


    // INICIO EVENTOS DEL DIALOG PARA AÑADIR UN NIVEL DE APROBACIÓN ======>
    dialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 280,
        modal: true,
        dialogClass: "no-close",
        buttons: {
            "Agregar": addNivel,
            "Cancelar": function () {
                dialog.dialog("close");
            }
        },
        close: function () {
            form[0].reset();
        }
    });

    // Añadir nivel desde dialog hacia el servidor
    function addNivel() {
        var valid = true;

        $.ajax({
            type: 'POST',
            url: '/centrocosto/DataNiveles',
            data: {
                descripcion: nivel_des.val(),
                prioridad: niv_pri.val(),
                userId: niv_user.val(),
                userName: $("#combobox option:selected").text()
            },
            success: function (data) {
                $('#cc-niveles').html(data);
            }
        });

        dialog.dialog("close");

        return valid;
    }

    form = dialog.find("form").on("submit", function (event) {
        event.preventDefault();
    });

    //Acción "click" para abrir dialog
    $("#create-nivel").button().on("click", function () {
        dialog.dialog("open");
    });


    //Activar widget UI
    $("#Estado").selectmenu();

    //Autocomplete events
    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })
              .tooltip({
                  classes: {
                      "ui-tooltip": "ui-state-highlight"
                  }
              });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
                wasOpen = false;

            $("<a>")
                .attr("tabIndex", -1)
                .attr("title", "Show All Items")
                .tooltip()
                .appendTo(this.wrapper)
                .button({
                    icons: {
                        primary: "ui-icon-triangle-1-s"
                    },
                    text: false
                })
                .removeClass("ui-corner-all")
                .addClass("custom-combobox-toggle ui-corner-right")
                .on("mousedown", function () {
                    wasOpen = input.autocomplete("widget").is(":visible");
                })
                .on("click", function () {
                    input.trigger("focus");

                    // Close if already visible
                    if (wasOpen) {
                        return;
                    }

                    // Pass empty string as value to search for, displaying all results
                    input.autocomplete("search", "");
                });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
              .val("")
              .attr("title", value + " didn't match any item")
              .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });

    $("#combobox").combobox();

    //Spinner
    var spinner = $("#niv-pri").spinner();
    /////   <====== FIN EVENTOS DIALOG PARA AÑADIR NIVEL DE APROBACIÓN

});