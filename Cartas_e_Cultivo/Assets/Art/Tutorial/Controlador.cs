using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    DialogueSystem dialogueSystem;
  
    public void Awake()
    {
       
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    public void Start(){
        dialogueSystem.Next();     
    }
    
    public void Update()
    {
        
       if(Input.GetKeyDown(KeyCode.E)){
            dialogueSystem.Next();
            StopAllCoroutines();
       }    
        
    }
}
