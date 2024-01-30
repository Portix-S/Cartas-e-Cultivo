
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Abobora", menuName = "Abobora")]

public class Abobora : PlantCard {
    public override void OnGrowth(Animator anim,GameManager2 gm, RoomManager roomManager, GameObject card)
    {
        gm.GetAdjacentRooms(roomManager).ForEach(room =>
        {
            CardMovement cardScript = room.GetCardScript();
            if (cardScript == null)
                room.PlayClone(card);
        });
        anim.SetInteger(ID, cardID);
    }
}
