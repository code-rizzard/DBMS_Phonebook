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
        int a = InteractiveInput(
            "PhoneBook \n \nWhat would you like to do?",
            new string[] { "View All Contacts", "Find contact", "Add Contact", "Delete Contact", "Delete All", "Quit" });
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
                DeleteContactPage();
                break;
            case 4:
                DeleteAllPage();
                break;

        }
    }
    static void DeleteAllPage()
    {
        ClearScreen();
        var allContacts = RetrieveData(connection);
        Console.WriteLine("Delete all?" + "\n");
        if (allContacts.Count > 0)
        {
            int res = InteractiveInput("Are you sure you want to delete everything?", new string[] { "No", "Yes" });
            Console.WriteLine("\n");
            if (res == 1)
            {
                allContacts.Clear();
                ACout("Deletion succesfull!");
            }
            else
            {
                ACout("Deletion aborted.");
            }
        }
        else
        {
            ACout("No contacts to delete!");
        }
        Pause();
        HomePage();
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
        HomePage();
    }

    public static void AddContactPage()
    {
        ClearScreen();
        Console.Write("Create a new contact\n\n");
        Contact newContact;
        newContact.name = GetLine("Enter name: ");
        newContact.number = GetLine("Enter Number: ", IsNotValidPhoneNumber);
        newContact.address = GetLine("Enter Address: ");
        Console.WriteLine();

        var allContacts = RetrieveData(connection);

        bool isUsed = false;
        foreach (Contact c in allContacts)
        {
            if (c.number == newContact.number)
            {
                ACout("Contact number is already in used.");
                isUsed = true;
                break;
            }
        }
        if (!isUsed)
        {
            newContact.TrimMembers();
            allContacts.Add(newContact);
            Console.WriteLine("Added sucessfully!");
        }

        Pause();
        HomePage();
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
        HomePage();
    }

    public static void DeleteContactPage()
    {
        ClearScreen();
        Console.Write("Delete a contact\n\n");
        string name = "";
        List<Contact> contactsFound = new();
        List<int> indexes = new();
        int index = 0;
        name = GetLine("Enter name: ");
        var allContacts = RetrieveData(connection);

        foreach (Contact c in allContacts)
        {
            if (c.name == name)
            {
                contactsFound.Add(c);
                indexes.Add(index);
            }
            index++;
        }

        if (contactsFound.Count > 0)
        {
            if (contactsFound.Count == 1)
            {
                Contact n = contactsFound[0];
                Console.WriteLine(n);
                int res = InteractiveInput(
                    "Are you sure you want to delete \n\n" + n,
                    new string[] { "Back", "Delete" });
                if (res == 1)
                {
                    allContacts.RemoveAt(indexes.First());
                    ACout("Contact deleted!");
                }
            }
            else
            {
                List<string> choices = new() { "Back" };
                foreach (Contact c in contactsFound)
                {
                    choices.Add(c.ToString());
                }

                int res = InteractiveInput("Select contact to delete: ", choices.ToArray());
                switch (res)
                {
                    case 0:
                        break;
                    default:
                        Contact n = contactsFound[res - 1];
                        Console.Write(n);
                        int res1 = InteractiveInput(
                            "Are you sure you want to delete \n\n" + n, new string[]
                            {"Back", "Delete"});
                        if (res1 == 1)
                        {
                            allContacts.RemoveAt(res);
                            Console.WriteLine();
                            ACout("Contact deleted!");
                        }
                        break;
                }
            }
        }
        else
        {
            ACout("No contact found with the name. :(");
        }

        Console.ReadKey();
        HomePage();
    }


}