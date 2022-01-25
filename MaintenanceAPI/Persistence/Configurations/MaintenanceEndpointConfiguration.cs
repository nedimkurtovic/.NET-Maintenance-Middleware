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
    public class MaintenanceEndpointConfiguration : IEntityTypeConfiguration<MaintenanceEndpoint>
    {
        public void Configure(EntityTypeBuilder<MaintenanceEndpoint> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action).IsRequired();
            builder.Property(x => x.Controller).IsRequired();
            builder.Property(x => x.RouteTemplate).IsRequired();
            builder.Property(x => x.MaintenanceId).IsRequired();

            builder.HasOne(x => x.Maintenance)
                .WithMany(x => x.AffectedEndpoints)
                .HasForeignKey(x => x.MaintenanceId);
        }
    }
}
