using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlZone : MonoBehaviour
{
    private Dictionary<int, float> timesForTeam = new Dictionary<int, float>() { { 1, 0f }, { 2, 0f } };

    private List<int> scoresForEachteam = new List<int>() { 0, 0 };

    [SerializeField] private float timeForEachPoint;

    public static Action<int> OnTeamGainControlPoint = delegate { };
    private void Update()
    {
        for (int i = 0;  i < timesForTeam.Count; i++)
        {
            //Debug.Log($"Team {i + 1} has {(int)(timesForTeam[i + 1] / timeForEachPoint)} points.");
            if ((int)(timesForTeam[i + 1] / timeForEachPoint) > scoresForEachteam[i] && GetComponent<PhotonView>().IsMine)
            {
                scoresForEachteam[i]++;
                OnTeamGainControlPoint?.Invoke(i + 1);
                Debug.LogWarning("team gained a point");
            }
        }
    }
    public void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Tank>() != null)
        {
            int team = collision.gameObject.GetComponent<Tank>().teamIndex;

            float time = timesForTeam[team];

            time += Time.deltaTime;

            timesForTeam[team] = time;
        }
    }
}
