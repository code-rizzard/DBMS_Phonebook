namespace DBMS_Phonebook;
using static FileSave;
using static Utils;
using MySql.Data.MySqlClient;
using Spectre.Console;

class Program
{
    public const string connectionString = "server=localhost;port=3306;uid=root;database=phonebook;";
    static readonly MySqlConnection connection = new(connectionString);

    static void Main(string[] args)
    {
        Exception? error = null;
        AnsiConsole.Status().Start("Connecting to database...", (ctx) =>
        {
            try
            {
                using (connection)
                {
                    connection.Open();
                }
            }
            catch (Exception err)
            {
                error = err;
            }
        });
        if (error != null)
        {
            AnsiConsole.MarkupLine("[red]Error connecting to database. Please check your connection settings.[/]");
            AnsiConsole.MarkupLine("[red]" + error.Message + "[/]");
            AnsiConsole.MarkupLine("\n\nContinuing without database connection may create unexpected events.");
            Console.ReadKey();
        }

        HomePage();
        AnsiConsole.Markup("\n\n\n Good Bye!");
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
        AnsiConsole.MarkupLine("[red]Archive all?[/]" + "\n");
        if (allContacts.Count > 0)
        {
            int res = InteractiveInput("Are you sure you want to archive everything?", new string[] { "No", "Yes" });
            AnsiConsole.MarkupLine("\n");
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
        AnsiConsole.Markup("Archived History\n\n");
        var allContacts = GetHistory(connection);
        if (allContacts.Count == 0)
        {
            ACout("Empty.... :( \n");
        }
        else
        {
            var table = new Table().AddColumns("Date Archived", "Name", "Number", "Address");
            foreach (Contact h in allContacts)
            {
                table.AddRow(h.dateArchived!, h.name, h.number, h.address);
            }
            AnsiConsole.Write(table);
        }
        Pause();
    }

    public static void ViewAllContactsPage()
    {
        var allContacts = RetrieveData(connection);
        ClearScreen();
        AnsiConsole.Markup("All contacts list \n\n");
        if (allContacts.Count == 0)
        {
            ACout("No contacts to show :( \n");
        }
        allContacts.Sort((x, y) => x.name.CompareTo(y.name));
        int index = 0;
        var table = CreateContactTable();
        foreach (Contact h in allContacts)
        {
            AnsiConsole.Clear();
            table.AddRow((index + 1).ToString(), h.name, h.number, h.address);
            index++;
            AnsiConsole.Write(table);
            Thread.Sleep(70);
        }
        Pause();
    }

    public static void AddContactPage()
    {
        ClearScreen();
        AnsiConsole.Markup("Create a new contact\n\n");
        Contact newContact = new()
        {
            name = AnsiConsole.Ask<string>("[green]Enter name: [/]"),
            number = AnsiConsole.Prompt(new TextPrompt<string>("[green]Enter Number: [/]").ValidationErrorMessage("[red]Invalid phone number.[/]").Validate(IsValidPhoneNumber)),
            address = AnsiConsole.Ask<string>("[green]Enter Address: [/]")
        };
        AnsiConsole.MarkupLine("\nAdding to database....");

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
                AnsiConsole.MarkupLine($"Error inserting contact. Number: ({newContact.number}) already in used. ");
                HomePage();
            }
        }
        AnsiConsole.MarkupLine("[green]Added sucessfully![/]");

        Pause();
    }

    public static void FindContactPage()
    {
        ClearScreen();
        AnsiConsole.Markup("Find a contact\n\n");
        string name = "";
        List<Contact> contactsFound = new();
        var allContacts = RetrieveData(connection);
        name = AnsiConsole.Ask<string>("[green]Enter name: [/]");
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
            var table = CreateContactTable("Name", "Number", "Address");
            AnsiConsole.Markup("Here are the contacts with the name: " + name + "\n\n");
            foreach (Contact c in contactsFound)
            {
                table.AddRow(c.name, c.number, c.address);
            }
            AnsiConsole.Write(table);
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
        AnsiConsole.Markup("Archive a contact\n\n");
        string name = AnsiConsole.Ask<string>("Enter name: ");
        List<Contact> contactsFound = new();
        try
        {
            contactsFound = GetContact(connection, name);
        }
        catch (Exception err)
        {
            AnsiConsole.MarkupLine(err.Message);
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
                        AnsiConsole.MarkupLine(n.ToString());
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
                                AnsiConsole.Markup(n.ToString());
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