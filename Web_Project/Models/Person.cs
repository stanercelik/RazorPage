using System;
namespace Web_Project.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Age { get; set; }
        public byte[] Photo { get; set; }
    }
}

