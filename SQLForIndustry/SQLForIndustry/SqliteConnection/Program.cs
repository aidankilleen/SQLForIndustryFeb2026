

using Microsoft.Data.Sqlite;

Console.WriteLine("SQLite Connection");

SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder
{
    DataSource="C:\\work\\training\\databases\\userdb.db"
};


var conn = new SqliteConnection(builder.ConnectionString);


conn.Open();

var cmd = conn.CreateCommand();



string sql;

sql = "DROP TABLE users;";

cmd.CommandText = sql;
cmd.ExecuteNonQuery();

cmd.Ex

sql = @"CREATE TABLE 'users' (
	'id'	INTEGER UNIQUE,
	'name'	TEXT,
	'email'	TEXT,
	'active'	INTEGER,
    PRIMARY KEY('id' AUTOINCREMENT)
);";
cmd.CommandText = sql;
cmd.ExecuteNonQuery();

sql = @"
INSERT INTO users
(name, email, active)
VALUES 
('Zoe', 'zoe@gmail.com', 1), 
('Yvonne', 'yvonne@gmail.com', 1), 
('Xavier', 'xavier@gmail.com', 0), 
('Wendy', 'wendy@gmail.com', 0), 
('Vikki', 'vikki@gmail.com', 1);
";
cmd.CommandText = sql;
cmd.ExecuteNonQuery();

conn.Close();


