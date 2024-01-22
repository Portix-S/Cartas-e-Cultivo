using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Action Card", menuName = "Lirio")]

public class Lirio : PlantCard {
    public override void OnGrowth(Animator anim,GameManager2 gm, RoomManager roomManager)
    {
        gm.GetAdjacentRooms(roomManager).ForEach(room =>
        {
            CardMovement card = room.GetCardScript();
            if(card != null)
                card.Heal(2);
        });
        anim.SetInteger(ID, cardID);
    }
}
