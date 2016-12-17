namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdGenerateDefectCountByProduct;

        public DefectsManager(ICommand cmdGenerateDefectCountByProduct)
        {
            _cmdGenerateDefectCountByProduct = cmdGenerateDefectCountByProduct;
        }

        public void GenerateReviewCountByMonth()
        {
            _cmdGenerateDefectCountByProduct.Execute();
        }
    }
}
