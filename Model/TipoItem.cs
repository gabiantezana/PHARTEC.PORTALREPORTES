using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class TipoItem
    {
        public string tipo { get; set; }
        public string descripcion { get; set; }

        public List<TipoItem> listar()
        {
            var lista = new List<TipoItem>();
            lista.Add(new TipoItem { tipo = "A", descripcion = "Artículo" });
            lista.Add(new TipoItem { tipo = "S", descripcion = "Servicio" });

            return lista;
        }
    }
}
