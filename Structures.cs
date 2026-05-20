using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentGradesSystem
{
    public class Student : IEntity
    {
        public string Name { get; }
        public string DisplayName => Name;
        public List<int> Grades { get; } = new List<int>();

        public Student(string name)
        {
            Name = name;
        }

        // Метод экземпляра: добавляет оценку только после валидации.
        public bool AddGrade(int grade)
        {
            if (!StudentService.IsValidGrade(grade))
            {
                return false;
            }

            Grades.Add(grade);
            return true;
        }

        // Метод экземпляра: средний балл конкретного студента.
        public double CalculateAverageGrade()
        {
            return Grades.Count == 0 ? 0 : Grades.Average();
        }
    }
}
