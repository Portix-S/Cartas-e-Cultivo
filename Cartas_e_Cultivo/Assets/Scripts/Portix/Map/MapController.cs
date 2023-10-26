using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    int currentAvailableMap = 0;
    [SerializeField] List<Button> mapButtons;
    [SerializeField] GameObject map;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            UnlockNext();   
        }
    }


    public void UnlockNext()
    {
        currentAvailableMap++;
        mapButtons[currentAvailableMap].interactable = true;
    }

    public void EnterMap(int id)
    {
        Debug.Log("<color=red>Abriu o mapa: </color>" + id);
        map.SetActive(false);
    }


}
