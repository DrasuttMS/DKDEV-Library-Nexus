using BlazorBiblioteca.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorBiblioteca.Context;

public class LibroDBContext : DbContext
{
    public LibroDBContext(DbContextOptions<LibroDBContext> options)
        : base(options)
    {
    }

    public DbSet<Libro> Libros => Set<Libro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.ToTable("Libros", "dbo");
            entity.HasKey(libro => libro.Id);

            entity.Property(libro => libro.Id)
                .UseIdentityColumn(1, 1);

            entity.Property(libro => libro.NombreLibro)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(libro => libro.Autor)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(libro => libro.NumPaginas)
                .IsRequired();

            entity.Property(libro => libro.FechaPublicacion)
                .HasColumnType("date")
                .IsRequired();
        });
    }
}