using System.IO;

namespace LibraryInformationSystem
{
    public struct Book : IEntity
    {
        public int Id;
        public int AuthorId;
        public string Title;
        public int ReleaseYear;
        public string Genre;

        public void WriteToBinary(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(AuthorId);
            writer.Write(PadString(Title, 50));
            writer.Write(ReleaseYear);
            writer.Write(PadString(Genre, 30));
        }

        public static Book ReadFromBinary(BinaryReader reader)
        {
            Book book = new Book();
            book.Id = reader.ReadInt32();
            book.AuthorId = reader.ReadInt32();
            book.Title = reader.ReadString().Trim();
            book.ReleaseYear = reader.ReadInt32();
            book.Genre = reader.ReadString().Trim();
            return book;
        }

        private static string PadString(string str, int length) =>
            (str ?? "").PadRight(length).Substring(0, length);
    }

    public struct Author : IEntity
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Country;

        public void WriteToBinary(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(PadString(FirstName, 30));
            writer.Write(PadString(LastName, 30));
            writer.Write(PadString(Country, 30));
        }

        public static Author ReadFromBinary(BinaryReader reader)
        {
            Author author = new Author();
            author.Id = reader.ReadInt32();
            author.FirstName = reader.ReadString().Trim();
            author.LastName = reader.ReadString().Trim();
            author.Country = reader.ReadString().Trim();
            return author;
        }

        private static string PadString(string str, int length) =>
            (str ?? "").PadRight(length).Substring(0, length);
    }

    public struct Loan : IEntity
    {
        public int Id;
        public int BookId;
        public string ReaderName;
        public int LoanDays;

        public void WriteToBinary(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(BookId);
            writer.Write(PadString(ReaderName, 50));
            writer.Write(LoanDays);
        }

        public static Loan ReadFromBinary(BinaryReader reader)
        {
            Loan loan = new Loan();
            loan.Id = reader.ReadInt32();
            loan.BookId = reader.ReadInt32();
            loan.ReaderName = reader.ReadString().Trim();
            loan.LoanDays = reader.ReadInt32();
            return loan;
        }

        private static string PadString(string str, int length) =>
            (str ?? "").PadRight(length).Substring(0, length);
    }
}
