namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdGenerateDefectCountByProduct;
        ICommand _cmdGenerateDefectSeverityByProduct;
        ICommand _cmdGenerateDefectCountByInjectionStage;
        ICommand _cmdGenerateDefectCountByType;
        ICommand _cmdDefectsDistributionByType;

        public DefectsManager
        (
            ICommand cmdGenerateDefectCountByProduct,
            ICommand cmdGenerateDefectSeverityByProduct,
            ICommand cmdGenerateDefectCountByInjectionStage,
            ICommand cmdGenerateDefectCountByType,
            ICommand cmdDefectsDistributionByType
        )
        {
            _cmdGenerateDefectCountByProduct = cmdGenerateDefectCountByProduct;
            _cmdGenerateDefectSeverityByProduct = cmdGenerateDefectSeverityByProduct;
            _cmdGenerateDefectCountByInjectionStage = cmdGenerateDefectCountByInjectionStage;
            _cmdGenerateDefectCountByType = cmdGenerateDefectCountByType;
            _cmdDefectsDistributionByType = cmdDefectsDistributionByType;
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

        public void GenerateDefectCountByType()
        {
            _cmdGenerateDefectCountByType.Execute();
        }

        public void GenerateDefectsDistributionByType()
        {
            _cmdDefectsDistributionByType.Execute();
        }
    }
}
