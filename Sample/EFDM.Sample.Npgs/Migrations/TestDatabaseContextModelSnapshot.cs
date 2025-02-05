﻿// <auto-generated />
using System;
using EFDM.Sample.DAL.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EFDM.Sample.Npgs.Migrations
{
    [DbContext(typeof(TestDatabaseContext))]
    partial class TestDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EFDM.Sample.Core.Models.Audit.AuditGroupEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ActionId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer");

                    b.Property<string>("ObjectId")
                        .HasColumnType("text");

                    b.Property<string>("ObjectType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditGroupEvents", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Audit.AuditGroupProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AuditId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NewValue")
                        .HasColumnType("text");

                    b.Property<string>("OldValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditGroupProperties", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Audit.AuditTaskAnswerEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ActionId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer");

                    b.Property<string>("ObjectId")
                        .HasColumnType("text");

                    b.Property<string>("ObjectType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditTaskAnswerEvents", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Audit.AuditTaskAnswerProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AuditId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NewValue")
                        .HasColumnType("text");

                    b.Property<string>("OldValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditTaskAnswerProperties", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ModifiedById")
                        .HasColumnType("integer");

                    b.Property<int>("SubTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("TextField1")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("TextField2")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnOrder(1);

                    b.Property<int>("TypeId")
                        .HasColumnType("integer")
                        .HasColumnOrder(2);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("TypeId");

                    b.ToTable("Groups", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Created = new DateTimeOffset(new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            CreatedById = 1,
                            IsDeleted = false,
                            Modified = new DateTimeOffset(new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            ModifiedById = 1,
                            SubTypeId = 0,
                            Title = "Пользователи",
                            TypeId = 1
                        },
                        new
                        {
                            Id = 2,
                            Created = new DateTimeOffset(new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            CreatedById = 1,
                            IsDeleted = false,
                            Modified = new DateTimeOffset(new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            ModifiedById = 1,
                            SubTypeId = 0,
                            Title = "Администраторы",
                            TypeId = 2
                        });
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.GroupType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ModifiedById")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("GroupTypes", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Created = new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            CreatedById = 1,
                            IsDeleted = false,
                            Modified = new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            ModifiedById = 1,
                            Title = "Пользователи"
                        },
                        new
                        {
                            Id = 2,
                            Created = new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            CreatedById = 1,
                            IsDeleted = false,
                            Modified = new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            ModifiedById = 1,
                            Title = "Администраторы"
                        });
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.GroupUser", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUsers", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.TaskAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AnswerValue")
                        .HasColumnType("numeric");

                    b.Property<string>("TextField1")
                        .HasColumnType("text");

                    b.Property<string>("TextField2")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("ValidFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("ValidTill")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("TaskAnswers", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.TaskAnswerComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TaskAnswerComments", (string)null);
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer");

                    b.Property<string>("Department")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("JobTitle")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnOrder(1);

                    b.Property<DateTimeOffset>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ModifiedById")
                        .HasColumnType("integer");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnOrder(2);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Created = new DateTimeOffset(new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            CreatedById = 1,
                            IsDeleted = false,
                            Login = "efdm\\SYSTEM",
                            Modified = new DateTimeOffset(new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)),
                            ModifiedById = 1,
                            Title = "SYSTEM"
                        });
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.Group", b =>
                {
                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EFDM.Sample.Core.Models.Domain.GroupType", "Type")
                        .WithMany("Groups")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.GroupType", b =>
                {
                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.GroupUser", b =>
                {
                    b.HasOne("EFDM.Sample.Core.Models.Domain.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.TaskAnswerComment", b =>
                {
                    b.HasOne("EFDM.Sample.Core.Models.Domain.TaskAnswer", "TaskAnswer")
                        .WithOne("AnswerComment")
                        .HasForeignKey("EFDM.Sample.Core.Models.Domain.TaskAnswerComment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaskAnswer");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.User", b =>
                {
                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EFDM.Sample.Core.Models.Domain.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.Group", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.GroupType", b =>
                {
                    b.Navigation("Groups");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.TaskAnswer", b =>
                {
                    b.Navigation("AnswerComment");
                });

            modelBuilder.Entity("EFDM.Sample.Core.Models.Domain.User", b =>
                {
                    b.Navigation("Groups");
                });
#pragma warning restore 612, 618
        }
    }
}
