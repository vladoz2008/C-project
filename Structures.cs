using System;

namespace StudentGradesSystem
{
    public struct Book : IEntity
    {
        public int BookCode { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public string Genre { get; set; }

        public string DisplayName => $"[{BookCode}] {Title}";
    }

    public struct Reader : IEntity
    {
        public int ReaderCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }

        public string DisplayName => $"[{ReaderCode}] {LastName} {FirstName}";
    }

    public struct LibraryCard : IEntity
    {
        public int BookCode { get; set; }
        public int ReaderCode { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Returned { get; set; }

        public string DisplayName => $"Книга {BookCode} -> Читатель {ReaderCode}";
    }
}
