// See https://aka.ms/new-console-template for more information

using Mir_Utilities;

class Program
{
    static void Main(string[] args)
    {
        String Logo = """
               __  __ _____ _____    _    _ _   _ _ _ _   _           
              |  \/  |_   _|  __ \  | |  | | | (_) (_) | (_)          
              | \  / | | | | |__) | | |  | | |_ _| |_| |_ _  ___  ___ 
              | |\/| | | | |  _  /  | |  | | __| | | | __| |/ _ \/ __|
              | |  | |_| |_| | \ \  | |__| | |_| | | | |_| |  __/\__ \
              |_|  |_|_____|_|  \_\  \____/ \__|_|_|_|\__|_|\___||___/
               

              #                                                   
              #                                            
              #                                            
              # Enter Command: 
              #                                                                     
              """;
        Console.SetCursorPosition(0,0);
        Console.WriteLine(Logo);
        List<String> messageStorage = new List<string>();
        Boolean displayMessages = false;
        String? running = null;
        int confirmationTaskCount = 0;
        Task task = Task.Run(ContinuousRollingDisplay);
        

        void DisplayMessage(String message, ConsoleColor color)
        {
            displayMessages = false;
            Console.SetCursorPosition(0, 11);
            Console.ForegroundColor = color;
            int padRight = (Console.WindowWidth < (message.Length +1))? 0 :  Console.WindowWidth - message.Length - 1;
            Console.WriteLine($"# {message}"+ " ".PadRight(padRight));
            Console.ResetColor();
            Console.SetCursorPosition(0, 13);
            displayMessages = true;
        }


        async Task<Boolean> GetConfirmation(String message)
        {
            Task<Boolean> getConfirmationTask = Task.Run(() =>
            {
                displayMessages = false;
                confirmationTaskCount++;
                int tries = 0;
                while (tries != 3)
                {
                    DisplayMessage($"!Caution! {message}", ConsoleColor.Yellow);
                    String input = Console.ReadLine();
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine($"# {input}" + " ".PadRight(Console.WindowWidth));
                    Console.SetCursorPosition(0, 13);
                    Console.WriteLine(" ".PadRight(Console.WindowWidth));
                    if (input == "yes")
                    {
                        displayMessages = true;
                        return true;
                    }
                    else if (input == "no")
                    {
                        displayMessages = true;
                        return false;
                    }
                    else
                    {
                        DisplayMessage($"!Caution! Invalid Message", ConsoleColor.Yellow);
                        tries++;
                    }
                }

                DisplayMessage($"!Caution! Too many invalid commands, defaulting to no.", ConsoleColor.Yellow);
                displayMessages = true;
                return false;
            });
            Boolean b = await getConfirmationTask;
            confirmationTaskCount--;
            return b;
        }


        async void ContinuousRollingDisplay()
        {
            int count = 0;
            List<String> rotation = new List<String> { "-", "\\", "|", "/" };
            while (true)
            {
                if (displayMessages && (confirmationTaskCount == 0))
                {
                    
                    if (running != null)
                    {
                        Console.SetCursorPosition(0, 8);
                        Console.WriteLine($"# {running} -----------------   {rotation[count]}");
                        Console.SetCursorPosition(0, 13);
                        count = count >=3 ?0: count+1 ;
                    }

                    if (messageStorage.Count != 0)
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine(messageStorage[0]+" ".PadRight(Console.WindowWidth));
                        messageStorage.RemoveAt(0);
                        Console.SetCursorPosition(0, 13);
                    }

                    await Task.Delay(10);
                }
            }
        }


