using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// usually all players
/// </summary>

public interface IGameplayEntity{
  Vector3 position {
    get;
  }

  Transform transform {
    get;
  }

  bool isCameraTarget();
}
