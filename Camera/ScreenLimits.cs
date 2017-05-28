using UnityEngine;
using System.Collections;

static public class ScreenLimits
{
  static public BoxCollider cameraBox;

  static public Vector3 levelBottomLeft; // top left of screen
  static public Vector3 levelTopRight; // bottom right of screen

  static private Camera refCamera;

  static private GameObject carry;
  static private BoxCollider2D bounds;

  static public GameObject left;
  static public GameObject right;

  [RuntimeInitializeOnLoadMethod]
  static public void init()
  {
    carry = GameObject.FindGameObjectWithTag("arena-limits");
    if (carry == null)
    {
      Debug.LogWarning("no screen limits defined, need an object with box collider tagged as limits");
      return;
    }

    bounds = carry.GetComponent<BoxCollider2D>();
    if (bounds == null)
    {
      Debug.LogError("limits need to be defined by box2Dcollider");
      return;
    }

  }

  /* doit etre apl après que les scènes d'engine soit présente */
  static public void setup(){
    if (!hasLimits()) return;

    if (refCamera == null) refCamera = Camera.main;
    
    if (left == null)
    {
      left = GameObject.CreatePrimitive(PrimitiveType.Cube);
      left.name = "limit_left";
      left.transform.parent = carry.transform;
      
    }

    if (right == null)
    {
      right = GameObject.CreatePrimitive(PrimitiveType.Cube);
      right.name = "limit_right";
      right.transform.parent = carry.transform;
      
    }

  }
  
  static public void resize()
  {
    if (!hasLimits()) return;

    if (refCamera == null) setup();
    
    float height = refCamera.orthographicSize;
    float width = refCamera.aspect * (refCamera.orthographicSize);

    //Debug.Log(width+","+height);

    Vector3 camPos = refCamera.transform.position;
    levelBottomLeft = new Vector3(camPos.x - width, camPos.y - height, camPos.z);
    levelTopRight = new Vector3(camPos.x + width, camPos.y + height, camPos.z);

    if (left != null) left.transform.position = levelBottomLeft;
    if (right != null) right.transform.position = levelTopRight;
    
    Vector3 size = cameraBox.bounds.size;
    size.x = levelTopRight.x - levelBottomLeft.x;
    size.y = levelTopRight.y - levelBottomLeft.y;
    size.z = 100f;
    cameraBox.size = size;
  }
  
  static public bool hasLimits(){
    return bounds != null;
  }
}
