using UnityEngine;
using System.Collections;

public class AI_Controller_RandomMovement : MonoBehaviour {
  public bool ended;

  public float movementSpeed;

  private AI_Entity entity;
  private Rigidbody myBody;

  private Vector3 forceVector;
  
  void Start () {
    myBody = this.gameObject.GetComponent<Rigidbody>();
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (condition) {
      if (myBody == null) {
        myBody = this.gameObject.GetComponent<Rigidbody>();
      }

      if (entity == null) {
        entity = this.gameObject.GetComponent<AI_Entity>();
      }

      if (entity.flying == true) {
        forceVector = new Vector3(
          Random.Range(-movementSpeed, movementSpeed),
          Random.Range(-movementSpeed, movementSpeed),
          Random.Range(-movementSpeed, movementSpeed)
        );
      } else {
        forceVector = new Vector3(
          Random.Range(-movementSpeed, movementSpeed),
          0,
          Random.Range(-movementSpeed, movementSpeed)
        );
      }

      // this.transform.LookAt(forceVector);
      
      myBody.AddForce(forceVector, ForceMode.Impulse);
    }

    return condition;
  }

  public void activateBehavior () {
    ended = false;
  }

  public void deactivateBehavior () {
    ended = true;
  }

}
