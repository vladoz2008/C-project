using System;
using System.Text;

namespace LibraryInformationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

          
            ILibraryService library = new LibraryService();
            library.InitializeDefaultData();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("       СПРАВОЧНАЯ СИСТЕМА: БИБЛИОТЕКА      ");
                Console.WriteLine("1 - Добавление данных");
                Console.WriteLine("2 - Просмотр таблиц");
                Console.WriteLine("3 - Поиск и Фильтрация");
                Console.WriteLine("4 - Суммарные характеристики");
                Console.WriteLine("5 - Сортировка данных");
                Console.WriteLine("6 - Удаление данных");
                Console.WriteLine("0 - Выход");

                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": library.AddDataMenu(); break;
                    case "2": library.ViewDataMenu(); break;
                    case "3": library.SearchMenu(); break;
                    case "4": library.SummaryMenu(); break;
                    case "5": library.SortMenu(); break;
                    case "6": library.DeleteMenu(); break;
                    case "0": return;
                }
            }
        }
    }
}


