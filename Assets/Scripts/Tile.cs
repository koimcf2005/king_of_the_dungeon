using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public SpriteRenderer rend;
    public Sprite[] tileGraphics;

    public float hoverAmount;

    public LayerMask obstacleLayer;

    public Color highlightedColor;
    public bool isWalkable;
    public GameObject highlight;

    GameMaster gm;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        int randTile = Random.Range(0, tileGraphics.Length);
        rend.sprite = tileGraphics[randTile];

        gm = FindObjectOfType<GameMaster>();
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosRounded = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));

        foreach (Card card in FindObjectsOfType<Card>())
        {
            if (card.isDragging == true && card.movePositions.x == transform.position.x && card.movePositions.y == transform.position.y && isWalkable == true)
            {
                highlight.SetActive(true);
            }
            if (mousePosRounded.x != transform.position.x || mousePosRounded.y != transform.position.y)
            {
                Invoke("DeselectHighlight", 0.02f);
            }
        }
    }

    private void OnMouseEnter()
    {
        transform.localScale += Vector3.one * hoverAmount;
        this.transform.position += new Vector3(0, 0, -1);
    }

    private void OnMouseExit()
    {
        transform.localScale -= Vector3.one * hoverAmount;
        this.transform.position += new Vector3(0, 0, 1);
    }

    public bool IsClear()
    {
        Collider2D obstacle = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
        if (obstacle != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Highlight()
    {
        rend.color = highlightedColor;
        isWalkable = true;
    }

    public void Reset()
    {
        rend.color = Color.white;
        isWalkable = false;
    }

    private void OnMouseDown()
    {
        if (isWalkable && gm.selectedUnit != null && gm.selectedUnit.isAttacking == false)
        {
            gm.selectedUnit.Move(this.transform.position);
        }
    }

    private void DeselectHighlight()
    {
        highlight.SetActive(false);
    }

}
