namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth(string settingsKey);
        void GenerateReviewCountByProduct(string settingsKey);
        void GenerateCodeCommentDensityUploaded(string settingsKey);
        void GenerateReviewCountByEmployeeOfProduct();
    }
}
