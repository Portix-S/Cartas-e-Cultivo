using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomManager : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private int maxCards = 1;
    public int currentCards;

    private GameObject currentRoomPrefab;
    private bool isEnemyRoom;
    private CardMovement cardScript;
    bool _isShowingCardInfo;
    private TextMeshProUGUI _cardHealthIndicator;
    private void Awake()
    {
        if (this.CompareTag("EnemyRoom"))
            isEnemyRoom = true;
        if (!this.CompareTag("Hand"))
        {
            _cardHealthIndicator = GetComponentInChildren<TextMeshProUGUI>();
            _cardHealthIndicator.gameObject.SetActive(false);
        }
    }
    
    //[SerializeField] Transform roomWorldPositon;
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isShowingCardInfo)
        {
            cardScript.ShowCard();
            _isShowingCardInfo = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentCards == 0 || isEnemyRoom || this.CompareTag("Hand")) return;  // Shouldn't show cards with these conditions
        _isShowingCardInfo = !_isShowingCardInfo;
        cardScript.ShowCard();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // If Something was dropped on this place on the board
        cardScript = eventData.pointerDrag.GetComponent<CardMovement>();
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
    
    public TextMeshProUGUI GetCardHealthIndicator()
    {
        return _cardHealthIndicator;
    }
    
    public CardMovement GetCardScript()
    {
        return cardScript;
    }
}
