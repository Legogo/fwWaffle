using UnityEngine;
using System.Collections;

/// <summary>
/// to draw some information on screen and linked to the D keyboard key
/// </summary>

public class DebugManager : MonoBehaviour {

  public bool show = false;

  GUIStyle skin;
  Rect guiRec = new Rect(15f, 15f, 0f, 0f);

  void Update () {
    if (Input.GetKeyUp(KeyCode.D)) show = !show;
	}

  void OnGUI() {
    if (!show) return;
    
    if (skin == null)
    {
      skin = new GUIStyle();
      skin.fontSize = (Screen.width / Screen.height) * 8;
      skin.normal.textColor = Color.red;
      skin.fontStyle = FontStyle.Bold;
    }

    guiRec.width = Camera.main.pixelWidth;
    guiRec.height = Camera.main.pixelWidth;

    string str = toString();
    if(str.Length > 0) GUI.Label(guiRec, str, skin);
  }

  virtual protected string toString(){
    return "";
  }
}
