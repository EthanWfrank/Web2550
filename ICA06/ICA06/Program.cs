using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ICA06
{

    // Validating player names --- gameactive = p1 != "" && p2 != ""
    // Check length of string  --- p1.length
    // status = bool ? true : false
    // Math.clamp(number, lwoer, upper)





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

                return Results.Json(studentData); // RETURNS A JSON OBJECT OF STUDENTS
            });
            ////////////////////////////////////////////////////////////////////////////SELECT DATA FOR TABLE/////////////////////////////////////////////////
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

            //////////////////////////////////////////////////////////////////////////WELCOME/////////////////////////////////////////////////////////////////
            app.MapGet("/Welcome", ()=> 
            {
                return new
                {
                    msg = "welcome"
                };
            });

            ///////////////////////////////////////////////////////////////////////////CREATING A SELECT////////////////////////////////////////////////////////////
            app.MapGet("/location", () =>
            {
                return new
                {
                    location = Locations
                };
            });

            ///////////////////////////////////////////////////////////////////////////POST///////////////////////////////////////////////////////////////////
            
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
            ///////////////////////////////////////////////////////////////////////////DELETING//////////////////////////////////////////////////////////////////
            
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
            ////////////////////////////////////////////////////////////////////////////UPDATING//////////////////////////////////////////////////////////////
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

            ////////////////////////////////////////////////////////////////////////////SELECT CLASSES//////////////////////////////////////////////////////////
            app.MapGet("/GetClasses", () =>
            {
                // Step one open a new connection
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Create the script
                string script = "SELECT * FROM Classes";

                // Executing the script
                // giving the script and the connections made
                SqlCommand command = new SqlCommand(script, conn);

                SqlDataReader reader = command.ExecuteReader();

                // Creating a list of objects
                string classOptions = " ";
                while (reader.Read())
                {
                    classOptions += $"<option value = '{reader["class_id"]}'> {reader["class_desc"]}</option>";
                }

                return classOptions; // Returning HTML
            });

            ////////////////////////////////////////////////////////////////////////////INSERTING//////////////////////////////////////////////////////////////
            app.MapPost("/AddStud", (addStudent ast) =>
            {
                // Connect to data
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // using a try catch to see if it is a number
                try
                {
                    int i = int.Parse(ast.studentId);
                }
                catch(Exception e)
                {
                    return new { status = "Not a Number" };
                }

                //creating a script for the data base
                // Creating Command
                // Executing command
                string script = $"INSERT INTO Students (last_name,first_name, school_id) VALUES ('{ast.lname}', '{ast.fname}', {ast.studentId});" +
                                $"SELECT SCOPE_IDENTITY() as studid";
                SqlCommand command = new SqlCommand(script, conn);
                SqlDataReader reader = command.ExecuteReader();

                // Getting the student id
                // closes the connection
                reader.Read();
                string studID = reader["studid"].ToString();
                reader.Close();

                // inserting in the class to student 
                // Remeber to close the reader
                foreach (string str in ast.classes)
                {
                   string addClass_Stud = $"INSERT INTO Class_to_student (class_id, student_id) VALUES ({str}, {studID});";
                   Console.WriteLine(addClass_Stud);
                   SqlCommand command2 = new SqlCommand(addClass_Stud, conn);
                   SqlDataReader reader2 = command2.ExecuteReader();
                   reader2.Close();
                }

                return new { status = "added the student" };
            });

            app.MapGet("/sendData/{num1}", (int num1) =>
            {
            int newNum = num1 * 10;

            List<object> objectList = new List<object>();

            objectList.Add(new
            {
                num1 = newNum,
                num2 = newNum + 5
            });

            objectList.Add(new
            {
                 num1 = newNum * 10,
                 num2 = newNum + 50
            });

                /*                List<Data> numbers = new List<Data>();

                                numbers.Add(new Data(newNum));

                                foreach(Data d in numbers)
                                {
                                    Console.WriteLine(d.ToString());
                                }*/

                foreach (object obj in objectList)
            {
                Console.WriteLine(obj.ToString());
            }

                return objectList;
            });
           
        app.Run();
        }
        record class ClientD(string sid);
        record class ClientData(string fname, string age);
        record class Student(int studentID, string lname, string fname, int schoolID);
        record class UpdateStudent(string lname, string fname, string sid);
        record class addStudent(string lname, string fname, string studentId, string[] classes);

        record class Data(int num1);
    
        public void StudentTable()
        {

        }

    }
}
