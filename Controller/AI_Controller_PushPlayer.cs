using UnityEngine;
using System.Collections;

public class AI_Controller_PushPlayer : MonoBehaviour {
  public bool ended;

  public float movementSpeed;
  public GameObject contact;

  private AI_Entity entity;
  private Rigidbody contactBody;

  void Start () {
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (condition) {
      contactBody = contact.GetComponent<Rigidbody>();

      if (entity == null) {
        entity = this.gameObject.GetComponent<AI_Entity>();
      }

      if (entity.pushOneShotParticles != null) {
        entity.pushOneShotParticles.Emit(entity.pushOneShotAmount);
      }

      if (entity.playPushSound) {
        entity.playSound();
      }
      
      contactBody.velocity = Vector3.zero;

      if (contact.GetComponent<IncreaseFallingSpeed>() != null) {
        contact.GetComponent<IncreaseFallingSpeed>().resetStrength();
      }

      contactBody.AddForce(
        (contact.transform.position - this.transform.position) * movementSpeed, ForceMode.Impulse);
    }

    return condition;
  }
}
