﻿using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresSeguridad;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AuthorBook>().HasKey(x => new { x.AuthorId, x.BookId });
    }

    public DbSet<Author> Autores { get; set; }
    public DbSet<Book> Libros { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AuthorBook> AutoresLibros { get; set; }
}
