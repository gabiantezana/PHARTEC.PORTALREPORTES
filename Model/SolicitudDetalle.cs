namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SolicitudDetalle")]
    public partial class SolicitudDetalle
    {
        public int Solicitud_id { get; set; }

        public int? Articulo_id { get; set; }

        [StringLength(250)]
        public string descripcion { get; set; }

        public int cantidad { get; set; }

        public DateTime? fechaNecesaria { get; set; }

        [StringLength(250)]
        public string Articulo_codigosap { get; set; }

        [StringLength(250)]
        public string temporal_id { get; set; }

        public string comentario { get; set; }

        public int id { get; set; }

        public virtual Articulo Articulo { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
