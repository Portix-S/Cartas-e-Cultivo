using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager2 : MonoBehaviour
{
    [Header("Player Configs")]
    public List<Deck> deckPreset = new();
    public List<CardMovement> deck = new();
    public List<CardMovement> graveyard = new();
    [SerializeField] private List<RoomManager> playerRooms;
    public List<CardMovement> playerPlayedCards = new();
    
    // can be removed?
    public Transform[] handSlots;
    public bool[] availableSlots;

    // texts
    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI manaCountText;
    public TextMeshProUGUI graveyardCountText;
    [SerializeField] GameObject _cardFullScreen;
    public Animator animator;

    private int currentTurn;
    private int mana = 1;
    private int growth = 0;
    private bool canPlayerDraw = true;
    private int numOfCardsToBeBought;
    [SerializeField] private RoomManager handScript;

    [FormerlySerializedAs("enemyDeckPreset")] 
    [Header("Enemy AI Configs")]
    public List<Deck> enemyDeckPreset = new();
    public List<CardMovement> enemyCards = new();
    public List<CardMovement> enemyPlayableCards = new();
    public List<CardMovement> enemyPlayedCards = new();
    public List<CardMovement> enemyDeck = new(); 
    public List<CardMovement> enemyGraveyard = new();
    public Transform[] enemyHandSlots;
    public bool[] enemyAvailableSlots;
    public int enemyCardsInHand;
    [SerializeField] private int enemyMana = 1;
    public int enemyCardsGrown;
    public List<CardMovement> enemyCardFields;
    [SerializeField] public List<GameObject> enemyAvailableRooms;
    [SerializeField] private List<RoomManager> enemyRooms;
    [SerializeField] private RoomManager enemyHandScript;
    public bool checkingPlayableCards;


    [Header("Turn Configs")]
    public bool playerTurn = true;
    public int maxCardsToDraw = 3;
    private static System.Random rng = new();

    [Header("Growth Stats")]
    [SerializeField] private bool stimulateGrowth;
    public event EventHandler OnEnemyTurnBegin;
    public event EventHandler OnPlayerTurnBegin;
    public int cardsGrown;
   
    [Header("Game Manager")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    public int cardsOnRooms;
    public List<CardMovement> cardFields;

    public GameObject[] rooms;
    public int maxCardsOnRooms = 8;
    public bool isPlaying;
    public bool isOnMenu;

    [Header("Mulligan System")]
    [SerializeField] GameObject mulligan;
    [SerializeField] GameObject returnCardsButton;
    [SerializeField] private Button[] mulliganButtons;
    public Transform[] mulliganSlots;
    public List<Transform> mulliganFreeSlots;
    public List<CardMovement> cardsOnMulligan;
    public List<CardMovement> cardsToBeReturned;
    public CardMovement teste;
    private static readonly int Inicio = Animator.StringToHash("INICIO");

    public void Start() {
        // Adiciona todas as cartas ao deck e à mao
        //*
        foreach(Deck deck in deckPreset)
        {
            for(int i = 0; i < deck.amount; i++)
            {
                CardMovement card = Instantiate(deck.card, handScript.transform).GetComponent<CardMovement>();
                card.gameObject.SetActive(false);
                this.deck.Add(card);
            }
        }
        
        foreach(Deck deck in enemyDeckPreset)
        {
            for(int i = 0; i < deck.amount; i++)
            {
                CardMovement card = Instantiate(deck.card, enemyHandScript.transform).GetComponent<CardMovement>();
                card.isAICard = true;
                card.EnemySubscribe();
                card.gameObject.SetActive(false);                
                card.TurnCard();
                this.enemyDeck.Add(card);
            }
        }
        //*/

        ShuffleDeck(ref deck);
        ShuffleDeck(ref enemyDeck);
        MulliganSystem();
        EnemyDrawCard(5, false);
        UpdateMana();
        UpdateDeckCount();
    }

    public bool canAffordMana(int value) {
        return (value <= mana);
    }

    public void LoseMana(int value) {
        mana = mana - value;
        if(mana < 0)
            mana = 0;
        if(playerTurn)
            UpdateMana();
    } 

    public void gainMana(int value) {
        mana = mana + value;
        if(mana > 10)
            mana = 10;
    } 

    public bool IsPlayerTurn()
    {
        return playerTurn;
    }
    
    public void TryDraw() {
        if (canPlayerDraw)
        {
            DrawCard();
            canPlayerDraw = false;
        }
        else
        {
            Debug.Log(!playerTurn ? "Not your turn" : "Already bought once");
        }
    }

    public void DrawCard() {
        Debug.Log("Drawing");
        if (deck.Count >= 1)
        { // Se houverem cartas no deck:
        
            int numOfCardsInHand = handScript.currentCards;
            if(numOfCardsInHand >= handScript.GetMaxCards()) return;
            numOfCardsToBeBought = currentTurn == 0 ? 0 : 1;
            Debug.Log("Buying " + numOfCardsToBeBought + " cards");
            for (int i = 0; i < numOfCardsToBeBought; i++)
            {
                CardMovement card = deck[0]; // Seleciona a primeira carta do deck
                handScript.currentCards++;
                card.gameObject.SetActive(true); // Dá visibilidade à carta comprada
                                                 //card.transform.position = handSlots[i].position; // Insere a carta no espaço correto
                                                 //card.currentSlot = i;
                card.isOnHand = true;
                //availableSlots[i] = false; // Avisa que o espaço agora está ocupado
                // card.onDraw(); // Realiza evento ao comprar
                deck.Remove(card); // Remove carta comprada do deck
                FindObjectOfType<AudioManager>().Play("cardDrawn");  // plays cardDrawn sounds
                //return;
            }
            UpdateDeckCount();
        }
        else
            Debug.Log("Don't have any more cards left");
    }

    public void ShuffleDeck(ref List<CardMovement> deck) {
        int n = deck.Count;
        //Debug.Log("Shuffling");
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (deck[k], deck[n]) = (deck[n], deck[k]);
            /*
            CardMovement value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value; 
            //*/
            
        }
        Debug.Log("Shuffling");

    }

    public void EndTurn() {
        playerTurn = !playerTurn; // Can be changed to use with AI Player
        if(playerTurn)
        {
            currentTurn += 1;
            stimulateGrowth = true;
            canPlayerDraw = true;
            TryDraw();
            mana = (currentTurn + 1 > 10) ? 10 : currentTurn + 1;
            UpdateMana();
            OnPlayerTurnBegin?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            canPlayerDraw = false;
            stimulateGrowth = false;
            enemyMana = (currentTurn + 1 > 10) ? 10 : currentTurn + 1;
            EnemyDrawCard(1, false);
            OnEnemyTurnBegin?.Invoke(this, EventArgs.Empty);
        }

        FindObjectOfType<AudioManager>().Play("buttonClick1"); // plays a button sound
        growth = (int) currentTurn / 2; // Talvez esteja errado --> Ver depois
    }

    public void TryEndTurn()
    {
        if(playerTurn)
        {
            EndTurn();
        }
        else
        {
            Debug.Log("Can't end turn now");
        }
    }

    private void Update() {
        // deckCountText.text = deck.Count.ToString();
        // manaCountText.text = mana.ToString();
        // graveyardCountText.text = graveyard.Count.ToString();
        if(cardsGrown >= maxCardsOnRooms)
        {
            Debug.Log("Show Win");
            WinGame();
        }

        if(enemyCardsGrown >= maxCardsOnRooms)
        {
            Debug.Log("Show Lose");
            ShowLoseUI();
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
        Invoke("GameOverMenu", 3f);
    }

    public void ShowLoseUI()
    {
        isPlaying = false;
        isOnMenu = true;
        loseUI.SetActive(true);
        Invoke("GameOverMenu", 3f);
    }

    private void GameOverMenu()
    {
        SceneManager.LoadScene("GameOver");
    }

    // Not used anymore
    public void PlayCard(CardMovement card, string room)
    {
        int value = (room.Last() - '0') - 1;
        int[] adj = AdjacentFields(value);
         // int value = (room.Last().ParseInt());
        Debug.Log(value);
        cardFields[value] = card; 
        cardsOnRooms++; 
        FindObjectOfType<AudioManager>().Play("cardThrown");  // plays cardThrown sounds
        

        card.gameObject.GetComponent<Animator>().SetTrigger(Inicio);
        //"Funções" onPlay() das cartas, eventualmente mudar de cardName para cardID;
       
        if(card.nameText.text == "Cafe") {DrawCard();}
        // if (card.cardName == "Melancia") {for(int i=0; i<adj.length; i++) {PlayCard(melanciafilho, rooms[adj[i]].name);}}
            
    }

    private void UpdateMana()
    {
        manaCountText.text = mana.ToString();
    }

    private void UpdateDeckCount()
    {
        deckCountText.text = deck.Count.ToString();
    }

    public int[] AdjacentFields(int value) {
        // Jeito burro, posso mudar futuramente pra uma matriz
        // Sendo matriz os adjacentes seriam [i+1, j], [i, j+1], [i-1, j], [i, j-1], tendo q tratar pra ser NULL quando extrapolar os limites da matriz
        int[] adjacent = null;
        switch(value) {
            case 0:
                adjacent = new[] {1,4};
                return adjacent;
            case 1:
                adjacent = new[] {0,2,5};   
                return adjacent;
            case 2:
                adjacent = new[] {1,3,6};
                return adjacent;
            case 3:
                adjacent = new[] {2,7};
                return adjacent;
            case 4:
                adjacent =   new[] {0,5};
                return adjacent;
            case 5:
                adjacent = new[] {4,6,1};
                return adjacent;
            case 6:
                adjacent = new[] {5,7,2};
                return adjacent;
            case 7:
                adjacent = new[] {6,3};
                return adjacent;
            default:
            return adjacent;

        } 
    }
  
    public List<RoomManager> GetAdjacentRooms(RoomManager currentRoom)
    {
        if (enemyRooms.Contains(currentRoom))
        {
            Debug.Log("enemyRoom");
            return AddAdjacentsRooms(currentRoom, enemyRooms);
        }
        else if (playerRooms.Contains(currentRoom))
        {
            Debug.Log("playerRoom");
            return AddAdjacentsRooms(currentRoom, playerRooms);
        }
        return null;
    }

    private List<RoomManager> AddAdjacentsRooms(RoomManager currentRoom, List<RoomManager> listOfRooms)
    {
        List<RoomManager> adjacentRooms = new();
        int roomNumber = currentRoom.gameObject.name.Last() - '0';
        roomNumber--;
        Debug.Log("Room number to adjacents: " + roomNumber);
        if(roomNumber - 1 >= 0 && roomNumber - 1 != 3)
            adjacentRooms.Add(listOfRooms[roomNumber - 1]);
        if(roomNumber + 1 < listOfRooms.Count && roomNumber + 1 != 4)
            adjacentRooms.Add(listOfRooms[roomNumber + 1]);
        if(roomNumber + 4 < listOfRooms.Count)
            adjacentRooms.Add(listOfRooms[roomNumber + 4]);
        if(roomNumber - 4 >= 0)
            adjacentRooms.Add(listOfRooms[roomNumber - 4]);
        foreach (RoomManager room in adjacentRooms)
        {
            Debug.Log("Adjacent room to : " + roomNumber + ": " + room.gameObject.name);
        }
        
        return adjacentRooms;
    }

    public CardMovement GetRandomCard()
    {
        if (playerTurn)
        {
            int enemyCardCount = enemyPlayedCards.Count;
            if (enemyCardCount == 0) return null;
            int randomCard = UnityEngine.Random.Range(0, enemyCardCount);
            return enemyPlayedCards[randomCard];
        }
        else
        {
            int playerCardCount = playerPlayedCards.Count;
            if (playerCardCount == 0) return null;
            int randomCard = UnityEngine.Random.Range(0, playerCardCount);
            return playerPlayedCards[randomCard];
        }
    }

    public void MulliganSystem()
    {
        mulligan.SetActive(true);
        for(int i = 0; i < 5; i++)
        {
            DrawToMulligan(mulliganSlots[i]);
        }
    }

    private void SendCardsToHand()
    {
        for (int i = 0; i < 5; i++)
        {
            CardMovement card = cardsOnMulligan[0];
            Transform transform1;
            (transform1 = card.transform).SetParent(handScript.gameObject.transform);
            transform1.localScale = new Vector3(1f, 1f);
            handScript.currentCards++;
            card.isOnHand = true;
            cardsOnMulligan.Remove(card);
        }
        mulligan.SetActive(false);
    }

    public void SelectCard(GameObject test)
    {
        Button b = test.GetComponent<Button>();
        ColorBlock cb = b.colors;

        teste = test.gameObject.transform.parent.GetComponentInChildren<CardMovement>();
        if (cardsToBeReturned.Contains(teste))
        {
            cb.normalColor = Color.white;
            cardsToBeReturned.Remove(teste);
            mulliganFreeSlots.Remove(teste.transform.parent);
        }
        else
        {
            cb.normalColor = Color.red;
            cardsToBeReturned.Add(teste);
            mulliganFreeSlots.Add(teste.transform.parent); // Can save just one of them
        }
        cb.selectedColor = cb.normalColor;
        b.colors = cb;
    }

    public void ReturnCards()
    {
        foreach (Button button in mulliganButtons)
        {
            ColorBlock cb = button.colors;
            cb.normalColor = Color.white;
            button.colors = cb;
            button.gameObject.SetActive(false);
        }
        returnCardsButton.SetActive(false);
        int number = cardsToBeReturned.Count;
        for(int i = 0; i < number; i++)
        {
            CardMovement card = cardsToBeReturned[i];
            deck.Add(card);
            Transform transform1;
            (transform1 = card.transform).SetParent(handScript.transform);
            transform1.localScale = new Vector3(1f, 1f);
            card.gameObject.SetActive(false);
            Transform freeSlot = mulliganFreeSlots[i];
            DrawToMulligan(freeSlot);
            cardsOnMulligan.Remove(card);
        }
        ShuffleDeck(ref deck);
        for (int i = 0; i < number; i++)
        {
            Transform test = mulliganFreeSlots[0];
            cardsToBeReturned.Remove(cardsToBeReturned[0]);
            mulliganFreeSlots.Remove(test);
        }
        Invoke("SendCardsToHand", 3f);
    }

    private void DrawToMulligan(Transform mulliganPos)
    {
        CardMovement card = deck[0];
        cardsOnMulligan.Add(card);
        Transform transform1;
        (transform1 = card.transform).SetParent(mulliganPos);
        transform1.localScale = new Vector3(2.2f, 2.2f);
        transform1.localPosition = new Vector3(0f, 0f, 0f);
        card.gameObject.SetActive(true);
        card.isOnHand = false;
        deck.Remove(card);
    }

    public void ShowCardFullScreen(GameObject card, GameObject stats)
    {
        if (!mulligan.activeSelf)
        {
            mulligan.SetActive(true);
            //mulliganSlots[2]
            stats.SetActive(true);
            _cardFullScreen = Instantiate(card, mulliganSlots[2].transform);
            _cardFullScreen.transform.localPosition = Vector3.zero;
            _cardFullScreen.transform.localScale = new Vector3(2.2f, 2.2f);
        }
        else
        {
            mulligan.SetActive(false);
            if(_cardFullScreen != null)
                Destroy(_cardFullScreen);
        }        
    }
    
    public void EnemyDrawCard(int numOfCardsToBeBought, bool drawByAction)
    {
        if (enemyDeck.Count >= 1)
        { // Se houverem cartas no deck:
            Debug.Log("Buying " + numOfCardsToBeBought + " cards");
            if(enemyHandScript.currentCards >= enemyHandScript.GetMaxCards()) return;
            for (int i = 0; i < numOfCardsToBeBought; i++)
            {
                CardMovement card = enemyDeck[0]; // Seleciona a primeira carta do deck
                card.gameObject.SetActive(true); // Dá visibilidade à carta comprada -> mudar para gameObjects depois
                enemyCards.Add(card); // Adciona carta comprada à mão da IA
                //card.onDraw(); // Realiza evento ao comprar
                Debug.Log("Enemy drew " + card.cardSO.cardName);
                enemyDeck.Remove(card); // Remove carta comprada do deck
            }
            if(!drawByAction)
                CheckPlayableCards();
        }
        else
        {
            Debug.Log("Don't have any more cards left");
            // Player wins
            if(!playerTurn)
                EndTurn();
        }
    }

    private void CheckPlayableCards() // Checa quais cartas podem ser jogadas
    {
        enemyPlayableCards.Clear();
        foreach (CardMovement card in enemyCards)
        {
            if (enemyMana >= card.cardSO.manaCost)
            {
                enemyPlayableCards.Add(card);
            }
        }
        enemyPlayableCards = enemyPlayableCards.OrderByDescending(x => x.cardSO.manaCost).ToList();
        Invoke("PlayCardsAI", 1.5f);
    }

    private void PlayCardsAI()
    {
        if (enemyPlayableCards.Count > 0 && !playerTurn && enemyAvailableRooms.Count > 0)
        {
            CardMovement card = enemyPlayableCards[0]; // Escolhe a carta mais cara
            // enemyPlayableCards.Remove(card);
            enemyCards.Remove(card); // Remove da mao
            enemyMana -= card.cardSO.manaCost; // Diminui a mana
            int randomRoom = UnityEngine.Random.Range(0, enemyAvailableRooms.Count);
            GameObject selectedRoom = enemyAvailableRooms[randomRoom]; // Escolhe uma sala disponível aleatória
            
            int value = 0;
            for(int i = 0; i < enemyRooms.Count; i++) // Encontra o index da sala selecionada
            {
                if(enemyRooms[i].gameObject.name == selectedRoom.name)
                {
                    value = i;
                    break;
                }
            }
            
            RoomManager roomToBeDropped = enemyRooms[value]; // Seleciona a sala a ser jogada
            // card.transform.SetParent(roomToBeDropped.gameObject.transform); // Muda o pai
            // card.played = true;
            enemyPlayedCards.Add(card);
            Debug.Log("Played card at " + roomToBeDropped.gameObject.name);
            roomToBeDropped.currentCards++; // Aumenta o número de cartas na sala
            roomToBeDropped.SetCurrentCard(card); // Adiciona a carta à sala
            card.PlayCard(roomToBeDropped);
            // Debug.Log("Enemy played " + card.cardSO.cardName);
            Invoke("CheckPlayableCards", 0.5f);
            card.gameObject.GetComponent<Animator>().SetTrigger("PLAYED");
            enemyAvailableRooms.Remove(selectedRoom); // Remove a sala das salas disponíveis
        }
        else if(enemyPlayableCards.Count == 0 && !playerTurn || enemyAvailableRooms.Count == 0 && !playerTurn)
        {
            Debug.Log("End Enemy Turn");
            EndTurn();
        }
    }

    public void KillCard(CardMovement card, RoomManager room)
    {
        try
        {
            Debug.Log("Killing " + card.name + " is AI:" + card.isAICard);
            graveyard.Add(card);
            graveyardCountText.text = graveyard.Count.ToString();
            room.RemoveCurrentCard();
            if (card.isAICard)
            {
                enemyAvailableRooms.Add(room.gameObject);
                Debug.Log(enemyPlayedCards.FindIndex(x => x == card));
                enemyPlayedCards.Remove(card);
            }
            else
            {
                playerPlayedCards.Remove(card);
            }
            card.played = false; // Shouldn't be able to change this
            card.gameObject.SetActive(false);
            card.transform.SetParent(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Serializable]
    public class Deck{
        public Transform card;
        public int amount;
    }
}
