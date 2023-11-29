using System;
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

			if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARMv7) != 0)
			{
				throw new BuildFailedException("Wave SDK No Longer Support 32 bit.");
			}

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

            CheckSpectatorCamera360CaptureFeature(settings);
        }

        /// <summary>Override of <see cref="IPostprocessBuildWithReport"/></summary>
        /// <param name="report">Build report.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();
        }

        #region Spectator camera feature check and pre-processing

        void CheckSpectatorCamera360CaptureFeature(WaveXRSettings waveXRSettings)
        {
	        Debug.Log("Check Spectator Camera 360 Capture Feature");
	        
	        const string WaveXRSettingsNotFoundWarningMessage =
		        "WaveXRSettings is null. " +
		        "Please check or reinstall the WaveXR SDK.";
            
            const string WaveSpectatorCameraFeatureDisableMessage =
                "Spectator camera feature is not enabled. " +
                "No need to check related setting when building.";
	        
	        const string Wave360EnableButPlayerSetting360DisableWarningMessage =
		        "Attribute \"allowSpectatorCameraCapture360Image\" is enabled on the WaveXR Setting, " +
		        "but the \"enable360StereoCapture\" in the Unity Player Settings is closed. " +
		        "The above settings are mutually exclusive. Will close the 360 image capture feature on" +
		        "Wave XR Setting";
	        
	        #region Check Wave XR Setting is exist
	        
	        if (waveXRSettings == null)
	        {
		        Debug.LogWarning(WaveXRSettingsNotFoundWarningMessage);
		        
		        return;
	        }
	        
	        #endregion
	        
	        // If the spectator camera feature is not enabled,
	        // not need to run the logic related to 360 capture
	        if (waveXRSettings.allowSpectatorCamera is false)
	        {
		        Debug.Log(WaveSpectatorCameraFeatureDisableMessage);
		        return;
	        }
	        
	        #region Check 360 setting in Wave XR and Unity Player Setting are not mutually exclusive

	        if (waveXRSettings.allowSpectatorCameraCapture360Image &&
	            PlayerSettings.enable360StereoCapture is false)
	        {
		        Debug.LogWarning(Wave360EnableButPlayerSetting360DisableWarningMessage);
		        waveXRSettings.allowSpectatorCameraCapture360Image = false;
	        }

	        #endregion
        }

        #endregion

        #region Handle Android Manifest

        private static class ManifestAttributeStringDefinition
        {
            public static class MetaDataName
            {
                public static readonly string SupportedFPS             = "com.htc.vr.content.SupportedFPS";
                public static readonly string WaveCmdLine              = "com.htc.vr.content.WaveCmdLine";
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

            GetManifestPath(path);
            XmlDocument doc = new XmlDocument();
            doc.Load(_manifestFilePath);

            var androidManifest = new AndroidManifest(_manifestFilePath);

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
            bool addSimultaneousInteraction = (EditorPrefs.GetBool(BuildCheck.CheckIfSimultaneousInteractionEnabled.MENU_NAME, false) && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.SimultaneousInteraction));

            if (waveXRSettings != null)
            {
                addSupportedFPS = waveXRSettings.supportedFPS != WaveXRSettings.SupportedFPS.HMD_Default && !CheckWaveMetaData(doc, ManifestAttributeStringDefinition.MetaDataName.SupportedFPS);
                androidManifest.AddWaveMetaData(appendSupportedFPS: addSupportedFPS);

                addSceneMesh = waveXRSettings.EnableScenePerception && waveXRSettings.EnableSceneMesh && !CheckWavePermission(doc, ManifestAttributeStringDefinition.PermissionName.SceneMesh);
                androidManifest.AddWavePermissions(appendSceneMeshPermission: addSceneMesh);
				androidManifest.AddViveSDKVersion();
				androidManifest.AddUnityVersion();

				addHandTracking = (waveXRSettings.EnableNaturalHand || waveXRSettings.EnableElectronicHand) && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.HandTracking);
                addTracker = waveXRSettings.EnableTracker && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.Tracker);
                addEyeTracking = waveXRSettings.EnableEyeTracking && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.EyeTracking);
                addLipExpression = waveXRSettings.EnableLipExpression && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.LipExpression);
                addScenePerception = waveXRSettings.EnableScenePerception && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.ScenePerception);
                addMarker = waveXRSettings.EnableMarker && !CheckWaveFeature(doc, ManifestAttributeStringDefinition.FeatureName.Marker);

                androidManifest.AddWaveCmdLineMetaData(waveXRSettings);

                androidManifest.AddWaveFeatures(
                    appendHandTracking: addHandTracking,
                    appendTracker: addTracker,
                    appendSimultaneousInteraction: addSimultaneousInteraction,
                    appendEyeTracking: addEyeTracking,
                    appendLipExpression: addLipExpression,
                    appendScenePerception: addScenePerception,
                    appendMarker: addMarker);

                androidManifest.AddSkipPermissionDialogMetaData();
                
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

                androidManifest.AddSkipPermissionDialogMetaData();
                //androidManifest.Save();  // TODO why not save it?
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

        static bool CheckWaveFeature(XmlDocument doc, string featureName)
		{
            XmlNodeList featureNodeList = doc.SelectNodes("/manifest/uses-feature");

            if (featureNodeList == null) return false;
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
            return false;
        }

        static bool CheckWavePermission(XmlDocument doc, string permissionName)
        {
            XmlNodeList permissionNodeList = doc.SelectNodes("/manifest/uses-permission");

            if (permissionNodeList == null) return false;
            foreach (XmlNode permissionNode in permissionNodeList)
            {
                var androidNameAttribute = permissionNode.Attributes["android:name"];
                string name = null;

                if (androidNameAttribute != null) name = androidNameAttribute.Value;

                if (name != null && name.Equals(permissionName))
                    return true;
            }
            return false;
        }

        static bool CheckWaveMetaData(XmlDocument doc, string metaDataName)
        {
            XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/application/meta-data");

            if (metadataNodeList == null) return false;
            foreach (XmlNode metadataNode in metadataNodeList)
            {
                var androidNameAttribute = metadataNode.Attributes["android:name"];
                string name = null;

                if (androidNameAttribute != null) name = androidNameAttribute.Value;

                if (name != null && name.Equals(metaDataName))
                    return true;
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

        // You can check the the result in <project>\Temp\gradleOut\unityLibrary\src\main\AndroidManifest.xml
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

            // The function add commands into AndroidManifest.xml.  In Activity, the value will be added into command line.
            // Here is a command line example:
            //     adb shell am start -e unity "-platform-android-gfxdeviceworker-priority\ -20\ -platform-android-unitymain-priority\ -20" com.your.package/com.htc.vr.unity.WVRUnityVRActivity
            // See more information at these place
            //     https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html
            //     https://docs.unity3d.com/Manual/android-custom-activity-command-line.html
            //     https://docs.unity3d.com/Manual/android-thread-configuration.html
            internal void AddWaveCmdLineMetaData(WaveXRSettings settings)
            {
                StringBuilder sb = new StringBuilder();
                // Override Thread Priority
                if (settings.overrideThreadPriority)
                {
                    if (settings.gameThreadPriority != 0)
                    {
                        sb.Append(" -platform-android-unitymain-priority ");
                        sb.Append(settings.gameThreadPriority);
                    }
                    if (settings.renderThreadPriority != -2)
                    {
                        sb.Append(" -platform-android-gfxdeviceworker-priority ");
                        sb.Append(settings.renderThreadPriority);
                    }
                    if (settings.jobWorkerThreadPriority != 0)
                    {
                        sb.Append(" -platform-android-jobworker-priority ");
                        sb.Append(settings.jobWorkerThreadPriority);
                    }
                }

                var cmdLine = sb.ToString();
                cmdLine = cmdLine.Trim();
                if (string.IsNullOrEmpty(cmdLine)) return;

                var waveCmdLineElm = CreateElement("meta-data");
                waveCmdLineElm.Attributes.Append(CreateAndroidAttribute("name",
                    ManifestAttributeStringDefinition.MetaDataName.WaveCmdLine));
                waveCmdLineElm.Attributes.Append(CreateAndroidAttribute("value",
                    cmdLine));
                ApplicationElement.AppendChild(waveCmdLineElm);
            }

            // Add SkipPermissionDialog into the activity with meta-data "unityplayer.UnityActivity"
            //  <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
            //  <meta-data android:name="unityplayer.SkipPermissionsDialog" android:value="true" />
            // Generate the meta-data if it doesn't exist
            internal bool AddSkipPermissionDialogMetaData()
            {
                XmlNodeList activityNodes = SelectNodes("/manifest/application/activity");
                if (activityNodes == null) return false;

                bool foundUnityActivityMetaData = false;
                foreach (XmlNode activityNode in activityNodes)
                {
                    XmlNodeList childNodes = activityNode.ChildNodes;

                    foreach (XmlNode childNode in childNodes)
                    {
                        // Check Activity element if exist unityplayer.UnityActivity meta-data
                        if (childNode.Name == "meta-data" &&
                            childNode.Attributes["android:name"].Value == "unityplayer.UnityActivity" &&
                            childNode.Attributes["android:value"].Value == "true")
                        {
                            foundUnityActivityMetaData = true;

                            bool hasSkipPermissionsDialogMetaData = false;
                            foreach (XmlNode metaNode in childNodes)
                            {
                                if (metaNode.Name == "meta-data" &&
                                    metaNode.Attributes["android:name"].Value == "unityplayer.SkipPermissionsDialog" &&
                                    metaNode.Attributes["android:value"].Value == "true")
                                {
                                    hasSkipPermissionsDialogMetaData = true;
                                    break;
                                }
                            }

                            if (!hasSkipPermissionsDialogMetaData)
                            {
                                // Add unityplayer.SkipPermissionsDialog meta-data into the Activity element
                                XmlElement skipPermissionsDialogMetaDataElement = CreateElement("meta-data");
                                skipPermissionsDialogMetaDataElement.Attributes.Append(CreateAndroidAttribute("name", "unityplayer.SkipPermissionsDialog"));
                                skipPermissionsDialogMetaDataElement.Attributes.Append(CreateAndroidAttribute("value", "true"));
                                activityNode.AppendChild(skipPermissionsDialogMetaDataElement);
                            }

                            break;
                        }
                    }
                }

                // If not found unityplayer.UnityActivity meta-data?add unityplayer.SkipPermissionsDialog meta-data to all Activity element
                if (!foundUnityActivityMetaData)
                {
                    foreach (XmlNode activityNode in activityNodes)
                    {
                        XmlElement skipPermissionsDialogMetaDataElement = CreateElement("meta-data");
                        skipPermissionsDialogMetaDataElement.Attributes.Append(CreateAndroidAttribute("name", "unityplayer.SkipPermissionsDialog"));
                        skipPermissionsDialogMetaDataElement.Attributes.Append(CreateAndroidAttribute("value", "true"));
                        activityNode.AppendChild(skipPermissionsDialogMetaDataElement);
                    }
                }

                return true;
            }
        }
		#endregion
	}
}
