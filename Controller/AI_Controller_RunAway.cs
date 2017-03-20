using UnityEngine;
using System.Collections;

public class AI_Controller_RunAway : MonoBehaviour {
  public bool ended;

  public float movementSpeed;
  public GameObject contact;

  private bool startedBehavior;

  private AI_Entity entity;
  private Rigidbody myBody;

  private Vector3 forceVector;

  void Start () {
    entity = this.gameObject.GetComponent<AI_Entity>();
    myBody = this.gameObject.GetComponent<Rigidbody>();
  }

  public bool doBehavior (bool condition) {
    if (myBody == null) {
      myBody = this.gameObject.GetComponent<Rigidbody>();
    }

    if (entity == null) {
      entity = this.gameObject.GetComponent<AI_Entity>();
    }

    if (condition) {
      forceVector = contact.transform.position - this.transform.position;
      
      if (entity.flying != true) {
        forceVector.y = 0;
      }

      if (entity.activationHint != null) {
        entity.activationHint.SetActive(true);
      }

      myBody.AddForce(-forceVector.normalized * movementSpeed, ForceMode.Impulse);

      startedBehavior = true;
      return condition;
    }

    if (entity.activationHint != null) {
      entity.activationHint.SetActive(false);
    }

    startedBehavior = false;
    return condition;
  }
}
