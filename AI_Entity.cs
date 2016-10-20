using UnityEngine;
using System.Collections;

public class AI_Entity : MonoBehaviour {
  
  public AI_Settings[] behaviors;

  public AI_Contact contact;
  public GameObject contactRepresentation;

  private AI_Settings lastBehavior;
  private AI_Settings currentBehavior;
  private AI_Settings futureBehavior;

  private bool foundContact;

  public float executionInterval = 0.5f;
  private float currentExecutionStep = 0;

  private Vector3 home;

  public bool useHackingPostProcessings;

  public bool playActivationSound;
  public Sounds activationSound;
  public float activationSoundLevel;
  
  public bool playCombatMusic;
  
  public bool activate;
  public bool repeatActivation;
  public GameObject[] activateTargets;

  public GameObject activationHint;

  public bool off;

  public bool flying = true;
  
  public float dashLength;
  public float dashMass;
  public float dashDrag;
  public float timeBetweenDashs;
  public Renderer dashRenderer;
  public ParticleSystem dashEffect;
  
  public int multiJumps;
  public float initialJumpStrength;
  public float multiJumpStrength;
  public float jumpLength;
  public float timeBetweenJumps;
  public GameObject multiJumpCube;
  public ChecksGrounded groundCheck;

  public Animator transmitAnimation;

  private bool reachedMaximumPriority;
  private int maximumPriority = 10;

  public IncreaseFallingSpeed fallSpeedAccelerator;

  public bool failRandomly;

  [Range(0, 100)]
  public int failChance;
  public float failLength;
  private float currentFailLength;

  // Use this for initialization
  void Start () {
    home = this.transform.position;

    if (contact == AI_Contact.PLAYER) {
      contactRepresentation = GameObject.FindGameObjectWithTag("Player");
    }
  }
	
  // Update is called once per frame
  void FixedUpdate () {
    currentExecutionStep += 1 * Time.deltaTime;
    
    if (currentFailLength > 0) {
      currentFailLength -= 1 * Time.deltaTime;
      return;
    }

    if (currentExecutionStep >= executionInterval) {
      executeBehaviors();
      currentExecutionStep = 0;
    }
  }
    
  private bool check;
  private AI_Settings behavior;
  void executeBehaviors () {
    for (int p = 0; p < maximumPriority; p++) {
      for (int i = 0; i < behaviors.Length; i++) {
        behavior = behaviors[i];

        if (behavior.type == AI_Behavior.FOLLOW_PLAYER) {
          AI_Controller_FollowPlayer controller = this.gameObject.GetComponent<AI_Controller_FollowPlayer>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_FollowPlayer>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }

          if (ManagesGame.getInstance().noHostileAI) {
            continue;
          }
          
          check = check_for_contact(behavior);
          controller.contact = contactRepresentation;
          controller.movementSpeed = behavior.movementSpeed;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.RETURN) {
          AI_Controller_Return controller = this.gameObject.GetComponent<AI_Controller_Return>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_Return>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }

          check = check_for_contact(behavior);
          controller.home = home;
          controller.movementSpeed = behavior.movementSpeed;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.MOVE_RANDOMLY) {
          AI_Controller_RandomMovement controller = this.gameObject.GetComponent<AI_Controller_RandomMovement>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_RandomMovement>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }

          check = check_for_contact(behavior);
          controller.movementSpeed = behavior.movementSpeed;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.PUSH_PLAYER) {
          AI_Controller_PushPlayer controller = this.gameObject.GetComponent<AI_Controller_PushPlayer>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_PushPlayer>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }

          if (ManagesGame.getInstance().noHostileAI) {
            continue;
          }
          
          check = check_for_contact(behavior);
          controller.contact = contactRepresentation;
          controller.movementSpeed = behavior.movementSpeed;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.ACTIVATE_OBJECT) {
          AI_Controller_ActivateGameObject controller = this.gameObject.GetComponent<AI_Controller_ActivateGameObject>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_ActivateGameObject>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }
          
          check = check_for_contact(behavior);

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.DASH) {
          AI_Controller_Dash controller = this.gameObject.GetComponent<AI_Controller_Dash>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_Dash>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }
          
          check = check_for_contact(behavior);

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.MULTIJUMP) {
          AI_Controller_Multijump controller = this.gameObject.GetComponent<AI_Controller_Multijump>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_Multijump>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }
          
          check = check_for_contact(behavior);

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        }

      }

      if (reachedMaximumPriority) {
        p = maximumPriority;
        reachedMaximumPriority = false;
      }
    }
  }

  private Collider[] hits;
  IsPlayer player;
  bool check_for_contact (AI_Settings settings) {
    if (off) {
      return false;
    }

    if (failRandomly) {
      if (Random.Range(0, 100) < failChance) {
        currentFailLength = failLength;
        return false;
      }
    }

    if (settings.condition == AI_Condition.ALWAYS) {
      return true;
    }

    if (settings.condition == AI_Condition.IN_RANGE ||
        settings.condition == AI_Condition.OUT_OF_RANGE) {
      // get all objects in the nearest area 
      hits = Physics.OverlapSphere(this.transform.position, settings.distance);

      for (int i = 0; i < hits.Length; i++) {
        if (contact == AI_Contact.PLAYER) {
          if (hits[i].gameObject.tag == "Player" && !foundContact) {
            // Has detected contact
            if (hits[i].gameObject.GetComponent<CanDie>().hasDied()) {
              return false;
            }

            if (settings.condition == AI_Condition.IN_RANGE) {
              return true;
            } else {
              return false;
            }
          } 
        } else if (contact == AI_Contact.PREDEFINED) {
          if (hits[i].gameObject == contactRepresentation) {
            if (hits[i].gameObject.GetComponent<CanDie>() != null && hits[i].gameObject.GetComponent<CanDie>().hasDied()) {
              return false;
            }

            if (settings.condition == AI_Condition.IN_RANGE) {
              return true;
            } else {
              return false;
            }
          } 
        }
      }
      if (settings.condition == AI_Condition.OUT_OF_RANGE) {
        return true;
      } else {
        return false;
      }
    }

    if (settings.condition == AI_Condition.BELOW) {
      if (this.transform.position.y - contactRepresentation.transform.position.y < settings.distance) {
        return true;
      }
    }

    if (settings.condition == AI_Condition.AT_HOME) {
      if (Vector3.Distance(this.transform.position, home) < settings.distance) {
        return true;
      }
    }


    if (settings.condition == AI_Condition.AWAY_FROM_HOME) {
      if (Vector3.Distance(this.transform.position, home) > settings.distance) {
        return true;
      }
    }
    
    // could not find condition
    return false;
  }
}
