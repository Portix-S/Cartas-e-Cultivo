using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum STATE{
   DISABLED,
   WAITING,
   TYPING
}
public class DialogueSystem : MonoBehaviour
{
    public DialogueData dialogueData;
    bool finished = false;
    TypeTextAnimation typeText;
    int currentText = 0;
    STATE state; 

    public Image refe;

    private void Awake() {
        typeText = FindObjectOfType<TypeTextAnimation>();

        typeText.TypeFinished = OnTypeFinished;
    }
    void Start()
    {
        state = STATE.DISABLED;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == STATE.DISABLED) return;

        switch(state){
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;
        }

    }

   public void Next(){
        refe.color = Color.white;
        refe.sprite = dialogueData.talkScript[currentText].sprite;
        
        
        if(refe.sprite == null) refe.color = Color.clear;

        typeText.fullText = dialogueData.talkScript[currentText++].text;
        
        if(currentText == dialogueData.talkScript.Count) finished = true;
        

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void OnTypeFinished(){
        state = STATE.WAITING;
    }
    void Waiting(){
        if(Input.GetMouseButtonDown(0)){

        if(!finished){
            Next();
        }
        else {
            state = STATE.DISABLED;
            currentText = 0;
            finished = false;
        }


    }
}
    void Typing(){

    }
}
