namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SucursalSeries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SucursalSeries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SucursalId = c.Int(nullable: false),
                        Serie = c.String(nullable: false),
                        Descripcion = c.String(),
                        Enable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sucursal", t => t.SucursalId, cascadeDelete: true)
                .Index(t => t.SucursalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SucursalSeries", "SucursalId", "dbo.Sucursal");
            DropIndex("dbo.SucursalSeries", new[] { "SucursalId" });
            DropTable("dbo.SucursalSeries");
        }
    }
}
