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
            // CheckDword()
            // 
            Console.WriteLine("Welcome to ExeBlocker.");
            Console.WriteLine("Input 1 to add exe names you want to be blocked");
            Console.WriteLine("Input 2 to delete exe from a list");
            Console.WriteLine("Input 3 to delete all exes from blocklist");
            string userInput = Console.ReadLine();
            int input = InputChecker(userInput);

            switch (input)
            {
                case 1:
                    AddToBlocklist();
                    break;
                case 2:
                    DeleteFromBlocklist();
                    break;
                case 3:
                    DeleteAllFromBlocklist();
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    Main(null);
                    break;

            }

        }

        private static void DeleteAllFromBlocklist()
        {
            throw new NotImplementedException();
        }

        private static void DeleteFromBlocklist()
        {
            throw new NotImplementedException();
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

        public static void AppendToBlocklist(List<string> list)
        {
            string directorypath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "ExeBlocker");
            string filename = "blocklist.txt";
            string fullpath = Path.Combine(directorypath, filename);

            File.AppendAllLines(fullpath, list);
        }

        public static void ReadFromTextfile()
        {
            string directorypath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "ExeBlocker");
            string filename = "blocklist.txt";

            string fullpath = Path.Combine(directorypath, filename);
            List<string> blocklist = File.ReadLines(fullpath).ToList();
        }

        public static void CreateDWORD()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\CurrentVersion\\Policies");
            baseRegistryKey.SetValue("DisallowRun", "1", RegistryValueKind.QWord);
        }

        public static bool CheckIfThereIsKeyAndString(string keyName)
        {
            string text;
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\CurrentVersion\\Policies");
            RegistryKey key = baseRegistryKey.OpenSubKey(keyName);
            if (key != null)
            {
                try
                {
                    text = (string)key.GetValue("Debugger");
                    if (text != null && text == "ntsd - c q")
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error" + e);
                    return false;
                }
            }
            return false;
        }



        public static void CreateKey(string keyName, object value)
        {

        }

        public static void DeleteKey(string keyName)
        {

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
