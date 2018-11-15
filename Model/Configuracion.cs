using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("Configuracion")]
    public class Configuracion
    {
        public int id { get; set; }

        [StringLength(300)]
        public string nombre_empresa { get; set; }

        [StringLength(300)]
        public string logo_url { get; set; }

        [StringLength(300)]
        public string servidor_correo { get; set; }

        public int puerto { get; set; }

        [StringLength(100)]
        public string usuario { get; set; }

        [StringLength(100)]
        public string password { get; set; }

        public bool enviar_correos { get; set; }

        public bool push_notification { get; set; }

        //funciones propias
        public string Guardar()
        {
            var res = String.Empty;

            try
            {
                using (var context = new PhartecContext())
                {
                    if (this.id == 0)
                        context.Entry(this).State = EntityState.Added;
                    else
                        context.Entry(this).State = EntityState.Modified;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }

        public Configuracion obtener()
        {
            Configuracion obj = null;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    var res = ctx.Configuracion.ToList();

                    if (res.Count > 0)
                        obj = res[0];
                }
            }
            catch (Exception)
            {
                obj = null;
            }

            return obj;
        }
    }
}
