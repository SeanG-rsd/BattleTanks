using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundController : MonoBehaviour
{

    [SerializeField] int value;
    [SerializeField] TMP_Text roundNumberText;

    public static Action<int, Button> OnNumberOFRoundsChanged = delegate { };
    // Start is called before the first frame update
    void Start()
    {
        roundNumberText.SetText(value.ToString());
    }

    public void OnClickRoundCount()
    {
        Debug.Log("set round number");
        OnNumberOFRoundsChanged?.Invoke(value, gameObject.GetComponent<Button>());
    }
}
