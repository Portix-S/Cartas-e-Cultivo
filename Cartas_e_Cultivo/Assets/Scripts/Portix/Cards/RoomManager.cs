using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomManager : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int maxCards = 1;
    public int currentCards = 0;

    private GameObject currentRoomPrefab;
    private bool isEnemyRoom;

    private void Awake()
    {
        if (this.CompareTag("EnemyRoom"))
            isEnemyRoom = true;
    }

    //[SerializeField] Transform roomWorldPositon;
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        // If Something was dropped on this place on the board
        CardMovement cardScript = eventData.pointerDrag.GetComponent<CardMovement>();
        if (!cardScript.CanAffordMana()) return;
        if (!CanDropCards(cardScript)) return;
        Debug.Log(cardScript.CanBePlayedOnEnemyRoom() + " " + isEnemyRoom);
        currentCards++;
        cardScript.PlayCard(this);
    }


    private bool CanDropCards(CardMovement cardScript)
    {
        if (cardScript != null && currentCards < maxCards && cardScript.parentToReturnTo != this.transform 
            && !cardScript.isAICard && !isEnemyRoom)
            return true;

        if (cardScript != null && cardScript.CanBePlayedOnEnemyRoom() && isEnemyRoom)
            return true;

        
        return false;
    }

    public int GetMaxCards()
    {
        return this.maxCards;
    }

    public GameObject GetRoomPrefab()
    {
        return this.currentRoomPrefab;
    }
}
