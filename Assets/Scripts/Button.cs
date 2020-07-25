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
    public GameObject blueBlankCards;
    public GameObject redCards;
    public GameObject redBlankCards;
    public GameObject scene;

    Deck deck;
    Animator camAnim;

    private void Start()
    {
        deck = FindObjectOfType<Deck>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) == 10 && deck.count(deck.cardsInRedSlot, true) == 10)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) != 10 || isStartButton == true && deck.count(deck.cardsInRedSlot, true) != 10)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    private void OnMouseOver()
    {
        transform.localScale = new Vector3(1.1f, 1.2f, 0);

        if (isDeckButton == true && Input.GetMouseButtonDown(0) && buttonNumber == 1)
        {
            transform.localScale = Vector2.one;
            blueDeckBuilder.SetActive(false);
            blueCards.SetActive(false);
            redDeckBuilder.SetActive(true);
            redCards.SetActive(true);
        }
        else if (isDeckButton == true && Input.GetMouseButtonDown(0) && buttonNumber == 2)
        {
            transform.localScale = Vector2.one;
            blueDeckBuilder.SetActive(true);
            blueCards.SetActive(true);
            redDeckBuilder.SetActive(false);
            redCards.SetActive(false);
        }
        if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) == 10 && deck.count(deck.cardsInRedSlot, true) == 10 && Input.GetMouseButtonDown(0))
        {
            transform.localScale = Vector2.one;
            scene.SetActive(true);
            blueDeckBuilder.SetActive(false);
            blueCards.SetActive(true);
            blueBlankCards.SetActive(false);
            redDeckBuilder.SetActive(false);
            redCards.SetActive(true);
            redBlankCards.SetActive(false);
            Invoke("StartGame", 4.0f);
            foreach (Card card in FindObjectsOfType<Card>())
            {
                card.transform.position = deck.deckSprite.transform.position;
            }
        }
        else if (Input.GetMouseButtonDown(0) && isStartButton == true) camAnim.SetTrigger("Shake");
    }

    private void OnMouseExit()
    {
        transform.localScale = Vector2.one;
    }

    private void StartGame()
    {
        deck.gameHasStarted = true;
    }

}
