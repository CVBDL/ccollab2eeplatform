namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdGenerateDefectCountByProduct;
        ICommand _cmdGenerateDefectSeverityByProduct;

        public DefectsManager
        (
            ICommand cmdGenerateDefectCountByProduct,
            ICommand cmdGenerateDefectSeverityByProduct
        )
        {
            _cmdGenerateDefectCountByProduct = cmdGenerateDefectCountByProduct;
            _cmdGenerateDefectSeverityByProduct = cmdGenerateDefectSeverityByProduct;
        }

        public void GenerateReviewCountByMonth()
        {
            _cmdGenerateDefectCountByProduct.Execute();
        }

        public void GenerateDefectSeverityByProduct()
        {
            _cmdGenerateDefectSeverityByProduct.Execute();
        }
    }
}
