
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using System.Data.Common;

public class Program
{
    //private static void Main(string[] args)
    //{
        
    //}

    public static async Task Main(string[] args)
    {
        //Console.WriteLine("Database Independent Connection");

        var appBuilder = Host.CreateApplicationBuilder(args);

        appBuilder.Configuration.AddUserSecrets<Program>();


        var password = appBuilder.Configuration["masterpassword"];
        Console.WriteLine($"Secret password{password}");


        string? appName = appBuilder
                            .Configuration
                            .GetSection("Info")
                            .GetValue<string>("AppName");

        Console.WriteLine(appName);

        var environmentName = appBuilder.Environment.EnvironmentName;
        Console.WriteLine(environmentName);

        string provider;
        string connectionString;
        if (environmentName.Equals("Production"))
        {
            provider = "MySqlConnector";
            connectionString = appBuilder.Configuration.GetConnectionString("mysql");
        }
        else
        {
            provider = "Microsoft.Data.Sqlite";
            connectionString = appBuilder.Configuration.GetConnectionString("sqlite");
        }
        //Console.WriteLine(appBuilder.Environment.);


        /*
        var connectionStringBuilder = new SqliteConnectionStringBuilder
       {
           DataSource = "c:\\work\\training\\databases\\userdb.db",
       };
       */
        /*
        var provider = "MySqlConnector";
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "10.30.197.238",
            UserID = "ptdbuser",
            Database = "ProductionDB",
            Port = 3306,
            Password = "xxx"
        };
        */
        //await RunSelect(provider, connectionStringBuilder.ConnectionString);
        await RunSelect(provider, connectionString);

    }

    public static DbProviderFactory GetFactory(string provider)
    {
        return provider switch
        {
            "MySqlConnector" => MySqlConnectorFactory.Instance, 
            "Microsoft.Data.Sqlite" => SqliteFactory.Instance,
            _ => throw new ArgumentException($"unknown provider {provider}")
        };
    }

    public static async Task RunSelect(string provider, string connectionString)
    {
        var factory = GetFactory(provider);
        var conn = factory.CreateConnection();
        conn.ConnectionString = connectionString;

        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM users";

        var rdr = await cmd.ExecuteReaderAsync();

        while (rdr.Read()) {
            Console.WriteLine(rdr.GetString(1));
        }
        rdr.Close();
        conn.Close();
    }
}