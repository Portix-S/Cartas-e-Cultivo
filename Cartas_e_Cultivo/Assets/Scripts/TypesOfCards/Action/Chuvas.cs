//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "Chuvas", menuName = "Chuvaaa")]

public class Chuvas : ActionCard {
    public override void OnPlay(RoomManager room, GameManager2 gm)
    {
        gm.GetEnemyRooms(room).ForEach(card =>
        {
            card.ReduceGrowthTime(2);
        });
    }

    public override void OnPlay(Animator anim)
    {
        anim.SetTrigger("Dano");
    }
}
