//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class CardSO : ScriptableObject {

    public int cardID;
    
    public string cardName;
    public string description;

    public Sprite mask;
    public Sprite artwork;
    public Sprite imagemDaCarta;

    public int manaCost;
    
    public bool hasGrowthTime;
    public bool hasHealth;
    public bool hasOnDraw;
    public bool hasOnPlay;
    public bool hasOnDie;
    [SerializeField] protected bool canBePlayedOnEnemyRoom;
    [SerializeField] protected bool canBePlayedOnPlayerRoom;

    public bool GetCanBePlayedOnPlayerRoom()
    {
        return canBePlayedOnPlayerRoom;
    }
    public bool GetCanBePlayedOnEnemyRoom()
    {
        return canBePlayedOnEnemyRoom;
    }

    public abstract bool IsPlantCard();
    
    public abstract int GetGrowthTime();
    public abstract int GetHealth();
    public abstract void OnDraw();
    public abstract void OnGrowth(Animator anim);
    public abstract void OnPlay();
    public abstract void OnPlay(RoomManager room);
    public abstract void OnPlay(Animator anim);
    public abstract void OnDie();



}
