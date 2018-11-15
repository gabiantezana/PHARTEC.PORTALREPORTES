using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class AccionModel
    {
        public string accion { get; set; }
        public string descripcion { get; set; }


        public List<AccionModel> listar()
        {
            var list = new List<AccionModel>();
            list.Add(new AccionModel
            {
                accion = "A",
                descripcion = "Aprobado"
            });

            list.Add(new AccionModel
            {
                accion = "D",
                descripcion = "Rechazado"
            });

            return list;
        }
    }
}
