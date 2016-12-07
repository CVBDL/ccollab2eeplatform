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
            
            var ccollabGenerator = CreateDataGenerator();
            
            var eeReviewsGenerator = CreateReviewsDataGenerator(ccollabGenerator);

            eeReviewsGenerator.Execute();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            // TODO: Remove files
            //Remove all temp files
            //foreach( var ccRawFile in ccRawFiles)
            //{
            //    // File.Delete(ccRawFile);
            //}

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
        protected static IEagleEyeDataGenerator CreateDataGenerator( )
        {
            return new ccollabDataGenerator.CcollabDataGenerator();
        }

        protected static EagleEyeReviewsDataGenerator CreateReviewsDataGenerator(IEagleEyeDataGenerator ccollabGenerator)
        {
            return new eeDataGenerator.EagleEyeReviewsDataGenerator(ccollabGenerator);
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
