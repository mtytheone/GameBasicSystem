namespace HatzeLaboratory.GameBasicSystem.Editor.Settings.Interface
{
    public interface ISettingProviderDrawer
    {
        string SectionTitle { get; }
        void Draw();
    }
}