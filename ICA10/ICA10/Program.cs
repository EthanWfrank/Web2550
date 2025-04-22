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

                EthanRContext context = new EthanRContext();

                // Gets all the table data
                var results = context.Locations;  // locations
                var ritems = context.Items;      // items
                var rorders = context.Orders;   // orders

                var script = context.Orders.Where(x => x.Cid == 3)
                                           .Select(x => new
                                           {
                                               fname = x.CidNavigation.Fname,
                                               orderID = x.OrderId,
                                               itemName = x.Item.ItemName
                                           });


                return script.ToList();
                              

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
