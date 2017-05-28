using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScreen : EngineObject {

  Camera uiCamera;
  Canvas canvas;

  protected bool lockGameState = false;
  
  public override void loadingDone()
  {
    base.loadingDone();

    uiCamera = getUiCamera();

    canvas = gameObject.GetComponentInChildren<Canvas>();
    if(canvas != null) {
      canvas.worldCamera = uiCamera;
    }

    gather();
    
    //Debug.Log(name+" found ui cam "+uiCamera.name, gameObject);
  }

  virtual public void open() {
    lockGameState = true;
    uiCamera.enabled = true;
    //Debug.Log("UiScreen " + name + " open");
  }
  virtual public void close() {
    lockGameState = false;
    //Debug.Log("UiScreen " + name + " close");
  }

  virtual public bool isLocking(){
    return lockGameState;
  }
  
  protected bool someoneHasPad(){ return PadManager.get().countConnected() > 0; }
  
  protected bool padSkip()
  {
    //Debug.Log("skip ? "+ PadManager.get().hasSomeConnected());

    if(!PadManager.get().hasSomeConnected()){
      //Debug.Log("nothing connected");
      return Input.GetKeyUp(KeyCode.Escape);
    }

    ControllerPad[] pads = PadManager.pads;
    
    for (int i = 0; i < pads.Length; i++)
    {
      if(pads[i].pressSkip()) {
        return true;
      }
    }

    return false;
  }


  static protected UiScreen[] screens;
  static protected void gather() {
    screens = GameObject.FindObjectsOfType<UiScreen>();
  }

  static public UiScreen getScreen(string name) {
    if (screens == null){
      Debug.LogWarning("asking for screen " + name + " but screens are not ready yet");
      return null;
    }

    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(name)) return screens[i];
    }
    return null;
  }

  static public Camera getUiCamera()
  {
    GameObject obj = GameObject.Find("(camera-ui)");
    if (obj == null) Debug.LogError("no ui camera found");

    return obj.GetComponent<Camera>();
  }

}
