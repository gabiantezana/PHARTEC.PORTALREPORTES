namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsuarioPermisoItems")]
    public partial class UsuarioPermisoItems
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Usuario(Id)
        /// </summary>
        [Required]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Identifica si el usuario tiene permiso de ver el artículo
        /// </summary>
        public int TipoTablaSAP { get; set; }

        /// <summary>
        /// OITM(ItemCode) *Tabla de SAP
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Identifica si el usuario tiene permiso de ver el artículo
        /// </summary>
        public bool? PermisoVisualizacion { get; set; }


        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        #region Business


        #endregion
    }
}
