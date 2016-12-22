namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdGenerateDefectCountByProduct;
        ICommand _cmdGenerateDefectSeverityByProduct;
        ICommand _cmdGenerateDefectCountByInjectionStage;

        public DefectsManager
        (
            ICommand cmdGenerateDefectCountByProduct,
            ICommand cmdGenerateDefectSeverityByProduct,
            ICommand cmdGenerateDefectCountByInjectionStage
        )
        {
            _cmdGenerateDefectCountByProduct = cmdGenerateDefectCountByProduct;
            _cmdGenerateDefectSeverityByProduct = cmdGenerateDefectSeverityByProduct;
            _cmdGenerateDefectCountByInjectionStage = cmdGenerateDefectCountByInjectionStage;
        }

        public void GenerateDefectCountByProduct()
        {
            _cmdGenerateDefectCountByProduct.Execute();
        }

        public void GenerateDefectSeverityByProduct()
        {
            _cmdGenerateDefectSeverityByProduct.Execute();
        }

        public void GenerateDefectCountByInjectionStage()
        {
            _cmdGenerateDefectCountByInjectionStage.Execute();
        }
    }
}
