namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct(string settingsKey);
        void GenerateDefectCountBySeverity(string settingsKey);
        
        void GenerateDefectsDistributionByType(string settingsKey);


        void GenerateDefectCountByCreatorFromProduct();


        void GenerateDefectCountByInjectionStageFromProduct();


        void GenerateDefectCountByTypeFromProduct();
    }
}
