using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    public int cardsOnRooms;
    public int maxCardsOnRooms = 10;
    [SerializeField] DropZone handScript;
    public bool isPlaying;
    public bool isOnMenu;
    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomHand();
    }

    // Update is called once per frame
    void Update()
    {
        if(cardsOnRooms == maxCardsOnRooms && !isPlaying && !isOnMenu)
        {
            Debug.Log("Show Win");
            WinGame();
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangePerspective();
        }
    }

    public void ChangePerspective()
    {
    }

    public void PlayGame()
    {
        ChangePerspective();
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

    public void TryAgain()
    {
        isPlaying = true;
        isOnMenu = false;
        loseUI.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void GenerateRandomHand()
    {

    }
}
