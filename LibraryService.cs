using System;
using System.IO;
using System.Collections.Generic;

namespace LibraryInformationSystem
{
    public class LibraryService : ILibraryService
    {
        private const string BooksFile = "books.dat";
        private const string AuthorsFile = "authors.dat";
        private const string LoansFile = "loans.dat";

        public void InitializeDefaultData()
        {
            if (!File.Exists(AuthorsFile))
            {
                using (var bw = new BinaryWriter(File.Open(AuthorsFile, FileMode.Create)))
                {
                    new Author { Id = 1, FirstName = "Лев", LastName = "Толстой", Country = "Россия" }.WriteToBinary(bw);
                    new Author { Id = 2, FirstName = "Джордж", LastName = "Оруэлл", Country = "Великобритания" }.WriteToBinary(bw);
                }
            }
            if (!File.Exists(BooksFile))
            {
                using (var bw = new BinaryWriter(File.Open(BooksFile, FileMode.Create)))
                {
                    new Book { Id = 101, AuthorId = 1, Title = "Война и мир", ReleaseYear = 1869, Genre = "Роман" }.WriteToBinary(bw);
                    new Book { Id = 102, AuthorId = 2, Title = "1984", ReleaseYear = 1949, Genre = "Антиутопия" }.WriteToBinary(bw);
                }
            }
            if (!File.Exists(LoansFile))
            {
                using (var bw = new BinaryWriter(File.Open(LoansFile, FileMode.Create)))
                {
                    new Loan { Id = 1, BookId = 101, ReaderName = "Иванов И.И.", LoanDays = 14 }.WriteToBinary(bw);
                }
            }
        }

        public void AddDataMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЛЕНИЕ ДАННЫХ ===");
            Console.WriteLine("1 - Добавить Книгу | 2 - Добавить Автора | 3 - Добавить Выдачу");
            Console.Write("Выбор: ");
            string type = Console.ReadLine();

            if (type == "1")
            {
                Book b = new Book();
                Console.Write("ID Книги: "); b.Id = int.Parse(Console.ReadLine());
                Console.Write("ID Автора: "); b.AuthorId = int.Parse(Console.ReadLine());
                Console.Write("Название: "); b.Title = Console.ReadLine();
                Console.Write("Год издания: "); b.ReleaseYear = int.Parse(Console.ReadLine());
                Console.Write("Жанр: "); b.Genre = Console.ReadLine();

                using (var bw = new BinaryWriter(File.Open(BooksFile, FileMode.Append))) b.WriteToBinary(bw);
            }
            else if (type == "2")
            {
                Author a = new Author();
                Console.Write("ID Автора: "); a.Id = int.Parse(Console.ReadLine());
                Console.Write("Имя: "); a.FirstName = Console.ReadLine();
                Console.Write("Фамилия: "); a.LastName = Console.ReadLine();
                Console.Write("Страна: "); a.Country = Console.ReadLine();

                using (var bw = new BinaryWriter(File.Open(AuthorsFile, FileMode.Append))) a.WriteToBinary(bw);
            }
            else if (type == "3")
            {
                Loan l = new Loan();
                Console.Write("ID Выдачи: "); l.Id = int.Parse(Console.ReadLine());
                Console.Write("ID Книги: "); l.BookId = int.Parse(Console.ReadLine());
                Console.Write("ФИО Читателя: "); l.ReaderName = Console.ReadLine();
                Console.Write("Срок выдачи (дней): "); l.LoanDays = int.Parse(Console.ReadLine());

                using (var bw = new BinaryWriter(File.Open(LoansFile, FileMode.Append))) l.WriteToBinary(bw);
            }
            Console.WriteLine("Запись успешно добавлена!");
            Pause();
        }

