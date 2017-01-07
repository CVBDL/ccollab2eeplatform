namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth(string settingsKey);
        void GenerateReviewCountByProduct(string settingsKey);
        void GenerateReviewCountByCreator();
        

        void GenerateCommentDensityChangedByProduct(string settingsKey);
        void GenerateCommentDensityChangedByMonthFromProduct();


        void GenerateCommentDensityUploadedByProduct(string settingsKey);


        void GenerateDefectDensityChangedByProduct(string settingsKey);
        void GenerateDefectDensityChangedByMonthFromProduct();


        void GenerateDefectDensityUploadedByProduct(string settingsKey);


        void GenerateInspectionRateByMonthFromProduct();
    }
}
