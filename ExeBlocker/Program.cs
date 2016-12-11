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
            Console.WriteLine("Welcome to ExeBlocker.");
            if (CheckIfThereIsDWORD() == false)
            {
                CreateDWORD();
                Console.WriteLine("Creating DWORD");
            }
            if (CheckIfThereIsKey() == false) CreateKey();
            if (IsBlockOn()) Console.WriteLine("Block is currenly on");
            else Console.WriteLine("Block is currently off");


            Console.WriteLine("Input 1 to add exe names you want to be blocked");
            Console.WriteLine("Input 2 to delete exe from blocklist");
            Console.WriteLine("Input 3 to delete all exes from blocklist");
            Console.WriteLine("Input 4 to list all exes from blocklist");
            string userInput = Console.ReadLine();
            int input = InputChecker(userInput);

            switch (input)
            {
                case 1:
                    AddToBlocklist();
                    break;
                case 2:
                    Console.WriteLine("Input exe name you want to remove from blocklist");
                    string deleteInput = Console.ReadLine();
                    if (ExeNameChecker(deleteInput) == true)
                    {
                        DeleteFromBlocklist(deleteInput);
                    }

                    break;
                case 3:
                    DeleteAllFromBlocklist();
                    break;
                case 4:
                    ListAllFromBlocklist();
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    Main(null);
                    break;
            }
        }

        private static void DeleteAllFromBlocklist()
        {
            object c;
            Console.WriteLine("Are you sure to delete all exes from blocklist? Y/N");
            c = Console.ReadKey();
            string s = c.ToString();
            switch (s)
            {
                case "y":
                    RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
                    String[] array = baseRegistryKey.GetValueNames();
                    foreach (String text in array)
                    {
                        baseRegistryKey.DeleteValue(text);
                    }
                    break;
                case "n":
                    Console.WriteLine("Returning to main menu");
                    return;
                default:
                    Console.WriteLine("Press Y to delete all from blocklist or N to get back to main menu");
                    break;
            }
        }

        private static void StopBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
            String[] array = baseRegistryKey.GetValueNames();
            foreach(String text in array)
            {
                baseRegistryKey.SetValue(text, text + "*");
            }
        }

        public static bool IsBlockOn()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
            String[] array = baseRegistryKey.GetValueNames();
            foreach (String text in array)
            {
                if(text.EndsWith("*"))
                {
                    return true;
                }
            }
            return false;
        }

        private static void StartBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
            String[] array = baseRegistryKey.GetValueNames();
            foreach (String text in array)
            {
                if(text.EndsWith("*"))
                {
                    baseRegistryKey.SetValue(text, text.Substring(0, text.Length - 1));
                }
            }
        }

        private static void DeleteFromBlocklist(String name)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
            if (name != null && name.Equals(baseRegistryKey.GetValue(name)))
            {
                baseRegistryKey.DeleteValue(name);
            }
        }

        public static void AddToBlocklist()
        {
            Console.WriteLine("Add to blocklist, write exe name and then press enter. When you want to stop write 1 and if you want to abort write 0");

            bool run = true;
            while (run);
            String userInput;
            userInput = Console.ReadLine();
            List<string> exes = new List<string>();
            switch (userInput)
            {
                case "0":
                    run = false;
                    return;
                case "1":
                    break;
                default:
                    if (ExeNameChecker(userInput))
                        exes.Add(userInput);
                    break;
            }
        }

        public static void ListAllFromBlocklist()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
            String[] list = baseRegistryKey.GetSubKeyNames();
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
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
            if (baseRegistryKey.GetValue("DisallowRun") != null) return true;
            return false;
        }

        public static void CreateDWORD()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
            baseRegistryKey.SetValue("DisallowRun", "1", RegistryValueKind.DWord);
        }

        public static bool CheckIfThereIsKey()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                return false;
            }
            return true;
        }

        public static bool CheckIfThereIsKeyString(string keyName)
        {
            string text;
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            try
            {
                text = (string)key.GetValue(keyName);
                if (text != null && text == keyName) return true;
            }
            catch (Exception e) {
                Console.WriteLine("Error" + e);
                return false;
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
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies");
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                key.CreateSubKey("DisallowRun");
            }
        }

        public static void DeleteKeyString(string keyName)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DisallowRun");
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
