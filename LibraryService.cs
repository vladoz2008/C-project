using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace StudentGradesSystem
{
    public class StudentService : IStudentService
    {
        // Данные по заданию хранятся в списке класса.
        private static readonly List<Student> Students = new List<Student>();

        public void ShowMenu()
        {
            while (true)
            {
                SafeClear();
                Console.WriteLine("=== УПРАВЛЕНИЕ СТУДЕНТАМИ И ОЦЕНКАМИ ===");
                Console.WriteLine("1. Добавить нового студента");
                Console.WriteLine("2. Добавить оценки студенту");
                Console.WriteLine("3. Вывести среднюю оценку студента");
                Console.WriteLine("4. Вывести среднюю оценку всех студентов");
                Console.WriteLine("5. Проверить допустимость оценки");
                Console.WriteLine("6. Показать всех студентов");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        AddGradesToStudent();
                        break;
                    case "3":
                        ShowStudentAverage();
                        break;
                    case "4":
                        ShowAllStudentsAverage();
                        break;
                    case "5":
                        ValidateGradeFromInput();
                        break;
                    case "6":
                        ShowStudents();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда. Введите число из меню.");
                        break;
                }

                Pause();
            }
        }

        private void AddStudent()
        {
            Console.Write("Введите имя студента: ");
            string? name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: имя не может быть пустым.");
                return;
            }

            if (FindStudentByName(name) is not null)
            {
                Console.WriteLine("Ошибка: студент с таким именем уже существует.");
                return;
            }

            Students.Add(new Student(name));
            Console.WriteLine($"Студент '{name}' добавлен.");
        }

        private void AddGradesToStudent()
        {
            Student? student = PromptStudent();
            if (student is null)
            {
                return;
            }

            Console.WriteLine("Введите оценки через пробел (например: 5 4 3) или по одной строкой. Пустая строка завершает ввод.");

            while (true)
            {
                Console.Write("Оценки: ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    if (!int.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out int grade))
                    {
                        Console.WriteLine($"Предупреждение: '{part}' не является числом.");
                        continue;
                    }

                    if (!student.AddGrade(grade))
                    {
                        Console.WriteLine($"Предупреждение: оценка {grade} недопустима. Разрешено только от 1 до 5.");
                        continue;
                    }

                    Console.WriteLine($"Оценка {grade} добавлена студенту {student.Name}.");
                }
            }
        }

        private void ShowStudentAverage()
        {
            Student? student = PromptStudent();
            if (student is null)
            {
                return;
            }

            if (student.Grades.Count == 0)
            {
                Console.WriteLine($"У студента {student.Name} пока нет оценок.");
                return;
            }

            Console.WriteLine($"Средняя оценка студента {student.Name}: {student.CalculateAverageGrade():F2}");
        }

        private void ShowAllStudentsAverage()
        {
            double average = CalculateAllStudentsAverage();

            if (Students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст.");
                return;
            }

            int gradedStudentsCount = Students.Count(s => s.Grades.Count > 0);
            if (gradedStudentsCount == 0)
            {
                Console.WriteLine("У студентов пока нет оценок.");
                return;
            }

            Console.WriteLine($"Средняя оценка по всем студентам: {average:F2}");
        }

        private void ValidateGradeFromInput()
        {
            Console.Write("Введите оценку для проверки: ");
            if (!int.TryParse(Console.ReadLine(), out int grade))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            Console.WriteLine(IsValidGrade(grade)
                ? "Оценка допустима (1..5)."
                : "Оценка недопустима. Разрешено только от 1 до 5.");
        }

        private void ShowStudents()
        {
            if (Students.Count == 0)
            {
                Console.WriteLine("Студенты еще не добавлены.");
                return;
            }

            Console.WriteLine("Список студентов:");
            foreach (Student student in Students)
            {
                string gradesText = student.Grades.Count == 0 ? "нет оценок" : string.Join(", ", student.Grades);
                Console.WriteLine($"- {student.Name}: {gradesText}");
            }
        }

        // Метод класса (static): средняя оценка всех студентов.
        public static double CalculateAllStudentsAverage()
        {
            List<int> allGrades = Students.SelectMany(s => s.Grades).ToList();
            return allGrades.Count == 0 ? 0 : allGrades.Average();
        }

        // Статический метод валидации оценки.
        public static bool IsValidGrade(int grade)
        {
            return grade >= 1 && grade <= 5;
        }

        private static Student? FindStudentByName(string name)
        {
            return Students.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private static Student? PromptStudent()
        {
            if (Students.Count == 0)
            {
                Console.WriteLine("Сначала добавьте хотя бы одного студента.");
                return null;
            }

            Console.Write("Введите имя студента: ");
            string? name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: имя не может быть пустым.");
                return null;
            }

            Student? student = FindStudentByName(name);
            if (student is null)
            {
                Console.WriteLine("Студент не найден.");
                return null;
            }

            return student;
        }

        private static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }

        private static void SafeClear()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch (IOException)
            {
                // Игнорируем очистку экрана в окружениях без полноценной консоли.
            }
        }
    }
}
