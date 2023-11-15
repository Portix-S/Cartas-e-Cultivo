using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionManager : MonoBehaviour {

    static public Dictionary<int, Action> onDrawFunctions = new Dictionary<int, Action>(); 
    static public Dictionary<int, Action> onPlayFunctions = new Dictionary<int, Action>(); 
    static public Dictionary<int, Action> onDieFunctions = new Dictionary<int, Action>(); 
    public GameController gc;

    void Start() {
        onDrawFunctions.Add(1, () => {PotatoOnDraw();});
        onPlayFunctions.Add(2, () => {PotatoOnPlay(); /*CallaLillyOnPlay();*/});
        onDieFunctions.Add(1, () => {PotatoOnDie();});
    }

    void DrawEvent(int amount, string reason) {
        if (amount > 1)
        {
            for (int i = 0; i < amount; i++)
                gc.DrawCard();
        }
        Debug.Log("Drew " + amount.ToString() + " because of " + reason + " event.");
    }

    // OnDraw Events
    // pls make it an alphabetic order
    // i beg you

    void PotatoOnDraw() {
        Console.Write("Does not does anything");
    }

    // OnPlay Events
    // pls make it an alphabetic order
    // i beg you 
    /* void CallaLillyOnPlay(int value) {

        int[] adj = gc.AdjacentFields(value);
        for(int i=0; i<3; i++) {
         
            gc.cardFields[adj[i]].health++;
            
        }
    } */
         
    void PotatoOnPlay() {
        Console.Write("Does not does anything");
    }

    // OnDie Events
    // pls make it an alphabetic order
    // i beg you

    void PotatoOnDie() {
        Console.Write("Does not does anything");
    }

    /* void WatermelonOnDie() {
           
    } */
}
