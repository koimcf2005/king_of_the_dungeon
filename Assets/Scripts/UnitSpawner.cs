using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Unit unit;
    GameMaster gm;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 movePositions = new Vector3(Mathf.Round(transform.position.x + Input.GetAxis("Horizontal")), 
        Mathf.Round(transform.position.y + Input.GetAxis("Vertical")), transform.position.z);
        transform.position = movePositions;
    }

    private void Update()
    {
        if (gm.movesLeft == 0)
        {
            SpawnUnit();
        }
    }

    public void SpawnUnit()
    {
        Unit instance = Instantiate(unit, transform.position, Quaternion.identity);
        instance.crystalPos = gm.crystalsPos[instance.playerNumber - 1];
        instance.healthIndicator = gm.healthIndicator;
        Destroy(gameObject);
    }

}
