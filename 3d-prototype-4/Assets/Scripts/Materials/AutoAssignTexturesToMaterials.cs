#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class AutoAssignTexturesToMaterials : EditorWindow
{
    private string materialsFolder = "Assets/ExtractedMaterials";
    private string texturesFolder = "Assets/Textures";

    [MenuItem("Tools/Auto Assign Textures to Materials")]
    public static void ShowWindow()
    {
        GetWindow<AutoAssignTexturesToMaterials>("Assign Textures to Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Textures to Extracted Materials", EditorStyles.boldLabel);
        materialsFolder = EditorGUILayout.TextField("Materials Folder", materialsFolder);
        texturesFolder = EditorGUILayout.TextField("Textures Folder", texturesFolder);

        if (GUILayout.Button("Assign Textures"))
        {
            AssignTexturesToMaterials();
        }
    }

    void AssignTexturesToMaterials()
    {
        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { materialsFolder });

        foreach (string matGuid in matGuids)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(matGuid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            string matName = mat.name.Replace("_albedo", "");

            AssignTextureToProperty(mat, matName + "_albedo", "_BaseMap");
            AssignTextureToProperty(mat, matName + "_normal", "_BumpMap", isNormalMap: true);
            AssignTextureToProperty(mat, matName + "_metallic", "_MetallicGlossMap");
            AssignTextureToProperty(mat, matName + "_AO", "_OcclusionMap");
            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All textures assigned to extracted materials.");
    }

    void AssignTextureToProperty(Material mat, string textureName, string propertyName, bool isNormalMap = false)
    {
        string[] textureGuids = AssetDatabase.FindAssets(textureName + " t:Texture2D", new[] { texturesFolder });

        if (textureGuids.Length > 0)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(textureGuids[0]);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            mat.SetTexture(propertyName, texture);

            if (isNormalMap)
            {
                // Tell Unity this is a normal map
                TextureImporter ti = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                if (ti != null && !ti.textureType.Equals(TextureImporterType.NormalMap))
                {
                    ti.textureType = TextureImporterType.NormalMap;
                    ti.SaveAndReimport();
                    Debug.Log($"Converted to normal map: {textureName}");
                }

                mat.EnableKeyword("_NORMALMAP");
            }

            Debug.Log($"Assigned '{textureName}' to '{mat.name}' → {propertyName}");
        }
        else
        {
            Debug.LogWarning($"Texture not found for '{mat.name}' → {textureName}");
        }
    }
}
#endif