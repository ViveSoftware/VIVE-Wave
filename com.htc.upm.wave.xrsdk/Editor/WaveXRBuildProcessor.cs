using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;
using Wave.XR.Settings;

namespace Wave.XR
{
    public class WaveXRBuildProcessor : IPostGenerateGradleAndroidProject, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
		private const string FAKE_VERSION = "0.0.0";

		/// <summary>Override of <see cref="IPreprocessBuildWithReport"/> and <see cref="IPostprocessBuildWithReport"/></summary>
		public int callbackOrder
        {
            get { return 0; }
        }

        void CleanOldSettings()
        {
            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null)
                return;

            var oldSettings = from s in preloadedAssets
                              where s != null && s.GetType() == typeof(WaveXRSettings)
                              select s;

            if (oldSettings != null && oldSettings.Any())
            {
                var assets = preloadedAssets.ToList();
                foreach (var s in oldSettings)
                {
                    assets.Remove(s);
                }

                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPreprocessBuildWithReport"/></summary>
        /// <param name="report">Build report.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();

            WaveXRSettings settings = null;
            UnityEngine.Object obj = null;
            bool hasObj = EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out obj);
            if (obj == null || !(obj is WaveXRSettings))
            {
                if (hasObj)
                    EditorBuildSettings.RemoveConfigObject(Constants.k_SettingsKey);
                return;
            }

