using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
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
    public DbSet<Unit> Units { get; set; }
    public DbSet<Desk> Desks { get; set; }
    public DbSet<DeskAssignment> DeskAssignments { get; set; }
    public DbSet<SchedulerGptSession> SchedulerGptSessions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Entity-Tables
        modelBuilder.Entity<Employee>()
            .ToTable("Employees")
            .HasKey(emp => emp.Id);

        modelBuilder.Entity<Shift>()
            .ToTable("Shifts")
            .HasKey(shift => new { shift.DeskId, shift.StartDateTime });

        modelBuilder.Entity<ShiftException>()
            .ToTable("ShiftExceptions")
            .HasKey(ex => new { 
                ex.DeskId, ex.ShiftStartDateTime, ex.EmployeeId });

        modelBuilder.Entity<ShiftSwap>()
            .ToTable("ShiftSwaps")
            .HasKey(swap => swap.SwapId);

        modelBuilder.Entity<Process>()
            .ToTable("Processes");

        modelBuilder.Entity<AutoScheduleProcess>()
            .ToTable("AutoScheduleProcesses");
        
        modelBuilder.Entity<Step>()
            .ToTable("Steps");

        modelBuilder.Entity<Unit>()
            .ToTable("Units")
            .HasKey(unit => unit.Id);

        modelBuilder.Entity<Desk>()
            .ToTable("Desks")
            .HasKey(desk => desk.Id);

        modelBuilder.Entity<DeskAssignment>()
            .ToTable("DeskAssignments")
            .HasKey(da => new { da.DeskId, da.EmployeeId });

        modelBuilder.Entity<SchedulerGptSession>()
            .ToTable("SchedulerGptSessions")
            .HasKey(session => session.ThreadId);
        
        //Relationships
        modelBuilder.Entity<Shift>()
            .HasOne<Employee>(shift => shift.Employee)
            .WithMany()
            .HasForeignKey(shift => shift.EmployeeId);

        modelBuilder.Entity<Shift>()
            .HasMany<ShiftException>()
            .WithOne(ex => ex.Shift)
            .HasForeignKey(ex => new { ex.DeskId, ex.ShiftStartDateTime });

        modelBuilder.Entity<Shift>()
            .HasMany<ShiftSwap>()
            .WithOne(swap => swap.Shift)
            .HasForeignKey(swap => new { swap.DeskId, swap.ShiftStart });

        modelBuilder.Entity<ShiftSwap>()
            .HasOne<Employee>(swap => swap.PreviousEmployee)
            .WithMany()
            .HasForeignKey(swap => swap.PreviousEmployeeId);

        modelBuilder.Entity<Step>()
            .HasOne<Process>(step => step.Process)
            .WithMany()
            .HasForeignKey(step => step.ProcessId);
        
        modelBuilder.Entity<Unit>()
            .HasMany<Desk>()
            .WithOne(desk => desk.Unit)
            .HasForeignKey(desk => desk.UnitId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Unit>()
            .HasMany<Employee>()
            .WithOne(emp => emp.Unit)
            .HasForeignKey(emp => emp.UnitId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Desk>()
            .HasMany<DeskAssignment>()
            .WithOne(da => da.Desk)
            .HasForeignKey(da => da.DeskId);

        modelBuilder.Entity<Employee>()
            .HasMany<DeskAssignment>()
            .WithOne(da => da.Employee)
            .HasForeignKey(da => da.EmployeeId);

        modelBuilder.Entity<Desk>()
            .HasMany<Shift>()
            .WithOne(shift => shift.Desk)
            .HasForeignKey(shift => shift.DeskId);

        modelBuilder.Entity<Desk>()
            .HasMany<ShiftException>()
            .WithOne(shift => shift.Desk)
            .HasForeignKey(shift => shift.DeskId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Desk>()
            .HasMany<ShiftSwap>()
            .WithOne(swap => swap.Desk)
            .HasForeignKey(swap => swap.DeskId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Employee>()
            .HasMany<SchedulerGptSession>()
            .WithOne(session => session.Employee)
            .HasForeignKey(session => session.EmployeeId);
        
        //Custom Indexes
        modelBuilder.Entity<Employee>()
            .HasIndex(emp => emp.Name)
            .HasDatabaseName("IX_Employees_Name");
        
        modelBuilder.Entity<Shift>()
            .HasIndex(shift => shift.ScheduleStartDateTime)
            .HasDatabaseName("IX_Shifts_ScheduleStartDateTime");

        modelBuilder.Entity<AutoScheduleProcess>()
            .HasIndex(autoProcess => autoProcess.ScheduleStart)
            .HasDatabaseName("IX_AutoScheduleProcesses_ScheduleStart");
        
        modelBuilder.Entity<Step>()
            .HasIndex(step => step.ProcessId)
            .HasDatabaseName("IX_Steps_ProcessId");

        modelBuilder.Entity<DeskAssignment>()
            .HasIndex(da => da.DeskId)
            .HasDatabaseName("IX_DeskAssignments_DeskId");

        modelBuilder.Entity<DeskAssignment>()
            .HasIndex(da => da.EmployeeId)
            .HasDatabaseName("IX_DeskAssignments_EmployeeId");
        
        
        //ColumnTypes and Conversions
        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Name)
            .HasColumnType("nvarchar(225)");
        
        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Role)
            .HasColumnType("nvarchar(50)");
        
        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Gender)
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

        modelBuilder.Entity<SchedulerGptSession>()
            .Property(session => session.ConversationState)
            .HasColumnType("nvarchar(50)");

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<Employee>()
            .Property(emp => emp.Gender)
            .HasConversion(
                g => g.ToString(), // Convert enum to string when saving to the database
                g => (Gender)Enum.Parse(typeof(Gender), g) // Convert string back to enum when reading from the database
            );
        
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
        
        modelBuilder.Entity<SchedulerGptSession>()
            .Property(session => session.ConversationState)
            .HasConversion(
                state => state.ToString(), // Convert enum to string when saving to the database
                state => (ShabtzanGptConversationState)Enum.Parse(typeof(ShabtzanGptConversationState), state) // Convert string back to enum when reading from the database
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
        
        modelBuilder.Entity<Employee>(entity => entity
            .Property(e => e.Gender)
            .HasDefaultValue(Gender.Unknown));

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
        
        modelBuilder.Entity<SchedulerGptSession>(entity => entity
            .Property(session => session.ConversationState)
            .HasDefaultValue(ShabtzanGptConversationState.NotCreated));
    }
}