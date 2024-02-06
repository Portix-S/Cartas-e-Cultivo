//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "Chuva", menuName = "Chuva")]

public class Chuva : ActionCard {
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
