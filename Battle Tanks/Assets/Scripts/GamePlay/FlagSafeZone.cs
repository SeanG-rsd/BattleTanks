using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSafeZone : MonoBehaviour
{
    [SerializeField] private int teamIndex;

    public static Action<int> OnTeamReachedSafeZone = delegate { };

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Tank>() != null)
            {
                if (collision.gameObject.GetComponent<Tank>().teamIndex == this.teamIndex && collision.gameObject.GetComponent<Tank>().myFlag != null)
                {
                    Debug.Log($"Team {teamIndex} has brought the opposing flag back!");
                    OnTeamReachedSafeZone?.Invoke(teamIndex);
                    collision.gameObject.GetComponent<Tank>().myFlag.GoHome();
                }
            }
        }
    }
}
