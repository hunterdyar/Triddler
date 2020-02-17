using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHider : MonoBehaviour
{
    public GameObject toggle;
    public bool hideOnStart;
    void Start(){
        toggle.SetActive(!hideOnStart);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)){
            toggle.SetActive(!toggle.activeInHierarchy);
        }
    }
}
