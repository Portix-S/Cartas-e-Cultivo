using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;


public class GameController : MonoBehaviour {

    public List<Draggable> deck = new List<Draggable>();
    public List<Draggable> graveyard = new List<Draggable>();
    public Transform[] handSlots;
    public bool[] availableSlots;

    

    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI manaCountText;
    public TextMeshProUGUI graveyardCountText;

   
    private int currentTurn = 0;
    private int mana = 1;
    private int growth = 0;
    private bool canPlayerDraw = true;
    private int numOfCardsToBeBought;
    [Header("Player Configs")]
    [SerializeField] private DropZone handScript;

   
    [Header("Turn Configs")]
    public bool playerTurn = true;
    public int maxCardsToDraw = 3;
    private static System.Random rng = new System.Random();

    [Header("Growth Stats")]
    [SerializeField] private bool stimulateGrowth;
    public event EventHandler OnPlayerTurnBegin;
    public int cardsGrown;
   
    [Header("Game Manager")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    public int cardsOnRooms;
    public List<Draggable> cardFields;

    public GameObject[] rooms;
    public int maxCardsOnRooms = 8;
    public bool isPlaying;
    public bool isOnMenu;


    public void Start() {
        ShuffleDeck();
    }

    public bool canAffordMana(int value) {
        return (value <= mana);
    }

    public void loseMana(int value) {
        mana = mana - value;
        if(mana < 0)
            mana = 0;
    } 

    public void gainMana(int value) {
        mana = mana + value;
        if(mana > 10)
            mana = 10;
    } 

    public void DrawClick() {
        if (canPlayerDraw)
        {
            DrawCard();
            canPlayerDraw = false;
        }
        else
        {
            if (!playerTurn)
                Debug.Log("Not your turn");
            else
                Debug.Log("Already bought once");
        }
    }

    public void DrawCard() {
        Debug.Log("Drawing");
        if (deck.Count >= 1)
        { // Se houverem cartas no deck:
        
                int numOfCardsInHand = handScript.currentCards;
                if (currentTurn == 0)
                {
                    numOfCardsToBeBought = 5;
                }
                else { 
                    numOfCardsToBeBought = 1;
                    }
            Debug.Log("Buying " + numOfCardsToBeBought + " cards");
            for (int i = 0; i < numOfCardsToBeBought; i++)
            {
                Draggable card = deck[0]; // Seleciona a primeira carta do deck
                handScript.currentCards++;
                card.gameObject.SetActive(true); // Dá visibilidade à carta comprada
                                                 //card.transform.position = handSlots[i].position; // Insere a carta no espaço correto
                                                 //card.currentSlot = i;
                //availableSlots[i] = false; // Avisa que o espaço agora está ocupado
                card.onDraw(); // Realiza evento ao comprar
                deck.Remove(card); // Remove carta comprada do deck
                FindObjectOfType<AudioManager>().Play("cardDrawn");  // plays cardDrawn sounds
                //return;
            }
        }
        else
            Debug.Log("Don't have any more cards left");
    }

    public void ShuffleDeck() {
        int n = deck.Count;
        Debug.Log("Shuffling");
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Draggable value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value;  
        }
    }

    public void EndTurn() {
        playerTurn = !playerTurn; // Can be changed to use with AI Player
        if(playerTurn)
        {
            currentTurn += 1;
            stimulateGrowth = true;
            canPlayerDraw = true;
            mana = (currentTurn + 1 > 10) ? 10 : currentTurn + 1;
            OnPlayerTurnBegin?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            canPlayerDraw = false;
            stimulateGrowth = false;
        }

        FindObjectOfType<AudioManager>().Play("buttonClick1"); // plays a button sound
        growth = (int) currentTurn / 2; 
    }

    private void Update() {
        deckCountText.text = deck.Count.ToString();
        manaCountText.text = mana.ToString();
        graveyardCountText.text = graveyard.Count.ToString();
        if(cardsGrown >= maxCardsOnRooms)
        {
            Debug.Log("Show Win");
            WinGame();
        }
    }

    public void PlayGame()
    {
        isOnMenu = false;
    }

    public void WinGame()
    {
        isPlaying = false;
        isOnMenu = true;
        winUI.SetActive(true);
    }

    public void ShowLoseUI()
    {
        isPlaying = false;
        isOnMenu = true;
        loseUI.SetActive(true);
    }

    public void PlayCard(Draggable card, string room)
    {
        int value = (room.Last() - '0') - 1;
        int[] adj = AdjacentFields(value);
         // int value = (room.Last().ParseInt());
        Debug.Log(value);
        cardFields[value] = card; 
        cardsOnRooms++; 
        FindObjectOfType<AudioManager>().Play("cardThrown");  // plays cardThrown sounds
        
        //"Funções" onPlay() das cartas, eventualmente mudar de cardName para cardID;
        
        //if(card.nameText.text == "Copo de Leite") {
            for(int i=0; i<adj.Length; i++) {
                cardFields[adj[i]].health++;
                Debug.Log("Cartas curadas?");
            }
        }
        if(card.nameText.text == "Cafe") {DrawCard();}
        // if (card.cardName == "Melancia") {for(int i=0; i<adj.length; i++) {PlayCard(melanciafilho, rooms[adj[i]].name);}}
            
    }

    public int[] AdjacentFields(int value) {
        // Jeito burro, posso mudar futuramente pra uma matriz
        // Sendo matriz os adjacentes seriam [i+1, j], [i, j+1], [i-1, j], [i, j-1], tendo q tratar pra ser NULL quando extrapolar os limites da matriz
        int[] adjacent = null;
        switch(value) {
            case 0:
                adjacent = new int[] {1,4};
                return adjacent;
            break;
            case 1:
                adjacent = new int[] {0,2,5};   
                return adjacent;
            break;
            case 2:
                adjacent = new int[] {1,3,6};
                return adjacent;
            break; 
            case 3:
                adjacent = new int[] {2,7};
                return adjacent;
            break; 
            case 4:
                adjacent =   new int[] {0,5};
                return adjacent;
            break; 
            case 5:
                adjacent = new int[] {4,6,1};
                return adjacent;
            break; 
            case 6:
                adjacent = new int[] {5,7,2};
                return adjacent;
            break; 
            case 7:
                adjacent = new int[] {6,3};
                return adjacent;
            break;
            default:
            return adjacent;

        } 
    }

}