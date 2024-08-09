using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlagHolder : MonoBehaviour
{
    [SerializeField] private int teamIndex;

    [SerializeField] public Flag thisFlag;

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collision");

        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<Tank>() != null)
        {
            if (collision.gameObject.GetComponent<Tank>().teamIndex != this.teamIndex)
            {
                Debug.Log("a tank has picked up the opposite flag!");
                thisFlag.SetTankToFollow(collision.gameObject.GetComponent<Tank>());
            }
        }
    }
}
