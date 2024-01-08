//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "New Action Card", menuName = "ActionCard")]

public class ActionCard : CardSO {

    public bool isActionCard = true;

    public override int GetGrowthTime()
    {
        return -1;
    }
    
    public override int GetHealth()
    {
        return -1;
    }

    public override void OnGrowth(Animator anim)
    {
    }
    
    public override void OnDraw()
    {
    }

    public override void OnPlay()
    {
        Debug.Log("Carta de Ação");
    }

    public override void OnDie()
    {
        
    }
}
