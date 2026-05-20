using System;
using System.Text;

namespace StudentGradesSystem
{
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            IStudentService service = new StudentService();
            service.ShowMenu();
        }
    }
}
