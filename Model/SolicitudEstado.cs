namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SolicitudEstado")]
    public partial class SolicitudEstado
    {
        public int id { get; set; }

        public int Solicitud_id { get; set; }

        public int Usuario { get; set; }

        [StringLength(1)]
        public string accion { get; set; }

        public DateTime fechaRegistro { get; set; }

        public DateTime? fechaActualizacion { get; set; }

        public bool activo { get; set; }

        [StringLength(300)]
        public string observacion { get; set; }

        public int prioridad { get; set; }

        public virtual Solicitud Solicitud { get; set; }

        [NotMapped]
        public string Usuario_nombre { get; set; }

    }
}
