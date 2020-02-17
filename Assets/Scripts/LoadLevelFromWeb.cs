using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Blooper.Triangles{
    public class LoadLevelFromWeb : MonoBehaviour
    {
        public string address;
        public InputField addressInput;
        public TriddlePuzzle puzzle;
        public TriangleGridSystem triangleGridSystem;
        void Start () {
        //    TextUploaderInit ();
        }
        // Update is called once per frame
        public void LoadLevelFromAddress()
        {
            address = addressInput.text;
            StartCoroutine(DoLoadFromAddress());
        }
        public void LoadLevelFromAddressOnClipboard()
        {
            string clipboard = GUIUtility.systemCopyBuffer;
            if(!System.Uri.IsWellFormedUriString(clipboard,System.UriKind.Absolute)){
                //invalid URL in clipboard, must be something else, try to load whatever was typed.
                LoadLevelFromAddress();
            }else{
                //load from the clipboard.
                address = clipboard;
                addressInput.text = address;
                StartCoroutine(DoLoadFromAddress());
            }
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
        ///

        ///Loading from file picker

        // [DllImport("__Internal")]
        // private static extern void TextUploaderInit();
    
        IEnumerator LoadText (string address) {
            UnityWebRequest www = UnityWebRequest.Get(address);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else
            {
                puzzle.levelAsTextData = www.downloadHandler.text;
                triangleGridSystem.LoadLevel();
                Debug.Log("loaded from File!");
            }
        }
        //called by the TextUploder.jslib library
        void FileSelected (string url) {
            StartCoroutine(LoadText (url));
        }
    
        
        }
}
