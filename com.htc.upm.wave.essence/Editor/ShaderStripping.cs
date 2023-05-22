#if UNITY_2018_2_OR_NEWER && UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Wave.Essence
{
	[InitializeOnLoad]
	public static class ShaderStripping
	{
		private const string MenuItem = "Wave/Enable Shader Stripping";
		private const string EditorPrefEntry = "WaveShaderStripping";

		static ShaderStripping()
		{
			isShaderStrippingEnabled = EditorPrefs.GetBool(EditorPrefEntry, false);
		}

		static bool isShaderStrippingEnabled = false;

		[MenuItem(MenuItem, true)]
		private static bool SetShaderStrippingVaildate()
		{
			isShaderStrippingEnabled = EditorPrefs.GetBool(EditorPrefEntry, false);

#if URP_INSTALLED

			if (GraphicsSettings.renderPipelineAsset != null)
			{
				isShaderStrippingEnabled = false;
				EditorPrefs.SetBool(EditorPrefEntry, isShaderStrippingEnabled);
				Menu.SetChecked(MenuItem, isShaderStrippingEnabled);
				Debug.Log("Wave Essence Shader Stripping: Current Rendering Pipeline not supported.");
				return false;
			}

#endif
			Menu.SetChecked(MenuItem, isShaderStrippingEnabled);

			return true;
		}

		[MenuItem(MenuItem, false, 2)]
		private static void SetShaderStripping()
		{
			isShaderStrippingEnabled = !isShaderStrippingEnabled;
			Menu.SetChecked(MenuItem, isShaderStrippingEnabled);
			EditorPrefs.SetBool(EditorPrefEntry, isShaderStrippingEnabled);
		}

		private class ShaderStrippingBuildProcessor : IPreprocessShaders
		{
			public int callbackOrder { get { return 0; } }

			public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
			{
				isShaderStrippingEnabled = EditorPrefs.GetBool(EditorPrefEntry, false);

				if (!isShaderStrippingEnabled) return;

				if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
				{
					//Debug.Log("Wave Essence Shader Stripping: Stripping Shaders");
					List<GraphicsTier> stripTierList = new List<GraphicsTier>();
					stripTierList.Add(GraphicsTier.Tier1);
					stripTierList.Add(GraphicsTier.Tier3);

					for (int i = data.Count - 1; i >= 0; --i)
					{
						if (stripTierList.Contains(data[i].graphicsTier))
						{
							//Debug.Log("WaveEditorShaderStripping: Remove Shader at " + i);
							data.RemoveAt(i);
						}
					}
				}
			}
		}
	}
}
#endif
