#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// https://wiki.unity3d.com/index.php/FindMissingScripts
// https://answers.unity.com/questions/15225/how-do-i-remove-null-components-ie-missingmono-scr.html

namespace AssetBundleTool
{
    // This tool can't remove "missing script" components
    public class FindGameObjectWithScript : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;
        static List<GameObject> UndoList = new List<GameObject>();

        public void OnGUI()
        {
            if (GUILayout.Button("Find GameObject with Scripts in selected GameObjects"))
            {
                FindInSelected();
            }

            if (UndoList.Count > 0)
            {
                GUILayout.Label("Check the result in Console.\nClick to choose GameObject.");
                if (GUILayout.Button("Remove found components"))
                    RemoveScriptComponent();
            }
        }

        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            UndoList.Clear();
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }

            foreach (var g in UndoList)
            {
                Debug.Log("Has script on " + g.name, g);
            }
        }

        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    UndoList.Add(g);
                    break;
                }
                else
                {
                    var componentNameSpace = components[i].GetType().Namespace;
                    if (string.IsNullOrEmpty(componentNameSpace))
                    {
                        // Found it
                        if (!UndoList.Contains(g))
                            UndoList.Add(g);
                        break;
                    }
                    else if (componentNameSpace.StartsWith("UnityEngine"))
                    {
                        continue;
                    }
                    else
                    {
                        // It have a namespace
                        if (!UndoList.Contains(g))
                            UndoList.Add(g);
                        break;
                    }
                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                FindInGO(childT.gameObject);
            }
        }

        private static void RemoveScriptComponent()
        {
            if (UndoList == null || UndoList.Count <= 0)
            {
                Debug.Log("Nothing in the list");
                return;
            }

            foreach (var g in UndoList)
            {
                Component[] components = g.GetComponents<Component>();

                Undo.RegisterCompleteObjectUndo(g, "RemoveScripts");

                // Track how many components we've removed
                int r = 0;
                // Iterate over all components
                for (int j = 0; j < components.Length; j++)
                {
                    bool removeIt = false;

                    // Check if the ref is null
                    if (components[j] == null)
                    {
                        continue;
                    }
                    else
                    {
                        var componentNameSpace = components[j].GetType().Namespace;
                        if (string.IsNullOrEmpty(componentNameSpace))
                            removeIt = true;
                        else if (!componentNameSpace.StartsWith("UnityEngine"))
                            removeIt = true;
                    }
                    if (removeIt)
                    {
                        Undo.DestroyObjectImmediate(components[j]);
                        r++;
                    }
                    EditorUtility.SetDirty(g);
                }
            }
            UndoList.Clear();
        }
    }
}
#endif
