using UnityEngine;
using System.Collections;

public class AI_Controller_Dash : MonoBehaviour {
  private float currentDashLength;

  private AI_Entity entity;
  private Rigidbody myBody;

  private bool dashing;

  private float originalMass;
  private float originalDrag;

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

    if (condition && !dashing && currentDashLength > entity.dashLength) {
      currentDashLength += entity.executionInterval;

      if (currentDashLength > entity.timeBetweenDashs) {
        currentDashLength = 0;
      } else {
        return false;
      }
    }

    if (dashing) {
      if (entity.fallSpeedAccelerator != null) {
        entity.fallSpeedAccelerator.resetStrength();
      }
    }

    if (condition && !dashing) {
      dashing = true;
      currentDashLength = 0;

      ManagesGame.getInstance().utilityFunctions.particleEmission(entity.dashEffect, true);
      
      originalMass = myBody.mass;
      originalDrag = myBody.drag;

      myBody.mass = entity.dashMass;
      myBody.drag = entity.dashDrag;

      if (entity.dashRenderer != null) {
        entity.dashRenderer.enabled = false;
      }
      
      return condition;
    } 

    if (condition && dashing) {
      currentDashLength += entity.executionInterval;

      if (currentDashLength > entity.dashLength) {
        condition = false;
      }
    }

    if (!condition && dashing) {
      dashing = false;
      
      myBody.mass = originalMass;
      myBody.drag = originalDrag;

      ManagesGame.getInstance().utilityFunctions.particleEmission(entity.dashEffect, false);

      if (entity.dashRenderer != null) {
        if (entity.deathController != null && !entity.deathController.hasDied()) {
          entity.dashRenderer.enabled = true;
        }
      }
    }

    return condition;
  }
}
