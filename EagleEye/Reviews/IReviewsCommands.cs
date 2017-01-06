namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth(string settingsKey);
        void GenerateReviewCountByProduct(string settingsKey);
        void GenerateCommentDensityUploadedByProduct(string settingsKey);
        void GenerateCommentDensityChangedByProduct(string settingsKey);
        void GenerateDefectDensityUploadedByProduct(string settingsKey);
        void GenerateDefectDensityChangedByProduct(string settingsKey);
        void GenerateReviewCountByCreator();
        void GenerateInspectionRateByMonthFromProduct();
        void GenerateDefectDensityChangedByMonthFromProduct();
        void GenerateCommentDensityChangedByMonthFromProduct();
    }
}
