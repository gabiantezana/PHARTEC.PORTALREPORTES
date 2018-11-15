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

    [Table("Solicitud")]
    public partial class Solicitud: IAuditable
    {
        public Solicitud()
        {
            SolicitudDetalle = new List<SolicitudDetalle>();
            SolicitudEstado = new List<SolicitudEstado>();
        }

        public int id { get; set; }

        public int Usuario_id { get; set; }

        public string Usuario_nombre { get; set; }

        public string Usuario_correo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el centro de costo")]
        public int centroCosto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione la empresa")]
        public int empresa { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha de registro")]
        public DateTime fechaRegistro { get; set; }

        public DateTime? fechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha necesaria")]
        public DateTime fechaNecesaria { get; set; }

        [StringLength(250)]
        public string comentarios { get; set; }

        [StringLength(1)]
        public string tipoSolicitud { get; set; }

        [StringLength(1)]
        public string estado { get; set; }

        public virtual ICollection<SolicitudDetalle> SolicitudDetalle { get; set; }

        public virtual ICollection<SolicitudEstado> SolicitudEstado { get; set; }

        public virtual Usuario Usuario { get; set; }

        [NotMapped]
        public int cantidadItems { get; set; }

        [NotMapped]
        public string Empresa_descripcion { get; set; }

        [NotMapped]
        public string CentroCosto_descripcion { get; set; }

        [NotMapped]
        public bool Modificado { get; set; }

        //Métodos
        public List<Solicitud> listar()
        {
            var list = new List<Solicitud>();
            try
            {
                using (var context = new PhartecContext())
                {
                    list = context.Solicitud.Include("Usuario").ToList();

                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.cantidadItems = item.SolicitudDetalle.Count;
                            var objEmp = context.Empresa.Where(e => e.id == item.empresa).Single();
                            item.Empresa_descripcion = objEmp.descripcion;

                            var objCC = context.CentroCosto.Where(c => c.id == item.centroCosto).Single();
                            item.CentroCosto_descripcion = objCC.descripcion;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }

        //Obtiene las solicitudes registradas por el usuario
        public List<Solicitud> listar(int idUser)
        {
            var list = new List<Solicitud>();
            try
            {
                using (var context = new PhartecContext())
                {
                    list = context.Solicitud.Include("Usuario").Where(s => s.Usuario_id == idUser).ToList();

                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.cantidadItems = item.SolicitudDetalle.Count;
                            var objEmp = context.Empresa.Where(e => e.id == item.empresa).Single();
                            item.Empresa_descripcion = objEmp.descripcion;

                            var objCC = context.CentroCosto.Where(c => c.id == item.centroCosto).Single();
                            item.CentroCosto_descripcion = objCC.descripcion;

                            if (item.CreadoPor != item.ActualizadoPor)
                                item.Modificado = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }

        //Obtiene las solicitudes pendientes del usuario
        public List<SolicitudListModel> listarXUsuario(int id)
        {
            List<SolicitudListModel> list = null;

            try
            {
                using (var context = new PhartecContext())
                {
                    list = (from T0 in context.Solicitud
                                   join T1 in context.SolicitudEstado
                                           on T0.id equals T1.Solicitud_id
                                   join T2 in context.Empresa
                                           on T0.empresa equals T2.id
                                   join T3 in context.CentroCosto
                                           on T0.centroCosto equals T3.id
                                   where T1.Usuario == id &&  (T1.prioridad > 
                                       (from X0 in context.SolicitudEstado
                                        where X0.Solicitud_id == T0.id && X0.activo == true
                                        select X0).FirstOrDefault().prioridad || T1.activo == true)
                                   select new SolicitudListModel { SolicitudId = T0.id,
                                                UsuarioId = T0.Usuario_id,
                                                EmpresaDes = T2.descripcion,
                                                CentorCostoDes = T3.descripcion, 
                                                UsuarioNom = T0.Usuario_nombre,
                                                FechaRegistro = T0.fechaRegistro,
                                                FechaNecesaria = T0.fechaNecesaria,
                                                CantidadItems = T0.SolicitudDetalle.Count()
                                   }).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }

        //Obtiene las solicitudes que fueron aprobadas o rechazadas por un usuario
        public List<SolicitudListModel> listarHistorialXUsuario(int id)
        {
            List<SolicitudListModel> list = null;

            try
            {
                using (var context = new PhartecContext())
                {
                    list = (from T0 in context.Solicitud
                            join T1 in context.SolicitudEstado
                                    on T0.id equals T1.Solicitud_id
                            join T2 in context.Empresa
                                    on T0.empresa equals T2.id
                            join T3 in context.CentroCosto
                                    on T0.centroCosto equals T3.id
                            where T1.Usuario == id && T1.activo == false
                            select new SolicitudListModel
                            {
                                SolicitudId = T0.id,
                                UsuarioId = T0.Usuario_id,
                                EmpresaDes = T2.descripcion,
                                CentorCostoDes = T3.descripcion,
                                UsuarioNom = T0.Usuario_nombre,
                                FechaRegistro = T0.fechaRegistro,
                                FechaNecesaria = T0.fechaNecesaria,
                                CantidadItems = T0.SolicitudDetalle.Count(),
                                accion = T1.accion
                            }).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }

        //Obtener solicitud por id
        public Solicitud obtener(int id)
        {
            var solicitud = new Solicitud();
            try
            {
                using (var context = new PhartecContext())
                {
                    solicitud = context.Solicitud
                                     .Include("Usuario")
                                     .Include("SolicitudDetalle")
                                     .Include("SolicitudEstado")
                                     .Where(u => u.id == id)
                                     .Single();

                    if (solicitud != null)
                    {
                        var objEmp = context.Empresa.Where(e => e.id == solicitud.empresa).Single();
                        solicitud.Empresa_descripcion = objEmp.descripcion;

                        var objCC = context.CentroCosto.Where(c => c.id == solicitud.centroCosto).Single();
                        solicitud.CentroCosto_descripcion = objCC.descripcion;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return solicitud;
        }
    
        //Guardar/Editar solicitud
        public string Guardar()
        {
            string result = String.Empty;
            bool newObject = false;

            try
            {
                using (var context = new PhartecContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        if (this.id == 0)
                        {
                            newObject = true;
                            context.Entry(this).State = EntityState.Added;
                        }
                        else
                        {
                            //Eliminar los datos en la relación SOLICITUD-DETALLE actual, para actualizar
                            context.Database.ExecuteSqlCommand(
                                "DELETE FROM SolicitudDetalle WHERE Solicitud_id = @id",
                                new SqlParameter("id", this.id)
                            );
                            var detalleBK = this.SolicitudDetalle;
                            this.SolicitudDetalle = null;

                            context.Entry(this).State = EntityState.Modified;
                            context.Entry(this).Property(s => s.Usuario_id).IsModified = false;
                            context.Entry(this).Property(s => s.Usuario_nombre).IsModified = false;
                            context.Entry(this).Property(s => s.Usuario_correo).IsModified = false;
                            context.Entry(this).Property(s => s.empresa).IsModified = false;
                            context.Entry(this).Property(s => s.centroCosto).IsModified = false;
                            context.Entry(this).Property(s => s.estado).IsModified = false;
                            this.SolicitudDetalle = detalleBK;

                            foreach (var n in this.SolicitudEstado)
                                context.Entry(n).State = EntityState.Unchanged;
                        }

                        foreach (var e in this.SolicitudDetalle)
                            context.Entry(e.Articulo).State = EntityState.Unchanged;

                        context.Entry(this.Usuario).State = EntityState.Unchanged;

                        try
                        {
                            int saveRes = context.SaveChanges();

                            if (newObject)
                            {
                                //guardar registro de solicitud original
                                foreach (var item in this.SolicitudDetalle)
                                {
                                    context.SolDOriginal.Add(new SolDOriginal
                                    {
                                        Solicitud_id = this.id,
                                        Articulo_id = item.Articulo_id,
                                        descripcion = item.descripcion,
                                        cantidad = item.cantidad,
                                        fechaNecesaria = item.fechaNecesaria,
                                        Articulo_codigosap = item.Articulo_codigosap,
                                        comentario = item.comentario
                                    });
                                }

                                context.SaveChanges();
                            }

                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            result = e.Message;
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                string errors = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    errors += "Type " + eve.Entry.Entity.GetType().Name + " has the following vaidation errors: " + eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errors += " Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;
                    }
                }

                result = "Entity errors. " + errors;
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }

        public int obtenerUltimoId(int userID)
        {
            int res = 0;
            try
            {
                using (var ctx = new PhartecContext())
                {
                    res = ctx.Solicitud.Where(s => s.Usuario_id == userID).Max(s => s.id);
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return res;
        }

        public string Aprobar(int userID)
        {
            var result = string.Empty;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        var estados = ctx.SolicitudEstado
                                        .Where(e => e.Solicitud_id == this.id)
                                        .OrderBy(e => e.prioridad)
                                        .ToList();

                        if (estados != null && estados.Count > 0)
                        {
                            foreach (var item in estados)
                            {
                                if (item.Usuario == userID)
                                {
                                    //Establecer todos los niveles de aprobación en estado "inactivo"
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE SolicitudEstado SET activo = 0 where Solicitud_id = @id",
                                        new SqlParameter("id", this.id)
                                    );

                                    //Establecer el estado "A" (Aprobado) para el nivel perteneciente al usuario actual -sin comentarios
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE SolicitudEstado SET accion = 'A', fechaActualizacion = GETDATE(), observacion = @comment " +
                                        "where Solicitud_id = @id AND Usuario = @user",
                                        new SqlParameter("id", this.id),
                                        new SqlParameter("user", userID),
                                        new SqlParameter("comment", this.comentarios != null? this.comentarios: "")
                                    );

                                    //Establecer el estado "P" (Procesando) para la solicitud
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE Solicitud SET estado = 'P' " +
                                        "where id = @id",
                                        new SqlParameter("id", this.id)
                                    );

                                    //Si no existe otro nivel de aprobación para aprobar actualizar la solicitud a "Aprobada"
                                    if (item.id == estados.Last().id)
                                    {
                                        ctx.Database.ExecuteSqlCommand(
                                            "UPDATE Solicitud SET estado = 'A' " +
                                            "where id = @id",
                                            new SqlParameter("id", this.id)
                                        );
                                    }
                                    else //Si existe activarlo
                                    {
                                        var nextLevel = from e in estados
                                                        where e.prioridad > item.prioridad
                                                        orderby e.prioridad
                                                        select e;

                                        foreach (var n in nextLevel)
                                        {
                                            ctx.Database.ExecuteSqlCommand(
                                                "UPDATE SolicitudEstado SET activo = 1 " +
                                                "where id = @id",
                                                new SqlParameter("id", n.id)
                                            );
                                            
                                            //Solo actualizar el primero
                                            break;
                                        }
                                    }

                                    //Solo procesar el primero, pues podría existir el mismo usuario en otro nivel
                                    break;
                                }
                            }

                            //Ejecutar los cambios
                            try
                            {
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                result = e.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public string Rechazar(int userID)
        {
            var result = string.Empty;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        var estados = ctx.SolicitudEstado
                                        .Where(e => e.Solicitud_id == this.id)
                                        .OrderBy(e => e.prioridad)
                                        .ToList();

                        if (estados != null && estados.Count > 0)
                        {
                            foreach (var item in estados)
                            {
                                if (item.Usuario == userID)
                                {
                                    //Establecer todos los niveles de aprobación en estado "inactivo"
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE SolicitudEstado SET activo = 0 where Solicitud_id = @id",
                                        new SqlParameter("id", this.id)
                                    );

                                    //Establecer el estado "D" (Rechazada) para el nivel perteneciente al usuario actual
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE SolicitudEstado SET accion = 'D', fechaActualizacion = GETDATE(), observacion = @comment " +
                                        "where Solicitud_id = @id AND Usuario = @user",
                                        new SqlParameter("id", this.id),
                                        new SqlParameter("user", userID),
                                        new SqlParameter("comment", this.comentarios != null ? this.comentarios : "")
                                    );

                                    //Actualizar la solicitud a "Rechazada"
                                    ctx.Database.ExecuteSqlCommand(
                                        "UPDATE Solicitud SET estado = 'D' " +
                                        "where id = @id",
                                        new SqlParameter("id", this.id)
                                    );

                                    //Solo procesar el primero, pues podría existir el mismo usuario en otro nivel
                                    break;
                                }
                            }

                            //Ejecutar los cambios
                            try
                            {
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                result = e.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        //Obtener estado activo de una solicitud
        public SolicitudEstado obtenerEstadoActivo(int solID)
        {
            var solicitud = new SolicitudEstado();
            try
            {
                using (var context = new PhartecContext())
                {
                    solicitud = context.SolicitudEstado.Where(e => e.Solicitud_id == solID && e.activo == true).Single();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return solicitud;
        }

        //Obtener la cantidad de solicitudes registradas por el usuario, con estado REGISTRADA y EN PROCESO
        public int solXUsuRegistradas(int userId)
        {
            int result = 0;
            
            try
            {
                using (var ctx = new PhartecContext())
                {
                    result = ctx.Solicitud.Where(s => s.Usuario_id == userId && (s.estado.Equals("R") || s.estado.Equals("P"))).ToList().Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return result;
        }

        //Obtener la cantidad de solicitudes registradas por el usuario, con estado APROBADA
        public int solXUsuAprobadas(int userId)
        {
            int result = 0;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    result = ctx.Solicitud.Where(s => s.Usuario_id == userId && s.estado.Equals("A")).ToList().Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return result;
        }

        //Obtener la cantidad de solicitudes registradas por el usuario, con estado RECHAZADA
        public int solXUsuRechazadas(int userId)
        {
            int result = 0;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    result = ctx.Solicitud.Where(s => s.Usuario_id == userId && s.estado.Equals("D")).ToList().Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return result;
        }

        public int solXUsuSAP(int userID, string estado)
        {
            int result = 0;

            try
            {
                using (var ctx = new PhartecContext())
                {
                    result = ctx.Solicitud.Where(s => s.Usuario_id == userID && s.estado.Equals(estado)).ToList().Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return result;
        }
    }
}
