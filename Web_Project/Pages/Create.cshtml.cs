using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_Project;
using Web_Project.Models;
using MySql.Data.MySqlClient;

public class CreateModel : PageModel
{
    [BindProperty]
    public Person Person { get; set; }

    public static List<Person> People = new List<Person>();
    // Geçici kişi listesi


    public IActionResult OnPost(IFormFile photo)
    {

        DateTime today = DateTime.Today;
        if (Person.Birthdate > today || Person.Birthdate.Year < 1900)
        {
            ModelState.AddModelError("Person.BirthDate", "Invalid birth date.");
            return Page();
        }

        Person.Age = CalculateAge(Person.Birthdate);

        if (photo != null && photo.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                Person.Photo = memoryStream.ToArray();
            }
        }

       // People.Add(Person); // Kişiyi listeye ekle

        try
        {
            String connectionString = "Server=localhost;Port=your_port;Database=ProjectSchema;Uid=root;Pwd=your_psw;";
            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                String sql = "INSERT INTO people " +
                    "(name, surname, gender, age, photo) VALUES" +
                    "(@name, @surname, @gender, @age, @photo);";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", Person.Name);
                    command.Parameters.AddWithValue("@surname", Person.Surname);
                    command.Parameters.AddWithValue("@gender", Person.Gender);
                    command.Parameters.AddWithValue("@age", Person.Age);
                    command.Parameters.AddWithValue("@photo", Person.Photo);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("New Client Added To Database Correctly!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ZOOOORRRTTTTT");
        }

        // personRepository.AddPerson(Person); // Kişiyi veritabanına ekle.

        return Page();
    }



    private string CalculateAge(DateTime birthDate)
    {
        DateTime today = DateTime.Today;
        int years = today.Year - birthDate.Year;
        int months = today.Month - birthDate.Month;
        int days = today.Day - birthDate.Day;

        if (months < 0 || (months == 0 && days < 0))
        {
            years--;
        }

        if (months < 0)
        {
            months += 12;
        }

        if (days < 0)
        {
            int daysInPreviousMonth = DateTime.DaysInMonth(today.Year, today.Month - 1);
            days += daysInPreviousMonth;
            months--;
        }

        return $"{years} yıl {months} ay {days} gün";
    }
}
