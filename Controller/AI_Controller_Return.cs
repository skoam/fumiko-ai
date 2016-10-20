using UnityEngine;
using System.Collections;

public class AI_Controller_Return : MonoBehaviour {
  public bool ended;

  public Vector3 home;
  public float movementSpeed;

  private AI_Entity entity;
  private Rigidbody myBody;
  private Vector3 forceVector;
  
  void Start () {
    myBody = this.gameObject.GetComponent<Rigidbody>();
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (myBody == null) {
      myBody = this.gameObject.GetComponent<Rigidbody>();
    }

    if (entity == null) {
      entity = this.gameObject.GetComponent<AI_Entity>();
    }

    if (condition) {
      forceVector = home - this.transform.position;

      if (entity.flying != true) {
        forceVector.y = 0;
      }

      myBody.AddForce(
        forceVector * movementSpeed, ForceMode.Impulse);
    }

    return condition;
  }
}
