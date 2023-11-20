namespace DBMS_Phonebook;
using System;
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

    public static List<Contact> RetrieveData()
    {
        List<Contact> allContacts = new();
        if (!File.Exists("contacts_save.txt"))
        {
            return allContacts;
        }
        string t = File.ReadAllText("contacts_save.txt");
        int index = 0;
        foreach (string line in t.Split("\n"))
        {
            Contact contact = new();
            // cout << t << " " << index << endl;
            switch (index)
            {
                case 0:
                    contact.name = t;
                    break;
                case 1:
                    contact.number = t;
                    break;
                case 2:
                    contact.address = t;
                    break;
            }
            if (index + 1 == 3)
            {

                allContacts.Add(contact);
            }
            if (!IsEmpty(t))
            {
                index = (index + 1) % 3;
            }
        }
        return allContacts;
    }
}
