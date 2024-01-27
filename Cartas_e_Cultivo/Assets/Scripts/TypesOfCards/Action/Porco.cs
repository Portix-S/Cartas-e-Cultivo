//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Porco")]

public class Porco : ActionCard {
    public override void OnPlay(RoomManager room)
    {
        room.GetCardScript().TakeDamage(10);   

    }

    public override void OnPlay(Animator anim)
    {
        anim.SetTrigger("Dano");
    }
}
