using UnityEngine;
using System.Collections;

public class AI_Controller_Pull : MonoBehaviour {
  public bool ended;

  public float movementSpeed;
  public GameObject contact;

  private AI_Entity entity;
  private Rigidbody contactBody;

  private bool pulling;

  public float distance;

  void Start () {
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (condition && !pulling) {
      contactBody = contact.GetComponent<Rigidbody>();

      if (entity == null) {
        entity = this.gameObject.GetComponent<AI_Entity>();
      }

      if (entity.pushOneShotParticles != null) {
        entity.pushOneShotParticles.Emit(entity.pushOneShotAmount);
      }

      contactBody.velocity = Vector3.zero;

      if (contact.GetComponent<IncreaseFallingSpeed>() != null) {
        contact.GetComponent<IncreaseFallingSpeed>().resetStrength();
      }

      if (entity.playPullSound) {
        entity.playSound();
      }
      
      StartCoroutine("pull");
    } else if (pulling) {
      return false;
    }

    return condition;
  }

  IEnumerator pull () {
    yield return new WaitForSeconds(movementSpeed);

    pulling = true;

    if (contact.GetComponent<IncreaseFallingSpeed>() != null) {
      contact.GetComponent<IncreaseFallingSpeed>().resetStrength();
    }

    if (entity.pushOneShotParticles != null) {
      entity.pushOneShotParticles.Emit(entity.pushOneShotAmount);
    }

    contactBody.AddForce(
      (contact.transform.position - this.transform.position).normalized * -entity.pullStrength, ForceMode.Impulse);
    
    if ((contact.transform.position - this.transform.position).magnitude > entity.pullCenter &&
        (contact.transform.position - this.transform.position).magnitude < distance) {
      StartCoroutine("pull");
    } else {
      pulling = false;
    }
  }
}
