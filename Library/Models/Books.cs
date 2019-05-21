using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "The book's title")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "Number of pages")]
        [Range(1,10000,ErrorMessage = "Number of pages can't be less than 1 and more than 10000!")]
        public int Pages { get; set; }
        [Display(Name = "Image")]
        public byte[] Image { get; set; }
        [Display(Name = "Book file")]
        public byte[] BookFile { get; set; }

        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "About book")]
        public string About { get; set; }
        [Display(Name = "Genre")]

      //  [Required(ErrorMessage = "Field must be fill")]
        public int genreId { get; set; }
        [Display(Name = "Genre")]
        public Genre Genre { get; set; }
        [Display(Name = "Author's name")]

        [Required(ErrorMessage = "Field must be fill")]
        public int authorId { get; set; }
        [Display(Name = "Author's name")]
        public Author Author { get; set; }
        [Display(Name = "Language of book")]

        [Required(ErrorMessage = "Field must be fill")]
        public int languageId { get; set; }
        [Display(Name = "Language of book")]
        public Language Language { get; set; }

        public ICollection<User> users { get; set; }
        public Book()
        {
            users = new List<User>();
        }
    }
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "Genre")]
        public string Name { get; set; }

        List<Book> books { get; set; }
        public Genre()
        {
            books = new List<Book>();
        }

    }
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "Author's name")]
        public string Name { get; set; }

        List<Book> books { get; set; }
        public Author()
        {
            books = new List<Book>();
        }
    }
    public class Language
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be fill")]
        [Display(Name = "Language")]
        public string Name { get; set; }

        List<Book> books { get; set; }
        public Language()
        {
            books = new List<Book>();
        }
    }

    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(c => c.books)
            .WithMany(s => s.users)
            .Map(t => t.MapLeftKey("UserId")
            .MapRightKey("BookId")
            .ToTable("UserBook"));
        }
    }
}