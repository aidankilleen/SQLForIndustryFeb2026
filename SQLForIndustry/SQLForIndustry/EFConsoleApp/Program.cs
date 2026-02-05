using EFConsoleApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


Console.WriteLine("Entity Framework Investigation");



var builder = Host.CreateApplicationBuilder(args);

var environmentName = builder.Environment.EnvironmentName;
Console.WriteLine(environmentName);

string database = environmentName.Equals("Production") ? "mysql" : "sqlite";


string connectionString = builder.Configuration.GetConnectionString(database);

// create the database context
builder.Services.AddDbContext<AppDbContext>(options => {
    if (database.Equals("mysql"))
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    } else
    {
        options.UseSqlite(connectionString);
    }   
});

var app = builder.Build();




var scope = app.Services.CreateScope();

// get the database context from the services
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

foreach (var user in db.Users)
{
    Console.WriteLine(user.Name);
}



//db.Users