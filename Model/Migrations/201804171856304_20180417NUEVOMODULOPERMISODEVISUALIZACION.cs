namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180417NUEVOMODULOPERMISODEVISUALIZACION : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsuarioPermisoItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        TipoTablaSAP = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        PermisoVisualizacion = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsuarioPermisoItems", "UsuarioId", "dbo.Usuario");
            DropIndex("dbo.UsuarioPermisoItems", new[] { "UsuarioId" });
            DropTable("dbo.UsuarioPermisoItems");
        }
    }
}
