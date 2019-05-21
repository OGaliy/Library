using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Library.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public int roleId { get; set; }
        [Display(Name = "Role")]
        public Role Role { get; set; }

        public ICollection<Book> books { get; set; }
        
        public User()
        {
            books = new List<Book>();
        }
    }
    public class Role
    {
        public int Id { get; set; }
        [Display(Name = "Role")]
        public string Name { get; set; }
        List<User> users { get; set; }
        public Role()
        {
            users = new List<User>();
        }
    }

    public class AccountContext : DbContext
    {
        public AccountContext() : base("BookContext")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}