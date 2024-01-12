using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int currentText = -1;
    STATE state;
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
        typeText.fullText = dialogueData.talkScript[currentText++].text;
        if(currentText == dialogueData.talkScript.Count) finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void OnTypeFinished(){
        state = STATE.WAITING;
    }
    void Waiting(){
        if(!finished){
            Next();
        }
        else {
            state = STATE.DISABLED;
            currentText = 0;
            finished = false;
        }


    }

    void Typing(){

    }
}
