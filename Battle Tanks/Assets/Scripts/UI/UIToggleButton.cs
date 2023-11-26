using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleButton : MonoBehaviour
{

    [SerializeField] private GameObject toggleObject;
    [SerializeField] private UITab tab;

    [SerializeField] private UITab otherTab;
    // Start is called before the first frame update
    public void ToggleObject()
    {
        if (tab.isOpen)
        {
            tab.CloseTab();
        }
        else
        {
            tab.OpenTab();
        }

        if (otherTab.isOpen)
        {
            otherTab.CloseTab();
        }
    }
}
