namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct(string settingsKey);
        void GenerateDefectCountBySeverity(string settingsKey);
        void GenerateDefectCountByInjectionStage(string settingsKey);
        void GenerateDefectCountByType(string settingsKey);
        void GenerateDefectsDistributionByType(string settingsKey);


        void GenerateDefectCountByCreatorFromProduct();
    }
}
