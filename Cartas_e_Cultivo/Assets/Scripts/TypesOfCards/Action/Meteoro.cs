//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "Meteoro", menuName = "Meteoro")]

public class Meteoro : ActionCard {
    public override void OnPlay(RoomManager room, GameManager2 gm)
    {
        // ToList() prevents it removing when iterating
        gm.GetEnemyRooms(room).ToList().ForEach(card =>
        { 
            card.TakeDamage(2);
        });
    }

    public override void OnPlay(Animator anim)
    {
        // anim.SetTrigger("Dano");
    }
}
