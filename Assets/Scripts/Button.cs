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
    GameMaster gm;
    Animator camAnim;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        deck = FindObjectOfType<Deck>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) == 10 && deck.count(deck.cardsInRedSlot, true) == 10 && deck.cardInBlueHeroSlot == true && deck.cardInRedHeroSlot == true)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) != 10 || isStartButton == true && deck.count(deck.cardsInRedSlot, true) != 10 && deck.cardInBlueHeroSlot == false && deck.cardInRedHeroSlot == false)
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
        if (isStartButton == true && deck.count(deck.cardsInBlueSlot, true) == 10 && deck.count(deck.cardsInRedSlot, true) == 10 && deck.cardInBlueHeroSlot == true && deck.cardInRedHeroSlot == true && Input.GetMouseButtonDown(0))
        {
            transform.localScale = Vector2.one;
            blueDeckBuilder.SetActive(false);
            blueCards.SetActive(true);
            blueBlankCards.SetActive(false);
            redDeckBuilder.SetActive(false);
            redCards.SetActive(true);
            redBlankCards.SetActive(false);
            scene.SetActive(true);
            Invoke("StartGame", 4.0f);
            foreach (Card card in FindObjectsOfType<Card>())
            {
                card.transform.position = deck.deckSprite.transform.position;
                if (card.isHero == true && card.cardNumber == 1 && card.isInDeckAsHero == true) gm.blueHero = card.unit;
                if (card.isHero == true && card.cardNumber == 2 && card.isInDeckAsHero == true) gm.redHero = card.unit;
                if (card.isHero == true && card.isInDeckAsHero == false) Destroy(card.unit);
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
