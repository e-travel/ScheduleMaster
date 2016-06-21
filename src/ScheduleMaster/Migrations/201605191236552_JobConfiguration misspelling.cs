namespace DlqMaster.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobConfigurationmisspelling : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActionConfigurations", "JobConfiguration_Id", "dbo.JobConfigurations");
            DropIndex("dbo.ActionConfigurations", new[] { "JobConfiguration_Id" });
            RenameColumn(table: "dbo.ActionConfigurations", name: "JobConfiguration_Id", newName: "JobConfigurationId");
            AlterColumn("dbo.ActionConfigurations", "JobConfigurationId", c => c.Int(nullable: false));
            CreateIndex("dbo.ActionConfigurations", "JobConfigurationId");
            AddForeignKey("dbo.ActionConfigurations", "JobConfigurationId", "dbo.JobConfigurations", "Id", cascadeDelete: true);
            DropColumn("dbo.ActionConfigurations", "JobConfgiruationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ActionConfigurations", "JobConfgiruationId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ActionConfigurations", "JobConfigurationId", "dbo.JobConfigurations");
            DropIndex("dbo.ActionConfigurations", new[] { "JobConfigurationId" });
            AlterColumn("dbo.ActionConfigurations", "JobConfigurationId", c => c.Int());
            RenameColumn(table: "dbo.ActionConfigurations", name: "JobConfigurationId", newName: "JobConfiguration_Id");
            CreateIndex("dbo.ActionConfigurations", "JobConfiguration_Id");
            AddForeignKey("dbo.ActionConfigurations", "JobConfiguration_Id", "dbo.JobConfigurations", "Id");
        }
    }
}
