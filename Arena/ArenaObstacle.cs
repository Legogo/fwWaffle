using UnityEngine;
using System.Collections;

/// <summary>
/// to manage collision with gameplay entities
/// </summary>

public class ArenaObstacle : ArenaObject {

  public BoxCollider2D[] _colliders;

  public override void afterLoading()
  {
    base.afterLoading();

    _colliders = transform.Find("collision").GetComponents<BoxCollider2D>();
	}
	
  public bool checkColliders(IArenaGameplayEntity entity) {
    if (entity == null) return false;

    Collider2D[] gpColliders = entity.getColliders();

    for (int i = 0; i < _colliders.Length; i++)
    {
      for (int j = 0; j < gpColliders.Length; j++)
      {

        if (_colliders[i].bounds.Intersects(gpColliders[j].bounds))
        {
          return true;
        }  

      }
    }
    return false;
  }
}
