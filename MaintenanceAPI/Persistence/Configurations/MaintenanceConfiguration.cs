using MaintenanceAPI.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Persistence.Configurations
{
    public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AllAffected).IsRequired();
            builder.Property(x => x.Enabled).IsRequired();

            builder.HasMany(x => x.AffectedEndpoints).WithOne(x => x.Maintenance)
                .HasForeignKey(x => x.MaintenanceId);
        }
    }
}
