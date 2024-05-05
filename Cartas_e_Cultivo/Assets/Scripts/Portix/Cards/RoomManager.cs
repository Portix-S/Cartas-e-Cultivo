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
    private CardMovement _currentCardScript;
    bool _isShowingCardInfo;
    private TextMeshProUGUI _cardHealthIndicator;

    public bool GetIsEnemyRoom()
    {
        return isEnemyRoom;
    }
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
            _currentCardScript.ShowCard();
            _isShowingCardInfo = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentCards == 0 || this.CompareTag("Hand")) return;  // Shouldn't show cards with these conditions
        _isShowingCardInfo = !_isShowingCardInfo;
        _currentCardScript.ShowCard();
        _currentCardScript.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // If Something was dropped on this place on the board
        cardScript = eventData.pointerDrag.GetComponent<CardMovement>();
        if (this.CompareTag("Hand")) return;
        if (!cardScript.isOnHand) return;
        if (!cardScript.CanAffordMana()) return;
        if (!CanDropCards(cardScript)) return;
        Debug.Log(cardScript.CanBePlayedOnEnemyRoom() + " " + isEnemyRoom);
        cardScript.PlayCard(this);
    }


    private bool CanDropCards(CardMovement cardScript)
    {
        if (cardScript != null && currentCards < maxCards && cardScript.parentToReturnTo != this.transform
            && !cardScript.isAICard && !isEnemyRoom && cardScript.cardSO.IsPlantCard())
        {
            currentCards++;
            _currentCardScript = cardScript;
            return true;
        }

        if (cardScript != null && _currentCardScript != null && ((cardScript.CanBePlayedOnEnemyRoom() && isEnemyRoom) || (cardScript.CanBePlayedOnPlayerRoom() && !isEnemyRoom)))
            return true;

        
        return false;
    }

    public void RemoveCurrentCard()
    {
        currentCards--;
        _isShowingCardInfo = false;
        _cardHealthIndicator.gameObject.SetActive(false);
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
        return _currentCardScript;
    }

    public void SetCurrentCard(CardMovement card)
    {
        _currentCardScript = card;
    }

    public void PlayClone(GameObject card)
    {
        cardScript = card.GetComponent<CardMovement>();
        if(cardScript.isClone) return;
        
        GameObject newCard = Instantiate(card, transform);
        currentCards++;
        _currentCardScript = newCard.GetComponent<CardMovement>();
        _currentCardScript.isClone = true;
        _currentCardScript.PlayCard(this);
        _currentCardScript.GrowPlant();
    }
}
