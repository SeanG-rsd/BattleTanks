using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundController : MonoBehaviour
{

    [SerializeField] int value;

    public static Action<int, Button> OnNumberOFRoundsChanged = delegate { };

    public void OnClickRoundCount()
    {
        Debug.Log("set round number");
        OnNumberOFRoundsChanged?.Invoke(value, gameObject.GetComponent<Button>());
    }
}
