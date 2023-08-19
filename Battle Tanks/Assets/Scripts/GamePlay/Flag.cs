using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public FlagHolder home;

    public bool isHeld = false;

    public Tank thisTank;

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            transform.position = thisTank.flagHolder.position;
        }
    }

    public void SetTankToFollow(Tank tank)
    {
        isHeld = true;
        thisTank = tank;
        tank.myFlag = this;
    }

    public void GoHome()
    {
        isHeld = false;
        if (thisTank != null)
        {
            thisTank.myFlag = null;
            thisTank = null;
        }

        transform.localPosition = Vector3.zero;
    }
}
