using Ccollab;
using Employees;
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
        
        static void Main(string[] args)
        {
            // BasicConfigurator replaced with XmlConfigurator.
            XmlConfigurator.Configure(new System.IO.FileInfo(logConfigFile));
            
            OptionSet optSet = new OptionSet() {
                { "t|task-id=",  "the task Id.",          v => taskId    = v },
                { "h|help", "show this message and exit", v => show_help = v != null },
            };
            
            // Parse the command line parameters.
            List<string> extra;
            try
            {
                extra = optSet.Parse(args);
            }
            catch (OptionException e)
            {
                log.Error(string.Format("An error occurred when parsing command line parameters: {0}", e.Message));
                return;
            }

            // Show help or usage.
            if (show_help)
            {
                ShowHelp(optSet);
                return;
            }

            if (EmployeesReader.Employees == null || EmployeesReader.Employees.Count == 0)
            {
                log.Error("An error occurred when reading employee settings.");
                return;
            }

            if (EagleEyeSettingsReader.Settings == null || !EagleEyeSettingsReader.Settings.IsValid())
            {
                log.Error("An error occurred when reading eagleeye settings.");
                return;
            }

            // ccollab data source
            ICcollabDataSource ccollabDataGenerator = new CcollabDataGenerator();
            
            // ccollab reviews charts related
            Reviews reviews = new Reviews(ccollabDataGenerator);

            if (reviews.GetValidRecords() == null || reviews.GetValidRecords().Count == 0)
            {
                log.Info("No valid review records.");
            }
            else
            {
                ReviewsManager reviewsManager = new ReviewsManager
                (
                    new ReviewCountByMonthCommand(reviews),
                    new ReviewCountByProductCommand(reviews),
                    new ReviewCountByCreatorCommand(reviews),
                    new CommentDensityUploadedByProductCommand(reviews),
                    new CommentDensityChangedByProductCommand(reviews),
                    new DefectDensityUploadedByProductCommand(reviews),
                    new DefectDensityChangedByProductCommand(reviews),
                    new InspectionRateByMonthFromProductCommand(reviews),
                    new DefectDensityChangedByMonthCommand(reviews),
                    new CommentDensityChangedByMonthCommand(reviews)
                );

                reviewsManager.GenerateReviewCountByMonth();
                reviewsManager.GenerateReviewCountByProduct();
                reviewsManager.GenerateReviewCountByEmployeeOfProduct();
                reviewsManager.GenerateCodeCommentDensityUploaded();
                reviewsManager.GenerateCodeCommentDensityChanged();
                reviewsManager.GenerateCodeDefectDensityUploaded();
                reviewsManager.GenerateCodeDefectDensityChanged();
                reviewsManager.GenerateInspectionRateByMonth();
                reviewsManager.GenerateDefectDensityChangedByMonth();
                reviewsManager.GenerateCommentDensityChangedByMonth();
            }

            // ccollab defects charts related
            Defects defects = new Defects(ccollabDataGenerator);

            if (defects.GetValidRecords() == null || defects.GetValidRecords().Count == 0)
            {
                log.Info("No valid defect records.");
            }
            else
            {
                DefectsManager defectsManager = new DefectsManager
                (
                    new DefectCountByProductCommand(defects),
                    new DefectCountByInjectionStageCommand(defects),
                    new DefectCountByTypeCommand(defects),
                    new DefectCountByCreatorCommand(defects),
                    new DefectCountOfTypeByProductCommand(defects),
                    new DefectCountOfTypeByCreatorCommand(defects),
                    new DefectCountOfSeverityByProductCommand(defects),
                    new DefectCountOfSeverityByCreatorCommand(defects)
                );

                defectsManager.GenerateDefectCountByProduct();
                defectsManager.GenerateDefectCountByInjectionStage();
                defectsManager.GenerateDefectCountByType();
                defectsManager.GenerateDefectCountByCreator();
                defectsManager.GenerateDefectCountOfTypeByProduct();
                defectsManager.GenerateDefectCountOfTypeByCreator();
                defectsManager.GenerateDefectSeverityByProduct();
                defectsManager.GenerateDefectSeverityCountByCreator();
            }
            
            // notify task state
            if (string.IsNullOrWhiteSpace(taskId))
            {
                log.Error("No task id provided, so unable to notify EagleEye task state.");
            }
            else
            {
                NotifyTaskStatus(taskId, true);
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
            log.Info(string.Format("Sending task state to server: {0} ...", EagleEyeSettingsReader.Settings.ApiRootEndpoint));

            HttpClient client = new HttpClient();
            
            try
            {
                string requestUri = EagleEyeSettingsReader.Settings.ApiRootEndpoint + "tasks/" + taskId;

                string json = isSuccess ? "{\"state\":\"success\"}" : "{\"state\":\"failure\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PutAsync(requestUri, content).Result;
                response.EnsureSuccessStatusCode();

                // use for debugging
                HttpContent responseContent = response.Content;
                string responseBody = responseContent.ReadAsStringAsync().Result;

                log.Info(string.Format("Notify task state response: {0}", responseBody));
            }
            catch (HttpRequestException e)
            {
                log.Error(string.Format("An error occurred when sending task state notification request."));
                log.Error(string.Format("Exception: {0}", e.Message));
            }

            log.Info("Sending task state to server ... Done");
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
