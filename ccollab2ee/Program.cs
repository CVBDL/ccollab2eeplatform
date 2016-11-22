using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;
using System.Reflection;



namespace CcollabLauncher
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        readonly static string logConfigFile = @"logConfig.xml";

        static string taskId;
        static bool show_help = false;

        public static string EMPLOYEES_FILE_NAME = "employees.json";
        public static string CCOLLABCMD_FILE_NAME = "ccollabCmd.json";
        static void Main(string[] args)
        {
            // BasicConfigurator replaced with XmlConfigurator.
            XmlConfigurator.Configure(new System.IO.FileInfo(logConfigFile));

            log.Info("initialize command line parser");

            var optSet = new OptionSet() {
                { "i|id=",  "the task Id.",               v => taskId = v },
                { "h|help", "show this message and exit", v => show_help = v != null },
            };

            log.Debug("parse command line with 'Options'");

            //Parse the command line parameters
            List<string> extra;
            try
            {
                extra = optSet.Parse(args);
            }
            catch (OptionException e)
            {
                log.Debug(string.Format("OptionException: {0}", e.Message));

                Console.Write("Integrated Security Testing: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `DfS.SecureTesting.Launcher --help' for more information.");

                log.Info("IntegratedSecurityTesting Exit");
                return;
            }

            //Show help or uage
            if (show_help)
            {
                ShowHelp(optSet);
                return;
            }

            //load Employees
            string json = null;
            StreamReader sr = new StreamReader(EMPLOYEES_FILE_NAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            List<Employee> employees = null;
            try
            {
                employees = Employee.InifFromJson(json);
            }
            catch (Exception)
            {
                log.Error(string.Format("Failed to load from JSON: {0}", EMPLOYEES_FILE_NAME));
            }

            //load ccollab commands
            sr = new StreamReader(CCOLLABCMD_FILE_NAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            List<ccollabCmd> ccollabCmds = null;
            try
            {
                ccollabCmds = ccollabCmd.InifFromJson(json);
            }
            catch(Exception)
            {
                log.Error(string.Format("Failed to load from JSON: {0}", CCOLLABCMD_FILE_NAME));
            }

            //enumerate the command to get the raw data
            var ccRawFiles = new List<string>();
            if(ccollabCmds == null || ccollabCmds.Count>0)
            {
                var msg = "Program exit: No command to be proceed";
                log.Info(msg);
                Console.WriteLine(msg);
                return;
            }

            foreach( var cmd in ccollabCmds )
            {
                var fileName = GetCCRawFile(cmd.relUrl);
                if (string.IsNullOrEmpty(fileName))
                    continue;

                ccRawFiles.Add(fileName);
                Console.WriteLine(fileName);
            }


            //Get data from the files


            Console.ReadKey();

            //Remove all temp files
            foreach( var ccRawFile in ccRawFiles)
            {
                File.Delete(ccRawFile);
            }

        }


        /// <summary>
        /// Get codecollaborator raw data
        /// </summary>
        /// <param name="relativeUrl">the URL used to retrive the raw data</param>
        /// <returns>file name which contains the raw data. null if have problem to get the data</returns>
        public static string GetCCRawFile( string relativeUrl )
        {
            string tempFile = Path.GetTempFileName();
            string outputFile = tempFile + ".csv";


            var ostrm = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var writer = new StreamWriter(ostrm, Encoding.UTF8);

            var exePathName = "ccollab.exe";
            var cmdLine = "admin wget ";
                

            cmdLine = cmdLine + relativeUrl;
            var workDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            try
            {

                var appProcess = new Process();
                var startInfo = new ProcessStartInfo(exePathName, cmdLine)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    ErrorDialog = true
                };

                appProcess.StartInfo = startInfo;
                appProcess.StartInfo.UseShellExecute = false;
                appProcess.StartInfo.RedirectStandardOutput = true;
                appProcess.StartInfo.RedirectStandardError = true;
                //set the working folder to System32 to meet ccollab requirements
                appProcess.StartInfo.WorkingDirectory = workDir;
                appProcess.OutputDataReceived += (s, _e) => OutputDataReceived(writer, _e.Data) ;
                appProcess.ErrorDataReceived += (s, _e) => Console.WriteLine(_e.Data);
                appProcess.Exited += (s, _e) => Console.WriteLine(string.Format("{0} Exited with {1}", exePathName, appProcess.ExitCode));
                appProcess.EnableRaisingEvents = true;


                using (ostrm)
                {
                    using (writer)
                    {

                        appProcess.Start();
                        log.InfoFormat("process Started: {0} {1}", exePathName, cmdLine);

                        appProcess.BeginOutputReadLine();
                        appProcess.BeginErrorReadLine();

                        appProcess.WaitForExit();
                        log.InfoFormat("process Exited: {0}, {1}", exePathName, cmdLine);
                    }
                }
            }
            catch( Exception exp)
            {
                Console.WriteLine("Failed to get raw data from CodeCollabrator: {0}", exp.Message);
                outputFile = null;
            }

            return outputFile;

        }

        /// <summary>
        /// Event handler for receiving data from standard output
        /// </summary>
        /// <param name="writer">instance of <see cref="StreamWriter"/> to receive the data</param>
        /// <param name="data">data being received</param>
        protected static void OutputDataReceived(StreamWriter writer, string data)
        {
            if (writer == null)
                return;

            if (string.IsNullOrEmpty(data))
                return;

            writer.WriteLine(data);

        }

        /// <summary>
        /// display the usage
        /// </summary>
        /// <param name="p">Instance of <see cref="OptionSet"/></param>
        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: ccollab2ee [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
