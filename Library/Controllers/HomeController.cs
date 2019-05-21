using Library.Models;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        BookContext db = new BookContext();


        //Work with books
        //Output all books
        public async Task<ActionResult> Books(int? author, int? genre, int? language, string name, int page = 1)
        {
            var user = db.Users.Include(x => x.books).FirstOrDefault(x => x.Email == User.Identity.Name);
            if(user != null)
            {
                ViewBag.Count = user.books.Count;
            }
            int pageSize = 6;
            IQueryable<Book> books = db.Books.Include(a => a.Author).Include(g => g.Genre).Include(l => l.Language);
            books = Filters(author, genre, language, books, name);
            //Paging
            var count = await books.CountAsync();
            var items = await books.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                FilterViewModel = new FilterViewModel(db.Authors.ToList(), author, db.Genres.ToList(), genre, db.Languages.ToList(), language, name),
                Books = items
            };

            return View(viewModel);
        }

        //Outout books of user
        [Authorize]
        public ActionResult MyBooks()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userBook = db.Users.Include(x => x.books).FirstOrDefault(x => x.Email == User.Identity.Name);
                return View(userBook);
            }
            return HttpNotFound();
        }


        //Show books
        public ActionResult GetBytes(int id)
        {
            Book book = db.Books.Find(id);
            byte[] array = book.BookFile;
            MemoryStream pdfStream = new MemoryStream();
            pdfStream.Write(array, 0, array.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }


        //Add to User books        
        [HttpPost]
        [Authorize]
        public ActionResult AddMy(int? id)
        {
            User user = db.Users.Include(x=>x.books).FirstOrDefault(u => u.Email == User.Identity.Name);
            if(user.books.Count < 6)
            {
                Book book = db.Books.Find(id);
                user.books.Add(book);
                db.SaveChanges();
                return RedirectToAction("MyBooks");
            }
            ModelState.AddModelError("", "You can add 6 books!");
            return RedirectToAction("Books");
        }

        //Delete from User books
        [HttpPost]
        [Authorize]
        public ActionResult DeleteMy(int id)
        {
            var books = db.Books.Include(x => x.users);
            Book b = db.Books.Find(id);
            Book book = books.FirstOrDefault(x => x.Name == b.Name);
            User user = book.users.First(x => x.Email == User.Identity.Name);
            user.books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("MyBooks");
        }

        //Add book
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateBook()
        {
            SelectList genres = new SelectList(db.Genres, "Id", "Name");
            SelectList authors = new SelectList(db.Authors, "Id", "Name");
            SelectList languages = new SelectList(db.Languages, "Id", "Name");
            ViewBag.Genre = genres;
            ViewBag.Author = authors;
            ViewBag.Language = languages;
            return PartialView();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateBook(Book book, HttpPostedFileBase uploadImage, HttpPostedFileBase uploadBook)
        {
            SelectList genres = new SelectList(db.Genres, "Id", "Name");
            SelectList authors = new SelectList(db.Authors, "Id", "Name");
            SelectList languages = new SelectList(db.Languages, "Id", "Name");
            ViewBag.Genre = genres;
            ViewBag.Author = authors;
            ViewBag.Language = languages;

            if (ModelState.IsValid && uploadImage != null && uploadBook != null)
            {
                byte[] imageDate = null;
                byte[] bookDate = null;

                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageDate = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                using (var binaryReader = new BinaryReader(uploadBook.InputStream))
                {
                    bookDate = binaryReader.ReadBytes(uploadBook.ContentLength);
                }

                book.Image = imageDate;
                book.BookFile = bookDate;

                Book b = null;
                b = db.Books.FirstOrDefault(x => x.Name == book.Name && x.authorId == book.authorId && x.genreId == book.genreId
                                             && x.languageId == book.languageId && x.authorId == book.authorId && x.Image == book.Image);
                if (b != null)
                {
                    ModelState.AddModelError("", "The book's alredy!");
                    return View(book);
                }
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            ModelState.AddModelError("", "Choose image or book file!");

            return View(book);
        }

        //Books Edit methods
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditBook(int id)
        {
            SelectList genres = new SelectList(db.Genres, "Id", "Name");
            SelectList authors = new SelectList(db.Authors, "Id", "Name");
            SelectList languages = new SelectList(db.Languages, "Id", "Name");
            ViewBag.Genre = genres;
            ViewBag.Author = authors;
            ViewBag.Language = languages;
            Book book = db.Books.Include(g => g.Genre).Include(a => a.Author).Include(l => l.Language).FirstOrDefault(x => x.Id == id);
            if (book == null)
                return HttpNotFound();
            return View(book);
        }
        [HttpPost]
        public ActionResult EditBook(Book book, HttpPostedFileBase uploadImage, HttpPostedFileBase uploadBook)
        {
            SelectList genres = new SelectList(db.Genres, "Id", "Name");
            SelectList authors = new SelectList(db.Authors, "Id", "Name");
            SelectList languages = new SelectList(db.Languages, "Id", "Name");
            ViewBag.Genre = genres;
            ViewBag.Author = authors;
            ViewBag.Language = languages;
            if (ModelState.IsValid && uploadBook != null)
            {
                byte[] imageDate = null;

                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageDate = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                book.Image = imageDate;
                byte[] bookDate = null;

                using (var binaryReader = new BinaryReader(uploadBook.InputStream))
                {
                    imageDate = binaryReader.ReadBytes(uploadBook.ContentLength);
                }
                book.BookFile = bookDate;

                Book b = null;
                b = db.Books.FirstOrDefault(x => x.Name == book.Name && x.authorId == book.authorId && x.genreId == book.genreId
                                              && x.languageId == book.languageId && x.authorId == book.authorId && x.Image == book.Image);
                if (b != null)
                {
                    ModelState.AddModelError("", "You didn't change anythink!");
                    return View(book);
                }
                var e = db.Entry(book);
                if (e.State == EntityState.Detached)
                    db.Books.Attach(book);
                db.Entry(book).State = EntityState.Modified;
                //db.Books.Attach(book);
                db.SaveChanges();
                return RedirectToAction("List");
            }
            else
                ModelState.AddModelError("", "Choose image!");
            return View(book);
        }

        //Delete books
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //List books for admin show
        [Authorize(Roles = "Admin")]
        public ActionResult ListBook(int? author, int? genre, int? language, string name, int page = 1)
        {
            int pageSize = 15;
            IQueryable<Book> books = db.Books.Include(a => a.Author).Include(g => g.Genre).Include(l => l.Language);
            books = Filters(author, genre, language, books, name);
            //Paging
            var count = books.Count();
            var items = books.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                FilterViewModel = new FilterViewModel(db.Authors.ToList(), author, db.Genres.ToList(), genre, db.Languages.ToList(), language, name),
                Books = items
            };

            return View(viewModel);
        }

        //------------------------------------------------------------------------------
        //Work with authors
        //Add author
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateAuthor()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateAuthor(Author author)
        {
            if (ModelState.IsValid)
            {
                Author a = null;
                a = db.Authors.FirstOrDefault(x => x.Name == author.Name);
                if (a != null)
                {
                    ModelState.AddModelError("", "The author's alredy!");
                    return View(author);
                }

                db.Authors.Add(author);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(author);
        }

        //Edit authors
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditAuthor(int id)
        {
            Author a = db.Authors.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }

            return View(a);
        }
        [HttpPost]
        public ActionResult EditAuthor(Author author)
        {
            db.Entry(author).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //Delete authors
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteAuthor(int id)
        {
            Author author = db.Authors.Find(id);
            if (author == null)
                return HttpNotFound();
            db.Authors.Remove(author);
            db.SaveChanges();
            return RedirectToAction("List");
        }


        //List authors for admin show
        [Authorize(Roles = "Admin")]
        public ActionResult ListAuthor()
        {
            var author = db.Authors;
            return PartialView(author);
        }
        //------------------------------------------------------------------------------
        //Work with genres

        //Add genre
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateGenre()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateGenre(Genre genre)
        {
            if (ModelState.IsValid)
            {
                Genre g = null;
                g = db.Genres.FirstOrDefault(x => x.Name == genre.Name);
                if (g != null)
                {
                    ModelState.AddModelError("", "The genre's alredy!");
                    return View(genre);
                }
                db.Genres.Add(genre);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(genre);
        }

        //Edit genres
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditGenre(int id)
        {
            Genre genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();
            return View(genre);
        }
        [HttpPost]
        public ActionResult EditGenre(Genre genre)
        {
            var e = db.Entry(genre);
            if (e.State == EntityState.Detached)
                db.Genres.Attach(genre);
            db.Entry(genre).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("List");
        }

        //Delete generes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteGenre(int id)
        {
            Genre genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();
            db.Genres.Remove(genre);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //List genres for admin show
        [Authorize(Roles = "Admin")]
        public ActionResult ListGenre()
        {
            var genre = db.Genres;
            return PartialView(genre);
        }
        //------------------------------------------------------------------------------
        //Work with languages

        //Add language
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateLanguage()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateLanguage(Language language)
        {
            if (ModelState.IsValid)
            {
                Language l = null;
                l = db.Languages.FirstOrDefault(x => x.Name == language.Name);
                if (l != null)
                {
                    ModelState.AddModelError("", "The language's alredy!");
                    return View(language);
                }
                db.Languages.Add(language);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(language);
        }

        //Edit language
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditLanguage(int id)
        {
            Language language = db.Languages.Find(id);
            if (language == null)
                return HttpNotFound();
            return View(language);
        }
        [HttpPost]
        public ActionResult EditLanguage(Language language)
        {
            var e = db.Entry(language);
            if (e.State == EntityState.Detached)
                db.Languages.Attach(language);
            db.Entry(language).State = EntityState.Modified;
            //db.Books.Attach(book);
            db.SaveChanges();

            return RedirectToAction("List");
        }

        //Delete languages
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteLanguage(int id)
        {
            Language language = db.Languages.Find(id);
            if (language == null)
                return HttpNotFound();
            db.Languages.Remove(language);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //List languages for admin show
        [Authorize(Roles = "Admin")]
        public ActionResult ListLanguage()
        {
            var language = db.Languages;
            return PartialView(language);
        }
        //------------------------------------------------------------------------------
        //Work with users
        //List users
        public async Task<ActionResult> ListUser(string email,int page = 1)
        {
            IQueryable<User> users = db.Users.Include(r => r.Role);
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(b => b.Email.Contains(email));
            }
            int itemsInPage = 20;
            var count = await users.CountAsync();
            var items = await users.OrderBy(x => x.Id).Skip((page - 1) * itemsInPage).Take(itemsInPage).ToListAsync();
            UserViewModel userViewModel = new UserViewModel
            {
                UserFilter = new UserFilter(email),
                PageViewModel = new PageViewModel(count, page, itemsInPage),
                Users = items
            };
            return View(userViewModel);
        }

       
        //Edit user methods
        [HttpGet]
        public ActionResult EditUser(int id)
        {
            User user = db.Users.Find(id);
            SelectList roles = new SelectList(db.Roles, "Id", "Name");
            ViewBag.Role = roles;
            if (user == null)
                return HttpNotFound();
            return View(user);
        }
        [HttpPost]
        public ActionResult EditUser(User user)
        {
            var e = db.Entry(user);
            if (e.State == EntityState.Detached)
                db.Users.Attach(user);
            user.roleId = 1;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListUser");
        }

        //Delete user methods
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }
        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteUserConfirm(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("ListUser");
        }

        //------------------------------------------------------------------------------
        //Other methods

        //Combined create methods
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //Combined list methods
        [Authorize(Roles = "Admin")]
        public ActionResult List()
        {
            return View();
        }

        //Filter book method
        public IQueryable<Book> Filters(int? author, int? genre, int? language, IQueryable<Book> books, string name)
        {
            if (author != null && author != 0)
            {
                books = books.Where(a => a.authorId == author);
            }
            if (genre != null && genre != 0)
            {
                books = books.Where(g => g.genreId == genre);
            }
            if (language != null && language != 0)
            {
                books = books.Where(l => l.languageId == language);
            }
            if (!string.IsNullOrEmpty(name))
            {
                books = books.Where(b => b.Name.Contains(name));
            }
            return books;
        }
    }
}