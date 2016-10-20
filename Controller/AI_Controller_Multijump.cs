using UnityEngine;
using System.Collections;

public class AI_Controller_Multijump : MonoBehaviour {
  private int currentJump;
  private float currentTime;

  private AI_Entity entity;
  private Rigidbody myBody;

  private bool jumping;
  private bool jumped;

  private float GCdistance = 0.2f;
  private float GCradius = 0.2f;

  private bool isGrounded;

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

    if (jumped) {
      currentTime += entity.executionInterval;

      if (currentTime >= entity.timeBetweenJumps) {
        jumped = false;
      }
    }

    if (jumping) {
      myBody.AddForce(Vector3.up * entity.multiJumpStrength, ForceMode.Impulse);

      if (currentTime >= entity.jumpLength) {
        jumping = false;
      }
    }

    if (condition && currentJump < entity.multiJumps && !jumped) {
      jumping = true;
      jumped = true;
      currentJump++;
      currentTime = 0;
      
      if (!entity.groundCheck.checkGrounded()) {
        GameObject.Instantiate(entity.multiJumpCube, new Vector3(
          this.transform.position.x, this.transform.position.y - 1, this.transform.position.z),
          this.transform.rotation);
      }

      if (entity.fallSpeedAccelerator != null) {
        entity.fallSpeedAccelerator.resetStrength();
      }

      myBody.velocity = Vector3.zero;
      myBody.AddForce(Vector3.up * entity.initialJumpStrength, ForceMode.Impulse);

      if (entity.transmitAnimation != null) {
        if (entity.transmitAnimation.GetInteger("JumpType") == 1 || 
            entity.transmitAnimation.GetInteger("JumpType") == 3) {
          entity.transmitAnimation.SetInteger("JumpType", 2);
        } else if (entity.transmitAnimation.GetInteger("JumpType") == 2) {
          entity.transmitAnimation.SetInteger("JumpType", 3);
        } 
      }

      return condition;
    } 

    if (entity.groundCheck.checkGrounded()) {
      currentJump = 0;
      jumped = false;
      jumping = false;
      currentTime = 0;
    }
  
    return condition;
  }
}
