using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour {
    
    public bool beenPlayed;
    public CardSO cardSO;

    private GameController gc;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public Image maskImage;
    public Image artworkImage;

    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI growthTimeText;

    public int? currentSlot = null;

    public void Start() {

        gc = FindObjectOfType(typeof(GameController)) as GameController;

        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.description;

        maskImage.sprite = cardSO.mask;
        artworkImage.sprite = cardSO.artwork;

        manaCostText.text = cardSO.manaCost.ToString();
        healthText.text = cardSO.health.ToString();
        growthTimeText.text = cardSO.growthTime.ToString();
    }

    public void onDraw() {
        cardSO.onDraw();
    }

    public void onPlay() {
        cardSO.onPlay();
    }

    public void onDie() {
        cardSO.onDie();
    }

    private void OnMouseDown() {
        if(beenPlayed) {
            this.gameObject.SetActive(false);
            //gc.graveyard.Add(this);
            onDie();
        } else {
            if(!gc.canAffordMana(cardSO.manaCost)) return;
            gc.loseMana(cardSO.manaCost);
            transform.position += Vector3.up * 5;
            gc.availableSlots[(int) currentSlot] = true; // Slot da mao
            currentSlot = null; // Qual slot da mao ele esta
            onPlay();
            beenPlayed = true;
        }
        
    }

}
