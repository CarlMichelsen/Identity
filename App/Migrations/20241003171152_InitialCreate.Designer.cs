﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20241003171152_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("identity")
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Database.Entity.LoginRecordEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(39)
                        .HasColumnType("character varying(39)");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(2056)
                        .HasColumnType("character varying(2056)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LoginRecord", "identity");
                });

            modelBuilder.Entity("Database.Entity.RefreshRecordEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(39)
                        .HasColumnType("character varying(39)");

                    b.Property<long>("LoginRecordId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(2056)
                        .HasColumnType("character varying(2056)");

                    b.HasKey("Id");

                    b.HasIndex("LoginRecordId");

                    b.ToTable("RefreshRecord", "identity");
                });

            modelBuilder.Entity("Database.Entity.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasMaxLength(1028)
                        .HasColumnType("character varying(1028)");

                    b.Property<int>("AuthenticationProvider")
                        .HasColumnType("integer");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasMaxLength(2056)
                        .HasColumnType("character varying(2056)");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("ProviderId")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId", "AuthenticationProvider")
                        .IsUnique();

                    b.ToTable("User", "identity");
                });

            modelBuilder.Entity("Database.Entity.LoginRecordEntity", b =>
                {
                    b.HasOne("Database.Entity.UserEntity", "User")
                        .WithMany("LoginRecords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Database.Entity.RefreshRecordEntity", b =>
                {
                    b.HasOne("Database.Entity.LoginRecordEntity", "LoginRecord")
                        .WithMany("RefreshRecords")
                        .HasForeignKey("LoginRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoginRecord");
                });

            modelBuilder.Entity("Database.Entity.LoginRecordEntity", b =>
                {
                    b.Navigation("RefreshRecords");
                });

            modelBuilder.Entity("Database.Entity.UserEntity", b =>
                {
                    b.Navigation("LoginRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
