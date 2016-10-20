using UnityEngine;
using System.Collections;

public class AI_Controller_FollowPlayer : MonoBehaviour {
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
      if (entity.useHackingPostProcessings &&
          !ManagesPostProcessing.getInstance().hackingEntities.Contains(this.gameObject)) {
        ManagesPostProcessing.getInstance().hackingEntities.Add(this.gameObject);
      }

      if (!startedBehavior && entity.playActivationSound) {
        ManagesSound.getInstance().playSound(
          entity.activationSound,
          entity.activationSoundLevel);
      }

      if (entity.playCombatMusic && !ManagesSound.getInstance().hackingEntities.Contains(this.gameObject)) {
        ManagesSound.getInstance().hackingEntities.Add(this.gameObject);
      }

      if (entity.activationHint != null) {
        entity.activationHint.SetActive(true);
      }
      
      forceVector = contact.transform.position - this.transform.position;
      
      if (entity.flying != true) {
        forceVector.y = 0;
      }

      myBody.AddForce(forceVector.normalized * movementSpeed, ForceMode.Impulse);

      startedBehavior = true;
      return condition;
    }

    if (entity.useHackingPostProcessings &&
        ManagesPostProcessing.getInstance().hackingEntities.Contains(this.gameObject)) {
      ManagesPostProcessing.getInstance().hackingEntities.Remove(this.gameObject);
    }

    if (entity.activationHint != null) {
      entity.activationHint.SetActive(false);
    }

    if (entity.playCombatMusic &&
        ManagesSound.getInstance().hackingEntities.Contains(this.gameObject)) {
      ManagesSound.getInstance().hackingEntities.Remove(this.gameObject);
    }

    startedBehavior = false;
    return condition;
  }
}
