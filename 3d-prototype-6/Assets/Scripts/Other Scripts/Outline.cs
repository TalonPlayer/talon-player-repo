using UnityEngine;
using UnityEngine.EventSystems;
using LineworkLite.Common.Attributes;
using System.Linq;
using System.Collections.Generic;

public class Outline : MonoBehaviour
{
    [SerializeField][RenderingLayerMask] private int outlineLayer;
    public List<Renderer> renderers;

    private List<uint> originalLayers = new List<uint>();

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
        renderers = renderers.FindAll(r => r != null);

        originalLayers.Clear();
        for (int i = 0; i < renderers.Count; i++)
            originalLayers.Add(renderers[i].renderingLayerMask);
    }

    private void SetOutline(bool enable)
    {
        if (renderers == null || renderers.Count == 0) return;

        int outlineBit = (int)Mathf.Log(outlineLayer, 2);
        uint outlineMask = 1u << outlineBit;

        for (int i = 0; i < renderers.Count; i++)
        {
            var rend = renderers[i];
            if (rend == null) continue;

            uint original = (i < originalLayers.Count) ? originalLayers[i] : rend.renderingLayerMask;

            rend.renderingLayerMask = enable
                ? (original | outlineMask)
                : original;
        }
    }

    public void SetOutlineActive(bool enable)
    {
        if (renderers != null)
            renderers = renderers.FindAll(r => r != null);

        SetOutline(enable);
    }

    void OnDisable()
    {
        SetOutline(false);
    }

    void OnDestroy()
    {
        SetOutline(false);
    }
}
