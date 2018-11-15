using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("SolDOriginal")]
    public partial class SolDOriginal
    {
        public int Solicitud_id { get; set; }

        public int? Articulo_id { get; set; }

        [StringLength(250)]
        public string descripcion { get; set; }

        public int cantidad { get; set; }

        public DateTime? fechaNecesaria { get; set; }

        [StringLength(250)]
        public string Articulo_codigosap { get; set; }

        public string comentario { get; set; }

        public int id { get; set; }

        public List<SolDOriginal> listarXIdSol(int SolID)
        {
            var res = new List<SolDOriginal>();

            try
            {
                using (var ctx = new PhartecContext())
                {
                    res = ctx.SolDOriginal.Where(o => o.Solicitud_id == SolID).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return res;
        }
    }
}
