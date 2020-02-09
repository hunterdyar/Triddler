using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Triangles;
public class Selector : MonoBehaviour
{
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100)){
                if(hit.collider.GetComponent<TriangleInteractor>() != null)
                {
                    Debug.Log(hit.collider.GetComponent<TriangleInteractor>().triangle.position);
                    hit.collider.GetComponent<MeshRenderer>().material.color = Color.blue;
                    hit.collider.GetComponent<TriangleInteractor>().triangle.status = 1;
                }
            } 

            Collider2D c = Physics2D.OverlapPoint((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(c != null){

                c.GetComponent<MeshRenderer>().material.color = Color.green;
                c.GetComponent<MeshRenderer>().enabled = true;
                c.GetComponent<TriangleInteractor>().triangle.status = 1;
            }
        }
    }
}
