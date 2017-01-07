namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct(string settingsKey);
        
        
        void GenerateDefectsDistributionByType(string settingsKey);


        void GenerateDefectCountByCreatorFromProduct();


        void GenerateDefectCountByInjectionStageFromProduct();


        void GenerateDefectCountByTypeFromProduct();


        void GenerateDefectCountOfTypeByCreatorFromProduct();


        void GenerateDefectSeverityCountByProduct(string settingsKey);


        void GenerateDefectSeverityCountByCreatorFromProduct();
    }
}
