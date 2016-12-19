namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth();
        void GenerateReviewCountByProduct();
        void GenerateReviewCountByEmployeeOfProduct();
    }
}
