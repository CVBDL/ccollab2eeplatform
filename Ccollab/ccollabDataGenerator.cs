﻿using log4net;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace Ccollab
{
    public class CcollabDataGenerator : ICcollabDataSource
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CcollabDataGenerator));

        private static readonly string SETTINGS_FILENAME = "ConfigurationFiles/ccollab-cmd.json";
        
        private List<ReviewRecord> reviewsRawData = null;
        private List<DefectRecord> defectsRawData = null;
        private bool hasFetchedCcollabData = false;

        public List<ReviewRecord> GetReviewsRawData()
        {
            if (!hasFetchedCcollabData)
            {
                FetchCcollabData();
            }

            return reviewsRawData;
        }

        public List<DefectRecord> GetDefectsRawData()
        {
            if (!hasFetchedCcollabData)
            {
                FetchCcollabData();
            }

            return defectsRawData;
        }
        
        /// <summary>
        /// Read raw CSV files downloaded from code collaborator.
        /// </summary>
        /// <returns></returns>
        private bool FetchCcollabData()
        {
            hasFetchedCcollabData = true;

            List<CcollabCmd> ccollabCmds = ReadCcollabConfigJson();

            if (ccollabCmds == null)
            {
                log.Info("No ccollab command to be proceed.");

                return false;
            }

            Dictionary<string, string> ccRawFiles = FetchRawCsvFilesFromCcollabServer(ccollabCmds);
            
            string reviewsRawFileName = string.Empty;

            if (ccRawFiles.TryGetValue("Reviews", out reviewsRawFileName))
            {
                reviewsRawData = new List<ReviewRecord>();

                foreach (var row in ReadInCsvFile(reviewsRawFileName))
                {
                    ReviewRecord record = new ReviewRecord(row);

                    reviewsRawData.Add(record);
                }
            }
            else
            {
                log.Info("Cannot find reviews CSV file.");
            }

            string defectsRawFileName = string.Empty;

            if (ccRawFiles.TryGetValue("Defects", out defectsRawFileName))
            {
                defectsRawData = new List<DefectRecord>();

                foreach (var row in ReadInCsvFile(defectsRawFileName))
                {
                    DefectRecord record = new DefectRecord(row);

                    defectsRawData.Add(record);
                }
            }
            else
            {
                log.Info("Cannot find defects CSV file.");
            }

            foreach (var ccRawFile in ccRawFiles)
            {
                //File.Delete(ccRawFile.Value);
            }

            if (reviewsRawData == null && defectsRawData == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read ccollab commands configuration json file.
        /// </summary>
        /// <returns>Deserialized ccollab commands json object.</returns>
        private List<CcollabCmd> ReadCcollabConfigJson()
        {
            string json = string.Empty;

            StreamReader sr = new StreamReader(SETTINGS_FILENAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            List<CcollabCmd> ccollabCmds = null;
            try
            {
                ccollabCmds = JsonConvert.DeserializeObject<List<CcollabCmd>>(json);
            }
            catch (Exception)
            {
                log.Error(string.Format("Failed to read json file: {0}", SETTINGS_FILENAME));
            }

            return ccollabCmds;
        }

        /// <summary>
        /// Run ccollab commands to fetch raw csv files from ccollab server.
        /// </summary>
        /// <param name="cmds">ccollab commands config object.</param>
        /// <returns>Downloaded csv files' filename.</returns>
        private Dictionary<string, string> FetchRawCsvFilesFromCcollabServer(List<CcollabCmd> cmds)
        {
            var ccRawFiles = new Dictionary<string, string>();

            //foreach (var cmd in cmds)
            //{
            //    var downloadLink = GenerateCCRawFileDownloadLink(cmd);
            //    var fileName = GetCCRawFile(downloadLink);
            //    if (string.IsNullOrEmpty(fileName))
            //    {
            //        continue;
            //    }

            //    ccRawFiles.Add(cmd.Id, fileName);
            //    Console.WriteLine(fileName);
            //}

            ccRawFiles.Add("Reviews", @"ConfigurationFiles/reviews-report.csv");
            ccRawFiles.Add("Defects", @"ConfigurationFiles/defects-report.csv");

            return ccRawFiles;
        }

        /// <summary>
        /// Read csv file and parse rows and columns.
        /// </summary>
        /// <param name="fileName">CSV filename.</param>
        /// <returns>All of the rows in csv file.</returns>
        private List<string[]> ReadInCsvFile(string fileName)
        {
            List<string[]> rows = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch(MalformedLineException e)
                    {
                        log.Error(string.Format("An error occurred when parsing the CSV row. Message: {0}", e.Message));
                        continue;
                    }

                    rows.Add(fields);
                }

                int length = rows.Count;

                if (length > 0)
                {
                    rows.RemoveAt(0);
                }

                return rows;
            }
        }

        /// <summary>
        /// Generate a file download link with the specified review creation date range.
        /// 
        /// Append "reviewCreationDateFilter" query parameter string to command's "RelUrl".
        /// 
        /// Raw format:
        ///     reviewCreationDateFilter=lo=2016-10-01|||hi=2016-10-02
        ///
        /// URL encoded format:
        ///     &reviewCreationDateFilter=lo%3D2016-10-01%7C%7C%7Chi%3D2016-10-02
        ///
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>Generated file download link.</returns>
        private string GenerateCCRawFileDownloadLink(CcollabCmd cmd)
        {
            string downloadLink = string.Empty;
            string query = "lo=" + cmd.ReviewsCreationDateLow + "|||hi=" + cmd.ReviewsCreationDateHigh;

            int len = cmd.RelUrl.Length;

            if (len > 0)
            {
                downloadLink = cmd.RelUrl.Substring(0, len - 1) + "&reviewCreationDateFilter=" + HttpUtility.UrlEncode(query) + "\"";
            }

            return downloadLink;
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
