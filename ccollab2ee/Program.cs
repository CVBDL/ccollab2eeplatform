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
using eeDataGenerator;
using ccollabDataGenerator;
using System.Net.Http;


namespace CcollabLauncher
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        readonly static string logConfigFile = @"logConfig.xml";

        static string taskId;
        static bool show_help = false;

        public static string CCOLLABCMD_FILE_NAME = "ccollabCmd.json";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // BasicConfigurator replaced with XmlConfigurator.
            XmlConfigurator.Configure(new System.IO.FileInfo(logConfigFile));

            log.Info("initialize command line parser");

            //TODO: support specific start&End time in the data query, use commandline parameter to receive the start&end time
            var optSet = new OptionSet() {
                { "t|task-id=",  "the task Id.",          v => taskId = v },
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

            //load ccollab commands
            string json = String.Empty;
            StreamReader sr = new StreamReader(CCOLLABCMD_FILE_NAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            List<ccollabCmd> ccollabCmds = null;
            try
            {
                ccollabCmds = ccollabCmd.InitFromJson(json);
            }
            catch(Exception)
            {
                log.Error(string.Format("Failed to load from JSON: {0}", CCOLLABCMD_FILE_NAME));
            }

            //enumerate the command to get the raw data
            var ccRawFiles = new List<string>();
            if(ccollabCmds == null || ccollabCmds.Count == 0)
            {
                var msg = "Program exit: No command to be proceed";
                log.Info(msg);
                Console.WriteLine(msg);
                return;
            }

            //foreach (var cmd in ccollabCmds)
            //{
            //    var fileName = GetCCRawFile(cmd.relUrl);
            //    if (string.IsNullOrEmpty(fileName))
            //        continue;

            //    ccRawFiles.Add(fileName);
            //    Console.WriteLine(fileName);
            //}
            ccRawFiles.Add(@"C:\reviews-report.csv");
            ccRawFiles.Add(@"C:\defects-report.csv");

            //Get data from the files
            var generator = CreateDataGenerator();
            if (generator != null)
                generator.Execute(ccRawFiles);

            EagleEyeDataGenerator ee = new eeDataGenerator.EagleEyeDataGenerator();
            ee.Execute(generator);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            //Remove all temp files
            foreach( var ccRawFile in ccRawFiles)
            {
                // File.Delete(ccRawFile);
            }

            //Update task state
            //TODO: Remove hard code and refactor
            //log.Info("Press Enter to notify task: " + taskId);
            //Console.ReadKey();

            //HttpClient client = new HttpClient();

            //try
            //{
            //    StringContent payload = new StringContent("{\"state\":\"success\"}", Encoding.UTF8, "application/json");
            //    log.Info("Sending request...");
            //    HttpResponseMessage response = client.PutAsync("http://127.0.0.1:3000/api/v1/tasks/" + taskId, payload).Result;
            //    response.EnsureSuccessStatusCode();
            //    var responseContent = response.Content;
            //    string responseBody = responseContent.ReadAsStringAsync().Result;

            //    Console.WriteLine(responseBody);
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine("\nUpdate task state error!");
            //    Console.WriteLine("Message :{0} ", e.Message);
            //}

        }

        /// <summary>
        /// Create data generator
        /// </summary>
        /// <param name="rawData">raw data to be handled</param>
        /// <returns></returns>
        protected static IeeDataGenerator CreateDataGenerator( )
        {
            return new ccollabDataGenerator.ccollabDataGenerator();
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
                appProcess.OutputDataReceived += (s, _e) => OutputDataReceived(writer, _e.Data);
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
            catch (Exception exp)
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
