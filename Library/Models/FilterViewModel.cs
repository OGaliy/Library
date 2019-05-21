using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Author> authors, int? author, List<Genre> genres, int? genre, List<Language> languages, int? language, string name)
        {
            authors.Insert(0, new Author { Name = "All", Id = 0 });
            Authors = new SelectList(authors, "Id", "Name", author);
            SelectedAuthor = author;
            genres.Insert(0, new Genre { Name = "All", Id = 0 });
            Genres = new SelectList(genres, "Id", "Name", genre);
            SelectedGenre = genre;
            languages.Insert(0, new Language { Name = "All", Id = 0 });
            Languages = new SelectList(languages, "Id", "Name", language);
            SelectedLanguage = language;
            SelectedName = name;
        }
        public SelectList Authors { get; private set; }
        public int? SelectedAuthor { get; private set; }
        public SelectList Genres { get; private set; }
        public int? SelectedGenre { get; private set; }
        public SelectList Languages { get; private set; }
        public int? SelectedLanguage { get; private set; }
        public string SelectedName { get; private set; }
    }
    public class UserFilter
    {
        public string Email { get; set; }
        public UserFilter(string email)
        {
            Email = email;
        }
    }
}