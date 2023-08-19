using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlZone : MonoBehaviour
{
    private Dictionary<int, float> timesForTeam = new Dictionary<int, float>() { { 1, 0f }, { 2, 0f } };

    [SerializeField] private float timeForEachPoint;
    private void Update()
    {
        for (int i = 0;  i < timesForTeam.Count; i++)
        {
            Debug.Log($"Team {i + 1} has {(int)(timesForTeam[i + 1] / timeForEachPoint)} points.");
        }
    }
    public void OnTriggerStay(Collider collision)
    {
        Debug.Log("stay");
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Tank>() != null)
        {
            int team = collision.gameObject.GetComponent<Tank>().teamIndex;

            float time = timesForTeam[team];

            time += Time.deltaTime;

            timesForTeam[team] = time;
        }
    }
}
