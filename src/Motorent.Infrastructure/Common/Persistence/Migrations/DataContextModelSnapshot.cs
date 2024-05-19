﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Motorent.Infrastructure.Common.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Motorent.Domain.Motorcycles.Motorcycle", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)")
                        .HasColumnName("license_plate");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("model");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pk_motorcycles");

                    b.HasIndex("LicensePlate")
                        .IsUnique()
                        .HasDatabaseName("ix_motorcycles_license_plate");

                    b.ToTable("motorcycles", (string)null);
                });

            modelBuilder.Entity("Motorent.Domain.Renters.Renter", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<DateOnly>("Birthdate")
                        .HasColumnType("date")
                        .HasColumnName("birthdate");

                    b.Property<string>("CNHImageUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("cnh_image_url");

                    b.Property<string>("CNHStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("cnh_status");

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("character varying(18)")
                        .HasColumnName("cnpj");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(26)
                        .HasColumnType("character varying(26)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_renters");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_renters_email");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_renters_user_id");

                    b.ToTable("renters", (string)null);
                });

            modelBuilder.Entity("Motorent.Infrastructure.Common.Identity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)")
                        .HasColumnName("id");

                    b.Property<IDictionary<string, string>>("Claims")
                        .HasColumnType("jsonb")
                        .HasColumnName("claims");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("password_hash");

                    b.Property<string[]>("Roles")
                        .HasColumnType("jsonb")
                        .HasColumnName("roles");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "01HY89QQM2T5THZCM1KXDZ10G5",
                            Claims = new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" },
                            Email = "john@admin.com",
                            PasswordHash = "tjwy+Xf1NuBHtFdh75cVsNtnfa9qwQJYZ+4BNYCgU+E=:1zaGX5TNm+4XWPwDIa4U6g==:50000:SHA256",
                            Roles = new[] { "admin" }
                        });
                });

            modelBuilder.Entity("Motorent.Infrastructure.Common.Messaging.MessageLog", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(65536)
                        .HasColumnType("character varying(65536)")
                        .HasColumnName("data");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("identifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<DateTimeOffset>("ReceivedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("received_at");

                    b.Property<DateTimeOffset?>("SentAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sent_at");

                    b.HasKey("Id")
                        .HasName("pk_message_logs");

                    b.ToTable("message_logs", (string)null);
                });

            modelBuilder.Entity("Motorent.Infrastructure.Common.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("data");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTimeOffset?>("ProcessedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_at");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("type");

                    b.Property<uint>("version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_outbox_messages");

                    b.ToTable("outbox_messages", (string)null);
                });

            modelBuilder.Entity("Motorent.Domain.Renters.Renter", b =>
                {
                    b.OwnsOne("Motorent.Domain.Renters.ValueObjects.CNH", "CNH", b1 =>
                        {
                            b1.Property<string>("RenterId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("id");

                            b1.Property<string>("Category")
                                .IsRequired()
                                .HasMaxLength(5)
                                .HasColumnType("character varying(5)")
                                .HasColumnName("cnh_category");

                            b1.Property<DateOnly>("ExpirationDate")
                                .HasColumnType("date")
                                .HasColumnName("cnh_exp");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasMaxLength(11)
                                .HasColumnType("character varying(11)")
                                .HasColumnName("cnh_number");

                            b1.HasKey("RenterId");

                            b1.HasIndex("Number")
                                .IsUnique()
                                .HasDatabaseName("ix_renters_cnh_number");

                            b1.ToTable("renters");

                            b1.WithOwner()
                                .HasForeignKey("RenterId")
                                .HasConstraintName("fk_renters_renters_id");
                        });

                    b.OwnsOne("Motorent.Domain.Renters.ValueObjects.FullName", "FullName", b1 =>
                        {
                            b1.Property<string>("RenterId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("id");

                            b1.Property<string>("FamilyName")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("character varying(30)")
                                .HasColumnName("family_name");

                            b1.Property<string>("GivenName")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("character varying(30)")
                                .HasColumnName("given_name");

                            b1.HasKey("RenterId");

                            b1.ToTable("renters");

                            b1.WithOwner()
                                .HasForeignKey("RenterId")
                                .HasConstraintName("fk_renters_renters_id");
                        });

                    b.Navigation("CNH")
                        .IsRequired();

                    b.Navigation("FullName")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
