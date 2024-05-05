using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Limao", menuName = "Limao")]

public class Limao : PlantCard {
    public override void OnGrowth(Animator anim,GameManager2 gm, RoomManager roomManager, GameObject card)
    {
        try
        {
            gm.GetRandomCard().TakeDamage(2);
        }
        catch (Exception e)
        {
            Debug.Log("No cards to take damage");
            throw;
        }
        anim.SetInteger(ID, cardID);
    }
    
}
