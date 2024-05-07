using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    DialogueSystem dialogueSystem;
    // Start is called before the first frame update
    public void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    // Update is called once per frame
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            dialogueSystem.Next();
            Debug.Log("Teste");
        }
    }
}
