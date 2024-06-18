﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchedulerApi.DAL;

#nullable disable

namespace Api.Scheduler.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession", b =>
                {
                    b.Property<string>("ThreadId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrentAssistantId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("NotCreated");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ThreadId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("GptSessions", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.Shift", b =>
                {
                    b.Property<string>("DeskId")
                        .HasColumnType("nvarchar(450)");

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

                    b.Property<DateTime>("ScheduleStartDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("DeskId", "StartDateTime");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ScheduleStartDateTime")
                        .HasDatabaseName("IX_Shifts_ScheduleStartDateTime");

                    b.ToTable("Shifts", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.ShiftException", b =>
                {
                    b.Property<string>("DeskId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ShiftStartDateTime")
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

                    b.HasKey("DeskId", "ShiftStartDateTime", "EmployeeId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("ShiftExceptions", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.ShiftSwap", b =>
                {
                    b.Property<int>("SwapId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SwapId"));

                    b.Property<string>("DeskId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

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

                    b.Property<DateTime>("ShiftStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Applied");

                    b.HasKey("SwapId");

                    b.HasIndex("PreviousEmployeeId");

                    b.HasIndex("DeskId", "ShiftStart");

                    b.ToTable("ShiftSwaps", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.Workers.Employee", b =>
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

                    b.Property<string>("Gender")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Unknown");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(225)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Employee");

                    b.Property<string>("UnitId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Employees_Name");

                    b.HasIndex("UnitId");

                    b.ToTable("Employees", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.Desk", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UnitId");

                    b.ToTable("Desks", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.DeskAssignment", b =>
                {
                    b.Property<string>("DeskId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.HasKey("DeskId", "EmployeeId");

                    b.HasIndex("DeskId")
                        .HasDatabaseName("IX_DeskAssignments_DeskId");

                    b.HasIndex("EmployeeId")
                        .HasDatabaseName("IX_DeskAssignments_EmployeeId");

                    b.ToTable("DeskAssignments", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.Unit", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentUnitId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ParentUnitId");

                    b.ToTable("Units", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Services.Workflows.Processes.Process", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CurrentStepName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CurrentStep");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Strategy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Processes", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SchedulerApi.Services.Workflows.Steps.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProcessId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProcessId")
                        .HasDatabaseName("IX_Steps_ProcessId");

                    b.ToTable("Steps", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.GathererGptSession", b =>
                {
                    b.HasBaseType("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession");

                    b.Property<string>("ConversationState")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("NotCreated");

                    b.Property<string>("DeskId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ScheduleStartDateTime")
                        .HasColumnType("datetime2");

                    b.ToTable("SchedulerGptSessions", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.ManagerSupportGptSession", b =>
                {
                    b.HasBaseType("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession");

                    b.ToTable("ManagerSupportGptSessions", (string)null);
                });

            modelBuilder.Entity("SchedulerApi.Services.Workflows.Processes.Classes.AutoScheduleProcess", b =>
                {
                    b.HasBaseType("SchedulerApi.Services.Workflows.Processes.Process");

                    b.Property<string>("DeskId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("FileWindowEnd")
                        .HasColumnType("datetime2");

                    b.Property<string>("NextPhaseJobId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ProcessStart")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PublishDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ScheduleEnd")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduleShiftDuration")
                        .HasColumnType("int");

                    b.Property<DateTime>("ScheduleStart")
                        .HasColumnType("datetime2");

                    b.HasIndex("DeskId");

                    b.HasIndex("ScheduleStart")
                        .HasDatabaseName("IX_AutoScheduleProcesses_ScheduleStart");

                    b.ToTable("AutoScheduleProcesses", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession", b =>
                {
                    b.HasOne("SchedulerApi.Models.Entities.Workers.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.Shift", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Desk", "Desk")
                        .WithMany()
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Workers.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.Navigation("Desk");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.ShiftException", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Desk", "Desk")
                        .WithMany()
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Workers.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("DeskId", "ShiftStartDateTime")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");

                    b.Navigation("Employee");

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.ShiftSwap", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Desk", "Desk")
                        .WithMany()
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Workers.Employee", "PreviousEmployee")
                        .WithMany()
                        .HasForeignKey("PreviousEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Shift", "Shift")
                        .WithMany()
                        .HasForeignKey("DeskId", "ShiftStart")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");

                    b.Navigation("PreviousEmployee");

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("SchedulerApi.Models.Entities.Workers.Employee", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.Desk", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("SchedulerApi.Models.Organization.ProcessParameters", "ProcessParameters", b1 =>
                        {
                            b1.Property<string>("DeskId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("CatchRangeString")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasColumnType("nvarchar(max)")
                                .HasDefaultValue("4.00:00:00")
                                .HasColumnName("CatchRange");

                            b1.Property<string>("FileWindowDurationString")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasColumnType("nvarchar(max)")
                                .HasDefaultValue("1.00:00:00")
                                .HasColumnName("FileWindowDuration");

                            b1.Property<string>("HeadsUpDurationString")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasColumnType("nvarchar(max)")
                                .HasDefaultValue("2.12:00:00")
                                .HasColumnName("HeadsUpDuration");

                            b1.HasKey("DeskId");

                            b1.ToTable("Desks");

                            b1.WithOwner()
                                .HasForeignKey("DeskId");
                        });

                    b.Navigation("ProcessParameters")
                        .IsRequired();

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.DeskAssignment", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Desk", "Desk")
                        .WithMany()
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Models.Entities.Workers.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("SchedulerApi.Models.Organization.Unit", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Unit", "ParentUnit")
                        .WithMany()
                        .HasForeignKey("ParentUnitId");

                    b.Navigation("ParentUnit");
                });

            modelBuilder.Entity("SchedulerApi.Services.Workflows.Steps.Step", b =>
                {
                    b.HasOne("SchedulerApi.Services.Workflows.Processes.Process", "Process")
                        .WithMany()
                        .HasForeignKey("ProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Process");
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.GathererGptSession", b =>
                {
                    b.HasOne("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession", null)
                        .WithOne()
                        .HasForeignKey("SchedulerApi.Models.ChatGPT.Sessions.GathererGptSession", "ThreadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchedulerApi.Models.ChatGPT.Sessions.ManagerSupportGptSession", b =>
                {
                    b.HasOne("SchedulerApi.Models.ChatGPT.Sessions.BaseClasses.GptSession", null)
                        .WithOne()
                        .HasForeignKey("SchedulerApi.Models.ChatGPT.Sessions.ManagerSupportGptSession", "ThreadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchedulerApi.Services.Workflows.Processes.Classes.AutoScheduleProcess", b =>
                {
                    b.HasOne("SchedulerApi.Models.Organization.Desk", "Desk")
                        .WithMany()
                        .HasForeignKey("DeskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerApi.Services.Workflows.Processes.Process", null)
                        .WithOne()
                        .HasForeignKey("SchedulerApi.Services.Workflows.Processes.Classes.AutoScheduleProcess", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desk");
                });
#pragma warning restore 612, 618
        }
    }
}
