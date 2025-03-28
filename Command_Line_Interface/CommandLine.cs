﻿// See https://aka.ms/new-console-template for more information


using System.Security.Policy;
using Console_Input.Commands;
using log4net.Config;

class CommandLine
{
    static void Main(string[] args)
    {
        XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

        while (true)
        {
            string? input = Console.ReadLine().Trim();
            if (input == null)
            {
                continue;
            }
            if (input == "exit")
            {
                break;
            }
            string[] token = input.Split(" ");
            string command = token[0].ToLower();
            var flags = new List<string>();
            var options = new List<string>();
            var arguments = new List<string>();
            for (int i = 1; i < token.Length; i++)
            {
                if (token[i].StartsWith("--"))
                {
                    string newOptions = token[i];
                    if (token[i].Contains('"') )
                    {
                        if (token[i].Count(c => c == '"') <= 1)
                        {
                            i++;
                            while (!token[i].Contains('"'))
                            {
                                newOptions += " " + token[i];
                                i++;
                            }
                            newOptions += " " + token[i];
                        }
                       
                    }
                    options.Add(newOptions);
                }
                else if (token[i].StartsWith("-"))
                {
                    char[] charArray = token[i].ToCharArray();
                    flags.AddRange(Array.ConvertAll(charArray, c => c.ToString()));
                }
                else
                {
                    string argument = token[i];
                    if (token[i].Contains('"'))
                    {
                        if (token[i].Count(c => c == '"') <= 1)
                        {
                            i++;
                            while (!token[i].Contains('"'))
                            {
                                argument += " " + token[i];
                                i++;
                            }
                            argument += " " + token[i];
                        }
                       
                    }
                    arguments.Add(argument);
                }

            }


            Action? commandHandler = command switch
            {
                "synsite" or "ss" => () => new SyncSite().CommandHandler(arguments, flags, options),
                "clearfootprint" or "cf" => () => new ClearFootprint().CommandHandler(arguments, flags, options),
                "clearmission" or "cm" => () => new ClearMission().CommandHandler(arguments, flags, options),
                "cleardashboard" or "cdash" => () => new ClearDashboard().CommandHandler(arguments, flags, options),
                "experimental" or "exp" => () => new Experimental().CommandHandler(arguments, flags, options),
                _ => null

            };
            try
            {
                if (commandHandler != null)
                {
                    commandHandler.Invoke();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An Unexpected Error Occured While Executing [{command}] Command");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                break;
            }

            
            
            /*if (command.Contains("syncsite") || command.Contains("ss"))
            {
                //syncsite [options]  <source> <targets> <site name>
                try
                {
                    new SyncSite().CommandHandler(arguments, flags, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An Unexpected Error Occured While Executing SyncSite Command");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }
            
            if (command.Contains("clearfootprint") || command.Contains("cf"))
            {
                //clearfootprint [options] <targets> 
                try
                {
                    new ClearFootprint().CommandHandler(arguments, flags, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An Unexpected Error Occured While Executing ClearFootprint Command");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }
            if (command.Contains("diagnose"))
            {
                //diagnose [options] <targets> 
                try
                {
                    new RobotDiagnostic().CommandHandler(arguments, flags, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An Unexpected Error Occured While Executing ClearFootprint Command");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }*/
        }
        
        Console.WriteLine("Press ENTER to continue....");
        Console.ReadLine();
    }
}