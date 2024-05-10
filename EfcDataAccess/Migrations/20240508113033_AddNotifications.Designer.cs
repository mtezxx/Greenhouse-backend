﻿// <auto-generated />
using System;
using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EfcDataAccess.Migrations
{
    [DbContext(typeof(EfcContext))]
    [Migration("20240508113033_AddNotifications")]
    partial class AddNotifications
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.18");

            modelBuilder.Entity("Domain.Entity.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Measurements");

                    b.HasDiscriminator<string>("Type").HasValue("Measurement");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.Entity.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MeasurementType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Threshold")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Domain.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entity.Humidity", b =>
                {
                    b.HasBaseType("Domain.Entity.Measurement");

                    b.HasDiscriminator().HasValue("Humidity");
                });

            modelBuilder.Entity("Domain.Entity.Temperature", b =>
                {
                    b.HasBaseType("Domain.Entity.Measurement");

                    b.HasDiscriminator().HasValue("Temperature");
                });
#pragma warning restore 612, 618
        }
    }
}
