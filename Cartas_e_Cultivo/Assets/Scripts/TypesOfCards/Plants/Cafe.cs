using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Café", menuName = "Café")]

public class Cafe : PlantCard {
    public override void OnGrowth(Animator anim,GameManager2 gm, RoomManager roomManager)
    {
        if(gm.IsPlayerTurn())
            gm.DrawCard();
        else
            gm.EnemyDrawCard(1);
        
        anim.SetInteger(ID, cardID);
    }
}
