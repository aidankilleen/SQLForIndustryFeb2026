using System;
using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;

internal static class Program
{
    private const string CsvPath = @"c:\temp\eircode_test.csv";
    private const string DbPath = @"c:\work\training\databases\eircode_import.db";

    private static void Main()
    {
        EnsureFreshDatabase(DbPath);

        var cs = new SqliteConnectionStringBuilder
        {
            DataSource = DbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        using var conn = new SqliteConnection(cs);
        conn.Open();

        CreateSchema(conn);
        ApplyBulkLoadPragmas(conn);

        ImportCsvStreaming(conn, CsvPath);

        Console.WriteLine("Done.");
    }

    private static void EnsureFreshDatabase(string dbPath)
    {
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(dbPath))
            File.Delete(dbPath);
    }

    private static void CreateSchema(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
        """
        CREATE TABLE IF NOT EXISTS Eircodes (
            Id     INTEGER PRIMARY KEY AUTOINCREMENT,
            County TEXT    NOT NULL,
            Eircode TEXT   NOT NULL
        );

        CREATE INDEX IF NOT EXISTS IX_Eircodes_Eircode ON Eircodes(Eircode);
        CREATE INDEX IF NOT EXISTS IX_Eircodes_County  ON Eircodes(County);
        """;
        cmd.ExecuteNonQuery();
    }

    private static void ApplyBulkLoadPragmas(SqliteConnection conn)
    {
        // Good for "I rebuild the DB from CSV" scenarios.
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
        """
        PRAGMA journal_mode = WAL;
        PRAGMA synchronous = NORMAL;
        PRAGMA temp_store = MEMORY;
        """;
        cmd.ExecuteNonQuery();
    }

    private static void ImportCsvStreaming(SqliteConnection conn, string csvPath)
    {
        const int batchSize = 50_000;

        using var insert = conn.CreateCommand();
        insert.CommandText = "INSERT INTO Eircodes (County, Eircode) VALUES ($county, $eircode);";

        var pCounty = insert.CreateParameter();
        pCounty.ParameterName = "$county";
        insert.Parameters.Add(pCounty);

        var pEircode = insert.CreateParameter();
        pEircode.ParameterName = "$eircode";
        insert.Parameters.Add(pEircode);

        // Big buffer helps with large sequential reads.
        using var reader = new StreamReader(
            csvPath,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024 * 1024);

        // Header row
        var header = reader.ReadLine();
        if (header == null)
            throw new InvalidOperationException("CSV file is empty.");

        SqliteTransaction? tx = conn.BeginTransaction();
        insert.Transaction = tx;

        int inserted = 0;
        int inBatch = 0;

        try
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Fast 2-column parse (assumes no quoted commas)
                int commaIndex = line.IndexOf(',');
                if (commaIndex <= 0 || commaIndex >= line.Length - 1)
                    continue; // skip malformed line

                var county = line[..commaIndex].Trim();
                var eircode = line[(commaIndex + 1)..].Trim();

                if (county.Length == 0 || eircode.Length == 0)
                    continue;

                pCounty.Value = county;
                pEircode.Value = eircode;

                insert.ExecuteNonQuery();

                inserted++;
                inBatch++;

                if (inBatch >= batchSize)
                {
                    tx!.Commit();
                    tx.Dispose();

                    tx = conn.BeginTransaction();
                    insert.Transaction = tx;

                    inBatch = 0;

                    Console.WriteLine($"Inserted so far: {inserted:n0}");
                }
            }

            // Commit remaining rows
            tx!.Commit();
        }
        catch
        {
            // If something goes wrong, rollback the current batch.
            try { tx?.Rollback(); } catch { /* ignore rollback errors */ }
            throw;
        }
        finally
        {
            tx?.Dispose();
        }

        Console.WriteLine($"Inserted rows: {inserted:n0}");
    }
}
