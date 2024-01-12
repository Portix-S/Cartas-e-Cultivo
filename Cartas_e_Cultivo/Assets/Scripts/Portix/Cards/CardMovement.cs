using System.Linq;
using TMPro;
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
    public bool changedByDropZone;
    private Image image;
    public bool played;
    //[SerializeField] private GameObject roomPrefab;
    public LayoutElement layoutElement;
    //[SerializeField] private bool isOnHand;
    //[SerializeField] private bool isOnRoom;
    [Header("Passando Do Sona")]
    [SerializeField] public CardSO cardSO;
    private GameManager2 gc;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public Image maskImage;
    public Image artworkImage;


    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI healthText;
    public int health;
    public TextMeshProUGUI growthTimeText;


    private Animator anim;

    [Header("Growth Stats")]
    private int growthLevel = 0;
    [SerializeField] private int maxGrowthLevel = 1;

    
    private void Awake()
    {
        // Initial Configuration of a card
        gc = FindObjectOfType(typeof(GameManager2)) as GameManager2;
        anim = GetComponent<Animator>();
        //isOnHand = true; // Ser� usado mais pra frente
        // nameText.text = cardSO.cardName;
        // descriptionText.text = cardSO.description;
        // maskImage.sprite = cardSO.mask;
        // artworkImage.sprite = cardSO.artwork;
        // manaCostText.text = cardSO.manaCost.ToString();
        if (!isAICard)
        {
            gc.OnPlayerTurnBegin += Gc_OnPlayerTurnBegin;
        }
        else
        {
            gc.OnEnemyTurnBegin += Gc_OnEnemyTurnBegin;
            //manaCostText.gameObject.SetActive(false);
        }
        // healthText.text = cardSO.health.ToString();
        // health = int.Parse(healthText.text);
        //
        if (cardSO.hasGrowthTime)
        {
            maxGrowthLevel = cardSO.GetGrowthTime();
            // growthTimeText.text = cardSO.growthTime.ToString();
        }
    }

    private void Gc_OnPlayerTurnBegin(object sender, System.EventArgs e)
    {
        if (played && growthLevel < maxGrowthLevel && !isAICard)
        {
            Debug.Log("Grow" + cardSO.cardName);
            growthLevel++;
            if (growthLevel == maxGrowthLevel)
            {
                gc.cardsGrown++;
                Debug.Log("Grow");

                // Funções OnGrowth() das cartas, localizacao temporaria
                // int value = (parentToReturnTo.name.Last() - '0') - 1;
                // int[] adj = gc.AdjacentFields(value);
                
                // Atualizar tudo pelo script especifico da planta
                cardSO.OnGrowth(anim);
                
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
        if (played && growthLevel < maxGrowthLevel && isAICard)
        {
            Debug.Log("Grow enemy card" + cardSO.cardName + " level" + growthLevel + "out of" + maxGrowthLevel);
            growthLevel++;
            if (growthLevel == maxGrowthLevel)
            {
                gc.enemyCardsGrown++;
                cardSO.OnGrowth(anim);
            }
            // artworkImage = grownSprite;
        }
    }

    // Pointer enter/exit lidar com visuais das salas?
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entrou");
        this.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Saiu");
        this.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        // As it begins to be dragged, saves the parent and tries to chang parent
        //if already played that card, it'll only be destroyed (if clicked, by now)
        if (!played)
        {
            // Debug.Log("OnBeginDrag");
            var parent = this.transform.parent;
            parentToReturnTo = parent;
            lastRoom = parent;
            this.transform.SetParent(parent.parent);

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

    public void OnDrag(PointerEventData eventdata) // Perfect for Dragging
    {
        if (!played)
            this.transform.position = eventdata.position;
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        if (!gc.canAffordMana(cardSO.manaCost) && !played)
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
        gc.LoseMana(cardSO.manaCost);
        
        // Change Card State
        played = true;
        
        // Remove card from last room
        if (lastRoom != null)
        {
            RoomManager roomScript = lastRoom.GetComponent<RoomManager>();
            roomScript.currentCards--;
        }
        // Maybe do Something?
        cardSO.OnPlay();
        
        // GetComponent<CanvasGroup>().blocksRaycasts = true;
        FindObjectOfType<AudioManager>().Play("cardThrown");
        
        anim.SetTrigger("INICIO");
    }
    
    public bool CanAffordMana()
    {
        return gc.canAffordMana(cardSO.manaCost);
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
        cardSO.OnPlay();
    }

    public void onDie()
    {
        cardSO.OnDie();
    }

}
