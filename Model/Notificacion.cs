namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;
    using System.Linq;

    [Table("Notificacion")]
    public partial class Notificacion
    {
        public int id { get; set; }

        public int Usuario_id { get; set; }

        public int referencia { get; set; }

        public bool pendiente { get; set; }

        public bool visto { get; set; }

        public bool leido { get; set; }

        [Required]
        [StringLength(250)]
        public string descripcion { get; set; }

        public int from_user_id { get; set; }

        [Required]
        [StringLength(200)]
        public string from_user_nom { get; set; }

        public DateTime fecha_registro { get; set; }

        [Required]
        [StringLength(50)]
        public string controller { get; set; }

        [Required]
        [StringLength(50)]
        public string action { get; set; }

        [StringLength(50)]
        public string tipo { get; set; }

        public List<Notificacion> obtenerMessagesPendXUsuario(int userId)
        {
            var list = new List<Notificacion>();
            try
            {
                using (var ctx = new PhartecContext())
                {
                    list = ctx.Notificacion.Where(n => n.Usuario_id == userId && n.pendiente == true).OrderByDescending(n => n.fecha_registro).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }

        public int obtenerNumPendMessages(int userId)
        {
            var num = 0;
            try
            {
                using (var ctx = new PhartecContext())
                {
                    num = ctx.Notificacion.Where(n => n.pendiente == true && n.Usuario_id == userId).ToList().Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return num;
        }

        //Actualizar el número de mensajes pendientes de un usuario
        public void updateNumPendMessages(int userId, Solicitud model, string type)
        {
            try
            {
                using (var ctx = new PhartecContext())
                {
                    this.Usuario_id = userId;
                    this.referencia = model.id;
                    this.pendiente = true;
                    this.visto = false;
                    this.leido = false;

                    if (type.Equals("info"))
                    {
                        this.descripcion = "Nueva solicitud registrada por el usuario.";
                        this.from_user_id = model.Usuario_id;
                        this.from_user_nom = model.Usuario_nombre;
                    }
                    else if (type.Equals("warning"))
                        this.descripcion = "El usuario " + userId + " modificó su solicitud N° " + model.id;
                    else if (type.Equals("success"))
                        this.descripcion = "El usuario " + userId + " aprobó su solicitud N° " + model.id;
                    else if (type.Contains("danger"))
                        this.descripcion = "El usuario " + userId + " rechazó su solicitud N°" + model.id;

                    this.fecha_registro = DateTime.Now;
                    this.controller = "Home";
                    this.action = "ViewReq";
                    this.tipo = type;

                    ctx.Entry(this).State = EntityState.Added;
                    ctx.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public void updateEstadoNotificacion(int id)
        {
            try
            {
                using (var ctx = new PhartecContext())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        ctx.Database.ExecuteSqlCommand(
                                    "UPDATE Notificacion SET pendiente = 0, visto = 0, leido = 1 where id = @id",
                                    new SqlParameter("id", id)
                                );

                        try
                        {
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
