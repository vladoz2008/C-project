using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace StudentGradesSystem
{
    internal class LibraryService : ILibraryService
    {
        private const int StageWidth = 92;

        private static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "data");
        private static readonly string BooksPath = Path.Combine(DataDir, "books.bin");
        private static readonly string ReadersPath = Path.Combine(DataDir, "readers.bin");
        private static readonly string CardsPath = Path.Combine(DataDir, "cards.bin");

        private readonly List<Book> books;
        private readonly List<Reader> readers;
        private readonly List<LibraryCard> cards;

        // Инициализирует директорию хранения и загружает данные из файлов в память.
        internal LibraryService()
        {
            Directory.CreateDirectory(DataDir);
            books = LoadList<Book>(BooksPath);
            readers = LoadList<Reader>(ReadersPath);
            cards = LoadList<LibraryCard>(CardsPath);
        }

        // Показывает главное меню и маршрутизирует выбор пользователя по функциям системы.
        public void ShowMenu()
        {
            while (true)
            {
                SafeClear();
                DrawMainScreen();
                Console.Write("Ваш выбор: ");
                string choice = ReadLineSafe().Trim();
                Console.WriteLine();
                SafeClear();

                switch (choice)
                {
                    case "1": AddBook(); break;
                    case "2": AddReader(); break;
                    case "3": AddCard(); break;
                    case "4": ShowTables(); break;
                    case "5": SearchAndFilter(); break;
                    case "6": SortData(); break;
                    case "7": DeleteData(); break;
                    case "8": ShowStats(); break;
                    case "0": return;
                    default: DrawStatus("ERROR", "Неизвестная команда. Введите число из меню.", ConsoleColor.Red); break;
                }

                Pause();
            }
        }

        // Добавляет новую книгу после проверки корректности и уникальности кода.
        private void AddBook()
        {
            DrawOperationTitle("Добавление книги");
            int code = ReadInt("Код книги (целое > 0): ", min: 1);
            if (books.Any(b => b.BookCode == code))
            {
                DrawStatus("ERROR", "Книга с таким кодом уже существует.", ConsoleColor.Red);
                return;
            }

            Book book = new Book(
                code,
                ReadText("Имя автора: ", 2, 30),
                ReadText("Фамилия автора: ", 2, 30),
                ReadText("Название книги: ", 2, 80),
                ReadInt("Год выпуска (1500..2100): ", 1500, 2100),
                ReadText("Жанр: ", 2, 30));

            books.Add(book);
            SaveAll();
            DrawStatus("SUCCESS", "Книга успешно добавлена.", ConsoleColor.Green);
        }

        // Добавляет нового читателя с валидацией всех полей формы.
        private void AddReader()
        {
            DrawOperationTitle("Добавление читателя");
            int code = ReadInt("Код читателя (целое > 0): ", min: 1);
            if (readers.Any(r => r.ReaderCode == code))
            {
                DrawStatus("ERROR", "Читатель с таким кодом уже существует.", ConsoleColor.Red);
                return;
            }

            Reader reader = new Reader(
                code,
                ReadText("Имя: ", 2, 30),
                ReadText("Фамилия: ", 2, 30),
                ReadText("Адрес: ", 5, 80),
                ReadInt("Возраст (10..120): ", 10, 120));

            readers.Add(reader);
            SaveAll();
            DrawStatus("SUCCESS", "Читатель успешно добавлен.", ConsoleColor.Green);
        }

        // Оформляет читательский билет, связывая книгу и читателя по их кодам.
        private void AddCard()
        {
            DrawOperationTitle("Выдача читательского билета");
            if (books.Count == 0 || readers.Count == 0)
            {
                DrawStatus("ERROR", "Сначала добавьте хотя бы одну книгу и одного читателя.", ConsoleColor.Red);
                return;
            }

            DrawHint("Перед выдачей билета проверьте коды в таблицах ниже");
            PrintBooks(books, "Книги");
            PrintReaders(readers, "Читатели");

            int bookCode = ReadInt("Код книги: ", min: 1);
            if (!books.Any(b => b.BookCode == bookCode))
            {
                DrawStatus("ERROR", "Книга с таким кодом не найдена.", ConsoleColor.Red);
                return;
            }

            int readerCode = ReadInt("Код читателя: ", min: 1);
            if (!readers.Any(r => r.ReaderCode == readerCode))
            {
                DrawStatus("ERROR", "Читатель с таким кодом не найден.", ConsoleColor.Red);
                return;
            }

            LibraryCard card = new LibraryCard(
                bookCode,
                readerCode,
                ReadDate("Дата выдачи (дд.мм.гггг): "),
                ReadYesNo("Возврат? (y/n): "));

            cards.Add(card);
            SaveAll();
            DrawStatus("SUCCESS", "Билет выдан и сохранен.", ConsoleColor.Green);
        }

        // Выводит все сущности проекта в виде таблиц.
        private void ShowTables()
        {
            DrawOperationTitle("Просмотр таблиц");
            PrintBooks(books, "Книги");
            PrintReaders(readers, "Читатели");
            PrintCards(cards, "Билеты");
        }

        // Выполняет поиск и фильтрацию по доступным критериям.
        private void SearchAndFilter()
        {
            DrawOperationTitle("Поиск и фильтрация");
            DrawHint("Текущие данные для ориентира по кодам и значениям");
            ShowTables();
            DrawMiniMenu(new[]
            {
                "1. Книги по жанру",
                "2. Книги по автору (фамилия)",
                "3. Читатели по возрасту (минимум)",
                "4. Невозвращенные книги"
            });

            Console.Write("Выберите фильтр: ");
            string choice = ReadLineSafe().Trim();

            switch (choice)
            {
                case "1":
                    string genre = ReadText("Жанр: ", 1, 30);
                    PrintBooks(books.Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList(), "Книги по жанру");
                    break;
                case "2":
                    string lastName = ReadText("Фамилия автора: ", 1, 30);
                    PrintBooks(books.Where(b => b.AuthorLastName.Equals(lastName, StringComparison.OrdinalIgnoreCase)).ToList(), "Книги по автору");
                    break;
                case "3":
                    int age = ReadInt("Минимальный возраст: ", 1, 120);
                    PrintReaders(readers.Where(r => r.Age >= age).ToList(), "Читатели по возрасту");
                    break;
                case "4":
                    PrintCards(cards.Where(c => !c.Returned).ToList(), "Невозвращенные книги");
                    break;
                default:
                    DrawStatus("ERROR", "Неизвестная команда фильтрации.", ConsoleColor.Red);
                    break;
            }
        }

        // Показывает отсортированные выборки данных по выбранному правилу.
        private void SortData()
        {
            DrawOperationTitle("Сортировка");
            DrawHint("Текущие данные перед сортировкой");
            ShowTables();
            DrawMiniMenu(new[]
            {
                "1. Книги: по году",
                "2. Книги: по названию",
                "3. Читатели: по фамилии",
                "4. Билеты: по дате выдачи"
            });

            Console.Write("Выберите сортировку: ");
            string choice = ReadLineSafe().Trim();

            switch (choice)
            {
                case "1": PrintBooks(books.OrderBy(b => b.PublishYear).ToList(), "Книги по году"); break;
                case "2": PrintBooks(books.OrderBy(b => b.Title).ToList(), "Книги по названию"); break;
                case "3": PrintReaders(readers.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToList(), "Читатели по фамилии"); break;
                case "4": PrintCards(cards.OrderBy(c => c.IssueDate).ToList(), "Билеты по дате"); break;
                default: DrawStatus("ERROR", "Неизвестная команда сортировки.", ConsoleColor.Red); break;
            }
        }

        // Удаляет записи по критериям и связанные данные в билетах.
        private void DeleteData()
        {
            DrawOperationTitle("Удаление данных");
            DrawHint("Перед удалением проверьте коды в таблицах");
            ShowTables();
            DrawMiniMenu(new[]
            {
                "1. Удалить книгу по коду",
                "2. Удалить читателя по коду",
                "3. Удалить билеты по коду книги"
            });

            Console.Write("Выберите удаление: ");
            string choice = ReadLineSafe().Trim();

            switch (choice)
            {
                case "1":
                    int bCode = ReadInt("Код книги: ", min: 1);
                    int removedBooks = books.RemoveAll(b => b.BookCode == bCode);
                    int removedCardsByBook = cards.RemoveAll(c => c.BookCode == bCode);
                    SaveAll();
                    DrawStatus("SUCCESS", $"Удалено книг: {removedBooks}; связанных билетов: {removedCardsByBook}.", ConsoleColor.Green);
                    break;
                case "2":
                    int rCode = ReadInt("Код читателя: ", min: 1);
                    int removedReaders = readers.RemoveAll(r => r.ReaderCode == rCode);
                    int removedCardsByReader = cards.RemoveAll(c => c.ReaderCode == rCode);
                    SaveAll();
                    DrawStatus("SUCCESS", $"Удалено читателей: {removedReaders}; связанных билетов: {removedCardsByReader}.", ConsoleColor.Green);
                    break;
                case "3":
                    int code = ReadInt("Код книги: ", min: 1);
                    int removedCards = cards.RemoveAll(c => c.BookCode == code);
                    SaveAll();
                    DrawStatus("SUCCESS", $"Удалено билетов: {removedCards}.", ConsoleColor.Green);
                    break;
                default:
                    DrawStatus("ERROR", "Неизвестная команда удаления.", ConsoleColor.Red);
                    break;
            }
        }

        // Считает и отображает суммарные показатели по библиотечной базе.
        private void ShowStats()
        {
            DrawOperationTitle("Суммарные характеристики");
            int notReturned = cards.Count(c => !c.Returned);
            int booksAfter2000 = books.Count(b => b.PublishYear >= 2000);
            int uniqueReadersWithCards = cards.Select(c => c.ReaderCode).Distinct().Count();
            int uniqueBooksInCards = cards.Select(c => c.BookCode).Distinct().Count();

            string[] lines =
            {
                $"Всего книг                    : {books.Count}",
                $"Всего читателей               : {readers.Count}",
                $"Всего билетов                 : {cards.Count}",
                $"Невозвращенных книг           : {notReturned}",
                $"Книг после 2000 года          : {booksAfter2000}",
                $"Читателей с активностью       : {uniqueReadersWithCards}",
                $"Уникальных книг в билетах     : {uniqueBooksInCards}"
            };

            DrawCard("LIBRARY ANALYTICS", lines, ConsoleColor.Cyan);
        }

        // Рисует таблицу книг с заголовком секции.
        private static void PrintBooks(List<Book> list, string title)
        {
            DrawPanelTitle(title);
            if (list.Count == 0)
            {
                DrawStatus("INFO", "Нет данных.", ConsoleColor.DarkYellow);
                return;
            }

            PrintTable(
                new[] { "Код", "Автор", "Название", "Год", "Жанр" },
                list.Select(b => new[]
                {
                    b.BookCode.ToString(CultureInfo.InvariantCulture),
                    $"{b.AuthorLastName} {b.AuthorFirstName}",
                    b.Title,
                    b.PublishYear.ToString(CultureInfo.InvariantCulture),
                    b.Genre
                }).ToList(),
                new[] { 6, 22, 32, 8, 16 });
        }

        // Рисует таблицу читателей с заголовком секции.
        private static void PrintReaders(List<Reader> list, string title)
        {
            DrawPanelTitle(title);
            if (list.Count == 0)
            {
                DrawStatus("INFO", "Нет данных.", ConsoleColor.DarkYellow);
                return;
            }

            PrintTable(
                new[] { "Код", "Фамилия", "Имя", "Возраст", "Адрес" },
                list.Select(r => new[]
                {
                    r.ReaderCode.ToString(CultureInfo.InvariantCulture),
                    r.LastName,
                    r.FirstName,
                    r.Age.ToString(CultureInfo.InvariantCulture),
                    r.Address
                }).ToList(),
                new[] { 6, 16, 16, 8, 32 });
        }

        // Рисует таблицу читательских билетов с заголовком секции.
        private static void PrintCards(List<LibraryCard> list, string title)
        {
            DrawPanelTitle(title);
            if (list.Count == 0)
            {
                DrawStatus("INFO", "Нет данных.", ConsoleColor.DarkYellow);
                return;
            }

            PrintTable(
                new[] { "Код книги", "Код читателя", "Дата", "Возвращена" },
                list.Select(c => new[]
                {
                    c.BookCode.ToString(CultureInfo.InvariantCulture),
                    c.ReaderCode.ToString(CultureInfo.InvariantCulture),
                    c.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    c.Returned ? "Да" : "Нет"
                }).ToList(),
                new[] { 12, 14, 14, 12 });
        }

        // Универсально отрисовывает стилизованную таблицу по колонкам и строкам.
        private static void PrintTable(IReadOnlyList<string> headers, List<string[]> rows, IReadOnlyList<int> widths)
        {
            string top = "┏" + string.Join("┳", widths.Select(w => new string('━', w + 2))) + "┓";
            string headerSplit = "┣" + string.Join("╋", widths.Select(w => new string('━', w + 2))) + "┫";
            string rowSep = "┠" + string.Join("┼", widths.Select(w => new string('─', w + 2))) + "┨";
            string bottom = "┗" + string.Join("┻", widths.Select(w => new string('━', w + 2))) + "┛";

            string headerLine = "┃ " + string.Join(" ┃ ", headers.Select((h, i) => FitCenter(h, widths[i]))) + " ┃";

            WriteCentered(top + "▐", ConsoleColor.Cyan);
            WriteCentered(headerLine + "▐", ConsoleColor.Yellow);
            WriteCentered(headerSplit + "▐", ConsoleColor.Cyan);

            for (int i = 0; i < rows.Count; i++)
            {
                ConsoleColor rowColor = i % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.White;
                WriteCentered("┃ " + string.Join(" │ ", rows[i].Select((c, idx) => Fit(c, widths[idx]))) + " ┃▐", rowColor);
                if (i < rows.Count - 1)
                {
                    WriteCentered(rowSep + "▐", ConsoleColor.DarkGray);
                }
            }

            WriteCentered(bottom + "▐", ConsoleColor.Cyan);
            WriteCentered(" " + new string('▀', Math.Max(8, bottom.Length - 2)), ConsoleColor.DarkGray);
        }

        // Формирует главный экран приложения: баннер, рамку и пункты меню.
        private static void DrawMainScreen()
        {
            DrawSplash();
            DrawFrameTop();
            WriteFramedCentered("***", ConsoleColor.DarkYellow);
            WriteFramedCentered("\" LIBRARY CONSTELLATION TERMINAL \"", ConsoleColor.Yellow);
            WriteFramedCentered("***", ConsoleColor.DarkYellow);
            DrawFrameDivider();
            WriteFramedCentered("MAIN MENU", ConsoleColor.Cyan);
            WriteFramedCentered("ГЛАВНОЕ МЕНЮ", ConsoleColor.Cyan);
            WriteFramedCentered("1  - Добавить книгу", ConsoleColor.Gray);
            WriteFramedCentered("2  - Добавить читателя", ConsoleColor.Gray);
            WriteFramedCentered("3  - Выдать билет", ConsoleColor.Gray);
            WriteFramedCentered("4  - Показать таблицы", ConsoleColor.Gray);
            WriteFramedCentered("5  - Поиск / фильтр", ConsoleColor.Gray);
            WriteFramedCentered("6  - Сортировка", ConsoleColor.Gray);
            WriteFramedCentered("7  - Удаление", ConsoleColor.Gray);
            WriteFramedCentered("8  - Сводка", ConsoleColor.Gray);
            WriteFramedCentered("0  - Выход", ConsoleColor.Gray);
            DrawFrameBottom();
        }

        // Очищает экран и выводит заголовок текущей операции.
        private static void DrawOperationTitle(string title)
        {
            SafeClear();
            DrawFrameTop();
            WriteFramedCentered($"[ {title.ToUpperInvariant()} ]", ConsoleColor.Green);
            DrawFrameBottom();
        }

        // Показывает компактное меню для подопераций (поиск/сортировка/удаление).
        private static void DrawMiniMenu(IEnumerable<string> items)
        {
            DrawFrameTop();
            WriteFramedCentered("ВЫБЕРИТЕ ДЕЙСТВИЕ", ConsoleColor.Cyan);
            DrawFrameDivider();
            foreach (string item in items)
            {
                WriteFramedCentered(item, ConsoleColor.Gray);
            }
            DrawFrameBottom();
        }

        // Выводит информационную подсказку пользователю.
        private static void DrawHint(string text)
        {
            DrawStatus("HINT", text, ConsoleColor.Magenta);
        }

        // Выводит статусное сообщение (успех/ошибка/подсказка) в едином стиле.
        private static void DrawStatus(string tag, string message, ConsoleColor color)
        {
            string content = $"{tag}: {message}";
            int uiWidth = GetUiWidth();
            string line = new string('-', Math.Min(uiWidth, Math.Max(30, content.Length + 8)));
            WriteCentered(line, ConsoleColor.DarkGray);
            WriteCentered(content, color);
            WriteCentered(line, ConsoleColor.DarkGray);
        }

        // Рисует карточку с многострочной информацией (используется для статистики).
        private static void DrawCard(string title, IEnumerable<string> lines, ConsoleColor accent)
        {
            int uiWidth = GetUiWidth();
            string top = "╔" + new string('═', uiWidth - 2) + "╗";
            string bottom = "╚" + new string('═', uiWidth - 2) + "╝";
            WriteCentered(top, accent);
            WriteCardLine($":: {title} ::", accent);
            WriteCentered("╠" + new string('═', uiWidth - 2) + "╣", accent);
            foreach (string line in lines)
            {
                WriteCardLine(line, ConsoleColor.White);
            }
            WriteCentered(bottom, accent);
        }

        // Отображает заголовок секции перед таблицей.
        private static void DrawPanelTitle(string title)
        {
            int uiWidth = GetUiWidth();
            WriteCentered(string.Empty, ConsoleColor.Gray);
            WriteCentered("╭" + new string('─', uiWidth - 2) + "╮", ConsoleColor.DarkGray);
            WriteCentered(FitCenter($"[ {title.ToUpperInvariant()} ]", uiWidth), ConsoleColor.Green);
            WriteCentered("╰" + new string('─', uiWidth - 2) + "╯", ConsoleColor.DarkGray);
        }

        // Печатает ASCII-баннер приложения.
        private static void DrawSplash()
        {
            string[] art =
            {
                "██╗     ██╗██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗",
                "██║     ██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝",
                "██║     ██║██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝ ",
                "██║     ██║██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝  ",
                "███████╗██║██████╔╝██║  ██║██║  ██║██║  ██║   ██║   ",
                "╚══════╝╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   "
            };

            foreach (string line in art)
            {
                WriteCentered(line, ConsoleColor.DarkYellow);
            }
        }

        // Рисует верхнюю границу рамки блока.
        private static void DrawFrameTop()
        {
            int uiWidth = GetUiWidth();
            WriteCentered("┏" + new string('━', uiWidth - 2) + "┓", ConsoleColor.DarkBlue);
        }

        // Рисует разделитель внутри рамки блока.
        private static void DrawFrameDivider()
        {
            int uiWidth = GetUiWidth();
            WriteCentered("┣" + new string('━', uiWidth - 2) + "┫", ConsoleColor.DarkBlue);
        }

        // Рисует нижнюю границу рамки блока.
        private static void DrawFrameBottom()
        {
            int uiWidth = GetUiWidth();
            WriteCentered("┗" + new string('━', uiWidth - 2) + "┛", ConsoleColor.DarkBlue);
        }

        // Печатает центрированный текст внутри декоративной рамки.
        private static void WriteFramedCentered(string text, ConsoleColor color)
        {
            int uiWidth = GetUiWidth();
            string inner = FitCenter(text, uiWidth - 4);
            WriteCentered($"┃ {inner} ┃", color);
        }

        // Печатает одну строку внутри карточки статистики.
        private static void WriteCardLine(string text, ConsoleColor color)
        {
            int uiWidth = GetUiWidth();
            string inner = Fit(text, uiWidth - 4);
            WriteCentered($"║ {inner} ║", color);
        }

        // Возвращает рабочую ширину UI в зависимости от текущего размера консоли.
        private static int GetUiWidth()
        {
            try
            {
                return Math.Clamp(Console.WindowWidth - 4, 60, StageWidth);
            }
            catch
            {
                return StageWidth;
            }
        }

        // Выводит строку по центру окна с указанным цветом.
        private static void WriteCentered(string text, ConsoleColor color)
        {
            int width;
            try
            {
                width = Math.Max(Console.WindowWidth, text.Length + 2);
            }
            catch
            {
                width = StageWidth;
            }

            int pad = Math.Max(0, (width - text.Length) / 2);
            WriteLineWithColor(new string(' ', pad) + text, color);
        }

        // Печатает строку заданным цветом и возвращает прежний цвет консоли.
        private static void WriteLineWithColor(string text, ConsoleColor color)
        {
            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        // Приводит текст к заданной ширине: обрезает или дополняет пробелами.
        private static string Fit(string? value, int width)
        {
            string text = (value ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ').Trim();
            if (text.Length <= width)
            {
                return text.PadRight(width);
            }

            return width <= 1 ? text[..1] : text[..(width - 1)] + ".";
        }

        // Приводит текст к заданной ширине и дополнительно центрирует его.
        private static string FitCenter(string? value, int width)
        {
            string plain = Fit(value, width).TrimEnd();
            if (plain.Length >= width)
            {
                return plain[..width];
            }

            int left = (width - plain.Length) / 2;
            int right = width - plain.Length - left;
            return new string(' ', left) + plain + new string(' ', right);
        }

        // Сохраняет все коллекции (книги, читатели, билеты) в файлы.
        private void SaveAll()
        {
            SaveList(BooksPath, books);
            SaveList(ReadersPath, readers);
            SaveList(CardsPath, cards);
        }

        // Загружает список объектов из файла; при ошибках возвращает пустой список.
        private static List<T> LoadList<T>(string path)
        {
            if (!File.Exists(path))
            {
                return new List<T>();
            }

            byte[] data = File.ReadAllBytes(path);
            if (data.Length == 0)
            {
                return new List<T>();
            }

            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        // Сериализует список в JSON и записывает его в бинарный файл.
        private static void SaveList<T>(string path, List<T> list)
        {
            string json = JsonSerializer.Serialize(list);
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
        }

        // Считывает и валидирует текст по ограничениям минимальной и максимальной длины.
        private static string ReadText(string prompt, int minLen, int maxLen)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = ReadLineSafe().Trim();
                if (value.Length < minLen)
                {
                    DrawStatus("ERROR", $"Минимальная длина: {minLen}.", ConsoleColor.Red);
                    continue;
                }

                if (value.Length > maxLen)
                {
                    DrawStatus("ERROR", $"Слишком длинно. Максимум символов: {maxLen}.", ConsoleColor.Red);
                    continue;
                }

                return value;
            }
        }

        // Считывает целое число и проверяет попадание в допустимый диапазон.
        private static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = ReadLineSafe();
                if (!int.TryParse(input, out int value))
                {
                    DrawStatus("ERROR", "Введите целое число.", ConsoleColor.Red);
                    continue;
                }

                if (value < min || value > max)
                {
                    DrawStatus("ERROR", $"Введите значение в диапазоне [{min}; {max}].", ConsoleColor.Red);
                    continue;
                }

                return value;
            }
        }

        // Считывает дату в формате дд.мм.гггг с повторным запросом при ошибке.
        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = ReadLineSafe();
                if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }

                DrawStatus("ERROR", "Неверный формат даты. Используйте дд.мм.гггг.", ConsoleColor.Red);
            }
        }

        // Считывает ответ да/нет в русской и английской форме.
        private static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = ReadLineSafe().Trim().ToLowerInvariant();
                if (input == "y" || input == "yes" || input == "д" || input == "да")
                {
                    return true;
                }

                if (input == "n" || input == "no" || input == "н" || input == "нет")
                {
                    return false;
                }

                DrawStatus("ERROR", "Введите y/n.", ConsoleColor.Red);
            }
        }

        // Безопасно считывает строку посимвольно (корректно работает с кириллицей).
        private static string ReadLineSafe()
        {
            var sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return sb.ToString();
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
        }

        // Ставит паузу до нажатия клавиши перед возвратом в меню.
        private static void Pause()
        {
            WriteCentered("Нажмите любую клавишу для продолжения...", ConsoleColor.DarkGray);
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }

        // Полностью очищает экран консоли и сбрасывает позицию курсора.
        private static void SafeClear()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.ResetColor();
                    Console.Write("\x1b[3J\x1b[2J\x1b[H");
                    Console.Clear();
                    try
                    {
                        Console.SetCursorPosition(0, 0);
                    }
                    catch
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
