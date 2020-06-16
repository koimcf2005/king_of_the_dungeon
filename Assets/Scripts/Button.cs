using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public bool isDeckButton;
    public bool isStartButton;
    public int buttonNumber;
    public GameObject blueDeckBuilder;
    public GameObject redDeckBuilder;
    public GameObject blueCards;
    public GameObject redCards;
    public GameObject scene;

    Deck deck;
    Animator camAnim;

    private void Start()
    {
        deck = FindObjectOfType<Deck>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
    }

    private void OnMouseOver()
    {
        if (isDeckButton == true && Input.GetMouseButtonDown(0) && buttonNumber == 1)
        {
            blueDeckBuilder.SetActive(false);
            blueCards.SetActive(false);
            redDeckBuilder.SetActive(true);
            redCards.SetActive(true);
            transform.localScale = new Vector3(1.5f, 1.5f, 0);
        }
        else if (isDeckButton == true && Input.GetMouseButtonDown(0) && buttonNumber == 2)
        {
            blueDeckBuilder.SetActive(true);
            blueCards.SetActive(true);
            redDeckBuilder.SetActive(false);
            redCards.SetActive(false);
            transform.localScale = new Vector3(1.5f, 1.5f, 0);
        }
        if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) == 10 && deck.count(deck.cardsInRedSlot, true) == 10 && Input.GetMouseButtonDown(0))
        {
            scene.SetActive(true);
            blueDeckBuilder.SetActive(false);
            blueCards.SetActive(true);
            redDeckBuilder.SetActive(false);
            redCards.SetActive(true);
            Invoke("StartGame", 4.0f);
            transform.localScale = new Vector3(1.5f, 1.5f, 0);
            foreach (Card card in FindObjectsOfType<Card>())
            {
                card.transform.position = deck.deckSprite.transform.position;
            }
        }
        else if (Input.GetMouseButtonDown(0) && isStartButton == true) camAnim.SetTrigger("Shake");
    }
    private void StartGame()
    {
        deck.gameHasStarted = true;
    }

}
