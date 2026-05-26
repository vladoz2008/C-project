using System;
using System.Text.Json.Serialization;

namespace StudentGradesSystem
{
    internal abstract class Entity : IEntity
    {
        public abstract string DisplayName { get; }
    }

    internal class Book : Entity
    {
        [JsonConstructor]
        public Book(int bookCode, string authorFirstName, string authorLastName, string title, int publishYear, string genre)
        {
            BookCode = bookCode;
            AuthorFirstName = authorFirstName;
            AuthorLastName = authorLastName;
            Title = title;
            PublishYear = publishYear;
            Genre = genre;
        }

        public int BookCode { get; }
        public string AuthorFirstName { get; }
        public string AuthorLastName { get; }
        public string Title { get; }
        public int PublishYear { get; }
        public string Genre { get; }

        public override string DisplayName => $"[{BookCode}] {Title}";
    }

    internal class Reader : Entity
    {
        [JsonConstructor]
        public Reader(int readerCode, string firstName, string lastName, string address, int age)
        {
            ReaderCode = readerCode;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Age = age;
        }

        public int ReaderCode { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; }
        public int Age { get; }

        public override string DisplayName => $"[{ReaderCode}] {LastName} {FirstName}";
    }

    internal class LibraryCard : Entity
    {
        [JsonConstructor]
        public LibraryCard(int bookCode, int readerCode, DateTime issueDate, bool returned)
        {
            BookCode = bookCode;
            ReaderCode = readerCode;
            IssueDate = issueDate;
            Returned = returned;
        }

        public int BookCode { get; }
        public int ReaderCode { get; }
        public DateTime IssueDate { get; }
        public bool Returned { get; }

        public override string DisplayName => $"Книга {BookCode} -> Читатель {ReaderCode}";
    }
}
