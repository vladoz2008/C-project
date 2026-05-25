using System;
using System.Text;

namespace StudentGradesSystem
{
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            Console.InputEncoding = new UTF8Encoding(false);
            Console.Title = "Library Console System";

            ILibraryService service = new LibraryService();
            service.ShowMenu();
        }
    }
}