        while (running == null)
        {
            String command = Console.ReadLine();
            Console.SetCursorPosition(0, 12);
            Console.WriteLine($"# {command}"+" ".PadRight(Console.WindowWidth));
            Console.SetCursorPosition(0, 13);
            Console.WriteLine(" ".PadRight(Console.WindowWidth));
            if (command != null)
            {
                List<String> commandBreak = command.Split(" ").ToList();
                if (commandBreak[0] == "export")
                {
                    Boolean exportAsFile = false;
                    String mapName = null;
                    String robotIp = null;
                    String authId = null;
                    String fileName = null;
                    if (commandBreak.Contains("-f"))
                    {
                        exportAsFile = true;
                        int index = commandBreak.IndexOf("--file-name");
                        if (index != -1)
                        {
                            fileName = commandBreak[index + 1];
                        }
                    }

                    int index2 = commandBreak.IndexOf("--map-name");
                    if (index2 != -1)
                    {
                        mapName = commandBreak[index2 + 1];
                    }

                    int index3 = commandBreak.IndexOf("--robot-ip");
                    if (index3 != -1)
                    {
                        robotIp = commandBreak[index3 + 1];
                    }
                    

                    if (exportAsFile && fileName == null)
                    {
                        DisplayMessage("Enter File Name", ConsoleColor.Green);
                        fileName = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {fileName}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(fileName == null)
                        {
                            DisplayMessage("Invalid File Name, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }

                    if (robotIp == null)
                    {
                        DisplayMessage("Enter Robot IP", ConsoleColor.Green);
                        robotIp = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {robotIp}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(robotIp == null)
                        {
                            DisplayMessage("Invalid Robot IP, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }
                    if(authId == null)
                    {
                        DisplayMessage("Enter Auth ID", ConsoleColor.Green);
                        authId = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {authId}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(authId == null)
                        {
                            DisplayMessage("Invalid Auth ID, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }
                    if(mapName == null)
                    {
                        DisplayMessage("Enter Map Name", ConsoleColor.Green);
                        mapName = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {mapName}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(mapName == null)
                        {
                            DisplayMessage("Invalid Map Name, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }
                    
                    
                    DisplayMessage($"Confirm Details | Map name: {mapName} | Robot IP: {robotIp} |Auth ID: {authId}", ConsoleColor.Green);
                    String conf = Console.ReadLine();
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine($"# {conf}"+" ".PadRight(Console.WindowWidth));
                    Console.SetCursorPosition(0,13);
                    Console.WriteLine(" ".PadRight(Console.WindowWidth));
                    if(conf == "yes")
                    {
                        MirUtilitiesOld mirUtilitiesOld = new MirUtilitiesOld(robotIp, authId);
                        if(mirUtilitiesOld.ApiStatus == false)
                        {
                            DisplayMessage("Unable to reach the Robot, Exiting", ConsoleColor.Red);
                            Task.Delay(3000).Wait();
                            DisplayMessage(" ",ConsoleColor.White);
                            Main(args);
                        }
                        running = "Exporting Map from Robot";
                        displayMessages = true;
                        try
                        {
                            String map = mirUtilitiesOld.Export(mapName);
                            if (exportAsFile)
                            {
                                File.WriteAllText(fileName, map);
                                DisplayMessage("Exported to File", ConsoleColor.Green);
                            }
                            else
                            {
                                DisplayMessage(map, ConsoleColor.Green);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.SetCursorPosition(0,15);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(e.Message + e.StackTrace);
                            Main(args);
                        }

                        
                        running = null;
                        displayMessages = false;
                    }
                    else
                    {
                        DisplayMessage("Exiting", ConsoleColor.Red);
                        Main(args);
                    }
                }
                else if (commandBreak[0] == "import")
                {
                    String input = null;
                    String fileName = null;
                    String robotIp = null;
                    String authId = null;
                    Boolean bypassConfirmation = false;
                    if (commandBreak.Contains("-f"))
                    {
                        int index = commandBreak.IndexOf("--file-name");
                        if (index != -1)
                        {
                            fileName = commandBreak[index + 1];
                        }
                        if(fileName == null)
                        {
                            DisplayMessage("Enter File Name", ConsoleColor.Green);
                            fileName = Console.ReadLine();
                            Console.SetCursorPosition(0, 12);
                            Console.WriteLine($"# {fileName}"+" ".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition(0,13);
                            Console.WriteLine(" ".PadRight(Console.WindowWidth));
                            if(fileName == null)
                            {
                                DisplayMessage("Invalid File Name, Exiting", ConsoleColor.Red);
                                Main(args);
                            }
                        }
                        input = File.ReadAllText(fileName);
                    }

                    int index2 = commandBreak.IndexOf("--robot-ip");
                    if (index2 != -1)
                    {
                        robotIp = commandBreak[index2 + 1];
                    }

                    if (input == null)
                    {
                        DisplayMessage("Enter Map Data:", ConsoleColor.Green);
                        input = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {input}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(input == null)
                        {
                            DisplayMessage("Invalid Data, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }
                    if(robotIp == null)
                    {
                        DisplayMessage("Enter Robot IP", ConsoleColor.Green);
                        robotIp = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {robotIp}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(robotIp == null)
                        {
                            DisplayMessage("Invalid Robot IP, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }

                    if (authId == null)
                    {
                        DisplayMessage("Enter Auth ID", ConsoleColor.Green);
                        authId = Console.ReadLine();
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine($"# {authId}"+" ".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition(0,13);
                        Console.WriteLine(" ".PadRight(Console.WindowWidth));
                        if(authId == null)
                        {
                            DisplayMessage("Invalid Auth ID, Exiting", ConsoleColor.Red);
                            Main(args);
                        }
                    }
            
                    if (commandBreak.Contains("--bypass-confirmation"))
                    {
                        bypassConfirmation = true;
                    }
                    
                    DisplayMessage($"Confirm Details | Robot IP: {robotIp} | Auth ID: {authId}", ConsoleColor.Green);
                    String conf = Console.ReadLine();
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine($"# {conf}"+" ".PadRight(Console.WindowWidth));
                    Console.SetCursorPosition(0,13);
                    Console.WriteLine(" ".PadRight(Console.WindowWidth));
                    if(conf == "yes")
                    {
                        MirUtilitiesOld mirUtilitiesOld = new MirUtilitiesOld(robotIp, authId);
                        if(mirUtilitiesOld.ApiStatus == false)
                        {
                            DisplayMessage("Unable to reach the Robot, Exiting", ConsoleColor.Red);
                            Task.Delay(3000).Wait();
                            Main(args);
                        }
                        void callback1(String message)
                        {
                            messageStorage.Add(message);
                        }

                        running = "Importing Map to Robot";
                        displayMessages = true;
                        try
                        {
                            mirUtilitiesOld.ImportMap(input, callback1, bypassConfirmation, GetConfirmation).Wait();
                        }
                        catch (Exception e)
                        {
                            Console.SetCursorPosition(0,15);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(e.Message + e.StackTrace);
                            Main(args);
                        }

                        displayMessages = false;
                        running = null;
                        Task.Delay(100).Wait();
                        DisplayMessage("Imported Map", ConsoleColor.Green);
                    }
                    else
                    {
                        DisplayMessage("Exiting", ConsoleColor.Red);
                        Main(args);
                    }
                    
                    
                    
                }
                else
                {
                    DisplayMessage("Invalid Command", ConsoleColor.Red);
                }
            }
        }
    }
}


/*

Boolean Running = true;

while (!Running)
{
    String? input = Console.ReadLine();
    if (input == "exit")
    {
        Environment.Exit(0);
    }
    else if (input == "export")
    {
        Console.WriteLine("Export MiR Map ");
        Console.WriteLine("Export to File or String?");
        String? ExportType = Console.ReadLine();
        String? FilePath = null;
        if (ExportType == "File")
        {
            Console.WriteLine("Enter File Path");
            FilePath = Console.ReadLine();

        }else if (!(ExportType == "String"))
        {
            Console.WriteLine("Invalid Options");
            continue;
        }
        Console.WriteLine("Enter Map Name");
        String? MapName = Console.ReadLine();
        Console.WriteLine("Enter Robot IP");
        String? RobotIp = Console.ReadLine();
        Console.WriteLine("Enter Auth ID");
        String? AuthId = Console.ReadLine();
        if(MapName == null || RobotIp == null || AuthId == null)
        {
            Console.WriteLine("Invalid Input");
            continue;
        }
        Console.WriteLine($"""
                          Confirm Details:
                          Map name: {MapName}
                          Robot IP: {RobotIp}
                          Auth ID: {AuthId}
                          """);
        String input2 = Console.ReadLine();
        if (input2 == "yes")
        {
            MirUtilities mirUtilities = new MirUtilities(RobotIp!, AuthId!);
            String map = mirUtilities.Export(MapName!);
            if (ExportType == "File")
            {
                File.WriteAllText(FilePath!, map);
            }
            else if (ExportType == "String")
            {
                Console.WriteLine(map);
            }
        }
        else if (input2 == "no")
        {
            continue;
        }
        else
        {
            Console.WriteLine("Invalid Command");
        }

    }
    else if (input == "help")
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("exit - Exits the program");
        Console.WriteLine("help - Displays this message");
    }
    else
    {
        Console.WriteLine("Invalid Command");
    }
}
*/