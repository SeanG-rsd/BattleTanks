using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapIndicator : MonoBehaviour
{
    [SerializeField] private Image iconVisual;
    public GameObject icon;
    
    public void SetIcon(Sprite s, Color c, GameObject icon)
    {
        iconVisual.sprite = s;
        iconVisual.color = c;
        this.icon = icon;
    }
}
