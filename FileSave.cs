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

    public static List<Contact> RetrieveData(MySqlConnection connection)
    {
        List<Contact> allContacts = new();
        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM contact";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Contact c = new()
                    {
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
