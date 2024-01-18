using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //
    // This script handles all drag systems of a card
    //

    [Header("Only for AI")]
    public bool isAICard = false;

    public Transform parentToReturnTo = null;
    private Transform lastRoom = null;
    public bool changedByDropZone;
    private Image image;
    public bool played;
    //[SerializeField] private GameObject roomPrefab;
    public LayoutElement layoutElement;
    //[SerializeField] private bool isOnHand;
    //[SerializeField] private bool isOnRoom;
    [Header("Passando Do Sona")]
    [SerializeField] public CardSO cardSO;
    private GameController gc;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public Image maskImage;
    public Image artworkImage;


    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI healthText;
    public int health;
    public TextMeshProUGUI growthTimeText;

   


    [Header("Growth Stats")]
    private int growthLevel = 0;
    [SerializeField] private int maxGrowthLevel = 1;

    private void Awake()
    {
        // Initial Congiguration of a card
        gc = FindObjectOfType(typeof(GameController)) as GameController;

        //isOnHand = true; // Ser� usado mais pra frente
        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.description;
        maskImage.sprite = cardSO.mask;
        artworkImage.sprite = cardSO.artwork;
        manaCostText.text = cardSO.manaCost.ToString();
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
        health = int.Parse(healthText.text);

        // maxGrowthLevel = cardSO.growthTime;
        // growthTimeText.text = cardSO.growthTime.ToString();
    }

    private void Gc_OnPlayerTurnBegin(object sender, System.EventArgs e)
    {   
        Debug.Log("Grow player card" + cardSO.cardName + " level" + growthLevel + "out of" + maxGrowthLevel);
        /*if(growthLevel >= 1) {
            this.gameObject.GetComponent<Animator>().SetTrigger("INIT4");
        }*/
        if (played && growthLevel < maxGrowthLevel) 
        {
            growthLevel++;  
            if(growthLevel == maxGrowthLevel) {
                gc.cardsGrown++;
                Debug.Log("Grow");

                // Funções OnGrowth() das cartas, localizacao temporaria
               int value = (parentToReturnTo.name.Last() - '0') - 1;
                int[] adj = gc.AdjacentFields(value); 
     
                if (this.nameText.text == "Lirio") {

                    for(int i=0; i<adj.Length; i++) 
                    {   
                        if (gc.cardFields[adj[i]] != null) {
                            gc.cardFields[adj[i]].health++; 
                             Debug.Log("Vida" + gc.cardFields[adj[i]].health);
                        }
                    }
                }
            }
        }
        // Animations();


    }

    private void Gc_OnEnemyTurnBegin(object sender, System.EventArgs e)
    {   
        if (played && growthLevel < maxGrowthLevel)
        {
            Debug.Log("Grow enemy card" + cardSO.cardName + " level" + growthLevel + "out of" + maxGrowthLevel);
            growthLevel++;
            if(growthLevel == maxGrowthLevel) gc.enemyCardsGrown++;

        }

        // Animations();

        
    }

    // Pointer enter/exit lidar com visuais das salas?
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entrou");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Saiu");
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        // As it begins to be dragged, saves the parent and tries to chang parent
        //if already played that card, it'll only be destroyed (if clicked, by now)
        if (!played)
        {
            Debug.Log("OnBeginDrag");
            parentToReturnTo = this.transform.parent;
            lastRoom = this.transform.parent;
            this.transform.SetParent(this.transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            Debug.Log("Essa carta j� foi jogada");
            //this.gameObject.SetActive(false);
            gc.graveyard.Add(this);
            // onDie();
            //DropZone roomScript = lastRoom.GetComponent<DropZone>();
            //roomScript.currentCards++;
        }
    }

    public void OnDrag(PointerEventData eventdata)
    {
        // Only makes the card to be glued to mouse position
        // Maybe add offset later
        if (!played)
        {
            this.transform.position = eventdata.position;
            Debug.Log("OnDrag");
        }
        
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        // As the card stopped being dragged, checks what to do
        Debug.Log("OnEndDrag");
        if(lastRoom != null)
            Debug.Log(parentToReturnTo.name);

        if (!gc.canAffordMana(cardSO.manaCost))
        {
            Debug.Log("N�o possui mana o suficiente");
        }

        // When it changes zone, get the room it was and remove a card from it
        if (changedByDropZone)
        {   
            //Debug.Log("-1");
            DropZone roomScript = lastRoom.GetComponent<DropZone>();
            roomScript.currentCards--;
            gc.PlayCard(this, parentToReturnTo.name);
            if(gc.canAffordMana(cardSO.manaCost))
                gc.loseMana(cardSO.manaCost);
        }

        // Changes the parent to new room and let raycast to work again
        this.transform.SetParent(parentToReturnTo);
        changedByDropZone = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;


        

        // Sistema de detec��o de salas, ser� usado no futuro
        /*
        if(parentToReturnTo.tag == "Room")
        {
            isOnRoom = true;
            isOnHand = false;
        }
        else if(parentToReturnTo.tag == "Hand")
        {
            isOnHand = true;
            isOnRoom = false;
        }
        //*/

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
    
    // public void onDraw()
    // {
    //     cardSO.onDraw();
    // }
    //
    // public void onPlay()
    // {
    //     cardSO.onPlay();
    // }
    //
    // public void onDie()
    // {
    //     cardSO.onDie();
    // }

    // public void Animations() {
    //     int timeforGrowth = maxGrowthLevel - growthLevel;
    //     if(played) {
    //        /* switch (timeforGrowth) {
    //             case 1:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INICIO"); //vamos precisar de um broto de uma folha só
    //                 break;
    //             case 2:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INICIO"); 
    //                 break;
    //             case 3:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INIT3");
    //                 break;
    //             case 4:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INIT4");
    //                 break;
    //             case 5:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INIT5");
    //                 break;
    //             default:
    //                 this.gameObject.GetComponent<Animator>().SetTrigger("INICIO");
    //                 break;
    //             
    //         }*/ 
    //     }    
    //     if(growthLevel == maxGrowthLevel) {
    //         if(this.nameText.text == "Batata") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 0);
    //         } 
    //         if (this.nameText.text == "Lirio") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 1);
    //
    //         }
    //         if (this.nameText.text == "Cafe") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 2);
    //
    //         }
    //         if (this.nameText.text == "Limao") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 4);
    //
    //         }
    //         if (this.nameText.text == "Melancia") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 5);
    //
    //         }
    //         if (this.nameText.text == "Abobora") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 7);
    //
    //         }
    //         if (this.nameText.text == "Acafrao") {
    //             this.gameObject.GetComponent<Animator>().SetInteger("ID", 8);
    //
    //         }
    //
    //     }
    //     
    // }
}
