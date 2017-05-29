using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundState : EngineObject {

  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    DontDestroyOnLoad(new GameObject("[round]").AddComponent<RoundState>());
  }

  static public RoundState _instance;
  
  public enum eRoundStates { RESTART = 0, LIVE = 1, POST_LIVE = 2, END = 3 };
  protected eRoundStates _state = eRoundStates.RESTART;

  protected override void build()
  {
    base.build();
    _instance = this;
  }
  
  public void roundRestart()
  {
    Debug.Log("<color=olive>round restart</color>");

    _state = eRoundStates.RESTART;

    if(EventRound.onRoundRestart != null) EventRound.onRoundRestart();
  }

  public void roundLaunch(){
    
    Debug.Log("<color=olive>round launch</color>");

    _state = eRoundStates.LIVE;

    if (EventRound.onRoundLaunch != null) EventRound.onRoundLaunch();
    
  }

  //pas de réaction a la pause
  protected override void subscribeSystemEvent(){}
  
  virtual protected void updateRound(){
    
  }

  protected override void updateEngine()
  {
    base.updateEngine();
    
    //debug key
    if(Input.GetKeyUp(KeyCode.R)) {
      roundRestart();
      return;
    }

    if (_state != eRoundStates.LIVE) return;

    updateRound();
  }
  
  public void roundEnd()
  {
    Debug.Log("<color=olive>round end !</color>");

    _state = eRoundStates.END;

    EventRound.onRoundEnd();

    if (hasLost()) roundLost();
  }

  virtual protected bool hasLost(){
    return false;
  }

  public void roundLost()
  {
    Debug.Log("<color=olive>round lose !</color>");

    EventRound.onRoundLose();
  }

  public eRoundStates getState() { return _state; }
}
