using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Triangles;
public class Selector : MonoBehaviour
{
    public TriddlePuzzle puzzle;//for palette.
    public TriangleGridSystem triangleGridSystem;
    void Awake()
    {
        triangleGridSystem = GameObject.FindObjectOfType<TriangleGridSystem>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Collider2D c = Physics2D.OverlapPoint((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(c != null){
                Triangle t = c.GetComponent<TriangleInteractor>().triangle;
                if(t.selectedStatus < puzzle.tridSize.colors)
                {
                    t.selectedStatus++;
                    c.GetComponent<MeshRenderer>().material.color = puzzle.palette[t.selectedStatus-1];
                    c.GetComponent<MeshRenderer>().enabled = true;
                }else{
                    c.GetComponent<MeshRenderer>().material.color = Color.white;
                    c.GetComponent<MeshRenderer>().enabled = false;
                    t.selectedStatus = 0;
                }
                triangleGridSystem.TriangleUpdated(t);
            }
        }
    }
}
