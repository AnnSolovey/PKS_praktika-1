using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


class Program
{
    public class Student
    {
        public int StudentID { get; set; }
        public string Surame { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
    }

    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int LectureHours { get; set; }
        public int PracticeHours { get; set; }
    }

    public class StudyPlan
    {
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public int Grade { get; set; }
    }

    public class UniversityData
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
    }

    public static class JsonHelper
    {
        private static string filePath = "structure.json";

        public static UniversityData LoadData()
        {
            if (!File.Exists(filePath))
            {
                return new UniversityData();
            }
            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<UniversityData>(jsonData);
        }

        public static void SaveData(UniversityData data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }

    public static void AddGrade(int studID, int subjID, int grade)
    {
        var data = JsonHelper.LoadData();

        var student = data.Students.FirstOrDefault(s => s.StudentID == studID);
        if (student == null)
        {
            Console.WriteLine("Студент не найден");
            return;
        }

        var studyPlan = data.StudyPlans.FirstOrDefault(sp => sp.StudentID == studID && sp.SubjectID == subjID);
        if (studyPlan != null)
        {
            studyPlan.Grade = grade;
        }
        else
        {
            data.StudyPlans.Add(new StudyPlan { StudentID = studID, SubjectID = subjID, Grade = grade });
        }

        JsonHelper.SaveData(data);
    }

    public static void PrintStudentGrades(int studID)
    {
        var data = JsonHelper.LoadData();

        var student = data.Students.FirstOrDefault(s => s.StudentID == studID);
        if (student == null)
        {
            Console.WriteLine("Студент не найден");
            return;
        }

        var studentPlans = data.StudyPlans.Where(sp => sp.StudentID == studID).ToList();
        if (!studentPlans.Any())
        {
            Console.WriteLine("Оценки не найдены");
            return;
        }

        var subjectGrades = studentPlans.Select(sp => new
        {
            Subject = data.Subjects.FirstOrDefault(sub => sub.SubjectID == sp.SubjectID)?.Title,
            sp.Grade
        }
        ).ToList();

        var totalGrades = studentPlans.Count;
        var excellentCount = studentPlans.Count(sp => sp.Grade == 5);
        var goodCount = studentPlans.Count(sp => sp.Grade == 4);
        var satisfactoryCount = studentPlans.Count(sp => sp.Grade == 3);

        Console.WriteLine($"Оценки студента {student.Surname} {student.Name} {student.Patronymic}:");
        foreach (var subjectGrade in subjectGrades)
        {
            Console.WriteLine($"Предмет: {subjectGrade.Subject}\nОценка: {subjectGrade.Grade}");
        }

        Console.WriteLine("\nПроцент оценок:");
        Console.WriteLine($"Отлично: {excellentCount * 100.0 / totalGrades}%");
        Console.WriteLine($"Хорошо: {goodCount * 100.0 / totalGrades}%");
        Console.WriteLine($"Удовлетворительно: {satisfactoryCount * 100.0 / totalGrades}%");
    }

    static void Main()
    {
        string choice;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Добавить оценку");
            Console.WriteLine("2 - Показать оценки студента");
            Console.WriteLine("0 - Выход");
            choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();

                Console.WriteLine("Введите код студента:");
                var studID = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите код предмета:");
                var subjID = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите оценку:");
                var grade = int.Parse(Console.ReadLine());

                AddGrade(studID, subjID, grade);
            }
            else if (choice == "2")
            {
                Console.Clear();

                Console.WriteLine("Введите код студента:");
                var studID = int.Parse(Console.ReadLine());

                Console.Clear();

                PrintStudentGrades(studID);
            }
            else if (choice == "0")
            {
                break;
            }
            choice = Console.ReadLine();
        }
    }
}
