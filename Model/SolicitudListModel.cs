using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class SolicitudListModel
    {
        public int SolicitudId { get; set; }
        public int UsuarioId { get; set; }
        public string EmpresaDes { get; set; }
        public string CentorCostoDes { get; set; }
        public string UsuarioNom { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaNecesaria { get; set; }
        public int CantidadItems { get; set; }
        public string accion { get; set; }
    }
}
