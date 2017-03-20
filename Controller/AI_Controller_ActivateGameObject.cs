using UnityEngine;
using System.Collections;

public class AI_Controller_ActivateGameObject : MonoBehaviour {
  public bool ended;
  
  private AI_Entity entity;

  void Start () {
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (condition && !ended) {
      if (entity == null) {
        entity = this.gameObject.GetComponent<AI_Entity>();
      }

      foreach (GameObject target in entity.activateTargets) {
        target.SetActive(entity.activate);
      }

      foreach (Trigger_IncreaseTriggerValue increaseTriggerValue in entity.runIncreaseTrigger) {
        increaseTriggerValue.run();
      }
      
      if (!entity.repeatActivation) {
        ended = true;
      }
    }

    return condition;
  }
}
