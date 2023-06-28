using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;

    public TMP_Text healthbarText;
    public GameObject healthbar;

    PhotonView view;
    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        
        

        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            healthbar.SetActive(true);
            currentHealth = maxHealth;
            playerPropeties["currentHealth"] = currentHealth;
            PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
            healthbarText.text = currentHealth.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            currentHealth = (int)playerPropeties["currentHealth"];
            healthbarText.text = currentHealth.ToString();
            
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int value)
    {
        playerPropeties["currentHealth"] = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }
}
