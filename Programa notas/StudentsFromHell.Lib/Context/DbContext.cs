using Academy.Lib.Models;
using System;
using System.Collections.Generic;

namespace Academy.Lib.Context
{
    public static class DbContext
    {
        #region Repositories

        public static Dictionary<Guid, Exam> Exams { get; set; } = new Dictionary<Guid, Exam>();
        public static Dictionary<Guid, Student> Students { get; set; } = new Dictionary<Guid, Student>();
        public static Dictionary<Guid, Subject> Subjects { get; set; } = new Dictionary<Guid, Subject>();
        #endregion

        #region Indexes 

        public static Dictionary<string, Student> StudentsByDni { get; set; } = new Dictionary<string, Student>();
        #endregion

        #region Crud

        public static bool AddStudent(Student student)
        {
            if (student.Id != Guid.Empty)
                return false;

            student.Id = Guid.NewGuid();
            Students.Add(student.Id, student);
            StudentsByDni.Add(student.Dni, student);

            return true;
        }

        public static bool UpdateStudent(Student student)
        {
            if (student.Id != Guid.Empty && Students.ContainsKey(student.Id))
            {
                var studentInMemory = Students[student.Id];

                if (student != studentInMemory)
                {
                    Students[student.Id] = student;
                    StudentsByDni[student.Dni] = student;
                }
            }
            else
            {
                AddStudent(student);
            }

            return true;
        }
        #endregion
    }
}

