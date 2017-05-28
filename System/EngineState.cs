using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineState : EngineObject {

  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    DontDestroyOnLoad(new GameObject("[engine]").AddComponent<EngineState>());
  }

  static public EngineState _instance;
  
  public enum eGameStates { PAUSED = 0, LIVE = 1 };
  protected eGameStates _state = eGameStates.PAUSED;
  
  protected override void build()
  {
    base.build();
    _instance = this;
  }
  
  public override void loadingDone()
  {
    base.loadingDone();
    _state = eGameStates.LIVE;
  }

  protected override void updateEngine()
  {
    base.updateEngine();

    if(Input.GetKeyUp(KeyCode.Escape)){
      if(_state == eGameStates.LIVE){
        _state = eGameStates.PAUSED;
        EventEngine.onPause(true);
      }else{
        _state = eGameStates.LIVE;
        EventEngine.onPause(false);
      }
      
    }
  }

  //pas de réaction a la pause
  protected override void subscribeSystemEvent(){}
  
  public eGameStates getState() { return _state; }

  public void endOfLoading(){
    ModBase mod = ModBase.getMod();
    if (mod == null)
    {
      Debug.LogWarning("no mod to launch");
      return;
    }
    mod.modRestart();
  }
}
