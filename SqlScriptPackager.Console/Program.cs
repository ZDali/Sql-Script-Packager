using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SqlScriptPackager.Core;
using SqlScriptPackager.Core.Packaging;
using System.Threading;
using System.IO;

namespace SqlScriptPackager.Console
{
    class Program
    {
        private const string COMMAND_EXEC = "exec";
        private const string COMMAND_LIST = "list";

        private const string SWITCH_VERBOSE = "/v";
        private const string SWITCH_QUIET = "/q";
        private const string SWITCH_ABORT = "/a";

        private static string _packageFile;
        private static bool _isListCommand;
        private static Switches _selectedSwitches;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (args.Length == 0)
            {
                PrintHelpText();
                System.Console.ReadKey();
                return;
            }

            _packageFile = args[0];
            _isListCommand = IsListCommand(args);
            _selectedSwitches = ParseSwitches(args);

            ScriptPackage package = LoadPackage(_packageFile);

            if (_isListCommand)
                ListScripts(package);
            else
                ExecuteScripts(package);
        }

        private static ScriptPackage LoadPackage(string path)
        {
            if (!File.Exists(path))
                ExitProgram("File '" + path + "' does not exist.", true);

            ScriptPackage package = new ScriptPackage();
            package.Load(_packageFile);
            return package;
        }

        private static void ListScripts(ScriptPackage package)
        {
            foreach (Script script in package.Scripts)
            {
                System.Console.WriteLine(string.Join("\t", script.ContentResource.Location, script.ScriptType, script.Connection.ConnectionName));

                if (_selectedSwitches.HasFlag(Switches.Verbose))
                {
                    System.Console.WriteLine(script.ScriptContents);
                    System.Console.WriteLine(script.Connection.ConnectionString);
                }
            }
        }

        static void ExecuteScripts(ScriptPackage package)
        {
            ScriptExecutor executor = new ScriptExecutor();

            if (_selectedSwitches.HasFlag(Switches.Verbose))
                executor.LogUpdated += new ScriptExecutor.LogChanged(executor_LogUpdated);
            else if (!_selectedSwitches.HasFlag(Switches.Verbose) && !_selectedSwitches.HasFlag(Switches.Quiet))
                foreach (Script script in package.Scripts)
                    script.StatusChanged += new Script.ScriptChange(script_StatusChanged);

            executor.ExecuteScripts(package.Scripts);

            while (executor.IsExecuting)
                Thread.Sleep(1000);

            foreach (Script script in package.Scripts)
            {
                if (script.Status == ScriptStatus.Disabled || script.Status == ScriptStatus.Executed)
                    continue;

                Environment.ExitCode = 1;
                System.Console.WriteLine("One or more scripts failed to execute properly.");
                break;
            }
        }

        static void script_StatusChanged(Script script)
        {
            switch(script.Status)
            {
                case ScriptStatus.Executing:
                    System.Console.WriteLine(script.ContentResource.Location + " is executing.");
                    break;

                case ScriptStatus.Failed:
                    System.Console.WriteLine(script.ContentResource.Location + " encountered an error.");
                    break;

                case ScriptStatus.Aborted:
                    System.Console.WriteLine(script.ContentResource.Location + " has been aborted.");
                    break;
            }
        }

        static void executor_LogUpdated(string latestMessage)
        {
            System.Console.WriteLine(latestMessage);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.ExitCode = 1;
            System.Console.Write(e.ExceptionObject.ToString());
        }

        static bool IsListCommand(string[] args)
        {            
            if (args.Length < 2)
                return true;

            string command = args[1];

            if (command == COMMAND_LIST)
                return true;
            else if (command == COMMAND_EXEC)
                return false;
            else
                ExitProgram("Unrecognized command '" + command + "'", true);

            return command == COMMAND_LIST;
        }

        static Switches ParseSwitches(string[] args)
        {
            Switches selectedSwitches = new Switches();
            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case SWITCH_VERBOSE:
                        selectedSwitches = selectedSwitches | Switches.Verbose;
                        break;

                    case SWITCH_QUIET:
                        selectedSwitches = selectedSwitches | Switches.Quiet;
                        break;

                    case SWITCH_ABORT:
                        selectedSwitches = selectedSwitches | Switches.AbortOnFailure;
                        break;
                }
            }

            if (selectedSwitches.HasFlag(Switches.Quiet) && selectedSwitches.HasFlag(Switches.Verbose))
                ExitProgram("Cannot use quiet and verbose switches at the same time!", true);

            return selectedSwitches;
        }

        static void PrintHelpText()
        {
            System.Console.Write("usage: ");
            System.Console.Write(Process.GetCurrentProcess().ProcessName);
            System.Console.WriteLine(" [package filename] [command] [switches]");
                        
            System.Console.Write("example: ");
            System.Console.Write(Process.GetCurrentProcess().ProcessName);
            System.Console.WriteLine(" test.sp exec /v");


            System.Console.WriteLine();
            System.Console.WriteLine("Available Commands");
            System.Console.WriteLine("list - Lists the contents of the script package.");
            System.Console.WriteLine("exec - Executes the contents of the script package");

            System.Console.WriteLine();
            System.Console.WriteLine("Switches");
            System.Console.WriteLine("v - Verbose output.  When used with list, script contents and connection string information will be printed.  When used with exec, detailed execution logging will be printed.");

            System.Console.WriteLine();
            System.Console.WriteLine("a - Aborts execution on first error.");

            System.Console.WriteLine();
            System.Console.WriteLine("q - Quietly executes scripts with no output.");
        }

        static void ExitProgram(string message, bool hasError)
        {
            System.Console.WriteLine(message);

            if (hasError)
                Environment.ExitCode = 1;

            Environment.Exit(Environment.ExitCode);
        }
    }
}
