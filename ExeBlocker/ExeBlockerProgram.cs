using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ExeBlocker
{

    /// <summary>
    /// Creates a key in regedit where exe names will be added. By doing so, program can be used to block 
    /// wanted exes from opening. Easy and light way of blocking exes. Program doesn't have to be running
    /// when block is on. When user tries to open blocked exe Windows will display error popup: "This operation
    /// has been cancelled due to restrictions in effect on this computer. Please contact your system administrator".
    /// Currently the whole program is in same class.
    /// 
    /// TODO: Make GUI for the program 
    /// </summary>
    public class ExeBlockerProgram
    {

        const string baseRegistryKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
        const string keyStringPath = "\\DisallowRun";

        /// <summary>
        /// When program stars, creates DWORD in registry and tells block status and opens main menu.
        /// TODO: Make string[] args to work, so you can add and start.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (CheckIfThereIsDWORD() == false)
            {
                CreateDWORD();
                Console.WriteLine("Creating DWORD");
            }
            if (CheckIfThereIsKey() == false) CreateKey();
            if (args.Length == 1 && "start".Equals(args[0]))
              {
                StartBlock();
                Console.WriteLine("Block is now on, closing program..");
                Environment.Exit(0);
              }
            if (args.Length == 2 && "add".Equals(args[0]))
            {
                if (ExeNameChecker(args[1]))
                {
                    List<string> exe = new List<string>();
                    exe.Add(args[1]);
                    CreateKeyStrings(exe);
                    Console.WriteLine("Exe added to blocklist");

                }
                else Console.WriteLine("Exe name must end with .exe");
                Environment.Exit(0);
            }
            Console.WriteLine("Welcome to ExeBlocker!\n");
            if (IsBlockOn()) Console.WriteLine("Block is currently on.\n");
            else Console.WriteLine("Block is currently off.\n");

            MainMenu();            
        }


        /// <summary>
        /// Checks if the needed DWORD is in registry.
        /// </summary>
        /// <returns>True if it is already made</returns>
        public static bool CheckIfThereIsDWORD()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath);
            if (baseRegistryKey.GetValue("DisallowRun") != null) return true;
            return false;
        }


        /// <summary>
        /// Creates needed DWORD.
        /// </summary>
        public static void CreateDWORD()
        {
            //var rs = new RegistryKey();
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath);
            baseRegistryKey.SetValue("DisallowRun", "1", RegistryValueKind.DWord);
            baseRegistryKey.Close();
        }


        /// <summary>
        /// Creates a key for DisallowRun where the blocklist will be added.
        /// </summary>
        public static void CreateKey()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath);
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                key.CreateSubKey("DisallowRun", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
        }


        /// <summary>
        /// Handles user input. Uses switch case.
        /// TODO: Make code clearer, quite a mess.
        /// </summary>
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

            // 1 Add
            // 2 List all
            // 3 Delete one
            // 4 Delete all
            // 5 Start block
            // 6 Stop block
            // 7 Exit
            switch (input)
            {
                case 1:
                    Console.Clear();
                    AddToBlocklist();
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Listing all open exes in blocklist:");
                    Console.WriteLine("-----------------------------------");
                    ListAllFromBlocklist();
                    Console.WriteLine("-----------------------------------\n");
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("Input exe name you want to remove from blocklist");
                    string deleteInput = Console.ReadLine();
                    if (!CheckIfThereIsKeyString(deleteInput))
                    {
                        Console.WriteLine("There is no exe with that name in blocklist\n");
                        Console.WriteLine("Back to main menu..\n\n");
                        break;
                    }
                    DeleteFromBlocklist(deleteInput);
                    Console.Clear();
                    Console.WriteLine("Exe deleted from blocklist\n");
                    break;
                case 4:
                    DeleteAllFromBlocklist();
                    Console.Clear();
                    Console.WriteLine("Every exe deleted from blocklist");
                    break;
                case 5:
                    StartBlock();
                    Console.Clear();
                    Console.WriteLine("Block is now on");
                    break;
                case 6:
                    StopBlock();
                    Console.Clear();
                    Console.WriteLine("Block is now off");
                    break;
                case 7:
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Wrong input");
                    break;
            }
            MainMenu();
        }

        /// <summary>
        /// Adds wanted exes to registry, so it will be blocked to start. User can input multiple exes.
        /// </summary>
        public static void AddToBlocklist()
        {
            Console.WriteLine("Add to blocklist, write exe name and then press enter. When you want to stop, write 1 and if you want to abort write 0");

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
        }


        /// <summary>
        /// Used when user wants to add exe to blocklist (Adds exe name to registry, so it will block the exe from opening) 
        /// </summary>
        /// <param name="exelist"></param>
        public static void CreateKeyStrings(List<string> exelist)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath, true);
            foreach (string exeName in exelist)
            {
                if (ExeNameChecker(exeName) == true && CheckIfThereIsKeyString(exeName) == false)
                {
                    baseRegistryKey.SetValue(exeName, exeName, RegistryValueKind.String);
                }
            }
        }


        /// <summary>
        /// List all exes from blocklist (Value names from registrykey)
        /// </summary>
        public static void ListAllFromBlocklist()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath);
            String[] list = baseRegistryKey.GetValueNames();
            foreach (String a in list)
            {
                Console.WriteLine(a);
            }
        }


        /// <summary>
        /// Deletes exe from blocklist (Value name from registrykey)
        /// </summary>
        /// <param name="name">Exe name</param>
        private static void DeleteFromBlocklist(String name)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath, true);
            if (name != null)
            {
                baseRegistryKey.DeleteValue(name);
            }
        }


        /// <summary>
        /// Deletes all exes from blocklist (Value names from registrykey)
        /// First asks if user is sure.
        /// </summary>
        private static void DeleteAllFromBlocklist()
        {
            ConsoleKeyInfo c;
            Console.WriteLine("Are you sure to delete all exes from blocklist? Y/N");
            c = Console.ReadKey();
            switch (c.KeyChar)
            {
                case 'y':
                    RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath, true);
                    String[] array = baseRegistryKey.GetValueNames();
                    foreach (String text in array)
                    {
                        baseRegistryKey.DeleteValue(text);
                    }
                    Console.WriteLine("\nExes deleted");
                    break;
                case 'n':
                    Console.WriteLine("Returning to main menu");
                    break;
                default:
                    Console.WriteLine("\nPress Y to delete all from blocklist or N to get back to main menu");
                    break;
            }
        }


        /// <summary>
        /// Will delete "*" from Value's name, so the block will work. (Value needs to be like notepad.exe"
        /// </summary>
        private static void StartBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath, true);
            String[] array = baseRegistryKey.GetValueNames();
            foreach (String text in array)
            {
                if (baseRegistryKey.GetValue(text).ToString().EndsWith("*"))
                {
                    baseRegistryKey.SetValue(text, text);
                }
            }
        }


        /// <summary>
        /// Stops the block from working. Adds "*" to every exe's value name, so it disables the block (Value will be like notepad.exe*)
        /// </summary>
        private static void StopBlock()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath, true);
            String[] array = baseRegistryKey.GetValueNames();
            foreach(String text in array)
            {
                baseRegistryKey.SetValue(text, text + "*");
            }
        }


        /// <summary>
        /// Checks if value ends with * and if it does, the block is off, else it is on. 
        /// </summary>
        /// <returns>True if block is on</returns>
        public static bool IsBlockOn()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath);
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

        /// <summary>
        /// Checks if there is the needed key in registry where the exes will be added. 
        /// </summary>
        /// <returns>True if there is key</returns>
        public static bool CheckIfThereIsKey()
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath);
            RegistryKey key = baseRegistryKey.OpenSubKey("DisallowRun");
            if (key == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Checks if there is KeyString in the Key. (If exe is already in blocklist)
        /// </summary>
        /// <param name="keyName">Name of the exe</param>
        /// <returns>True if the keystring is already there</returns>
        public static bool CheckIfThereIsKeyString(string keyName)
        {
            string[] keyStringNames;
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath);
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


        /// <summary>
        /// Deletes the wanted keystring from registry (Exe from blocklist)
        /// </summary>
        /// <param name="keyName"></param>
        public static void DeleteKeyString(string keyName)
        {
            RegistryKey baseRegistryKey = Registry.CurrentUser.OpenSubKey(baseRegistryKeyPath + keyStringPath);
            baseRegistryKey.DeleteValue(keyName);
        }


        /// <summary>
        /// Checks the user input, so it will only take numbers as input in main menu
        /// </summary>
        /// <param name="input">user input</param>
        /// <returns>input as a int if it is number</returns>
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


        /// <summary>
        /// Handles the exes name checking.
        /// </summary>
        /// <param name="name">exe name</param>
        /// <returns>true if the exe name is proper</returns>
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
