using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookstoreAPI.Models
{
    public partial class BookStoreContext : DbContext
    {
        public BookStoreContext()
        {
        }

        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Collection> Collections { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Authors).HasMaxLength(100);

                entity.Property(e => e.Genre).HasMaxLength(50);

                entity.Property(e => e.InfoLink)
                    .HasMaxLength(200)
                    .HasColumnName("Info_Link");

                entity.Property(e => e.PublishedDate)
                    .HasMaxLength(5)
                    .HasColumnName("Published_Date");

                entity.Property(e => e.SmallThumbnail)
                    .HasMaxLength(200)
                    .HasColumnName("Small_Thumbnail");

                entity.Property(e => e.Thumbnail).HasMaxLength(200);

                entity.Property(e => e.Titles).HasMaxLength(100);
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.ToTable("Collection");

                entity.Property(e => e.IdBook).HasColumnName("Id_Book");

                entity.Property(e => e.IdUser).HasColumnName("Id_User");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Rol).HasMaxLength(10);
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlist");

                entity.Property(e => e.IdBook).HasColumnName("Id_Book");

                entity.Property(e => e.IdUser).HasColumnName("Id_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
