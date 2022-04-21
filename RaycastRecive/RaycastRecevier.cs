using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastRecevier : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick (PointerEventData eventData) => Debug.Log( "<color=green>Clicked Cube</color>" );
}
