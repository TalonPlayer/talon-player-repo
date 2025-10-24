using UnityEngine;  
using UnityEngine.EventSystems;  
using LineworkLite.Common.Attributes;
using System.Linq;
using System.Collections.Generic;

public class Outline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  
{  
    [SerializeField] [RenderingLayerMask] private int outlineLayer;  
    public List<Renderer> renderers;
    private uint originalLayer;
    public int originalLayerMask;
    private bool isOutlineActive;
  
    public void Init()  
    {
	    renderers = GetComponentsInChildren<Renderer>().ToList();
        originalLayer = renderers[0].renderingLayerMask;
        originalLayerMask = 6;
    }  
    
    public void OnPointerEnter(PointerEventData eventData)
    {        
        if (GameManager.Instance.cameraView == CameraView.EntityFacing) return;
        renderers = renderers.FindAll(r => r != null);
        isOutlineActive = true;
        SetOutline(true);  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        renderers = renderers.FindAll(r => r != null);
        isOutlineActive = false;
        SetOutline(false);
    }

    public void EnableUIOverlay()
    {
        foreach (Renderer renderer in renderers)
            renderer.gameObject.layer = 17;
    }
    
    public void DisableUIOverlay()
    {
        foreach (Renderer renderer in renderers)
            renderer.gameObject.layer = originalLayerMask;
    }
    

    
    private void SetOutline(bool enable)  
    {
        if (renderers.Count == 0 || renderers == null) return;
	    foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
            ? originalLayer | 1u << (int)Mathf.Log(outlineLayer, 2)
            : originalLayer;
        }    
	}
}