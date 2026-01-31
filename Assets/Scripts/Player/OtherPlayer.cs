using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Player 
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        DownLoad(data);
    }
    void DownLoad(PlayerData data)
    {

    }
}
