using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Blooper.Triangles{
    public class LoadLevelFromWeb : MonoBehaviour
    {
        public string address;
        public InputField addressInput;
        public TriddlePuzzle puzzle;
        public TriangleGridSystem triangleGridSystem;
        // Update is called once per frame
        public void LoadLevelFromAddress()
        {
            address = addressInput.text;
            StartCoroutine(DoLoadFromAddress());
        }
        IEnumerator DoLoadFromAddress()
        {
            Debug.Log(UnityWebRequest.UnEscapeURL(address));
            UnityWebRequest www = UnityWebRequest.Get(address);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else
            {
                puzzle.levelAsTextData = www.downloadHandler.text;
                triangleGridSystem.LoadLevel();
                Debug.Log("loaded!");
            }
        }
    }
}
