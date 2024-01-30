using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Action Card", menuName = "Lirio")]

public class Lirio : PlantCard {
    public override void OnGrowth(Animator anim,GameManager2 gm, RoomManager roomManager, GameObject card)
    {
        gm.GetAdjacentRooms(roomManager).ForEach(room =>
        {
            CardMovement cardScript = room.GetCardScript();
            if(cardScript != null)
                cardScript.Heal(2);
        });
        anim.SetInteger(ID, cardID);
    }
}
