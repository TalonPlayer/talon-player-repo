#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoExtractFBXMaterials : EditorWindow
{
    private string modelFolder = "Assets/Models"; // Folder where FBXs are
    private string outputMaterialFolder = "Assets/ExtractedMaterials"; // Where to save .mat files

    [MenuItem("Tools/Auto Extract FBX Materials")]
    public static void ShowWindow()
    {
        GetWindow<AutoExtractFBXMaterials>("FBX Material Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Extract All FBX Materials", EditorStyles.boldLabel);
        modelFolder = EditorGUILayout.TextField("FBX Folder", modelFolder);
        outputMaterialFolder = EditorGUILayout.TextField("Material Output Folder", outputMaterialFolder);

        if (GUILayout.Button("Extract Materials"))
        {
            ExtractAllMaterials();
        }
    }

    void ExtractAllMaterials()
    {
        string[] fbxGuids = AssetDatabase.FindAssets("t:Model", new[] { modelFolder });

        if (!Directory.Exists(outputMaterialFolder))
        {
            Directory.CreateDirectory(outputMaterialFolder);
        }

        foreach (string guid in fbxGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

            if (modelImporter == null)
            {
                Debug.LogWarning($"Skipping non-model asset: {path}");
                continue;
            }

            modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            modelImporter.materialLocation = ModelImporterMaterialLocation.External;
            modelImporter.SaveAndReimport();

            string modelName = Path.GetFileNameWithoutExtension(path);
            string matFolderPath = Path.Combine(outputMaterialFolder, modelName);

            if (!Directory.Exists(matFolderPath))
                Directory.CreateDirectory(matFolderPath);

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (Object asset in assets)
            {
                if (asset is Material mat)
                {
                    string matPath = Path.Combine(matFolderPath, mat.name + ".mat").Replace("\\", "/");
                    if (!File.Exists(matPath))
                    {
                        Material newMat = new Material(mat);
                        AssetDatabase.CreateAsset(newMat, matPath);
                        Debug.Log($"Extracted material: {mat.name} → {matPath}");
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All FBX materials extracted successfully.");
    }
}
#endif