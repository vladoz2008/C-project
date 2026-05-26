using System;

namespace StudentGradesSystem
{
    public abstract class Entity : IEntity
    {
        public abstract string DisplayName { get; }
    }

    public class Book : Entity
    {
        public int BookCode { get; set; }
        public string AuthorFirstName { get; set; } = string.Empty;
        public string AuthorLastName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string Genre { get; set; } = string.Empty;

        public override string DisplayName => $"[{BookCode}] {Title}";
    }

    public class Reader : Entity
    {
        public int ReaderCode { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Age { get; set; }

        public override string DisplayName => $"[{ReaderCode}] {LastName} {FirstName}";
    }

    public class LibraryCard : Entity
    {
        public int BookCode { get; set; }
        public int ReaderCode { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Returned { get; set; }

        public override string DisplayName => $"Книга {BookCode} -> Читатель {ReaderCode}";
    }
}
