#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class URPMaterialTextureAssigner : EditorWindow
{
    private GameObject rootObject;
    private string textureFolder = "Assets/Textures";

    [MenuItem("Tools/URP Auto Assign Textures")]
    public static void ShowWindow()
    {
        GetWindow<URPMaterialTextureAssigner>("URP Texture Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Textures to URP Lit Materials", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);
        textureFolder = EditorGUILayout.TextField("Texture Folder", textureFolder);

        if (GUILayout.Button("Assign Textures"))
        {
            if (rootObject != null)
            {
                AssignTextures();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a root object.", "OK");
            }
        }
    }

    void AssignTextures()
    {
        Renderer[] renderers = rootObject.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;

                string matName = mat.name;

                Texture2D albedo = FindTexture($"{matName}_albedo");
                Texture2D normal = FindTexture($"{matName}_normal");
                Texture2D metallic = FindTexture($"{matName}_metallic");
                Texture2D ao = FindTexture($"{matName}_AO");

                // Confirm shader is URP Lit
                if (mat.shader.name != "Universal Render Pipeline/Lit")
                {
                    Debug.LogWarning($"Material {mat.name} does not use URP Lit Shader. Skipping.");
                    continue;
                }

                if (albedo) mat.SetTexture("_BaseMap", albedo);

                if (normal)
                {
                    mat.SetTexture("_BumpMap", normal);
                    mat.EnableKeyword("_NORMALMAP");
                }

                if (metallic)
                {
                    mat.SetTexture("_MetallicGlossMap", metallic);
                    mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                    mat.SetFloat("_Metallic", 1f);
                }

                if (ao)
                {
                    mat.SetTexture("_OcclusionMap", ao);
                    mat.SetFloat("_OcclusionStrength", 1f);
                }

                // Turns off emissions if enabled
                if (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor") == Color.white)
                {
                    Debug.LogWarning($"Disabling white emission on material: {mat.name}");
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }

                EditorUtility.SetDirty(mat);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("URP textures assigned successfully.");
    }

    Texture2D FindTexture(string name)
    {
        string[] guids = AssetDatabase.FindAssets(name, new[] { textureFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path).Equals(name))
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }

        return null;
    }
}
#endif