using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsCreator : MonoBehaviour
{
    private Ray ray;
    private Vector2 cameraRay;
    private RaycastHit2D hit;
    private LayerMask cellMask;
    private void Awake() 
    {
        cellMask = 1 << 3;    
    }

    private void Update()
    {
        CreateCellsOnGameField();
    }
    
    private void CreateCellsOnGameField()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        cameraRay = new Vector2(ray.origin.x, ray.origin.y);
        hit = Physics2D.Raycast(cameraRay, -Vector2.up, cellMask);

        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            if (hit.collider.GetComponent<SpriteRenderer>().color == Color.white)
                hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
            else 
                hit.collider.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
