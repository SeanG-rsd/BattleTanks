using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleButton : MonoBehaviour
{

    [SerializeField] private GameObject toggleObject;
    // Start is called before the first frame update
    public void ToggleObject()
    {
        toggleObject.SetActive(!toggleObject.activeSelf);
    }
}
