using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_Project.Models;
using MySql.Data.MySqlClient;


public class DashboardModel : PageModel
{
    public List<Person> People { get; set; }

    public int maleCount { get; set; }
    public int femaleCount { get; set; }

    public double ageGroup1Average { get; set; }
    public double ageGroup2Average { get; set; }
    public double ageGroup3Average { get; set; }
    public double ageGroup4Average { get; set; }


    public void OnGet()
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

        //People = PeopleModel.People;

        calculateStatistics();
    }

   
    //public IActionResult OnPostExportToPdf(string exportButton)
    //{
    //    calculateStatistics();

    //    using (MemoryStream stream = new MemoryStream())
    //    {
    //        using (PdfDocument document = new PdfDocument())
    //        {
    //            PdfPage page = document.AddPage();
    //            PdfGraphics graphics = page.Graphics;

    //            // Verileri PDF'e yazdır
    //            int x = 50;
    //            int y = 50;
    //            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

    //            graphics.DrawString("Cinsiyet Dağılımı", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"Erkek: {maleCount}", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"Kadın: {femaleCount}", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 40;

    //            graphics.DrawString("Yaş Gruplarına Göre Ortalama Yaş", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"0-15: {ageGroup1Average}", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"15-30: {ageGroup2Average}", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"30-45: {ageGroup3Average}", font, PdfBrushes.Black, new PointF(x, y));
    //            y += 20;
    //            graphics.DrawString($"45+: {ageGroup4Average}", font, PdfBrushes.Black, new PointF(x, y));

    //            // PDF dosyasını kaydet
    //            document.Save(stream);
    //        }

    //        stream.Position = 0;
    //        return File(stream, "application/pdf", "dashboard_data.pdf");
    //    }

    //}




    private void calculateStatistics()
    {
        // Cinsiyete göre kaydedilmiş kişilerin oranını hesapla
        maleCount = People.Count(p => p.Gender == "Erkek");
        femaleCount = People.Count(p => p.Gender == "Kadın");

        // Yaş gruplarına göre yaş ortalamalarını hesapla
        ageGroup1Average = calculateAverageAge(People, 0, 15);
        ageGroup2Average = calculateAverageAge(People, 15, 30);
        ageGroup3Average = calculateAverageAge(People, 30, 45);
        ageGroup4Average = calculateAverageAge(People, 45, int.MaxValue);
    }


    // Yaş ortalamasını hesaplayan yardımcı bir metod
    private double calculateAverageAge(List<Person> people, int minAge, int maxAge)
    {
        var filteredPeople = people.Where(p => getAgeInYears(p.Age) >= minAge && getAgeInYears(p.Age) < maxAge);
        if (filteredPeople.Any())
        {
            return filteredPeople.Average(p => getAgeInYears(p.Age));
        }
        else
        {
            return 0;
        }
    }

    // Yaşı yıl olarak hesaplayan yardımcı bir metod
    private int getAgeInYears(string age)
    {
        // Yaş bilgisini "x yıl y ay z gün" formatından yıl olarak dönüştür
        var parts = age.Split(' ');
        if (parts.Length >= 2)
        {
            int years = Convert.ToInt32(parts[0]);
            return years;
        }
        else
        {
            return 0;
        }
    }

}

