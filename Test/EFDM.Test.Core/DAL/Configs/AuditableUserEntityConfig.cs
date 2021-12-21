﻿using EFDM.Test.Core.Models.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Test.Core.DAL.Configs {

    public class AuditableUserEntityConfig<T> : IEntityTypeConfiguration<T>
        where T : class, IAuditableUserEntity {

        public void Configure(EntityTypeBuilder<T> builder) {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);
            builder.HasOne(x => x.ModifiedBy)
                .WithMany()
                .HasForeignKey(x => x.ModifiedById);
        }
    }
}
