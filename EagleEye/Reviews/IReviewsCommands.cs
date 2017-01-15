namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByProduct();
        void GenerateReviewCountByMonthFromProduct();
        
        void GenerateReviewCountByCreatorFromProduct();

        void GenerateCommentDensityChangedByProduct();
        void GenerateCommentDensityChangedByMonthFromProduct();

        void GenerateCommentDensityUploadedByProduct();

        void GenerateDefectDensityChangedByProduct();
        void GenerateDefectDensityChangedByMonthFromProduct();

        void GenerateDefectDensityUploadedByProduct();

        void GenerateInspectionRateByMonthFromProduct();
    }
}
