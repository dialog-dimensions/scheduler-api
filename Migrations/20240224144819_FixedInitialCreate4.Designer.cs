﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchedulerApi.DAL;

#nullable disable

namespace Api.Scheduler.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20240224144819_FixedInitialCreate4")]
    partial class FixedInitialCreate4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.1.24081.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Api.Scheduler.Models.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<double>("Balance")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(0.0);

                    b.Property<double>("DifficultBalance")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(0.0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(225)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Employees_Name");

                    b.ToTable("Employees", (string)null);
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.Shift", b =>
                {
                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModificationDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("ModificationUser")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(225)")
                        .HasDefaultValue("Computer");

                    b.Property<DateTime>("ScheduleKey")
                        .HasColumnType("datetime2");

                    b.HasKey("StartDateTime");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Shifts", (string)null);
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.ShiftException", b =>
                {
                    b.Property<DateTime>("ShiftKey")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("ExceptionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("ModificationDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("ModificationUser")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(225)")
                        .HasDefaultValue("Computer");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ShiftKey", "EmployeeId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("ShiftExceptions", (string)null);
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.ShiftSwap", b =>
                {
                    b.Property<int>("SwapId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SwapId"));

                    b.Property<DateTime>("ModificationDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("ModificationUser")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(225)")
                        .HasDefaultValue("Computer");

                    b.Property<int>("PreviousEmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ShiftKey")
                        .HasColumnType("datetime2");

                    b.HasKey("SwapId");

                    b.HasIndex("PreviousEmployeeId");

                    b.HasIndex("ShiftKey");

                    b.ToTable("ShiftSwaps", (string)null);
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.Shift", b =>
                {
                    b.HasOne("Api.Scheduler.Models.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.ShiftException", b =>
                {
                    b.HasOne("Api.Scheduler.Models.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Api.Scheduler.Models.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("ShiftKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("Api.Scheduler.Models.Entities.ShiftSwap", b =>
                {
                    b.HasOne("Api.Scheduler.Models.Entities.Employee", "PreviousEmployee")
                        .WithMany()
                        .HasForeignKey("PreviousEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Api.Scheduler.Models.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("ShiftKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PreviousEmployee");

                    b.Navigation("Shift");
                });
#pragma warning restore 612, 618
        }
    }
}
