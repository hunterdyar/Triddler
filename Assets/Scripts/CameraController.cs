using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Config")]
    public Transform trianglesParent;
    public Transform hintParent;
    [HideInInspector]
    public Vector3 centeredPos = Vector3.zero;
    float allInBoundsOrthoSize = 0;
    [Header("Settings")]
    public float doubleClickTime = 0.3f;
    public float maxOrthoSize;
    public float boundingBoxPadding = 0f;
    Vector3 previousMousePos;
    float dcmTimer = 0;
    void Start()
    {
        previousMousePos = Input.mousePosition;//fixes bug if mouse is held down on first frame.
        //allInBoundsOrthoSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        dcmTimer = dcmTimer+Time.deltaTime;
        //Zoom in and out with scroll.
        if(Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if(Camera.main.orthographicSize > 1){
                Camera.main.orthographicSize = Camera.main.orthographicSize-1;
            }
        }else if(Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.Minus)|| Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if(Camera.main.orthographicSize < maxOrthoSize){
                Camera.main.orthographicSize = Camera.main.orthographicSize+1;
            }
        }


        if(Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetToCenter();
        }
        if(Input.GetMouseButton(2))
        {
            Vector3 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - previousMousePos;
            transform.position = transform.position-mouseDelta;
        }
        if(Input.GetMouseButtonDown(2))
        {
            if(dcmTimer < doubleClickTime){
                ResetToCenter();
            }
            dcmTimer = 0;
        }
        previousMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void ResetToCenter(bool forceReset = false,bool ignoreHints = false){
        if(allInBoundsOrthoSize == 0 || forceReset)
        {
            Rect r = CalculateTargetsBoundingBox(ignoreHints);
            allInBoundsOrthoSize = CalculateOrthographicSize(r);
        }
        Camera.main.orthographicSize = allInBoundsOrthoSize;
        transform.position = centeredPos;
    }

    //FROM https://answers.unity.com/questions/1231701/fitting-bounds-into-orthographic-2d-camera.html
    Rect CalculateTargetsBoundingBox(bool ignoreHints)//
     {
         float minX = Mathf.Infinity;
         float maxX = Mathf.NegativeInfinity;
         float minY = Mathf.Infinity;
         float maxY = Mathf.NegativeInfinity;
 
         foreach (Transform target in trianglesParent) {
             Vector3 position = target.position;
 
             minX = Mathf.Min(minX, position.x);
             minY = Mathf.Min(minY, position.y);
             maxX = Mathf.Max(maxX, position.x);
             maxY = Mathf.Max(maxY, position.y);
         }
         //
         if(!ignoreHints){
            foreach (Transform target in hintParent) {
                Vector3 position = target.position;

                minX = Mathf.Min(minX, position.x);
                minY = Mathf.Min(minY, position.y);
                maxX = Mathf.Max(maxX, position.x);
                maxY = Mathf.Max(maxY, position.y);
            }
         }
         //
 
         return Rect.MinMaxRect(minX - boundingBoxPadding, maxY + boundingBoxPadding, maxX + boundingBoxPadding, minY - boundingBoxPadding);
     }

     /// <summary>
     /// Calculates a new orthographic size for the camera based on the target bounding box.
     /// </summary>
     /// <param name="boundingBox">A Rect bounding box containg all targets.</param>
     /// <returns>A float for the orthographic size.</returns>
     float CalculateOrthographicSize(Rect boundingBox)
     {
         float orthographicSize = Camera.main.orthographicSize;
         Vector3 topRight = new Vector3(boundingBox.x + boundingBox.width, boundingBox.y, 0f);
         Vector3 topRightAsViewport = Camera.main.WorldToViewportPoint(topRight);
        
         if (topRightAsViewport.x >= topRightAsViewport.y)
             orthographicSize = Mathf.Abs(boundingBox.width) / Camera.main.aspect / 2f;
         else
             orthographicSize = Mathf.Abs(boundingBox.height) / 2f;
 
         return Mathf.Clamp(orthographicSize, 1, maxOrthoSize);
     }

}
