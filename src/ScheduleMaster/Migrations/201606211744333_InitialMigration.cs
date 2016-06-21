namespace ScheduleMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RegularExpression = c.String(),
                        JobConfigurationId = c.Int(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        CC = c.String(),
                        Subject = c.String(),
                        SmtpHost = c.String(),
                        SmtpPort = c.Int(),
                        SmtpUsername = c.String(),
                        SmtpPassword = c.String(),
                        SmtpEnableSSL = c.Boolean(),
                        RoomName = c.String(),
                        ApiKey = c.String(),
                        NotificationColor = c.String(),
                        Mentions = c.String(maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JobConfigurations", t => t.JobConfigurationId, cascadeDelete: true)
                .Index(t => t.JobConfigurationId);
            
            CreateTable(
                "dbo.JobConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        SqsAccessKey = c.String(nullable: false),
                        SqsSecretKey = c.String(nullable: false),
                        SqsRegion = c.String(nullable: false),
                        SqsQueueUrl = c.String(nullable: false, maxLength: 1024),
                        LongPollingTimeSeconds = c.Int(nullable: false),
                        NumberOfDequeueMessages = c.Int(nullable: false),
                        DeleteMessageAfterSuccess = c.Boolean(nullable: false),
                        CronExpression = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SqsQueueUrl, unique: true, name: "IX_SqsQueue");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActionConfigurations", "JobConfigurationId", "dbo.JobConfigurations");
            DropIndex("dbo.JobConfigurations", "IX_SqsQueue");
            DropIndex("dbo.ActionConfigurations", new[] { "JobConfigurationId" });
            DropTable("dbo.JobConfigurations");
            DropTable("dbo.ActionConfigurations");
        }
    }
}
