using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// to define spawns for gameplay entities
/// </summary>

public class ArenaSpawn : ArenaObject {
  public Vector3 getSpawnPosition() { return transform.position; }
  public Transform getSpawn() { return transform; }
}
