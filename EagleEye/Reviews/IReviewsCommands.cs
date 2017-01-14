namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByProduct();
        void GenerateReviewCountByMonthFromProduct();
        
        void GenerateReviewCountByCreatorFromProduct();

        void GenerateCommentDensityChangedByProduct(string settingsKey);
        void GenerateCommentDensityChangedByMonthFromProduct();

        void GenerateCommentDensityUploadedByProduct(string settingsKey);

        void GenerateDefectDensityChangedByProduct(string settingsKey);
        void GenerateDefectDensityChangedByMonthFromProduct();

        void GenerateDefectDensityUploadedByProduct(string settingsKey);

        void GenerateInspectionRateByMonthFromProduct();
    }
}
