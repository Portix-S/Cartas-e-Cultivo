using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Only for AI")]
    public bool isAICard;

    public Transform parentToReturnTo;
    private Transform newRoom;
    private Transform lastRoom;
    private Image image; // ???
    public LayoutElement layoutElement;
    
    [Header("Reference to Hierarchy")]
    [SerializeField] public CardSO cardSO;
    private GameManager2 gc;
    private Transform priorityLayerUI;
    public Image frame;
    public Image artwork;
    [SerializeField] private GameObject cardBack;

    [SerializeField] GameObject stats;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI growthTimeText;

    [Header("Card Configs")] [SerializeField]
    private string cardName;
    [SerializeField] private string cardDescription;
    [SerializeField] private Sprite cardFrame, cardArtwork;
    [SerializeField] private int manaCost;
    [SerializeField] private int maxGrowthLevel = 1;

    private Animator anim;
    public bool played;
    private int growthLevel = 0;
    private bool isPlantCard;
    public bool isOnHand;
    public bool isTurnedBack;
    private bool isShowingCardInfo;

    [Header("Health System")] 
    public bool isClone;
    private int _health;
    [SerializeField] int maxHealth;
    private TextMeshProUGUI _cardHealthIndicatorOnRoom;
    private static readonly int Tempo = Animator.StringToHash("TEMPO");

    private void Awake()
    {
        // Initial Configuration of a card
        gc = FindObjectOfType(typeof(GameManager2)) as GameManager2;
        anim = GetComponent<Animator>();
        //isOnHand = true; // Ser� usado mais pra frente
        isPlantCard = cardSO.IsPlantCard();
        nameText.text = cardName;
        descriptionText.text = cardDescription;
        frame.sprite = cardFrame;
        artwork.sprite = cardArtwork;
        manaCostText.text = manaCost.ToString();
        _health = maxHealth;
        priorityLayerUI = GameObject.Find("PriorityLayerUI").transform;
        if (isPlantCard)
        {
            healthText.text = maxHealth.ToString();
            growthTimeText.text = maxGrowthLevel.ToString();
        }

        if (!isAICard)
        {
            gc.OnPlayerTurnBegin += Gc_OnPlayerTurnBegin;
        }
        else
        {
            gc.OnEnemyTurnBegin += Gc_OnEnemyTurnBegin;
            //manaCostText.gameObject.SetActive(false);
        }
    }

    public void EnemySubscribe()
    {
        Debug.Log("Enemy Card subscribing to event EnemyTurnBegin");
        gc.OnEnemyTurnBegin += Gc_OnEnemyTurnBegin;
    }

    private void Gc_OnPlayerTurnBegin(object sender, System.EventArgs e)
    {
        if (played && growthLevel < maxGrowthLevel && !isAICard)
        {
            Debug.Log("Grow" + cardSO.cardName);
            growthLevel++;
            if (anim == null) return;

            anim.SetInteger(Tempo, maxGrowthLevel - growthLevel);
            if (growthLevel == maxGrowthLevel)
            {
                gc.cardsGrown++;
                Debug.Log("Full grow " + this.gameObject + anim + gc + newRoom.GetComponent<RoomManager>());

                // Funções OnGrowth() das cartas, localizacao temporaria
                // int value = (parentToReturnTo.name.Last() - '0') - 1;
                // int[] adj = gc.AdjacentFields(value);
                
                // Atualizar tudo pelo script especifico da planta
                cardSO.OnGrowth(anim, gc, newRoom.GetComponent<RoomManager>(), this.gameObject);
                
                // if (this.nameText.text == "Batata")
                // {
                //     this.gameObject.GetComponent<Animator>().SetInteger("ID", 0);
                //
                // }
                //
                // if (this.nameText.text == "Lirio")
                // {
                //     this.gameObject.GetComponent<Animator>().SetInteger("ID", 1);
                //     foreach (var t in adj)
                //     {
                //         gc.cardFields[t].health++;
                //         Debug.Log("Vida" + gc.cardFields[t].health);
                //     }
                // }
            }
        }
    }

    private void Gc_OnEnemyTurnBegin(object sender, System.EventArgs e)
    {
        Debug.Log("Enemy Turn Begin "  + played + growthLevel + maxGrowthLevel + isAICard);
        if(maxGrowthLevel == 0) return;
        if (played && growthLevel < maxGrowthLevel && isAICard)
        {
            Debug.Log("Grow enemy card" + cardSO.cardName + " level" + growthLevel + "out of" + maxGrowthLevel);
            growthLevel++;
            anim.SetInteger(Tempo, maxGrowthLevel - growthLevel);
            if (growthLevel == maxGrowthLevel)
            {
                gc.enemyCardsGrown++;
                cardSO.OnGrowth(anim, gc, newRoom.GetComponent<RoomManager>(), this.gameObject);
            }
            // artworkImage = grownSprite;
        }
    }

    // Pointer enter/exit lidar com visuais das salas?
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entrou");
        if(isOnHand)
            this.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Saiu");
        if(isOnHand)
            this.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        // As it begins to be dragged, saves the parent and tries to chang parent
        //if already played that card, it'll only be destroyed (if clicked, by now)
        if (!played && isOnHand)
        {
            // Debug.Log("OnBeginDrag");
            var parent = this.transform.parent;
            parentToReturnTo = parent;
            lastRoom = parent;
            this.transform.SetParent(priorityLayerUI);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            Debug.Log("Essa carta j� foi jogada");
        }
    }
    
    public bool CanBePlayedOnEnemyRoom()
    {
        return cardSO.GetCanBePlayedOnEnemyRoom();
    }
    
    public bool CanBePlayedOnPlayerRoom()
    {
        return cardSO.GetCanBePlayedOnPlayerRoom();
    }

    public void OnDrag(PointerEventData eventdata) // Perfect for Dragging
    {
        if (!played && isOnHand)
            this.transform.position = eventdata.position;
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        if(!isOnHand) return;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        if (!gc.canAffordMana(manaCost) && !played)
        {
            Debug.Log("N�o tem mana suficiente");
        }

        if (lastRoom == newRoom || newRoom == null)
        {
            this.transform.SetParent(lastRoom);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    // Room manager calls this function to play a card in that room
    public void PlayCard(RoomManager roomManager)
    {
        Debug.Log("PlayCard");
        // Change Room
        newRoom = roomManager.transform;
        this.transform.SetParent(newRoom);
        
        // Lose mana
        gc.LoseMana(manaCost);
        if(!isAICard)
            gc.playerPlayedCards.Add(this);
        else if(!gc.enemyPlayedCards.Contains(this))
            gc.enemyPlayedCards.Add(this);
        
        // Change Card State
        played = true;
        
        // Remove card from last room
        if (lastRoom != null)
        {
            RoomManager roomScript = lastRoom.GetComponent<RoomManager>();
            roomScript.currentCards--;
        }
        anim.SetTrigger("PLAYED");
        anim.SetInteger(Tempo, maxGrowthLevel);
        
        // Maybe do Something?
        // cardSO.OnPlay(anim);
        cardSO.OnPlay(roomManager);

        // Get Card Health Indicator
        if (cardSO.hasHealth)
        {
            _cardHealthIndicatorOnRoom = roomManager.GetCardHealthIndicator();
            _cardHealthIndicatorOnRoom.gameObject.SetActive(true);
            _cardHealthIndicatorOnRoom.text = _health.ToString();
        }

        // GetComponent<CanvasGroup>().blocksRaycasts = true;
        FindObjectOfType<AudioManager>().Play("cardThrown");
        
        //provisorio -> para cartas de ação
        if(!cardSO.hasHealth)
            this.gameObject.SetActive(false);
    }
    
    public bool CanAffordMana()
    {
        return gc.canAffordMana(manaCost);
    }

    public void HideImage()
    {
        Color tempCor = image.color;
        tempCor.a = 0f;
        image.color = tempCor;
    }

    public void ShowImage()
    {
        Color tempCor = image.color;
        tempCor.a = 1f;
        image.color = tempCor;
    }
    
    public void onDraw()
    {
        cardSO.OnDraw();
    }

    public void onPlay()
    {
        cardSO.OnPlay(anim);
    }

    public void onDie()
    {
        cardSO.OnDie(anim);
    }

    public void TurnCard()
    {
        isTurnedBack = !isTurnedBack;
        cardBack.SetActive(isTurnedBack);
    }

    public void ShowCard()
    {
        isShowingCardInfo = !isShowingCardInfo;
        // anim.enabled = isShowingCardInfo ? false : true;
        // stats.SetActive(isShowingCardInfo);
        gc.ShowCardFullScreen(this.gameObject, stats);
        // Maybe show at the center of the screen, just like in Mulligan system,
        //maybe reuse the same code
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("Carta tem vida?" + cardSO.hasHealth + "jogada " + played);
        if (!cardSO.hasHealth || !played) return;
        
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            // Action to die
            cardSO.OnDie(anim, gc, newRoom.GetComponent<RoomManager>(), this.gameObject);

            Debug.Log("Carta morreu");
            gc.KillCard(this, newRoom.GetComponent<RoomManager>());
        }
        healthText.text = _health.ToString();
        _cardHealthIndicatorOnRoom.text = _health.ToString();
    }

    public void Heal(int amount)
    {
        if (!cardSO.hasHealth || !played) return;
        
        _health += amount;
        if (_health > maxHealth)
        {
            _health = maxHealth;
        }
        healthText.text = _health.ToString();
        _cardHealthIndicatorOnRoom.text = _health.ToString();
    }

    
    
    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(1);
        }
    }
#endif
}
