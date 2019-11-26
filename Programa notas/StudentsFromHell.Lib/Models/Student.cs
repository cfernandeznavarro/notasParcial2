using System;
using System.Collections.Generic;
using System.Linq;
using Academy.Lib.Context;
using Academy.Lib.Infrastructure;

namespace Academy.Lib.Models
{
    public class Student : User
    {

        /* Creamos una region para los motodos static. Al ser static pertenecen a la clase y no hace falta que instanciemos un objeto. Tiene todo el sentido, ya 
         que las validaciones de las properties de Student (dni, name, ChairNumber) las queremos hacer antes de instanciar estudiante (objeto clase Student).*/


        #region statics

        /* El metodo ValidateChairNumber quieres hacer varias comprobaciones de la property ChairNumber:
          - Dado que ChairNumber es una variable que pedimos al usuario por consola, chairNumberText es un STRING. 
          - Sin embargo, si las comprobaciones son ok, el valor que nos devolverá wserá un int.
          -  ValidationResult <int> : la funcion NO devulve un objeto de la clase  ValidationResult, sino que directamente nos devuelte un int ya que:
                  - Hemos definido la clase ValidationResult con generic <T> y la property T es  "public T ValidatedResult { get; set; }".
                  - Al ser la unica property T, nuestro metodo ya sabe que nos debe devolver la variable ValidatedResult, que es el resultado de la validacion (en este caso forzandola a que sea un int, 
                    porque estamos en ChairNumber) y no un obejto ValidationResult.    */

        public static ValidationResult<int> ValidateChairNumber(string chairNumberText)
        {
            var output = new ValidationResult<int>
            {
                IsSuccess = true,
            };

            var chairNumber = 0;
            var isConversionOk = false;

            #region check is null or empty

            if (string.IsNullOrEmpty(chairNumberText))
            {
                output.IsSuccess = false;
                output.Messages.Add("el número de silla está vacío, vuelva a escribirlo");
            }
            #endregion

            #region check format

            isConversionOk = int.TryParse(chairNumberText, out chairNumber);
            if (!isConversionOk)
            {
                output.IsSuccess = false;
                output.Messages.Add($"el texto introducido [{chairNumberText}] para el formato de silla es incorrecto, vuelva a escribirlo");
            }

            #endregion

            #region check duplicated

            var estudianteSentado = DbContext.Students.Values.FirstOrDefault(s => s.ChairNumber == chairNumber);

            // esto  default(Student) es casi lo mismo que el nulo
            //if (boolIsConversionOk && DbContext.Students.Values.Any(s=>s.ChairNumber == chairNumber))
            if (isConversionOk && estudianteSentado != default(Student))
            {
                output.IsSuccess = false;
                output.Messages.Add($"La silla {chairNumber} ya está ocupada por {estudianteSentado.Name}");
            }
            #endregion

            if (output.IsSuccess)
                output.ValidatedResult = chairNumber;

            return output;
        }

        public static ValidationResult<string> ValidateDni(string dni, Guid excludeExistingId)
        {
            var output = new ValidationResult<string>
            {
                IsSuccess = true,
            };

            #region check format

            if (string.IsNullOrEmpty(dni))
            {
                output.IsSuccess = false;
                output.Messages.Add("el dni está en formato incorrecto, vuelva a escribirlo");
            }
            #endregion

            #region check duplicated

            if (DbContext.StudentsByDni.ContainsKey(dni)) //redundante??? me salen los dos erroes
            {
                var studentWithDni = DbContext.StudentsByDni[dni];

                if (studentWithDni.Id == excludeExistingId)
                {
                    //todo bien
                }
                else
                {
                    output.IsSuccess = false;
                    output.Messages.Add("el dni ya existe");
                }
            }


            if (DbContext.StudentsByDni.ContainsKey(dni) && (DbContext.StudentsByDni[dni].Id != excludeExistingId))
            {
                output.IsSuccess = false;
                output.Messages.Add("el dni ya existe");
            }

                #endregion

                if (output.IsSuccess)
                output.ValidatedResult = dni;
                output.Messages.Add("el dni se ha guardado correctamente");


            return output;
        }

        public static ValidationResult<string> ValidateName(string name)
        {
            var output = new ValidationResult<string>
            {
                IsSuccess = true,
            };

            #region check format

            if (string.IsNullOrEmpty(name))
            {
                output.IsSuccess = false;
                output.Messages.Add("el nombre está en formato incorrecto, vuelva a escribirlo");
            }
            #endregion

            if (output.IsSuccess)
                output.ValidatedResult = name;

            return output;
        }



        #endregion

        public string Dni { get; set; }

        public int ChairNumber { get; set; }

        public List<Exam> Exams
        {
            get
            {
                return DbContext.Exams.Values.Where(e => e.student.Id == this.Id).ToList();
            }
        }

        public bool Save() {

            ValidationResult validation = ValidateDni(this.Dni, Id);
            if (!validation.IsSuccess)
                return false;

            /*ValidationResult<string> valresstr = ValidateDni(this.Dni, true);
            var b1 = valresstr.ValidatedResult;


            var validation = (ValidationResult)ValidateDni(this.Dni, true);
            var b2 = validation.ValidatedResult;

            var reconv = (ValidationResult<string>)validation;
            var b3 = reconv.ValidatedResult;*/

            validation = ValidateName(this.Name);
            if (!validation.IsSuccess)
                return false;

            validation = ValidateChairNumber(this.ChairNumber.ToString());
            if (!validation.IsSuccess)
                return false;

            if (this.Id == Guid.Empty)
            {
                DbContext.AddStudent(this);
            }
            else
            {
                DbContext.UpdateStudent(this);

            }


            return true;
        }
    }
}
