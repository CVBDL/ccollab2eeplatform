using Ccollab;
using EagleEye;
using EagleEye.Defects;
using EagleEye.Reviews;
using EagleEye.Settings;
using log4net;
using log4net.Config;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Ccollab2EagleEye
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        readonly static string logConfigFile = @"logConfig.xml";

        static string taskId;
        static bool show_help = false;
        
        /// <summary>
        /// Main method
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

            // ccollab data source
            ICcollabDataSource ccollabDataGenerator = new CcollabDataGenerator();

            // ccollab reviews charts related
            Reviews reviews = new Reviews(ccollabDataGenerator);

            ICommand cmdGenerateReviewCountByMonth = new GenerateReviewCountByMonthCommand(reviews);
            ICommand cmdGenerateReviewCountByProduct = new GenerateReviewCountByProductCommand(reviews);

            ReviewsManager reviewsManager = new ReviewsManager(
                cmdGenerateReviewCountByMonth,
                cmdGenerateReviewCountByProduct
            );

            reviewsManager.GenerateReviewCountByMonth();
            reviewsManager.GenerateReviewCountByProduct();

            // ccollab defects charts related
            Defects defects = new Defects(ccollabDataGenerator);

            ICommand cmdGenerateDefectCountByProduct = new GenerateDefectCountByProductCommand(defects);

            DefectsManager defectsManager = new DefectsManager(cmdGenerateDefectCountByProduct);

            defectsManager.GenerateReviewCountByMonth();

            // notify task state
            if (String.IsNullOrEmpty(taskId))
            {
                log.Error("No task id provided, so unable to notify EagleEye task state.");
            }
            else
            {
                //NotifyTaskStatus(taskId, true);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Change the task state in EagleEye Platform
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isSuccess"></param>
        static void NotifyTaskStatus(string taskId, bool isSuccess)
        {
            log.Info("Sending task notification to server ...");

            HttpClient client = new HttpClient();

            var settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            string json = isSuccess ? "{\"state\":\"success\"}" : "{\"state\":\"failure\"}";

            try
            {
                StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(settings.EagleEyeApiRootEndpoint + "tasks/" + taskId, payload).Result;
                response.EnsureSuccessStatusCode();

                // use for debugging
                var responseContent = response.Content;
                string responseBody = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                log.Info("Error: Notify task with id '" + taskId + "'");
                Console.WriteLine("Message :{0} ", e.Message);
            }
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
