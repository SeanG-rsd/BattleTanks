using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] private GameObject message;
    [SerializeField] private TMP_Text text;

    float timer = 0;
    public void TurnOn(float time, string message)
    {
        text.text = message;
        this.message.SetActive(true);
        timer = time;
    }

    private void Update()
    {
        if (this.message.activeSelf)
        {
            timer -= Time.deltaTime;
        }
        else if (timer < 0)
        {
            this.message.SetActive(false);
            text.text = "";
            timer = 0;
        }
    }
}
