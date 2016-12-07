using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using eeDataGenerator;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;


namespace ccollabDataGenerator
{
    public class CcollabDataGenerator : IEagleEyeDataGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CcollabDataGenerator));

        public static string CCOLLABCMD_FILE_NAME = "ccollabCmd.json";

        protected List<string[]> _reviewsRawData;
        protected List<string[]> _defectsRawData;

        public List<string[]> ReviewsRawData
        {
            get
            {
                return _reviewsRawData;
            }
        }

        public List<string[]> DefectsRawData
        {
            get
            {
                return _defectsRawData;
            }
        }

        public bool Execute()
        {
            var ccollabCmds = ReadInCcollabCommandConfigJson();

            if (ccollabCmds == null || ccollabCmds.Count == 0)
            {
                log.Info("Program exit: No command to be proceed");
                return false;
            }

            var ccRawFiles = FetchCsvFilesFromCcollabServer(ccollabCmds);

            _reviewsRawData = ReadInCsvFile(ccRawFiles[0]);
            _defectsRawData = ReadInCsvFile(ccRawFiles[1]);

            return true;
        }

        private List<ccollabCmd> ReadInCcollabCommandConfigJson()
        {
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
            catch (Exception)
            {
                log.Error(string.Format("Failed to load from JSON: {0}", CCOLLABCMD_FILE_NAME));
            }

            return ccollabCmds;
        }

        private List<string> FetchCsvFilesFromCcollabServer(List<ccollabCmd> ccollabCmds)
        {
            //enumerate the command to get the raw data
            var ccRawFiles = new List<string>();


            //foreach (var cmd in ccollabCmds)
            //{
            //    var fileName = GetCCRawFile(cmd.relUrl);
            //    if (string.IsNullOrEmpty(fileName))
            //        continue;

            //    ccRawFiles.Add(fileName);
            //    Console.WriteLine(fileName);
            //}

            ccRawFiles.Add(@"reviews-report.csv");
            ccRawFiles.Add(@"defects-report.csv");

            return ccRawFiles;
        }

        private List<string[]> ReadInCsvFile(string fileName)
        {
            //return File.ReadAllLines(fileName)
            //    .Skip(1)
            //    .Select(line => Regex.Split(line, ",(?=(?:[^']*'[^']*')*[^']*$)"))
            //    .ToList<string[]>();
            List<string[]> lines = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    lines.Add(fields);
                }

                int length = lines.Count;

                if (length > 0)
                {
                    lines.RemoveAt(0);
                }

                return lines;
            }
        }


        /// <summary>
        /// Get codecollaborator raw data
        /// </summary>
        /// <param name="relativeUrl">the URL used to retrive the raw data</param>
        /// <returns>file name which contains the raw data. null if have problem to get the data</returns>
        private string GetCCRawFile(string relativeUrl)
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
        private void OutputDataReceived(StreamWriter writer, string data)
        {
            if (writer == null)
                return;

            if (string.IsNullOrEmpty(data))
                return;

            writer.WriteLine(data);

        }

    }
}