            settings = (WaveXRSettings)obj;

            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(settings))
            {
                var assets = preloadedAssets.ToList();
                assets.Add(settings);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPostprocessBuildWithReport"/></summary>
        /// <param name="report">Build report.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();
        }

        #region Handle Android Manifest

        private static class ManifestAttributeStringDefinition
        {
            public static class MetaDataName
            {
                public static readonly string SupportedFPS             = "com.htc.vr.content.SupportedFPS";
            }

            public static class FeatureName
            {
                public static readonly string HandTracking             = "wave.feature.handtracking";
                public static readonly string Tracker                  = "wave.feature.tracker";
                public static readonly string EyeTracking              = "wave.feature.eyetracking";
                public static readonly string LipExpression            = "wave.feature.lipexpression";
                public static readonly string ScenePerception          = "wave.feature.sceneperception";
                public static readonly string Marker                   = "wave.feature.marker";
                public static readonly string SimultaneousInteraction  = "wave.feature.simultaneous_interaction";
            }

            public static class PermissionName
            {
                public static readonly string SceneMesh                = "wave.permission.GET_SCENE_MESH";
            }
        }

        public void OnPostGenerateGradleAndroidProject(string path) //IPostGenerateGradleAndroidProject
        {
            if (!BuildCheck.CheckIfWaveEnabled.ViveWaveXRAndroidAssigned) return; //Do not modify android manifest if the XR Loader is not Wave

            var androidManifest = new AndroidManifest(GetManifestPath(path));

            WaveXRSettings waveXRSettings;
            EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out waveXRSettings);

            bool addHandTracking = false;
            bool addTracker = false;
            bool addEyeTracking = false;
            bool addLipExpression = false;
            bool addScenePerception = false;
            bool addSceneMesh = false;
            bool addMarker = false;
            bool addSupportedFPS = false;
            bool addSimultaneousInteraction = (EditorPrefs.GetBool(BuildCheck.CheckIfSimultaneousInteractionEnabled.MENU_NAME, false) && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.SimultaneousInteraction));

            if (waveXRSettings != null)
            {
                addSupportedFPS = waveXRSettings.supportedFPS != WaveXRSettings.SupportedFPS.HMD_Default && !checkWaveMetaData(_manifestFilePath, ManifestAttributeStringDefinition.MetaDataName.SupportedFPS);
                androidManifest.AddWaveMetaData(appendSupportedFPS: addSupportedFPS);

                addSceneMesh = waveXRSettings.EnableScenePerception && waveXRSettings.EnableSceneMesh && !checkWavePermission(_manifestFilePath, ManifestAttributeStringDefinition.PermissionName.SceneMesh);
                androidManifest.AddWavePermissions(appendSceneMeshPermission: addSceneMesh);
				androidManifest.AddViveSDKVersion();
				androidManifest.AddUnityVersion();

				addHandTracking = waveXRSettings.EnableNaturalHand && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.HandTracking);
                addTracker = waveXRSettings.EnableTracker && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.Tracker);
                addEyeTracking = waveXRSettings.EnableEyeTracking && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.EyeTracking);
                addLipExpression = waveXRSettings.EnableLipExpression && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.LipExpression);
                addScenePerception = waveXRSettings.EnableScenePerception && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.ScenePerception);
                addMarker = waveXRSettings.EnableMarker && !checkWaveFeature(_manifestFilePath, ManifestAttributeStringDefinition.FeatureName.Marker);

                androidManifest.AddWaveFeatures(
                    appendHandTracking: addHandTracking,
                    appendTracker: addTracker,
                    appendSimultaneousInteraction: addSimultaneousInteraction,
                    appendEyeTracking: addEyeTracking,
                    appendLipExpression: addLipExpression,
                    appendScenePerception: addScenePerception,
                    appendMarker: addMarker);

                androidManifest.Save();
            }
            else
            {
                androidManifest.AddWaveMetaData(appendSupportedFPS: addSupportedFPS);
                androidManifest.AddWavePermissions(appendSceneMeshPermission: addSceneMesh);
				androidManifest.AddViveSDKVersion();
				androidManifest.AddUnityVersion();
				androidManifest.AddWaveFeatures(
                    appendHandTracking: addHandTracking,
                    appendTracker: addTracker,
                    appendSimultaneousInteraction: addSimultaneousInteraction,
                    appendEyeTracking: addEyeTracking,
                    appendLipExpression: addLipExpression,
                    appendScenePerception: addScenePerception,
                    appendMarker: addMarker);
            }
        }

        private string _manifestFilePath;
        private string GetManifestPath(string basePath)
        {
            if (!string.IsNullOrEmpty(_manifestFilePath)) return _manifestFilePath;

            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();

            return _manifestFilePath;
        }

        static bool checkWaveFeature(string filename, string featureName)
		{
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNodeList featureNodeList = doc.SelectNodes("/manifest/uses-feature");

            if (featureNodeList != null)
            {
                foreach (XmlNode featureNode in featureNodeList)
                {
                    var androidNameAttribute = featureNode.Attributes["android:name"];
                    var androidRequiredAttribute = featureNode.Attributes["android:required"];

                    string name = null, required = null;

                    if (androidNameAttribute != null ) name = androidNameAttribute.Value;
                    if (androidRequiredAttribute != null) required = androidRequiredAttribute.Value;

                    if (name != null && name.Equals(featureName))
                        return true;
                }
            }
            return false;
        }

        static bool checkWavePermission(string filename, string permissionName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNodeList permissionNodeList = doc.SelectNodes("/manifest/uses-permission");

            if (permissionNodeList != null)
            {
                foreach (XmlNode permissionNode in permissionNodeList)
                {
                    var androidNameAttribute = permissionNode.Attributes["android:name"];
                    string name = null;

                    if (androidNameAttribute != null) name = androidNameAttribute.Value;

                    if (name != null && name.Equals(permissionName))
                        return true;
                }
            }
            return false;
        }

        static bool checkWaveMetaData(string filename, string metaDataName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/application/meta-data");

            if (metadataNodeList != null)
            {
                foreach (XmlNode metadataNode in metadataNodeList)
                {
                    var androidNameAttribute = metadataNode.Attributes["android:name"];
                    string name = null;

                    if (androidNameAttribute != null) name = androidNameAttribute.Value;

                    if (name != null && name.Equals(metaDataName))
                        return true;
                }
            }
            return false;
        }

        private class AndroidXmlDocument : XmlDocument
        {
            private string m_Path;
            protected XmlNamespaceManager nsMgr;
            public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

            public AndroidXmlDocument(string path)
            {
                m_Path = path;
                using (var reader = new XmlTextReader(m_Path))
                {
                    reader.Read();
                    Load(reader);
                }

                nsMgr = new XmlNamespaceManager(NameTable);
                nsMgr.AddNamespace("android", AndroidXmlNamespace);
            }

            public string Save()
            {
                return SaveAs(m_Path);
            }

            public string SaveAs(string path)
            {
                using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
                {
                    writer.Formatting = Formatting.Indented;
                    Save(writer);
                }

                return path;
            }
        }

        private class AndroidManifest : AndroidXmlDocument
        {
            private readonly XmlElement ManifestElement;
            private readonly XmlElement ApplicationElement;

            public AndroidManifest(string path) : base(path)
            {
                ManifestElement = SelectSingleNode("/manifest") as XmlElement;
                ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
            }

            private XmlAttribute CreateAndroidAttribute(string key, string value)
            {
                XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
                attr.Value = value;
                return attr;
            }

			private static string SearchPackageVersion(string packageName)
			{
				var listRequest = Client.List(true);
				do
				{
					if (listRequest.IsCompleted)
					{
						if (listRequest.Result == null)
						{
							Debug.Log("List result: is empty");
							return FAKE_VERSION;
						}

						foreach (var pi in listRequest.Result)
						{
							//Debug.Log("List has: " + pi.name + " == " + packageName);
							if (pi.name == packageName)
							{
								Debug.Log("Found " + packageName);

								return pi.version;
							}
						}
						break;
					}
					Thread.Sleep(100);
				} while (true);
				return FAKE_VERSION;
			}

			internal void AddViveSDKVersion()
			{
				var newUsesFeature = CreateElement("meta-data");
				newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", "com.htc.ViveWaveAndroid.SdkVersion"));
				newUsesFeature.Attributes.Append(CreateAndroidAttribute("value", SearchPackageVersion("com.htc.upm.wave.xrsdk")));
				ApplicationElement.AppendChild(newUsesFeature);
			}

			internal void AddUnityVersion()
			{
				var newUsesFeature = CreateElement("meta-data");
				newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", "com.htc.vr.content.UnityVersion"));
				newUsesFeature.Attributes.Append(CreateAndroidAttribute("value", Application.unityVersion));
				ApplicationElement.AppendChild(newUsesFeature);
			}

			internal void AddWavePermissions(bool appendSceneMeshPermission = false)
            {
                if (appendSceneMeshPermission)
                {
                    var sceneMeshPermissionElement = ManifestElement.AppendChild(CreateElement("uses-permission"));
                    sceneMeshPermissionElement.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.PermissionName.SceneMesh));
                }

            }

            internal void AddWaveMetaData(bool appendSupportedFPS = false)
            {
                if (appendSupportedFPS)
                {
                    var supportedFPSMetaDataElement = CreateElement("meta-data");
                    supportedFPSMetaDataElement.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.MetaDataName.SupportedFPS));
                    supportedFPSMetaDataElement.Attributes.Append(CreateAndroidAttribute("value", "120"));
                    ApplicationElement.AppendChild(supportedFPSMetaDataElement);
                }

            }

            internal void AddWaveFeatures(bool appendHandTracking = false,
                                          bool appendTracker = false,
                                          bool appendSimultaneousInteraction = false,
                                          bool appendEyeTracking = false,
                                          bool appendLipExpression = false,
                                          bool appendScenePerception = false,
                                          bool appendMarker = false)
            {
                if (appendHandTracking)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.HandTracking));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendTracker)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.Tracker));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendSimultaneousInteraction)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.SimultaneousInteraction));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendEyeTracking)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.EyeTracking));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendLipExpression)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.LipExpression));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendScenePerception)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.ScenePerception));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
                if (appendMarker)
                {
                    var newUsesFeature = CreateElement("uses-feature");
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", ManifestAttributeStringDefinition.FeatureName.Marker));
                    newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
                    ManifestElement.AppendChild(newUsesFeature);
                }
            }
        }
		#endregion
	}
}