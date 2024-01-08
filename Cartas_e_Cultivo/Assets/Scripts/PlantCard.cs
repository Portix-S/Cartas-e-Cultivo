//using System;   
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Card", menuName = "PlantCard")]

public class PlantCard : CardSO {

    public bool isPlantCard = true;
    [SerializeField] int growthTime;
    [SerializeField] int health;
    private static readonly int ID = Animator.StringToHash("ID");

    public override int GetGrowthTime()
    {
        if (hasGrowthTime)
            return growthTime;
        else
            return -1;
    }
    
    public override int GetHealth()
    {
        return health;
    }

    public override void OnDraw()
    {
        
    }
    
    public override void OnGrowth(Animator anim)
    {
        anim.SetInteger(ID, cardID);
    }

    public override void OnPlay()
    {
        Debug.Log("Carta de Planta");
    }

    public override void OnDie()
    {
        
    }
    
}
