﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundState : EngineObject {

  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    _instance = UnityTools.getManager<RoundState>("[round]", true);
  }

  static public RoundState _instance;
  
  public enum eRoundStates { RESTART = 0, LIVE = 1, POST_LIVE = 2, END = 3 };
  protected eRoundStates _state = eRoundStates.RESTART;
  
  public void roundRestart()
  {
    Debug.Log("<color=lime>round restart</color>");

    _state = eRoundStates.RESTART;

    if(EventRound.onRoundRestart != null) EventRound.onRoundRestart();
  }

  public void roundLaunch(){
    
    Debug.Log("<color=lime>round launch</color>");

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

    //Debug.Log("update engine | round : "+_state);

    //debug key
    if(Input.GetKeyUp(KeyCode.R)) {
      roundRestart();
      return;
    }

    if (_state != eRoundStates.LIVE) return;

    updateRound();
  }
  
  /* called by modEnd() */
  public void roundEnd()
  {
    Debug.Log("<color=lime>round end !</color>");

    _state = eRoundStates.END;

    EventRound.onRoundEnd();

    //in a specific case where you need lost logic (solo play)
    if (hasLost()) roundLost();
    else if(hasWin()) roundWin();

  }

  virtual protected bool hasLost(){ return false; }
  virtual protected bool hasWin() { return false; }

  public void roundLost()
  {
    Debug.Log("<color=lime>round lose !</color>");

    EventRound.onRoundLose();
  }

  public void roundWin()
  {
    Debug.Log("<color=lime>round win !</color>");
  }

  public eRoundStates getState() { return _state; }
}
