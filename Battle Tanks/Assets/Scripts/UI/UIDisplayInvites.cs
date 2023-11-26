using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayInvites : MonoBehaviour
{

    [SerializeField] private Transform inviteContainer;
    [SerializeField] private UIInvite uiInvitePrefab;
    [SerializeField] private RectTransform contentArea;
    [SerializeField] private Vector2 originalSize;
    [SerializeField] private Vector2 increaseSize;

    [SerializeField] private GameObject counter;
    [SerializeField] private TMP_Text counterText;

    private List<UIInvite> invites;

    private void Awake()
    {
        invites = new List<UIInvite>();
        UpdateCounter();
        contentArea = inviteContainer.GetComponent<RectTransform>();
        originalSize = contentArea.sizeDelta;
        increaseSize = new Vector2(0, uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonChatController.OnRoomInvite += HandleRoomInvite;
        UIInvite.OnInviteAccept += HandleInviteAccept;
        UIInvite.OnInviteDecline += HandleInviteDecline;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnRoomInvite -= HandleRoomInvite;
        UIInvite.OnInviteAccept -= HandleInviteAccept;
        UIInvite.OnInviteDecline -= HandleInviteDecline;
    }

    private void HandleRoomInvite(string friend, string room)
    {
        Debug.Log($"Room invite for {friend} to room {room}");
        UIInvite uiInvite = Instantiate(uiInvitePrefab, inviteContainer);
        uiInvite.Initialize(friend, room);
        invites.Add(uiInvite);
        contentArea.sizeDelta += increaseSize;
        UpdateCounter();
    }

    private void HandleInviteAccept(UIInvite invite)
    {
        if (invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
            UpdateCounter();
        }
    }

    private void HandleInviteDecline(UIInvite invite)
    {
        if (invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
            UpdateCounter();
        }
    }

    private void UpdateCounter()
    {
        if (invites.Count > 0)
        {
            counter.SetActive(true);
            counterText.text = invites.Count.ToString();
        }
        else
        {
            counter.SetActive(false);
            counterText.text = "0";
        }
    }
}
