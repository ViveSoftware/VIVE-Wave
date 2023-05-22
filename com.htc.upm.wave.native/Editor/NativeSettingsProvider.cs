using UnityEditor;

namespace Wave.Native
{
    internal class NativeSettingsProvider : SettingsProvider
    {
        public NativeSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope)
        {
        }

        public override void OnGUI(string searchContext)
        {
        }

        [SettingsProvider]
        static SettingsProvider Create()
        {
            return new NativeSettingsProvider("Project/Wave XR/Native");
        }
    }
}