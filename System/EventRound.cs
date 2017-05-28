using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventRound {

  static public Action onRoundRestart; // pour setup des choses avant la partie
  static public Action onRoundLaunch; // la partie démarre

  static public Action onRoundEnd;
  static public Action onRoundLose;
  
}
