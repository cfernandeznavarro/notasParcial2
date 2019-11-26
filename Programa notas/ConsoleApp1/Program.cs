using Academy.Lib.Context;
using Academy.Lib.Infrastructure;
using Academy.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        public static string CurrentOption { get; set; }

        static void Main(string[] args)
         {

            Console.WriteLine("Bienvenidos al programa de gestión de clase");
            ShowMainMenu();

            while (true)
            {
                var option = Console.ReadKey().KeyChar;

                
                if (option == 'm')
                {
                    ClearCurrentConsoleLine();
                    if (CurrentOption != "m")
                    {
                        Console.WriteLine();
                        ShowMainMenu();
                    }
                }
                else if (option == 'a')
                {
                    ClearCurrentConsoleLine();
                    if (CurrentOption != "a")
                    {
                        Console.WriteLine();
                        ShowHandleStudentsMenu();
                    }
                }
                else if( option == 'n')
                {
                    ClearCurrentConsoleLine();
                    if (CurrentOption != "n")
                    {
                        Console.WriteLine();
                        ShowAddNotesMenu();
                    }
                }
                else if(option == 'c')
                {
                    ClearCurrentConsoleLine();
                    if (CurrentOption != "c")
                    {
                        Console.WriteLine();
                        ShowStatisticsMenu();
                    }
                }
            }
        }
        
        static void ShowMainMenu()
        {
            CurrentOption = "m";
            Console.WriteLine("Menu de opciones principal");

            Console.WriteLine("Opciones: m - para volver a este menu");
            Console.WriteLine("Opciones: a - gestionar alumnos");
            Console.WriteLine("Opciones: n - añadir notas de alumnos");
            Console.WriteLine("Opciones: c - Estadísticas");
        }

        static void ShowHandleStudentsMenu()
        {
            CurrentOption = "a";
            Console.WriteLine("Menu de gestionar alumnos.");
            Console.WriteLine("Opciones: a - para añadir un nuevo almuno");
            Console.WriteLine("Opciones: e + dni - para editar un alumno existente");
            Console.WriteLine("Opciones: n - ver información del alumno");
            Console.WriteLine("Opciones: n/e - ver exámenes de alumno");
            Console.WriteLine("Opciones: n/e - ver asignaturas de alumno");
            Console.WriteLine("Presione m para acabar y volver al menú principal");

            while (true)
            {
                var option = Console.ReadLine();

                
                if (option == "m")
                {
                    break;
                }
                else if (option == "a")
                {
                    Console.WriteLine("Para volver sin guardar alumno escriba  *.");
                    Console.WriteLine("escriba el dni:");

                    #region read dni
                    var dni = Console.ReadLine();

                    if (dni == "*")
                        break;

                    ValidationResult<string> dniValRes = Student.ValidateDni(dni, default(Guid));

                    while (!dniValRes.IsSuccess)
                    {
                        foreach (var msg in dniValRes.Messages)
                            Console.WriteLine(msg);

                        dni = Console.ReadLine();
                        dniValRes = Student.ValidateDni(dni, default(Guid));
                    }

                    if (dni == "*")
                        break;

                    #endregion

                    #region read name
                    Console.WriteLine("escriba el nombre y apellidos:");
                    var name = Console.ReadLine();

                    if (name == "*")
                        break;

                    ValidationResult<string> nameValRes = Student.ValidateName(name);

                    while (!nameValRes.IsSuccess)
                    {
                        foreach (var msg in nameValRes.Messages)
                            Console.WriteLine(msg);

                        name = Console.ReadLine();
                        nameValRes = Student.ValidateName(name);
                    }

                    #endregion

                    #region read chair number
                    Console.WriteLine("escriba el número de silla:");
                    var chairNumberText = Console.ReadLine();

                    if (chairNumberText == "*")
                        break;

                    ValidationResult<int> chairNumValRes = Student.ValidateChairNumber(chairNumberText);

                    while (!chairNumValRes.IsSuccess)
                    {
                        foreach (var msg in chairNumValRes.Messages)
                            Console.WriteLine(msg);

                        chairNumberText = Console.ReadLine();
                        chairNumValRes = Student.ValidateChairNumber(chairNumberText);
                    }

                    #endregion


                    if (dniValRes.IsSuccess && nameValRes.IsSuccess && chairNumValRes.IsSuccess)
                    {
                        var student = new Student
                        {
                            Dni = dniValRes.ValidatedResult,
                            Name = nameValRes.ValidatedResult,
                            ChairNumber = chairNumValRes.ValidatedResult
                        };

                        var save = student.Save();

                        if (save)
                        {
                            Console.WriteLine($"alumno guardado correctamente");
                        }
                        else
                        {
                            Console.WriteLine($"uno o más errores han ocurrido y el almuno no se guardado correctamente");
                        }
                    }
                    
                  
                }
                else if (option == "e")
                {
                    // option e + dni - para editar un alumno existente
                    Console.WriteLine("Para volver sin guardar alumno escriba  *.");

                    #region read DNI

                    Console.WriteLine("escriba el dni del alumno que desea editar:");

                    var dni = Console.ReadLine();

                    // opcion 1 - Linq
                    //if (DbContext.Students.Values.Any(s => s.Dni == dni))
                    //{
                    //    var existingStudent = DbContext.Students.Values.First(s => s.Dni == dni);
                    //}

                    // opcion 2 - Linq optimo
                    //var existingStudent = DbContext.Students.Values.FirstOrDefault(s => s.Dni == dni);
                    //if(existingStudent != null)
                    //{
                    //    // hago lo que quiera con el existing Student
                    //}

                    // opcion 3 - Sin Linq

                    if (DbContext.StudentsByDni.ContainsKey(dni))
                    {
                        var existingStudent = DbContext.StudentsByDni[dni];  //CRIS

                        Console.WriteLine("Escriba el nuevo DNI");
                        var dniNuevo = Console.ReadLine();

                        var dniValidResult = Student.ValidateDni(dniNuevo, existingStudent.Id);

                        while (!dniValidResult.IsSuccess)
                        {
                            foreach (var msg in dniValidResult.Messages)
                            Console.WriteLine(msg);

                            Console.WriteLine("Escriba de nuevo el nuevo DNI");

                            dniNuevo = Console.ReadLine();
                            dniValidResult = Student.ValidateDni(dniNuevo, existingStudent.Id);
                        }

                        #endregion

                        #region read DNI

                        Console.WriteLine("escriba el nombre correcto del alumno:");
                        var name = Console.ReadLine();

                        if (name == "*")
                            break;

                        ValidationResult<string> nameValRes = Student.ValidateName(name);

                        while (!nameValRes.IsSuccess)
                        {
                            foreach (var msg in nameValRes.Messages)
                                Console.WriteLine(msg);

                            name = Console.ReadLine();
                            nameValRes = Student.ValidateName(name);
                        }

                        #endregion

                        #region read Chairnumber

                        Console.WriteLine("escriba el  nuevo número de silla:");
                        var chairNumberText = Console.ReadLine();

                        if (chairNumberText == "*")
                            break;

                        ValidationResult<int> chairNumValRes = Student.ValidateChairNumber(chairNumberText);

                        while (!chairNumValRes.IsSuccess) 
                        {
                            foreach (var msg in chairNumValRes.Messages)
                                Console.WriteLine(msg);

                            Console.WriteLine("Vuelva a escribir el  nuevo número de silla:");

                            chairNumberText = Console.ReadLine();
                            chairNumValRes = Student.ValidateChairNumber(chairNumberText);
                        }

                        #endregion

                       
                        if (dniValidResult.IsSuccess && nameValRes.IsSuccess && chairNumValRes.IsSuccess)
                        {

                            existingStudent.Dni = dniValidResult.ValidatedResult;
                            existingStudent.Name = nameValRes.ValidatedResult;
                            existingStudent.ChairNumber = chairNumValRes.ValidatedResult;
                           
                            var save = existingStudent.Save();

                            if (save)
                                if (existingStudent.Save())
                            {
                                Console.WriteLine($"Las modificaciones se han guardado correctamente");
                            }
                            else
                            {
                                Console.WriteLine($"uno o más errores han ocurrido y el alumno no se ha modificado correctamente");
                            }
                        }
                    }


                else if (option == "n")
                {

                    for (int count = 0; count < DbContext.Students.Count; count++)
                    {
                      
                        
                        
                        var element = DbContext.Students.ElementAt(count);
                        var Key = element.Key;
                        var Value = element.Value;
                        Console.WriteLine(Key + "  " + Value);
                    }

                }

                
            }

   
            }

            ClearCurrentConsoleLine();
            Console.WriteLine();
            ShowMainMenu();



        }
        static void ShowAddNotesMenu()
        {
            CurrentOption = "n";
            Console.WriteLine("Menu de añadir notas. Añada notas [dni*nombre*nota] y presione al enter");
            Console.WriteLine("Presione m para acabar y volver al menú principal");

            while (true)
            {
                var notaText = Console.ReadLine();

                if (notaText == "m")
                {
                    break;
                }
                else                
                {
                    char[] c1 = { '*' };
                    var spaso = notaText.Split(c1);

                    var dni = spaso[0];
                    var name = spaso[1];
                    var markText = spaso[2].Replace(".", ",");

                    #region Nota
                    //double nota;
                    //if (double.TryParse(markText, out nota))
                    //{
                    //    if (!Marks.ContainsKey(dni))
                    //        Marks.Add(dni, new List<double>());

                    //    Marks[dni].Add(nota);
                    //}
                    //else
                    //{
                    //    Console.WriteLine($"valor introducidio [{notaText}] no válido");
                    //}
                    #endregion

                    #region Name

                    //if (!Students.ContainsKey(dni))
                    //    Students.Add(dni, name);
                    //else if(!string.IsNullOrEmpty(name))
                    ////else if (name != "" && name != null)
                    //    Students[dni] = name;

                    #endregion

                }
            }

            ClearCurrentConsoleLine();
            Console.WriteLine();
            ShowMainMenu();
        }

        static void ShowStatisticsMenu()
        {
            CurrentOption = "c";

            Console.WriteLine("Opción de Estadísticas");
            Console.WriteLine("Presione m para acabar y volver al menú principal");
            Console.WriteLine("Opciones: avg - obtener la media de las notas de los alumnos");
            Console.WriteLine("Opciones: max - obtener la máxima nota de los alumnos");
            Console.WriteLine("Opciones: max - obtener la mínima nota de los alumnos");

            while (true)
            {
                var optionText = Console.ReadLine();

                if (optionText == "m")
                {
                    break;
                }
                else if (optionText == "avg")
                {
                    ShowAverage();
                }
            }

            ClearCurrentConsoleLine();
            Console.WriteLine();
            ShowMainMenu();
        }

        static void ShowAverage()
        {
            //var avg =  Marks.Values
            //                .SelectMany(x => x)
            //                .Where(x => x > 3)
            //                .Average();

            var avg = 0.0;
            Console.WriteLine($"La media actual es: {avg}");
            Console.WriteLine();
        }

        static void ShowMinimum()
        {
            Console.WriteLine("La nota más baja es: ");
            Console.WriteLine();
        }

        static void ShowMaximum()
        {
            Console.WriteLine("La nota más alta es: ");
            Console.WriteLine();
        }     

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        #region Formulas

        public static double GetAverage()
        {
            var sum = 0.0;

            //for (var i = 0; i < Marks.Count; i++)
            //{
            //    sum += Marks[i];
            //}

            //return sum / Marks.Count;
            return sum;
        }


        #endregion
    }
}
