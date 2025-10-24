using System.Collections.Generic;
using UnityEngine;

public class NameBank : MonoBehaviour
{
    [SerializeField] public List<string> firstNames = new List<string>();
    [SerializeField] private TextAsset namesSource; // drop your .txt here

    // Script: NameBank.cs
    [ContextMenu("Import From TextAsset")]
    private void ImportFromText()  // new method in NameBank.cs
    {
        if (namesSource == null) return;
        string raw = namesSource.text;
        var parts = raw.Split(new[] {',','\n','\r','\t'}, System.StringSplitOptions.RemoveEmptyEntries);
        firstNames.Clear();
        foreach (var p in parts)
            firstNames.Add(p.Trim());
    }
}
