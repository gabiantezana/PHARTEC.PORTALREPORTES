using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Model
{
    [Table("MigracionLog")]
    public partial class MigracionLog
    {
        public int id { get; set; }

        public int DocumentoId { get; set; }

        [Required]
        [StringLength(1)]
        public string Estado_actual { get; set; }

        public string Mensage_error { get; set; }

        [Required]
        [StringLength(1)]
        public string Migracion_estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaActualizacion { get; set; }


        //Métodos
        public List<MigracionLog> listar()
        {
            var list = new List<MigracionLog>();

            try
            {
                using (var ctx = new PhartecContext())
                {
                    list = ctx.MigracionLog.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }
    }
}
