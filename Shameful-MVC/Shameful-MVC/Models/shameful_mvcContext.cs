using Microsoft.EntityFrameworkCore;

namespace Shameful_MVC.Models
{
    public partial class shameful_mvcContext : DbContext
    {
        public shameful_mvcContext()
        {
        }

        public shameful_mvcContext(DbContextOptions<shameful_mvcContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assignments");

                entity.Property(e => e.File)
                    .IsRequired()
                    .HasColumnName("assignment")
                    .HasMaxLength(1);

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.FacultyNumber)
                    .HasName("PK__students__C0E776E9106F8DAD");

                entity.ToTable("students");

                entity.Property(e => e.FacultyNumber)
                    .HasColumnName("facultyNumber")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FormOfEducation)
                    .IsRequired()
                    .HasColumnName("formOfEducation")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasColumnName("middleName")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Specialty)
                    .IsRequired()
                    .HasColumnName("specialty")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Year).HasColumnName("year");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
