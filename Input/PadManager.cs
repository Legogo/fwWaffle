using UnityEngine;
using XInputDotNetPure;
using System;

public class PadManager : MonoBehaviour {

  public const int MAX_PADS = 4;

  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    _instance = UnityTools.getManager<PadManager>("[pads]");
    DontDestroyOnLoad(_instance);
  }

  static public ControllerPad[] pads;

  void Awake(){
      
    if (pads == null)
    {
      pads = new ControllerPad[MAX_PADS];
      for (int i = 0; i < pads.Length; i++)
      {
        pads[i] = new ControllerPad((PlayerIndex)i);
      }
    }
    
    //info sur les controllers connectés au start
    Debug.Log("<color=gray>"+ countSystemConnected() + " controller(s) connected</color>");
  }
  
  void Update()
  {
    for (int i = 0; i < pads.Length; i++)
    {
      pads[i].update();
    }
  }

  public int countSystemConnected(){
    int count = 0;
    for (int i = 0; i < 4; i++)
    {
      GamePadState state = XInputDotNetPure.GamePad.GetState((PlayerIndex)i);
      if (state.IsConnected) count++;
    }
    return count;
  }

  public bool hasSomeConnected(){ return countConnected() > 0; }
  public int countConnected() {
    int count = 0;
    if (pads == null) return 0;
    for (int i = 0; i < pads.Length; i++)
    {
      if (pads[i].isConnected()) count++;
    }
    return count;
  }

  public string toString() {
    string info = "";

    info += "\n Input.joysticks count ? " + Input.GetJoystickNames().Length;
    info += "\n update ? " + enabled;
    info += "\n count ? " + pads.Length;

    for (int i = 0; i < pads.Length; i++)
    {
      info += "\n" + pads[i].gamePadIndex;
      info += " connected ? "+pads[i].state.IsConnected;
    }

    return info;
  }

  static protected PadManager _instance;
  static public PadManager get() {
    if (_instance == null) _instance = GameObject.FindObjectOfType<PadManager>();
    return _instance;
  }
}
