﻿using EFDM.Test.Core.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDM.Test.DAL.Configs
{
    public class AuditGroupPropertyConfig : IEntityTypeConfiguration<AuditGroupProperty>
    {
        public void Configure(EntityTypeBuilder<AuditGroupProperty> builder)
        {
            builder.ToTable("AuditGroupProperties");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
