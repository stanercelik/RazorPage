using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Web_Project
{
    public class PersonRepository
    {
        private readonly string connectionString;

        public PersonRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddPerson(Models.Person person)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO people (name, surname, gender, birthdate, age, photo) VALUES (@name, @surname, @gender, @birthdate, @age, @photo)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", person.Name);
                command.Parameters.AddWithValue("@surname", person.Surname);
                command.Parameters.AddWithValue("@gender", person.Gender);
                command.Parameters.AddWithValue("@birthdate", person.Birthdate);
                command.Parameters.AddWithValue("@age", person.Age);
                command.Parameters.AddWithValue("@photo", person.Photo);

                command.ExecuteNonQuery();
            }
        }
    }
}

