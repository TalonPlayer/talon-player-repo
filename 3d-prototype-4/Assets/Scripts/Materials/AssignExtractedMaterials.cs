#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssignExtractedMaterials : EditorWindow
{
    private GameObject modelRoot;
    private string materialsFolder = "Assets/ExtractedMaterials";

    [MenuItem("Tools/Assign Extracted Materials to Selected Model")]
    public static void ShowWindow()
    {
        GetWindow<AssignExtractedMaterials>("Assign Extracted Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Extracted Materials to Model", EditorStyles.boldLabel);

        modelRoot = (GameObject)EditorGUILayout.ObjectField("Model Root Object", modelRoot, typeof(GameObject), true);
        materialsFolder = EditorGUILayout.TextField("Extracted Materials Folder", materialsFolder);

        if (GUILayout.Button("Assign Materials"))
        {
            if (modelRoot == null)
            {
                EditorUtility.DisplayDialog("Missing Object", "Please assign a root GameObject.", "OK");
                return;
            }

            AssignMaterials();
        }
    }

    private void AssignMaterials()
    {
        Renderer[] renderers = modelRoot.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            Material[] originalMats = renderer.sharedMaterials;
            Material[] newMats = new Material[originalMats.Length];

            for (int i = 0; i < originalMats.Length; i++)
            {
                Material oldMat = originalMats[i];
                if (oldMat == null) continue;

                string cleanMatName = StripSuffixes(oldMat.name);
                string[] foundGuids = AssetDatabase.FindAssets(cleanMatName + " t:Material", new[] { materialsFolder });

                if (foundGuids.Length > 0)
                {
                    string matPath = AssetDatabase.GUIDToAssetPath(foundGuids[0]);
                    Material newMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    newMats[i] = newMat;
                    Debug.Log($"Reassigned: {renderer.name} → {newMat.name}");
                }
                else
                {
                    Debug.LogWarning($"Material not found for {cleanMatName}. Keeping original.");
                    newMats[i] = oldMat;
                }
            }

            renderer.sharedMaterials = newMats;
            EditorUtility.SetDirty(renderer);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Finished reassigning materials to model.");
    }

    /// <summary>
    /// Removes suffixes like _albedo, _normal, etc. to match base name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string StripSuffixes(string name)
    {
        string[] suffixes = { "_albedo", "_normal", "_metallic", "_AO", "_roughness", "_emission" };
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix))
                return name.Replace(suffix, "");
        }
        return name;
    }
}
#endif