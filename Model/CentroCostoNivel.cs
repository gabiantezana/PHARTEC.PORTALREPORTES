namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CentroCostoNivel")]
    public partial class CentroCostoNivel
    {
        public int id { get; set; }

        public int Usuario_id { get; set; }

        public int CentroCosto_id { get; set; }

        [Required]
        [StringLength(200)]
        public string descripcion { get; set; }

        public int prioridad { get; set; }

        [InverseProperty("CentroCostoNivel")]
        public virtual CentroCosto CentroCosto { get; set; }

        [InverseProperty("CentroCostoNivel")]
        public virtual Usuario Usuario { get; set; }

        public string Usuario_Nombre { get; set; }

        public string temporal_id { get; set; }

    }
}
