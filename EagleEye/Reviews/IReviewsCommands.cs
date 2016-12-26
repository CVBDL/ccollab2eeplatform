namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth(string settingskey);
        void GenerateReviewCountByProduct(string settingskey);
        void GenerateReviewCountByEmployeeOfProduct();
    }
}
