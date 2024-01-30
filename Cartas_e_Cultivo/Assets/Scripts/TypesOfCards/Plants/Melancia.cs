
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Melancia", menuName = "Melancia")]

public class Melancia : PlantCard {
    public override void OnDie(Animator anim,GameManager2 gm, RoomManager roomManager, GameObject card)
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
