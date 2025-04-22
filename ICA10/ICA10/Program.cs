using ICA10.ModelsRd;

namespace ICA10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();


            app.UseDefaultFiles(); // use idex.html in wwwroot folder
            app.UseStaticFiles(); // Load all the files in wwwroot folder




            app.MapGet("/OnLoad", () =>
            {

                Console.WriteLine("Inside ret");
                EthanRContext context = new EthanRContext();

                // Gets all the table data
                // var results = context.Locations;  // locations
                // var ritems = context.Items;      // items
                //var rorders = context.Orders;   // orders
                
                // Find always works with PK
                //if (context.Customers.Find(3) is Customer c)
                if(context.Orders.Find(1, 3) is Order o)
                {
                    var script = context.Orders
                                               .Where(x => x.Cid == 3)
                                               .Select(x => new
                                               {
                                                   fname = x.CidNavigation.Fname,
                                                   orderID = x.OrderId,
                                                   itemName = x.Item.ItemName
                                               });

                    // Add

                    /*
                    Order newOrder = new Order();
                    newOrder.ItemCount = 5;

                    context.Orders.Add(newOrder);
                    context.SaveChanges();
                    */
                    //Delete

                    context.Orders.Remove(o);
                    context.SaveChanges();


                    // Update


                    o.PaymentMethod = "Debit";

                    context.Orders.Update(o);

                    context.SaveChanges();

                    return (object)new
                    {
                        status = "Success",
                        data = script.ToList()
                    };
                }
                else
                {
                    return (object)new
                    {
                        status = "Success",
                        data = "Sorry no such customer in my DB"
                    };
                    
                }

/*
                return (object) new { 
        *//*            order = rorders*/
/*                    items = ritems,
                    locations = results,*//*
                };*/
            });


            app.Run();



        }
    }
}
