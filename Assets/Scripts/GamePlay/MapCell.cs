using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    public Vector2 position;
    [SerializeField] public GameObject center;
    [SerializeField] public List<GameObject> walls;
}
