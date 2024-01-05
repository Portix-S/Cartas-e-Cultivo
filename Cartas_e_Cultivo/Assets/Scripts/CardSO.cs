//using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject {

    public int cardID;

    public string cardName;
    public string description;

    public Sprite mask;
    public Sprite artwork;
    public Sprite imagemDaCarta;

    public int manaCost;
    public int growthTime;
    public int health;

    public bool hasOnDraw;
    public bool hasOnPlay;
    public bool hasOnDie;
    

    public void onDraw() {
        if(hasOnDraw) {
            FunctionManager.onDrawFunctions[cardID]?.Invoke();
        }
    }

    public void onPlay() {
        if(hasOnPlay) {
            FunctionManager.onPlayFunctions[cardID]?.Invoke();
        }
    }

    public void onDie() {
        if(hasOnDie) {
            FunctionManager.onDieFunctions[cardID]?.Invoke();
        }
    }

}
