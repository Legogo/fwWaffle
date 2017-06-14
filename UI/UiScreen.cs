using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class UiScreen : EngineObject {

  Camera uiCamera;
  Canvas canvas;
  
  protected bool lockGameState = false;
  protected bool use_verticalSelection = true;
  protected bool use_looping = false;

  protected UiInputLayer _input;
  
  UiOption[] options;
  UiOption option_selected;

  protected override void build()
  {
    base.build();

    //refresh screen list
    upateStaticScreensList();
  }
  
  protected void setupInputCallbacks()
  {

    if(_input == null) _input = new UiInputLayer();

    _input.setupDirectionEvents(onLeft, onRight, onUp, onDown);
    _input.setupEvents(onValidation, onSkip);

  }

  public override void afterLoading()
  {
    base.afterLoading();

    setupInputCallbacks();

    uiCamera = getUiCamera(transform);

    canvas = gameObject.GetComponentInChildren<Canvas>();
    if(canvas != null) {
      canvas.worldCamera = uiCamera;
    }
    
    //gather options
    options = transform.GetComponentsInChildren<UiOption>();

    //sort array based on OrderLayer param
    List<UiOption> tmp = new List<UiOption>();
    
    int maxIdx = 0;
    for (int i = 0; i < options.Length; i++)
    {
      maxIdx = Mathf.Max(options[i].orderLayer, maxIdx);
    }
    
    int idx = 0;
    while(idx <= maxIdx)
    {
      for (int i = 0; i < options.Length; i++)
      {
        if (options[i].orderLayer == idx) tmp.Add(options[i]);
      }
      idx++;
    }
    
    options = tmp.ToArray();
    
    if (options.Length > 0)
    {
      selectOption(options[0]);
    }
    
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

  protected override void updateSystem()
  {
    base.updateSystem();
    _input.updateSystem();
  }

  virtual public bool isLocking(){ return lockGameState; }

  //events on pad/keyboard press
  virtual protected void onValidation() {
    if(option_selected != null)
    {
      option_selected.validate();
    }
  }
  virtual protected void onSkip() { }

  //events on pad/keyboard direction
  virtual protected void onUp() { if (use_verticalSelection) optionPrevious(); }
  virtual protected void onDown() { if (use_verticalSelection) optionNext(); }

  virtual protected void onLeft() { if (!use_verticalSelection) optionPrevious(); }
  virtual protected void onRight() { if (!use_verticalSelection) optionNext(); }

  protected void optionPrevious(){
    if (options == null) return;
    if (options.Length <= 0) return;

    for (int i = 0; i < options.Length; i++)
    {
      if(options[i] == option_selected)
      {
        int idx = i - 1;

        //circle
        if (use_looping)
        {
          if (idx < 0) idx = options.Length - 1;
        }

        idx = Mathf.Clamp(idx, 0, options.Length - 1);

        selectOption(options[idx]);
        return;
      }
    }

    option_selected = options[0];
  }
  protected void optionNext() {
    if (options == null) return;
    if (options.Length <= 0) return;

    for (int i = 0; i < options.Length; i++)
    {
      if (options[i] == option_selected)
      {
        int idx = i + 1;

        //circle
        if (use_looping)
        {
          if (idx >= options.Length) idx = 0;
        }

        idx = Mathf.Clamp(idx, 0, options.Length - 1);

        selectOption(options[idx]);
        return;
      }
    }

    option_selected = options[0];
  }

  protected void selectOption(UiOption opt)
  {
    if (option_selected != null) option_selected.onUnSelection();
    option_selected = opt;
    option_selected.onSelection();
  }

  static protected UiScreen[] screens;
  static protected void upateStaticScreensList() {
    screens = GameObject.FindObjectsOfType<UiScreen>();
  }

  static public UiScreen getScreen(string name) {
    if (screens == null) upateStaticScreensList();

    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(name)) return screens[i];
    }
    return null;
  }

  static public Camera getUiCamera(Transform context = null)
  {
    GameObject obj = null;
    if (context != null)
    {
      Camera cam = context.GetComponentInParent<Camera>();
      if (cam != null) obj = cam.gameObject;
    }

    if(obj == null)
    {
      obj = GameObject.Find("(camera-ui)");
    }
    
    if (obj == null) Debug.LogError("no ui camera found");

    return obj.GetComponent<Camera>();
  }

}
