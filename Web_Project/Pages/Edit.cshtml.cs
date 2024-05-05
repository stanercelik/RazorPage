using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_Project;
using Web_Project.Models;
using MySql.Data.MySqlClient;

public class EditModel : PageModel
{
    [BindProperty]
    public Person person { get; set; }
    // Geçici kişi listesi

    public String successMessage = "SUCCESS";

    public void OnGet()
    {
        String id = Request.Query["id"];

        try
        {
            string connectionString = "Server=localhost;Port=3306;Database=ProjectSchema;Uid=root;Pwd=your_psw;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM people WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            person.Id = reader.GetInt32(0);
                            person.Name = reader.GetString(1);
                            person.Surname = reader.GetString(2);
                            person.Gender = reader.GetString(3);
                            person.Age = reader.GetString(4);
                            person.Photo = reader.IsDBNull(5) ? null : (byte[])reader.GetValue(5);
                            
                        }
                    }
                }
            }

        }
        catch
        {
            
        }
    }

    public void OnPost(IFormFile photo)
    {
        String id = Request.Form["id"];

        DateTime today = DateTime.Today;
        if (person.Birthdate > today || person.Birthdate.Year < 1900)
        {
            ModelState.AddModelError("Person.BirthDate", "Invalid birth date.");
        }

        person.Age = CalculateAge(person.Birthdate);

        if (photo != null && photo.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                person.Photo = memoryStream.ToArray();
            }
        }

        try
        {
            String connectionString = "Server=localhost;Port=3306;Database=ProjectSchema;Uid=root;Pwd=Tnrclk2001.;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE people" + 
                             "SET name = @name, surname = @surname, gender = @gender, age = @age, photo = @photo" +
                             "WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", person.Name);
                    command.Parameters.AddWithValue("@surname", person.Surname);
                    command.Parameters.AddWithValue("@gender", person.Gender);
                    command.Parameters.AddWithValue("@age", person.Age);
                    command.Parameters.AddWithValue("@photo", person.Photo);
                    command.Parameters.AddWithValue("@id", person.Id);

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

        Response.Redirect("/PeopleList");
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
