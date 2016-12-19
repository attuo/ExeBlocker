using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeBlocker
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ExeBlocker!\n");
            if (CheckIfThereIsDWORD() == false)
            {
                CreateDWORD();
                Console.WriteLine("Creating DWORD");
            }
            if (CheckIfThereIsKey() == false) CreateKey();
            if (IsBlockOn()) Console.WriteLine("Block is currently on.\n");
            else Console.WriteLine("Block is currently off.\n");


            MainMenu();
            
        }

        public static void MainMenu()
        {
            Console.WriteLine("1) Add exe names you want to be blocked");
            Console.WriteLine("2) List all exes from blocklist");
            Console.WriteLine("3) Delete exe from blocklist");
            Console.WriteLine("4) Delete all exes from blocklist");
            Console.WriteLine("5) Start block");
            Console.WriteLine("6) Stop block");
            Console.WriteLine("7) Exit program");

            string userInput = Console.ReadLine();
            int input = InputChecker(userInput);

            switch (input)
            {
                case 1:
                    AddToBlocklist();
                    break;
                case 2:
                    Console.WriteLine("\nListing all open exes in blocklist:");
                    ListAllFromBlocklist();
                    Console.WriteLine("\n");
                    break;
                case 3:
                    Console.WriteLine("Input exe name you want to remove from blocklist");
                    string deleteInput = Console.ReadLine();
                    if (!CheckIfThereIsKeyString(deleteInput))
                    {
                        Console.WriteLine("There is no exe with that name in blocklist\n");
                        Console.WriteLine("Back to main menu..\n\n");
                        break;
                    }
                    DeleteFromBlocklist(deleteInput);
                    Console.WriteLine("Exe deleted from blocklist\n");
                    break;
                case 4:
                    DeleteAllFromBlocklist();
                    break;
                case 5:
                    StartBlock();
                    Console.WriteLine("Block is now on");
                    break;
                case 6:
                    StopBlock();
                    Console.WriteLine("Block is now off");
                    break;
                case 7:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    Main(null);
                    break;
            }
            MainMenu();
        }

        private static void DeleteAllFromBlocklist()
        {
            ConsoleKeyInfo c;
            Console.WriteLine("Are you sure to delete all exes from blocklist? Y/N");
            c = Console.ReadKey();
            switch (c.KeyChar)
            {
                case 'y':
                    RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun", true);
                    String[] array = baseRegistryKey.GetValueNames();
                    foreach (String text in array)
                    {
                        baseRegistryKey.DeleteValue(text);
                    }
                    Console.WriteLine("\nExes deleted");
                    break;
                case 'n':
                    Console.WriteLine("Returning to main menu");
                    return;
                default:
                    Console.WriteLine("\nPress Y to delete all from blocklist or N to get back to main menu");
                    break;
            }
        }

        private static void StopBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun", true);
            String[] array = baseRegistryKey.GetValueNames();
            foreach(String text in array)
            {
                baseRegistryKey.SetValue(text, text + "*");
            }
        }

        public static bool IsBlockOn()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun");
            String[] array = baseRegistryKey.GetValueNames();
            foreach (String text in array)
            {
                if(text.EndsWith("*"))
                {
                    return false;
                }
            }
            return true;
        }

        private static void StartBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun", true);
            String[] array = baseRegistryKey.GetValueNames();
            foreach (String text in array)
            {
                if(baseRegistryKey.GetValue(text).ToString().EndsWith("*"))
                {
                    baseRegistryKey.SetValue(text, text);
                }
            }
        }

        private static void DeleteFromBlocklist(String name)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun", true);
            if (name != null)
            {
                baseRegistryKey.DeleteValue(name);
            }
        }

        public static void AddToBlocklist()
        {
            Console.WriteLine("Add to blocklist, write exe name and then press enter. When you want to stop write 1 and if you want to abort write 0");

            List<string> exes = new List<string>();
            bool run = true;
            while (run)
            {
                String userInput;
                userInput = Console.ReadLine();
                
                switch (userInput)
                {
                    case "0":
                        run = false;
                        return;
                    case "1":
                        if (exes.Count == 0)
                        {
                            Console.WriteLine("You have not added any exe.");
                            break;
                        }
                        CreateKeyStrings(exes);
                        Console.WriteLine("Exes added to blocklist. Returning to main menu.");
                        run = false;
                        break;
                    default:
                        if (ExeNameChecker(userInput))
                            exes.Add(userInput);
                        break;
                }
            }
            MainMenu();
        }

        public static void CreateKeyStrings(List<string> exelist)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun", true);
            foreach (string exeName in exelist)
            {
                if (ExeNameChecker(exeName) == true && CheckIfThereIsKeyString(exeName) == false)
                {
                    baseRegistryKey.SetValue(exeName, exeName, RegistryValueKind.String);
                }
            }
        }

        public static void ListAllFromBlocklist()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun");
            String[] list = baseRegistryKey.GetValueNames();
            foreach (String a in list)
            {
                Console.WriteLine(a);
            }
        }

        //public static void AppendToBlocklist(List<string> list)
        //{
        //    string directorypath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "ExeBlocker");
        //    string filename = "blocklist.txt";
        //    string fullpath = Path.Combine(directorypath, filename);

        //    File.AppendAllLines(fullpath, list);
        //}

        //public static void ReadFromTextfile()
        //{
        //    string directorypath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "ExeBlocker");
        //    string filename = "blocklist.txt";

        //    string fullpath = Path.Combine(directorypath, filename);
        //    List<string> blocklist = File.ReadLines(fullpath).ToList();
        //}

        public static bool CheckIfThereIsDWORD()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
            if (baseRegistryKey.GetValue("DisallowRun") != null) return true;
            return false;
        }

        public static void CreateDWORD()
        {
            //var rs = new RegistryKey();
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
            baseRegistryKey.SetValue("DisallowRun", "1", RegistryValueKind.DWord);
            baseRegistryKey.Close();
        }

        public static bool CheckIfThereIsKey()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                return false;
            }
            return true;
        }

        public static bool CheckIfThereIsKeyString(string keyName)
        {
            string[] keyStringNames;
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");

            keyStringNames = key.GetValueNames();
            if (keyStringNames != null) { 
            foreach (string name in keyStringNames)
            {
                    if (keyName.Equals(name)) return true;
            }
        }
            return false;
        }

        //public static bool CheckIfThereIsKeyAndString(string keyName)
        //{
        //    string text;
        //    RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
        //    RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
        //    if (key == null)
        //    {
        //        CreateKey("DisallowRun");
        //    }
        //        try
        //        {
        //            text = (string)key.GetValue(keyName);
        //            if (text != null && text == keyName)
        //            {
        //                return true;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Error" + e);
        //            return false;
        //        }
        //    return false;
        //}



        public static void CreateKey()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                key.CreateSubKey("DisallowRun", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
        }

        public static void DeleteKeyString(string keyName)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun");
            baseRegistryKey.DeleteValue(keyName);
        }

        public static int InputChecker(string input)
        {
            int number;

            try
            {
                number = Int32.Parse(input);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

            return number;
        }

        public static bool ExeNameChecker(string name)
        {
            int nameLength;
            string exe;
            if (name == null || name.Length < 4)
            {
                Console.WriteLine("Error! Empty or too short exe name inserted. Try again.");
                return false;

            }


            name.Trim();
            nameLength = name.Length;
            exe = name.Substring(name.LastIndexOf('.'));
            if (exe != ".exe")
            {
                Console.WriteLine("Error! Remember to put .exe in the end. Try again.");
                return false;
            }
            return true;

        }
    }
}
