namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth();
        void GenerateReviewCountByProduct();
        void GenerateReviewCountForProduct(string productName, string settingsKey);
    }
}
