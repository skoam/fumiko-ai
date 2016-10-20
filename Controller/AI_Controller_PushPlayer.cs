using UnityEngine;
using System.Collections;

public class AI_Controller_PushPlayer : MonoBehaviour {
  public bool ended;

  public float movementSpeed;
  public GameObject contact;

  private Rigidbody contactBody;

  void Start () {
  }

  public bool doBehavior (bool condition) {
    if (condition) {
      contactBody = contact.GetComponent<Rigidbody>();

      contactBody.AddForce(
        (contact.transform.position - this.transform.position) * movementSpeed, ForceMode.Impulse);
    }

    return condition;
  }
}
