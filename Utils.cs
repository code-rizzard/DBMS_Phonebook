namespace DBMS_Phonebook;
using System;
static class Utils
{

    public static void ClearScreen()
    {
        Console.Clear();

    }
    public static void ACout(string h, int delay = 50)
    {
        int index = 0;
        foreach (char c in h)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                Console.Write(h.Substring(index));
                break;
            }
            Console.Write(c);
            index++;
            Thread.Sleep(delay);
        }
    }
    public static int InteractiveInput(string label, string[] choices, string endLabel = "Press ENTER/SPACE to select. Arrow keys to move. \n")
    {
        int _index = 0;
        bool looping = true;
        int choiceIndex = 0;
        int maxNum = choices.Length - 1;

        ClearScreen();
        ACout(label + "\n\n");
        Thread.Sleep(75);
        foreach (string h in choices)
        {
            if (_index == choiceIndex)
            {
                Console.Write("=>");
            }
            else
            {
                Console.Write("  ");
            }
            Console.Write(h);

            Console.Write("\n");
            choiceIndex++;
            Thread.Sleep(75);
        }
        ACout("\n" + endLabel);
        while (looping)
        {
            choiceIndex = 0;
            ClearScreen();
            Console.WriteLine(label + "\n");

            foreach (string h in choices)
            {
                if (_index == choiceIndex)
                {
                    Console.Write("=>");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.Write(h);
                Console.Write("\n");
                choiceIndex++;
            }
            Console.Write("\n" + endLabel);

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    _index--;
                    if (_index < 0)
                    {
                        _index = maxNum;
                    }
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    _index++;
                    if (_index > maxNum)
                    {
                        _index = 0;
                    }
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    looping = false;
                    break;
            }
        }
        return _index;
    }

    public static string GetLine(string label, Func<string, bool>? invalidator = null)
    {
        invalidator ??= IsEmpty;
        string? input;
        do
        {
            ACout(label);
            input = Console.ReadLine();
        } while (invalidator(input!));
        return input!;
    }

    public static bool IsEmpty(string h)
    {
        return h == string.Empty || h.Trim() == string.Empty;
    }

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Check if the string contains only valid characters
        foreach (char ch in phoneNumber)
        {
            if (!int.TryParse(ch.ToString(), out int a) && ch != '-' && ch != '(' && ch != ')' && ch != ' ' && ch != '+')
                return false;
        }

        // Count the number of digits in the string
        int digitCount = 0;
        foreach (char ch in phoneNumber)
        {
            if (int.TryParse(ch.ToString(), out int a))
                digitCount++;
        }

        // Check if the phone number has at least 10 digits
        if (digitCount < 5)
            return false;

        // The phone number is valid
        return true;
    }
    public static bool IsNotValidPhoneNumber(string phoneNumber)
    {
        return !IsValidPhoneNumber(phoneNumber);
    }





}

public struct Contact
{
    public string number;
    public string name;
    public string address;

    public Contact(string number, string name, string address)
    {
        this.number = number;
        this.name = name;
        this.address = address;
    }

    public override string ToString()
    {
        string n = "\n";
        return "Name: " + name + n + "Number: " + number + n + "Address: " + address + n;
    }

    public void TrimMembers()
    {
        number = number.Trim();
        name = name.Trim();
        address = address.Trim();
    }
};