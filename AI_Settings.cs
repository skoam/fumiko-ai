using UnityEngine;
using System.Collections;

[System.Serializable]
public class AI_Settings : System.Object {
  public AI_Behavior type;

  public AI_Condition condition;
  public float distance = 10;

  [Range(0, 10)]
  public int priority = 5;

  public float movementSpeed;
}
