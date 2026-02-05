using Microsoft.Data.Sqlite;
using MySqlConnector;
using System.Data.Common;

Console.WriteLine("MariaDB Connection");


MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
builder.Server = "10.30.197.238";
builder.UserID = "domainuser";
builder.Database = "ProductionDB";
builder.Port = 3306;
builder.Password = "xxx";



var builder2 = new MySqlConnectionStringBuilder
{
    Server = "10.30.197.238",
    UserID = "ptdbuser",
    Database = "ProductionDB",
    Port = 3306,
    Password = "xxx"
};

var builder3 = new SqliteConnectionStringBuilder
{
    DataSource = "C:\\work\\training\\databases\\userdb.db"
};

//string connectionString = "Server=localhost;Port=3306;User ID=ptdbuser;Password=xxx;Database=ProductionDB";
//string connectionString = "Server=10.30.197.238;Port=3306;User ID=domainuser;Password=xxxx;Database=ProductionDB";



    





conn.Open();

var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT * FROM users";

var rdr = cmd.ExecuteReader();

while (rdr.Read())
{
    Console.WriteLine(rdr.GetString("name"));
}

rdr.Close();
conn.Close();




