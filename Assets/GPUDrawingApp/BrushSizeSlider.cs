using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BrushSizeSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool isInUse;
    public Slider slider;
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isInUse = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isInUse = true;
    }
}