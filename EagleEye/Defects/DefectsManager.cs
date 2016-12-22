namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdGenerateDefectCountByProduct;
        ICommand _cmdGenerateDefectSeverityByProduct;
        ICommand _cmdGenerateDefectCountByInjectionStage;
        ICommand _cmdGenerateDefectCountByType;

        public DefectsManager
        (
            ICommand cmdGenerateDefectCountByProduct,
            ICommand cmdGenerateDefectSeverityByProduct,
            ICommand cmdGenerateDefectCountByInjectionStage,
            ICommand cmdGenerateDefectCountByType
        )
        {
            _cmdGenerateDefectCountByProduct = cmdGenerateDefectCountByProduct;
            _cmdGenerateDefectSeverityByProduct = cmdGenerateDefectSeverityByProduct;
            _cmdGenerateDefectCountByInjectionStage = cmdGenerateDefectCountByInjectionStage;
            _cmdGenerateDefectCountByType = cmdGenerateDefectCountByType;
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
    }
}