        public void ViewDataMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ПРОСМОТР ДАННЫХ ===");
            Console.WriteLine("1 - Книги | 2 - Авторы | 3 - Выдачи");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine($"{"ID",-6} | {"ID Авт.",-8} | {"Название книги",-25} | {"Год",-6} | {"Жанр",-15}");
                Console.WriteLine(new string('-', 70));
                using (var br = new BinaryReader(File.Open(BooksFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Book b = Book.ReadFromBinary(br);
                        Console.WriteLine($"{b.Id,-6} | {b.AuthorId,-8} | {b.Title,-25} | {b.ReleaseYear,-6} | {b.Genre,-15}");
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine($"{"ID",-6} | {"Имя",-15} | {"Фамилия",-15} | {"Страна",-15}");
                Console.WriteLine(new string('-', 60));
                using (var br = new BinaryReader(File.Open(AuthorsFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Author a = Author.ReadFromBinary(br);
                        Console.WriteLine($"{a.Id,-6} | {a.FirstName,-15} | {a.LastName,-15} | {a.Country,-15}");
                    }
                }
            }
            else if (choice == "3")
            {
                Console.WriteLine($"{"ID",-6} | {"ID Книги",-8} | {"Читатель",-25} | {"Срок (дн)",-10}");
                Console.WriteLine(new string('-', 60));
                using (var br = new BinaryReader(File.Open(LoansFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Loan l = Loan.ReadFromBinary(br);
                        Console.WriteLine($"{l.Id,-6} | {l.BookId,-8} | {l.ReaderName,-25} | {l.LoanDays,-10}");
                    }
                }
            }
            Pause();
        }

        public void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК И ФИЛЬТРАЦИЯ ===");
            Console.WriteLine("1 - Название книги | 2 - Минимальный год | 3 - Страна автора");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите название: ");
                string query = Console.ReadLine().ToLower();
                using (var br = new BinaryReader(File.Open(BooksFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Book b = Book.ReadFromBinary(br);
                        if (b.Title.ToLower().Contains(query))
                            Console.WriteLine($"Найдено: ID {b.Id} - \"{b.Title}\"");
                    }
                }
            }
            else if (choice == "2")
            {
                Console.Write("Введите год: ");
                int year = int.Parse(Console.ReadLine());
                using (var br = new BinaryReader(File.Open(BooksFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Book b = Book.ReadFromBinary(br);
                        if (b.ReleaseYear >= year)
                            Console.WriteLine($"ID {b.Id} - \"{b.Title}\" ({b.ReleaseYear})");
                    }
                }
            }
            else if (choice == "3")
            {
                Console.Write("Введите страну: ");
                string country = Console.ReadLine().ToLower();
                using (var br = new BinaryReader(File.Open(AuthorsFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Author a = Author.ReadFromBinary(br);
                        if (a.Country.ToLower().Contains(country))
                            Console.WriteLine($"Автор: {a.FirstName} {a.LastName} ({a.Country})");
                    }
                }
            }
            Pause();
        }

        public void SummaryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== СУММАРНЫЕ ХАРАКТЕРИСТИКИ ===");
            Console.WriteLine("1 - Общее количество книг | 2 - Средний срок выдачи");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                int count = 0;
                using (var br = new BinaryReader(File.Open(BooksFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length) { Book.ReadFromBinary(br); count++; }
                }
                Console.WriteLine($"Всего книг: {count}");
            }
            else if (choice == "2")
            {
                int totalDays = 0, count = 0;
                using (var br = new BinaryReader(File.Open(LoansFile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Loan l = Loan.ReadFromBinary(br);
                        totalDays += l.LoanDays; count++;
                    }
                }
                Console.WriteLine($"Средний срок чтения: {(count > 0 ? (double)totalDays / count : 0):F1} дней.");
            }
            Pause();
        }

        public void SortMenu()
        {
            Console.Clear();
            Console.WriteLine("=== СОРТИРОВКА КНИГ ===");
            Console.WriteLine("1 - По Году Издания | 2 - По Названию");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine();

            var list = new List<Book>();
            if (!File.Exists(BooksFile)) return;

            using (var br = new BinaryReader(File.Open(BooksFile, FileMode.Open)))
            {
                while (br.BaseStream.Position < br.BaseStream.Length) list.Add(Book.ReadFromBinary(br));
            }

            if (choice == "1") list.Sort((x, y) => x.ReleaseYear.CompareTo(y.ReleaseYear));
            else if (choice == "2") list.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));

            using (var bw = new BinaryWriter(File.Open(BooksFile, FileMode.Create)))
            {
                foreach (var b in list) b.WriteToBinary(bw);
            }
            Console.WriteLine("Сортировка завершена!");
            Pause();
        }

        public void DeleteMenu()
        {
            Console.Clear();
            Console.WriteLine("=== УДАЛЕНИЕ КНИГИ ===");
            Console.Write("Введите ID книги для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int targetId)) return;

            var list = new List<Book>();
            bool found = false;

            if (File.Exists(BooksFile))
            {
                using (var br = new BinaryReader(File.Open(BooksFile, FileMode.Open)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Book b = Book.ReadFromBinary(br);
                        if (b.Id == targetId) found = true;
                        else list.Add(b);
                    }
                }
            }

            if (found)
            {
                using (var bw = new BinaryWriter(File.Open(BooksFile, FileMode.Create)))
                {
                    foreach (var b in list) b.WriteToBinary(bw);
                }
                Console.WriteLine("Успешно удалено.");
            }
            else Console.WriteLine("ID не найден.");
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
