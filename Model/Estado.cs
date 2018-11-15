using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Estado
    {
        public string estado { get; set; }
        public string descripcion { get; set; }

        public List<Estado> listar()
        {
            var lista = new List<Estado>();
            lista.Add(new Estado { estado = "A", descripcion = "Activo" });
            lista.Add(new Estado{ estado = "I", descripcion ="Inactivo"});

            return lista;
        }

    }
}
