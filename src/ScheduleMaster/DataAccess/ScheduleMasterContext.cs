using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using ScheduleMaster.Models.Entities;

namespace ScheduleMaster.DataAccess
{
    public class ScheduleMasterContext :DbContext
    {

        public ScheduleMasterContext() : base("name=ScheduleMasterDatabaseConnectionString") 
        {
            Database.SetInitializer(new DbInitializer());
        }

        public DbSet<ActionConfiguration> ActionConfigurations { get; set; }
        public DbSet<JobConfiguration> JobConfigurations { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobConfiguration>().Property(x => x.SqsQueueUrl)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_SqsQueue")
            {
                IsUnique = true
            }));
        }
    }
}