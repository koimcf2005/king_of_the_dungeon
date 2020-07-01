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
    GameMaster gm;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        int randTile = Random.Range(0, tileGraphics.Length);
        rend.sprite = tileGraphics[randTile];

        gm = FindObjectOfType<GameMaster>();
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
        } else
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

}
