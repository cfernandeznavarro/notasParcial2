using System;

namespace Academy.Lib.Models
{
    public class Exam : Entity
    {
        public DateTime DateStamp { get; set; }

        public Subject Subject { get; set; }

        public Student student { get; set; }
    }
}
