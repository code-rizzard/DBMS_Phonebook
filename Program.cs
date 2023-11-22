namespace DBMS_Phonebook;
using static FileSave;
using static Utils;
using MySql.Data.MySqlClient;


class Program
{
    public const string connectionString = "server=localhost;port=3306;uid=root;pwd=password123;database=phonebook;";
    static MySqlConnection connection = new(connectionString);

    static void Main(string[] args)
    {
        HomePage();
        Console.Write("\n\n\n Good Bye!");
    }


    static void Pause()
    {
        Console.WriteLine("\n\nPress any key to continue...");
        Console.ReadKey();
    }

    static void HomePage()
    {
        bool loop = true;
        do
        {
            int a = InteractiveInput(
            "PhoneBook \n \nWhat would you like to do?",
            new string[] { "View All Contacts", "Find contact", "Add Contact", "Archive a Contact", "Archive All", "Show History", "Quit" });
            switch (a)
            {
                case 0:
                    ViewAllContactsPage();
                    break;
                case 1:
                    FindContactPage();
                    break;
                case 2:
                    AddContactPage();
                    break;
                case 3:
                    ArchiveContactPage();
                    break;
                case 4:
                    ArchiveAllPage();
                    break;
                case 5:
                    ShowHistoryPage();
                    break;
                default:
                    loop = false;
                    break;
            }
        } while (loop);
    }
    static void ArchiveAllPage()
    {
        ClearScreen();
        var allContacts = RetrieveData(connection);
        Console.WriteLine("Archive all?" + "\n");
        if (allContacts.Count > 0)
        {
            int res = InteractiveInput("Are you sure you want to archive everything?", new string[] { "No", "Yes" });
            Console.WriteLine("\n");
            if (res == 1)
            {
                ArchiveAll(connection);
                ACout("Archive succesfull!");
            }
            else
            {
                ACout("Archive aborted.");
            }
        }
        else
        {
            ACout("No contacts to archive!");
        }
        Pause();
    }

    public static void ShowHistoryPage()
    {
        ClearScreen();
        Console.Write("Show History\n\n");
        var allContacts = GetHistory(connection);
        if (allContacts.Count == 0)
        {
            ACout("Empty.... :( \n");
        }
        else
        {
            int index = 0;
            foreach (Contact h in allContacts)
            {
                Console.Write(index + 1 + ".) ");
                Console.Write(h);
                index++;
                Thread.Sleep(70);
            }
        }
        Pause();
    }

    public static void ViewAllContactsPage()
    {
        var allContacts = RetrieveData(connection);
        ClearScreen();
        Console.Write("All contacts list \n\n");
        if (allContacts.Count == 0)
        {
            ACout("No contacts to show :( \n");
        }
        allContacts.Sort((x, y) => x.name.CompareTo(y.name));
        int index = 0;
        foreach (Contact h in allContacts)
        {
            Console.Write(index + 1 + ".) ");
            Console.Write(h);
            index++;
            Thread.Sleep(70);
        }

        Pause();
    }

    public static void AddContactPage()
    {
        ClearScreen();
        Console.Write("Create a new contact\n\n");
        Contact newContact = new()
        {
            name = GetLine("Enter name: "),
            number = GetLine("Enter Number: ", IsNotValidPhoneNumber),
            address = GetLine("Enter Address: ")
        };
        Console.WriteLine("\nAdding to database....");

        newContact.TrimMembers();

        using (connection)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO contact (name, number, address) VALUES (@name, @number, @address)";
                command.Parameters.AddWithValue("@name", newContact.name);
                command.Parameters.AddWithValue("@number", newContact.number);
                command.Parameters.AddWithValue("@address", newContact.address);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                Console.WriteLine($"Error inserting contact. Number: ({newContact.number}) already in used. ");
                HomePage();
            }
        }
        Console.WriteLine("Added sucessfully!");

        Pause();
    }

    public static void FindContactPage()
    {
        ClearScreen();
        Console.Write("Find a contact\n\n");
        string name = "";
        List<Contact> contactsFound = new();
        var allContacts = RetrieveData(connection);

        name = GetLine("Enter name: ");
        Console.WriteLine();
        foreach (Contact c in allContacts)
        {
            if (c.name == name)
            {
                contactsFound.Add(c);
            }
        }

        if (contactsFound.Count > 0)
        {
            Console.Write("Here are the contacts with the name: " + name + "\n\n");
            foreach (Contact c in contactsFound)
            {
                Console.WriteLine(c);
            }
        }
        else
        {
            ACout("No contact found with the name. :(");
        }
        Thread.Sleep(300);
        Pause();
    }

    public static void ArchiveContactPage()
    {
        ClearScreen();
        Console.Write("Archive a contact\n\n");
        string name = GetLine("Enter name: ");
        List<Contact> contactsFound = new();
        try
        {
            contactsFound = GetContact(connection, name);
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            Console.ReadKey();
        }

        if (contactsFound.Count <= 0)
        {
            ACout("No contact found with the name. :(");
        }
        else
        {
            switch (contactsFound.Count)
            {
                case 1:
                    {
                        Contact n = contactsFound[0];
                        Console.WriteLine(n);
                        int res = InteractiveInput(
                            "Are you sure you want to archive? \n\n" + n,
                            new string[] { "Back", "Archive" });
                        if (res == 1)
                        {
                            ArchiveContact(connection, n.id);
                            ACout("Contact archived!");
                        }

                        break;
                    }
                default:
                    {
                        List<string> choices = new() { "Back" };
                        foreach (Contact c in contactsFound)
                        {
                            choices.Add(c.ToString());
                        }

                        int res = InteractiveInput("Select contact to archived: ", choices.ToArray());
                        switch (res)
                        {
                            case 0:
                                break;
                            default:
                                Contact n = contactsFound[res - 1];
                                Console.Write(n);
                                int res1 = InteractiveInput(
                                    "Are you sure you want to archive? \n\n" + n, new string[]
                                    {"Back", "Archive"});
                                if (res1 == 1)
                                {
                                    ArchiveContact(connection, n.id);
                                    ACout("Contact archived!");
                                    Console.WriteLine();
                                }
                                break;
                        }

                        break;
                    }
            }
        }

        Console.ReadKey();
    }


}