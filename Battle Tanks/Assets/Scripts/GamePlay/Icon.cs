using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        MiniMap miniMap = FindObjectOfType<MiniMap>();
        transform.SetParent(miniMap.miniMapContainer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
