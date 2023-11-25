namespace DBMS_Phonebook;
using System;
using MySql.Data.MySqlClient;
using static Utils;


public static class FileSave
{

    public static void SaveToFile(List<Contact> allContacts)
    {
        using (StreamWriter writer = new StreamWriter("contacts_save.txt"))
        {
            foreach (Contact c in allContacts)
            {
                writer.WriteLine(c.name);
                writer.WriteLine(c.number);
                writer.WriteLine(c.address);
                writer.WriteLine("\n");
            }
        }
    }

    public static void ArchiveContact(MySqlConnection connection, int id)
    {
        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE contact SET dateArchived = NOW() WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }

    public static void ArchiveAll(MySqlConnection connection)
    {
        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE contact SET dateArchived = NOW() WHERE dateArchived IS NULL";
                command.ExecuteNonQuery();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
    public static List<Contact> GetContact(MySqlConnection connection, string name)
    {
        List<Contact> contactsFound = new();
        using (connection)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM contact WHERE name = @name AND dateArchived IS NULL";
            command.Parameters.AddWithValue("@name", name);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Contact c = new()
                {
                    id = (int)reader["id"],
                    name = reader["name"].ToString()!,
                    number = reader["number"].ToString()!,
                    address = reader["address"].ToString()!
                };
                contactsFound.Add(c);
            }
        }
        return contactsFound;

    }

    public static List<Contact> GetHistory(MySqlConnection connection)
    {
        List<Contact> contactsFound = new();
        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM contact WHERE dateArchived IS NOT NULL ORDER BY dateArchived DESC";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contact c = new()
                    {
                        id = (int)reader["id"],
                        name = reader["name"].ToString()!,
                        number = reader["number"].ToString()!,
                        address = reader["address"].ToString()!,
                        dateArchived = reader["dateArchived"].ToString()!
                    };
                    contactsFound.Add(c);
                }
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err);
            }
        }
        return contactsFound;
    }

    public static List<Contact> RetrieveData(MySqlConnection connection)
    {
        List<Contact> allContacts = new();
        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM contact WHERE dateArchived IS NULL";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Contact c = new()
                    {
                        id = (int)reader["id"],
                        name = reader["name"].ToString()!,
                        number = reader["number"].ToString()!,
                        address = reader["address"].ToString()!
                    };
                    allContacts.Add(c);
                }



            }
            catch (Exception er)
            {
                Console.WriteLine("Error connecting to database." + er.Message);
            }
        }
        return allContacts;
    }
}
