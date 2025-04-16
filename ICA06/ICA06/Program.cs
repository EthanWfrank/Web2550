using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ICA06
{
    public class Program
    {
        public static string[] Locations = { "l1", "L2", "L3" };
        static string age;
        static string name;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles(); // use idex.html in wwwroot folder
            app.UseStaticFiles(); // Load all the files in wwwroot folder

            /*            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(orgin => true));*/



            // Connecting to my database
            // Creating the connectoin string
            string connectionString = "Server = data.cnt.sast.ca,24680;" +
                                      "Database = EthanF_ClassTrack;" +
                                      "User Id = efrank3;" +
                                      "Password= aZx243mnb5342@;" +
                                      "Encrypt = false;";



            app.MapGet("/select", () =>
            {
                // Make connection to database
                SqlConnection conn = new SqlConnection(connectionString);

                // Connection to the server
                conn.Open();

                // Creating the script
                // This script will use a var @stid
                string script = "SELECT * FROM Students WHERE first_name like 'E%' or first_name like 'F%' ";

                // executing the scrip
                // Need to creat an SqlCommand object
                SqlCommand command  = new SqlCommand(script, conn);
                
                // This will run the Query
                // Itterates through the result set
                // Returns true or false
                SqlDataReader reader = command.ExecuteReader();

                // Create a list that can hold objects
                List<Student> studentData = new List<Student>();

                // Itterates throigh the returned data
                while (reader.Read()) 
                {
                    Console.WriteLine($"{reader["student_id"]} {reader["last_name"]}");
                    Console.WriteLine(reader.FieldCount.ToString());

                    // Creates an object
                    // field.Count is the amount of columns 
                    Student newstud = new Student((int)reader["student_id"], (string)reader["last_name"], (string)reader["first_name"], (int)reader["school_id"]);
                    studentData.Add(newstud);
                }

                return Results.Json(studentData);
            });

            app.MapPost("/GetStudentData", (ClientD s) =>
            {
                Console.WriteLine($"Inside Get Student Data {s.sid}");
                // Make connection to database
                SqlConnection conn = new SqlConnection(connectionString);

                // Connection to the server
                conn.Open();

                string query = "select c.class_id, c.class_desc, c.days, c.start_date, i.instructor_id, i.first_name, i.last_name " +
                               "from Students s " +
                               "join class_to_student cts on cts.student_id = s.student_id " +
                               "join Classes c on c.class_id = cts.class_id " +
                               "join Instructors i on i.instructor_id = c.instructor_id " +
                               "where s.student_id = " + s.sid;

                // executing the scrip
                // Need to creat an SqlCommand object
                SqlCommand command = new SqlCommand(query, conn);

                // This will run the Query
                // Itterates through the result set
                // Returns true or false
                SqlDataReader reader = command.ExecuteReader();

                // Create a list that can hold objects
                List<object> studentData = new List<object>();

                // Itterates throigh the returned data
                while (reader.Read())
                {
                   // Console.WriteLine($"{reader["student_id"]} {reader["last_name"]}");
                    //Console.WriteLine(reader.FieldCount.ToString());

                    // Creates an object
                    // field.Count is the amount of columns 
                    //Student newstud = new Student((int)reader["student_id"], (string)reader["last_name"], (string)reader["first_name"], (int)reader["school_id"]);
                    Console.WriteLine(reader["start_date"]);
                    studentData.Add(
                            new { classid = reader["class_id"],
                                  class_desc = reader["class_desc"],
                                  days = reader["days"] != DBNull.Value ? reader["days"] : 0,
                                  startDate = (DateTime)reader["start_date"],
                                  instructor = reader["instructor_id"],
                                  firstName = reader["first_name"],
                                  lastName = reader["last_name"]
                            }
                        );
                }
                return studentData.ToList();


            });

            // Makes an AJAX call and prints welcome on Main Page
            app.MapGet("/Welcome", ()=> 
            {
                return new
                {
                    msg = "welcome"
                };
            });

            app.MapGet("/location", () =>
            {
                return new
                {
                    location = Locations
                };
            });

            // When using post i must creat a record class to hold the client data
            app.MapPost("/NameAge", (ClientData cl) =>
            {
                // stores in global var 
                age = cl.age;
                name = cl.fname;
                int iage = int.Parse(cl.age);
                Random rand = new Random();
                int Time = rand.Next(1, 101);
                return $"Your name is {cl.fname} and your age is {cl.age} time it took is: {Time}m";
            });

            app.MapDelete("/DeleteStud/{id}", (int id) =>
            {
                // Openning a new connection
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = "DELETE FROM Results WHERE student_id =" + id +
                               "DELETE FROM class_to_student WHERE student_id =" + id +
                               "DELETE FROM Students WHERE student_id =" + id;

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataReader reader = command.ExecuteReader();
            });

            app.MapPut("/UpdateStud/{id}", (UpdateStudent us, int id) => 
            {
                Console.WriteLine($"lname = {us.lname}, fname = {us.fname}, id = {us.sid} student id = {id}");

                // Checking to make sure id is an int
                try
                {
                    int sid = int.Parse(us.sid);
                    if (sid < 0)
                        return "should be > 0";
                }
                catch (Exception e)
                {
                    return (object)new {
                        mesg = $"not a number {us.sid}",
                        status = "fail",
                        num = us.sid
                    };
                }

                // connectiong to the database
                SqlConnection conn = new SqlConnection(connectionString); // Making connection var
                conn.Open(); // Opening the connection
                string script = "UPDATE Students " +
                                $"SET last_name = '{us.lname}', first_name = '{us.fname}', school_id = '{us.sid}' " +
                                $"WHERE student_id = {id} ";
                SqlCommand command = new SqlCommand(script, conn); // Creating a command object
                SqlDataReader reader = command.ExecuteReader(); // Execute the commande

                return (object)new
                {
                    status = "Update Success"
                };

            });
           
        app.Run();
        }
        record class ClientD(string sid);
        record class ClientData(string fname, string age);
        record class Student(int studentID, string lname, string fname, int schoolID);
        record class UpdateStudent(string lname, string fname, string sid);
    
        public void StudentTable()
        {

        }

    }
}
