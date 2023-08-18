using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagHolder : MonoBehaviour
{
    [SerializeField] private int teamIndex;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Tank>().teamIndex == this.teamIndex)
            {
                Debug.Log("a tank has picked up the opposite flag!");
            }
        }
    }
}
