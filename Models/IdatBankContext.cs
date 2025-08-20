using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace idat_bank.Models;

public partial class IdatBankContext : DbContext
{
    public IdatBankContext()
    {
    }

    public IdatBankContext(DbContextOptions<IdatBankContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<Transferencia> Transferencias { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-MDIP876\\SQLEXPRESS;Initial Catalog=idat_bank;Integrated Security=SSPI;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cuentas__3214EC0770672515");

            entity.Property(e => e.Saldo).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cuentas__Usuario__32E0915F");
        });

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC074644D539");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Tipo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CuentaDestino).WithMany(p => p.TransferenciaCuentaDestinos)
                .HasForeignKey(d => d.CuentaDestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transfere__Cuent__36B12243");

            entity.HasOne(d => d.CuentaOrigen).WithMany(p => p.TransferenciaCuentaOrigens)
                .HasForeignKey(d => d.CuentaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transfere__Cuent__35BCFE0A");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3213E83F788670AF");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__AB6E6164F3635857").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contraseña");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
