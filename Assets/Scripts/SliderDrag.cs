using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void EndSliderDragEventHandler();

[RequireComponent(typeof(Slider))]

public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public EndSliderDragEventHandler EndDrag;


    public void OnPointerUp(PointerEventData data)
    {
        if (EndDrag != null)
        {
            EndDrag();
        }
    }
}
