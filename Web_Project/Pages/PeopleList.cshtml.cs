using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_Project.Models;


public class PeopleModel : PageModel
{
    public List<Person> People { get; set; }


    public void OnGet(string search, string sort)
    {
        string connectionString = "Server=localhost;Port=3306;Database=ProjectSchema;Uid=root;Pwd=your_psw;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string sql = "SELECT * FROM people";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    People = new List<Person>();
                    while (reader.Read())
                    {
                        Person person = new Person
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Surname = reader.GetString(2),
                            Gender = reader.GetString(3),
                            Age = reader.GetString(4),
                            Photo = reader.IsDBNull(5) ? null : (byte[])reader.GetValue(5)
                        };

                        People.Add(person);
                    }
                }
            }
        }

        // People = CreateModel.People;
        IQueryable<Person> query = People.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Surname.Contains(search));
        }

        switch (sort)
        {
            case "name":
                query = query.OrderBy(p => p.Name);
                break;
            case "surname":
                query = query.OrderBy(p => p.Surname);
                break;
            case "gender":
                query = query.OrderBy(p => p.Gender);
                break;
            case "age":
                query = query.OrderBy(p => p.Age).Reverse();
                break;
        }

        People = query.ToList();


    }
}


