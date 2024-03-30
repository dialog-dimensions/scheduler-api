using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.Workflows.Processes;
using SchedulerApi.Services.Workflows.Processes.Classes;
using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.DAL;

public class ApiDbContext : IdentityDbContext<IdentityUser>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) {}

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<ShiftException> Exceptions { get; set; }
    public DbSet<ShiftSwap> Swaps { get; set; }

    public DbSet<Process> Processes { get; set; }
    public DbSet<AutoScheduleProcess> AutoScheduleProcesses { get; set; }

    public DbSet<Step> Steps { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Entity-Tables
        modelBuilder.Entity<Employee>()
            .ToTable("Employees")
            .HasKey(emp => emp.Id);

        modelBuilder.Entity<Shift>()
            .ToTable("Shifts")
            .HasKey(shift => shift.StartDateTime);

        modelBuilder.Entity<ShiftException>()
            .ToTable("ShiftExceptions")
            .HasKey(ex => new { ex.ShiftKey, ex.EmployeeId });

        modelBuilder.Entity<ShiftSwap>()
            .ToTable("ShiftSwaps")
            .HasKey(swap => swap.SwapId);

        modelBuilder.Entity<Process>()
            .ToTable("Processes");

        modelBuilder.Entity<AutoScheduleProcess>()
            .ToTable("AutoScheduleProcesses");
        
        modelBuilder.Entity<Step>()
            .ToTable("Steps");
        
        //Relationships
        modelBuilder.Entity<Shift>()
            .HasOne<Employee>(shift => shift.Employee)
            .WithMany()
            .HasForeignKey(shift => shift.EmployeeId);

        modelBuilder.Entity<Shift>()
            .HasMany<ShiftException>()
            .WithOne(ex => ex.Shift)
            .HasForeignKey(ex => ex.ShiftKey);
        
        modelBuilder.Entity<Shift>()
            .HasMany<ShiftSwap>()
            .WithOne(swap => swap.Shift)
            .HasForeignKey(swap => swap.ShiftKey);

        modelBuilder.Entity<ShiftSwap>()
            .HasOne<Employee>(swap => swap.PreviousEmployee)
            .WithMany()
            .HasForeignKey(swap => swap.PreviousEmployeeId);

        modelBuilder.Entity<Step>()
            .HasOne<Process>(step => step.Process)
            .WithMany()
            .HasForeignKey(step => step.ProcessId);
        
        
        //Custom Indexes
        modelBuilder.Entity<Employee>()
            .HasIndex(emp => emp.Name)
            .HasDatabaseName("IX_Employees_Name");

        modelBuilder.Entity<Shift>()
            .HasIndex(shift => shift.ScheduleKey)
            .HasDatabaseName("IX_Shifts_ScheduleKey");

        modelBuilder.Entity<AutoScheduleProcess>()
            .HasIndex(autoProcess => autoProcess.ScheduleStart)
            .HasDatabaseName("IX_AutoScheduleProcesses_ScheduleStart");
        
        modelBuilder.Entity<Step>()
            .HasIndex(step => step.ProcessId)
            .HasDatabaseName("IX_Steps_ProcessId");
        
        
        //ColumnTypes and Conversions
        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Name)
            .HasColumnType("nvarchar(225)");
        
        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Role)
            .HasColumnType("nvarchar(50)");
        
        modelBuilder.Entity<Shift>()
            .Property(shift => shift.ModificationUser)
            .HasColumnType("nvarchar(225)");
        
        modelBuilder.Entity<ShiftException>()
            .Property(ex => ex.ExceptionType)
            .HasColumnType("nvarchar(50)");
        
        modelBuilder.Entity<ShiftException>()
            .Property(ex => ex.ModificationUser)
            .HasColumnType("nvarchar(225)");
        
        modelBuilder.Entity<ShiftSwap>()
            .Property(swap => swap.ModificationUser)
            .HasColumnType("nvarchar(225)");

        modelBuilder.Entity<ShiftSwap>()
            .Property(swap => swap.Status)
            .HasColumnType("nvarchar(50)");

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever();
        });
        
        modelBuilder.Entity<ShiftException>()
            .Property(ex => ex.ExceptionType)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (ExceptionType)Enum.Parse(typeof(ExceptionType), v) // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<ShiftSwap>()
            .Property(swap => swap.ModificationUser)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (User)Enum.Parse(typeof(User), v) // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<ShiftSwap>()
            .Property(swap => swap.Status)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (SwapStatus)Enum.Parse(typeof(SwapStatus), v) // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<Shift>()
            .Property(shift => shift.ModificationUser)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (User)Enum.Parse(typeof(User), v) // Convert string back to enum when reading from the database
            );

        modelBuilder.Entity<Process>()
            .Property(p => p.Status)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (TaskStatus)Enum.Parse(typeof(TaskStatus), v) // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<Process>()
            .Property(p => p.Strategy)
            .HasConversion(
                v => v.GetType().Name, // Convert enum to string when saving to the database
                v => default // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<AutoScheduleProcess>()
            .Property(p => p.Status)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (TaskStatus)Enum.Parse(typeof(TaskStatus), v) // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<AutoScheduleProcess>()
            .Property(p => p.Strategy)
            .HasConversion(
                v => v.GetType().Name, // Convert enum to string when saving to the database
                v => default // Convert string back to enum when reading from the database
            );
        
        modelBuilder.Entity<Step>()
            .Property(s => s.Status)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving to the database
                v => (TaskStatus)Enum.Parse(typeof(TaskStatus), v) // Convert string back to enum when reading from the database
            );
        
        
        //Default expressions
        modelBuilder.Entity<Employee>(entity => entity
                .Property(e => e.Balance)
                .HasDefaultValue(0.0));
        
        modelBuilder.Entity<Employee>(entity => entity
            .Property(e => e.DifficultBalance)
            .HasDefaultValue(0.0));
        
        modelBuilder.Entity<Employee>(entity => entity
            .Property(e => e.Active)
            .HasDefaultValue(true));
        
        modelBuilder.Entity<Employee>(entity => entity
            .Property(e => e.Role)
            .HasDefaultValue("Employee"));

        modelBuilder.Entity<Shift>(entity => entity
            .Property(s => s.ModificationDateTime)
            .HasDefaultValueSql("getdate()"));
        
        modelBuilder.Entity<Shift>(entity => entity
            .Property(s => s.ModificationUser)
            .HasDefaultValue(User.Computer));
        
        modelBuilder.Entity<ShiftException>(entity => entity
            .Property(s => s.ModificationDateTime)
            .HasDefaultValueSql("getdate()"));
        
        modelBuilder.Entity<ShiftException>(entity => entity
            .Property(s => s.ModificationUser)
            .HasDefaultValue(User.Computer));
        
        modelBuilder.Entity<ShiftSwap>(entity => entity
            .Property(s => s.ModificationDateTime)
            .HasDefaultValueSql("getdate()"));
        
        modelBuilder.Entity<ShiftSwap>(entity => entity
            .Property(s => s.Status)
            .HasDefaultValue(SwapStatus.Applied));
        
        modelBuilder.Entity<ShiftSwap>(entity => entity
            .Property(s => s.ModificationUser)
            .HasDefaultValue(User.Computer));
    }
}