using Exam4Practice.ModelsNWD;
using System.Runtime.Intrinsics.X86;

namespace Exam4Practice
{
    public class Program
    {


/*  Scaffold-DbContext "Server=data.cnt.sast.ca,24680;Database=Ethan_R;User Id = efrank3;Password=aZx243mnb5342@; Encrypt=False;"Microsoft.EntityFrameworkCore.SqlServer -OutputDir ModelsNWD*/


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles(); // use idex.html in wwwroot folder
            app.UseStaticFiles(); // Load all the files in wwwroot folder

            app.MapGet("/Select", () =>
            {
                Console.WriteLine("Inside Select");

                EthanRContext context = new EthanRContext();

                List<Order> orders = context.Orders.Where(x => x.OrderId % 2 == 0).ToList();

                string table = "<table>";
                table += "<tr>";
                table += "<th>OrderID</th>";
                table += "<th>Location</th>";
                table += "<th>PaymentMethod</th>";
                table += "<th>ItemID</th>";
                table += "<th>ItemCount/th>";
                table += "<th>OrderDate</th>";
                table += "</tr>";
                table += "<table>";
     



                foreach (Order o in orders)
                {
                    table += "<tr>";
                    table += $"<td>{o.OrderId}</td>";
                    table += $"<td>{o.Locationid}</td>";
                    table += $"<td>{o.Cid}</td>";
                    table += $"<td>{o.PaymentMethod}</td>";
                    table += $"<td>{o.Itemid}</td>";
                    table += $"<td>{o.ItemCount}</td>";
                    table += $"<td>{o.OrderDate}</td>";
                    table += "</tr>";
                }
                table += "</table>";

                return table;

            });

            app.MapGet("/SelectOne", () =>
            {
                EthanRContext context = new EthanRContext();
                /*
                select c.FName, c.LName, i.ItemName,o.Locationid
                from Orders o
                join Customers c on c.Cid = o.Cid
                join Items i on i.Itemid = o.Itemid
                where c.FName like 's%'*/

                string table = "<table>";
                table += "<tr>";
                table += "<th>Fname</th>";
                table += "<th>Lname</th>";
                table += "<th>ItemName</th>";
                table += "<th>Location</th>";
                table += "</tr>";
                table += "<table>";

                context.Orders.Where(x => x.CidNavigation.Fname.StartsWith("s"))
                              .Select(s => new
                              {
                                  fname = s.CidNavigation.Fname,
                                  lname = s.CidNavigation.Lname,
                                  itemName = s.Item.ItemName,
                                  location = s.Locationid
                              }).ToList().ForEach(s =>
                              {
                                  table += "<tr>";
                                  table += $"<td>{s.fname}</td>";
                                  table += $"<td>{s.lname}</td>";
                                  table += $"<td>{s.itemName}</td>";
                                  table += $"<td>{s.location}</td>";
                                  table += "</tr>";
                              });
                table += "</table>";

                return table;
           
            });

            app.MapPut("/add/{itemname}/{itemPrice}/{itemID}", (string itemname, string itemprice, int itemID) =>
            {
                int price = 0;
                Console.WriteLine("inside update");

                EthanRContext ctx = new EthanRContext();    
                try
                {
                    price = int.Parse(itemprice);
                }
                catch(Exception e)
                {
                    return new { status = "FAIL" };
                }


                if(price < 2)
                {
                    return new { status = "Needs to be more then $2" };
                }

                Item newItem = new Item();

                newItem.ItemName = itemname;
                newItem.ItemPrice = price;
                newItem.Itemid = itemID;


                ctx.Add(newItem); ;
                ctx.SaveChanges();

                return new { status = "Success" };
            });

            app.MapDelete("/delete/{id}", (int id) =>
            {
                Console.WriteLine("inside delete");
                EthanRContext ctx = new EthanRContext();

                if (ctx.Items.Find(id) is Item i)
                {
                    ctx.Remove(i);
                    ctx.SaveChanges();
                }
                else
                    return new { status = "Not found" };

                return new { status = "successs" };
            });

            app.MapPut("/Update", (UpdateData d) =>
            {
                EthanRContext ctx = new EthanRContext();

                if (ctx.Items.Find(d.id) is Item i)
                {
                    i.ItemName = d.name;
                    i.ItemPrice = d.price;

                    ctx.Items.Update(i);
                    ctx.SaveChanges();
                }
                else
                    return new { status = "not found" };

                return new { status = "success" };
            });

            app.Run();

        }
        record class UpdateData(int id, string name, int price);
    }
}
