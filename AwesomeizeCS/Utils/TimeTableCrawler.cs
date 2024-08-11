using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using TechnologySandbox.Data;


namespace AwesomeizeCS.Utils
{
    public class TimeTableCrawler
    {
        // datele de start si sfarsit semestru
        //private static readonly DateTime semesterStart = new DateTime(2024, 2, 26);
        //private static readonly DateTime semesterEnd = new DateTime(2024, 6, 9);
        //private static readonly DateTime semesterEndFinalYear = new DateTime(2024, 5, 24);
        private static readonly DateTime semesterStart = new DateTime(2023, 10, 2);
        private static readonly DateTime semesterEnd = new DateTime(2024, 1, 21);
        private static readonly DateTime semesterEndFinalYear = new DateTime(2024, 5, 24);

        // lista cu zile libere
        private static readonly List<DateTime> freeDays = new List<DateTime>
        {
            //new DateTime(2024,5,1), //
            new DateTime(2023, 12, 25), // Craciun
            //new DateTime(2023, 11, 30), // Sf. Andrei
            //new DateTime(2023, 12, 1), // Romania
            //new DateTime(2024, 1, 24), // Romania 2
        };

        public static async Task<List<TimeTable>> GetTimeTableFromWebsiteAsync(ApplicationDbContext dbContext)
        {
            await GenerateFreeDaysForSemesterAsync();
            var timeTables = new List<TimeTable>();
            string[] urls = { "https://www.cs.ubbcluj.ro/files/orar/2023-1/tabelar/IE3.html" };

            using var httpClient = new HttpClient();

            // pt fiecare link de orar luam orarul si il transformam in timetable objects
            foreach (var url in urls)
            {
                string htmlContent = await httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // tabelele de la fiecare grupa
                var tables = doc.DocumentNode.SelectNodes("//table");
                foreach (var table in tables)
                {
                    var rows = table.SelectNodes(".//tr[position()>1]");
                    if (rows != null)
                    {
                        foreach (var row in rows)
                        {
                            var cells = row.SelectNodes(".//td");
                            if (cells != null && cells.Count >= 8)
                            {
                                // luam ziua saptamanii si o parsam ca sa fie in datetime
                                var dayOfWeek = ParseDayOfWeek(cells[0].InnerText.Trim());
                                var startHour = cells[1].InnerText.Trim().Split('-')[0];
                                var endHour = cells[1].InnerText.Trim().Split('-')[1];
                                var frequency = cells[2].InnerText.Trim();
                                var week = 1;


                                // generam tuple cu ziua si ora start + ziua si ora end
                                var dates = GenerateDatesForSemester(dayOfWeek, startHour, endHour, frequency);

                                foreach (var datePair in dates)
                                {
                                    
                                    var courseName = cells[6].InnerText.Trim();
                                    var course = await dbContext.Course.FirstOrDefaultAsync(c => c.Name == courseName);

                                    if (course != null)
                                    {
                                        // generam obiectele timetable
                                        var timeTable = new TimeTable
                                        {
                                            Id = Guid.NewGuid(),
                                            StartsAt = datePair.Item1,
                                            EndsAt = datePair.Item2,
                                            Type = ParseInstructionType(cells[5].InnerText.Trim()),
                                            For = cells[4].InnerText.Trim(),
                                            Room = cells[3].InnerText.Trim(),
                                            AcademicYear = "2023-2024",
                                            Week = week,
                                            Course = course
                                        };
                                        if (!timeTables.Any(t =>
                                            t.StartsAt == timeTable.StartsAt &&
                                            t.EndsAt == timeTable.EndsAt &&
                                            t.Type == timeTable.Type &&
                                            t.For == timeTable.For &&
                                            t.Room == timeTable.Room &&
                                            t.AcademicYear == timeTable.AcademicYear) &&
                                            timeTable.Course == timeTable.Course)
                                            {
                                                timeTables.Add(timeTable);
                                            }

                                        week++;

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return timeTables;
        }

        private static DayOfWeek ParseDayOfWeek(string day)
        {
            return day switch
            {
                "Luni" => DayOfWeek.Monday,
                "Marti" => DayOfWeek.Tuesday, //Curse of Diacritice
                "Miercuri" => DayOfWeek.Wednesday,
                "Joi" => DayOfWeek.Thursday,
                "Vineri" => DayOfWeek.Friday,
                _ => DayOfWeek.Saturday,
            };
        }

        private static List<Tuple<DateTime, DateTime>> GenerateDatesForSemester(DayOfWeek dayOfWeek, string startHour,
            string endHour, string frequency)
        {
            int daysToBeAdded = 7;
            var dates = new List<Tuple<DateTime, DateTime>>();
            var start = semesterStart;
            while (start.DayOfWeek != dayOfWeek)
                start = start.AddDays(1);
            if (!frequency.Equals("&nbsp;"))
            {
                daysToBeAdded = 14;
                if (frequency.Equals("sapt. 2"))
                    start = start.AddDays(7);
            }
            for (var date = start; date <= semesterEnd; date = date.AddDays(daysToBeAdded))
            {
                if (!freeDays.Contains(date.Date))
                {
                    // starts at si ends at
                    var startTime = date.AddHours(int.Parse(startHour));
                    var endTime = date.AddHours(int.Parse(endHour));
                    dates.Add(new Tuple<DateTime, DateTime>(startTime, endTime));
                }
            }

            return dates;
        }

        private static async Task GenerateFreeDaysForSemesterAsync()
        {
            string targetHeader = "Limbile de predare română şi engleză, nivel licenţă şi master";
            string semesterName = "Semestrul I"; // These will be input parameters and gotten either Via parsing or using some other logic depending on what we want to generate
            string url = "https://www.cs.ubbcluj.ro/invatamant/structura-anului-universitar/";
            using var httpClient = new HttpClient();
            string htmlContent = await httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var headerNode = doc.DocumentNode.SelectSingleNode($"//h1[contains(., '{targetHeader}')]");

            //var tables = doc.DocumentNode.SelectNodes("//table");
            var table = headerNode.SelectSingleNode("following-sibling::table[1]");

                //var rows = table.SelectNodes(".//tr[position()>1]");
                var rows = table.SelectNodes(".//tr");
                if (rows != null)
                {
                    bool foundSemester = false;
                    foreach (var row in rows)
                    {
                        var header = row.SelectSingleNode(".//th");

                        if (foundSemester ==true && header?.InnerText.Contains("Semestrul") == true)
                        {
                            break;
                        }
                        if (header != null && header.InnerText.Trim() == semesterName)
                        {
                            foundSemester |= true;
                            continue;
                        }

                        if (foundSemester)
                        {
                            var cells = row.SelectNodes(".//td");
                            if (cells != null && cells.Count >= 3)
                            {

                                if (cells[1].InnerText.Trim() != "activitate didactică")
                                {
                                    var dateRangeString = HttpUtility.HtmlDecode(cells[0].InnerText.Trim());
                                    var dateParts = dateRangeString.Split("–"); // Splitting the date range string by '–'
                                    if (dateParts.Length == 2)
                                    {
                                        if (DateTime.TryParseExact(dateParts[0].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) &&
                                            DateTime.TryParseExact(dateParts[1].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
    
                                        {
                                            // Adding all dates between start date and end date to freeDays list
                                            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                                            {
                                                freeDays.Add(date);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                var description = cells[2].InnerText.Trim();

                                // Regular expression pattern to match dates in the format dd.mm.yyyy
                                var pattern = @"\b\d{2}\.\d{2}\.\d{4}\b";

                                // Find all matches of the pattern in the description
                                MatchCollection matches = Regex.Matches(description, pattern);

                                // Iterate over matches and add the extracted dates to freeDays
                                foreach (Match match in matches)
                                {
                                    var dateString = match.Value;
                                    // Parse the date string and add it to freeDays
                                    if (DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                                    {
                                        freeDays.Add(date);
                                    }
                                }

                            }

                            }
                        }
                    
                    }
                }
            
        }

        private static InstructionType ParseInstructionType(string type)
        {
            return type.Contains("Curs") ? InstructionType.Course :
                type.Contains("Laborator") ? InstructionType.Laboratory :
                InstructionType.Seminar;
        }
    }
}

//TODO:
// saptamana para si impara - trebuie decalate in caz ca sunt zile libere (pt detalii la Imre)
// check if what is here so far is correct (should be)