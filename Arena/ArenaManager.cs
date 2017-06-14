using UnityEngine;
using System.Collections;

abstract public class ArenaManager : RoundObject {

  static protected ArenaManager _manager;

  public BoxCollider2D arenaBounds;
  
  protected ArenaObstacle[] obstacles;
  protected IArenaGameplayEntity[] gameplays;

  protected ArenaSpawn[] spawns;

  public Transform getPlayerSpawn(int idx) {
    spawns = GameObject.FindObjectsOfType<ArenaSpawn>();
    if (idx > spawns.Length) return null;
    return spawns[idx].getSpawn();
  }

  protected override void build()
  {
    base.build();

    _manager = this;
    
    arenaBounds = gameObject.GetComponent<BoxCollider2D>();
    if (arenaBounds == null) Debug.LogError("arena need Box2D to define bounds");

    obstacles = transform.GetComponentsInChildren<ArenaObstacle>();
    
  }

  // les choses spécifics lors du build
  //abstract protected void setupArena();

  // récup les objets joueurs pour les comparer avec les obstacles de l'arène
  abstract protected void fetchGameplays();

  protected override void roundRestart()
  {
    base.roundRestart();
    fetchGameplays();
  }

  protected override void updateRound()
  {
    base.updateRound();
    checkCollisionWithObstacles();
  }

  public bool isOutOfArenaTop(Vector3 pos) {
    return pos.y > transform.position.y + arenaBounds.bounds.extents.y;
  }
  public bool isUnderground(Vector3 pos) {
    float downPos = (arenaBounds.transform.position + arenaBounds.bounds.center).y - arenaBounds.bounds.extents.y;
    //Debug.Log(pos.y + " , " + downPos);
    return pos.y < downPos;
  }

  public void checkCollisionWithObstacles() {
    if (gameplays == null) return;
    if (obstacles == null) return;

    for (int i = 0; i < obstacles.Length; i++)
    {
      for (int j = 0; j < gameplays.Length; j++)
      {
        if(obstacles[i].checkColliders(gameplays[j])) {
          gameplays[j].hit(obstacles[i]);

          //only one
          return;
        }
      }
    }
  }
  
  [ContextMenu("display random position")]
  static public Vector3 getRandomPositionInArena(float depth) {
    ArenaManager am;
    BoxCollider2D bnd;
    
    if (Application.isPlaying) {
      am = get();
      bnd = am.arenaBounds;
    }
    else {
      am = GameObject.FindObjectOfType<ArenaManager>();
      bnd = am.GetComponent<BoxCollider2D>();
    }
    
    Vector3 position = bnd.randomPositionWithinBounds(depth);
    
    //show for N secondes
    //Debug.DrawLine(Vector3.zero, position, Color.yellow, 2f);

    return position;

  }
  
  static public ArenaManager get() {
    if (!Application.isPlaying) return GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
