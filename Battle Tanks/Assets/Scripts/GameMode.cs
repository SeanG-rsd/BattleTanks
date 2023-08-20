using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "NutronLabs/Photon/Game Mode", fileName = "gameMode")]
public class GameMode : ScriptableObject
{
    [SerializeField] private string modeName;
    [SerializeField] private int maxPlayers;
    [SerializeField] private bool hasTeams;
    [SerializeField] private int teamSize;
    [SerializeField] private bool hasFlags;
    [SerializeField] private bool hasZones;
    [SerializeField] private bool hasHearts;

    public string Name
    {
        get { return name; }
        private set { name = value; }
    }

    public int MaxPlayers
    {
        get { return maxPlayers; }
        private set { maxPlayers = value; }
    }

    public bool HasTeams
    {
        get { return hasTeams; }
        private set { hasTeams = value; }
    }

    public int TeamSize
    {
        get { return teamSize; }
        private set { teamSize = value; }
    }

    public bool HasFlag
    {
        get { return hasFlags; }
        private set { hasFlags = value; }
    }

    public bool HasZones
    {
        get { return hasZones; }
        private set { hasZones = value; }
    }

    public bool HasHearts
    {
        get { return hasHearts; }
        private set { hasHearts = value; }
    }
}
