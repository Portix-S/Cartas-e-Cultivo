using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int maxCards = 1;
    public int currentCards = 0;
    private GameObject currentRoomPrefab;
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
        Draggable cardScript = eventData.pointerDrag.GetComponent<Draggable>();
        if (cardScript != null && currentCards < maxCards && cardScript.parentToReturnTo != this.transform && !cardScript.played && cardScript.CanAffordMana())
        {
            // Add a card to this room, changes entered card's new parent, tells this card that it changed rooms and that it has been played
            currentCards++;
            cardScript.parentToReturnTo = this.transform;
            cardScript.changedByDropZone = true;
            cardScript.played = true;

        }
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
