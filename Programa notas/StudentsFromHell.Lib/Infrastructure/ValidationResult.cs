 using System.Collections.Generic;

namespace Academy.Lib.Infrastructure
{
    public class ValidationResult
    {

        public bool IsSuccess { get; set; }

        public List<string> Messages { get; set; } = new List<string>();

    }

    public class ValidationResult<T> : ValidationResult
    {
        public T ValidatedResult { get; set; }


    }

}

