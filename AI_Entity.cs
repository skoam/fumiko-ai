using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Entity : MonoBehaviour {
  public AI_Settings[] behaviors;

  [Header("Contact Settings")]
  public AI_Contact contact;
  public GameObject contactRepresentation;
  public bool recheckAIContact;
  public float recheckInterval;

  private float currentRecheckInterval;
  private bool foundContact;

  [Header("Team Settings")]
  public AI_Team myTeam;
  public AI_Team contactTeam;

  [Header("Execution Settings")]
  public float executionInterval = 0.5f;
  private float currentExecutionStep = 0;
  public bool failRandomly;
  [Range(0, 100)]
  public int failChance;
  public float failLength;
  private float currentFailLength;
  
  [Header("Post Processing")]
  public bool useHackingPostProcessings;
  
  [Header("Sound")]
  public bool playActivationSound;
  public bool playRemoveAbilitiesSound;
  public bool playPullSound;
  public bool playPushSound;
  public Sounds activationSound;
  public AudioSource audioSource;
  public float activationSoundLevel;
  public bool playCombatMusic;
  
  [Header("Activate Target Settings")]
  public bool activate;
  public bool repeatActivation;
  public GameObject[] activateTargets;
  public Trigger_IncreaseTriggerValue[] runIncreaseTrigger;
  
  [Header("Activation Hint")]
  public GameObject activationHint;
  public bool keepHintActive;
  
  [Header("States")]
  public bool off;
  public bool flying = true;
  
  [Header("Dash Settings")]
  public float dashLength;
  public float dashMass;
  public float dashDrag;
  public float timeBetweenDashs;
  public Renderer dashRenderer;
  public ParticleSystem dashEffect;
  
  [Header("Multijump Settings")]
  public int multiJumps;
  public float initialJumpStrength;
  public float multiJumpStrength;
  public float jumpLength;
  public float timeBetweenJumps;
  public GameObject multiJumpCube;
  public ChecksGrounded groundCheck;
  public IncreaseFallingSpeed fallSpeedAccelerator;
  
  [Header("Animation")]
  public Animator transmitAnimation;
  
  [Header("TAG Contact tag")]
  public string contactTag;
  
  [HideInInspector]
  public CanDie deathController;

  [Header("Push Settings")]
  public ParticleSystem pushOneShotParticles;
  public int pushOneShotAmount;
  
  [Header("Pull Settings")]
  public float pullStrength;
  public float pullCenter;

  private bool reachedMaximumPriority;
  private int maximumPriority = 10;

  private AI_Entity contactAIController;
  private List<GameObject> contactTeamEntities = new List<GameObject>();

  private AI_Settings lastBehavior;
  private AI_Settings currentBehavior;
  private AI_Settings futureBehavior;

  private Vector3 home;

	// Use this for initialization
	void Start () {
    home = this.transform.position;
    updateContact();
	}

  public void updateContact () {
    if (contact == AI_Contact.PLAYER) {
      contactRepresentation = GameObject.FindGameObjectWithTag("Player");
    }

    if (contact == AI_Contact.TAG) {
      contactRepresentation = GameObject.FindGameObjectWithTag(contactTag);
    }

    if (contact == AI_Contact.RANDOM_FROM_TEAM) {
      contactRepresentation = nearest_team_contact(true);
    }

    if (contact == AI_Contact.NEAREST_FROM_TEAM) {
      contactRepresentation = nearest_team_contact(false);
    }
    
    if (deathController == null) {
      deathController = this.GetComponent<CanDie>();
    }

    if (contactRepresentation != null) {
      contactAIController = contactRepresentation.GetComponent<AI_Entity>();
    }
  }

  public void playSound () {
    if (audioSource != null) {
      if (audioSource.volume > ManagesSound.getInstance().soundFXVolumeMax) {
        audioSource.volume = ManagesSound.getInstance().soundFXVolumeMax;
      }

      audioSource.clip = ManagesSound.getInstance().soundFile(activationSound);
      audioSource.volume = activationSoundLevel;
      audioSource.Play();
    } else {
      ManagesSound.getInstance().playSound(
        activationSound,
        activationSoundLevel);
    }
  }

  public GameObject nearest_team_contact (bool randomly) {
    contactTeamEntities.Clear();
    GameObject[] team_entities = GameObject.FindGameObjectsWithTag("AI_Entity");
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    
    foreach (GameObject entity in team_entities) {
      contactTeamEntities.Add(entity);
    }
    
    if (contactTeam == AI_Team.PLAYER) {
      contactTeamEntities.Add(GameObject.FindGameObjectWithTag("Player"));
    }

    float lastDistance = 0;
    float currentDistance;
    GameObject nearestEntity = null;

    if (contactTeamEntities.Count > 0) {
      if (randomly) {
        // currently not random, only first found, to add later
        foreach (GameObject entity in contactTeamEntities) {
          AI_Entity entityAI = entity.GetComponent<AI_Entity>();
          if (entity != this.gameObject && entityAI.myTeam == contactTeam) {
            if (entityAI.deathController == null || !entityAI.deathController.hasDied()) {
              return entity;
            }
          }
        }
      } else {
        foreach (GameObject entity in contactTeamEntities) {
          AI_Entity entityAI = entity.GetComponent<AI_Entity>();
          if ((entityAI != null && entity != this.gameObject && entityAI.myTeam == contactTeam) ||
              (contactTeam == AI_Team.PLAYER && entity == player)) {
            if (entityAI == null || entityAI.deathController == null || !entityAI.deathController.hasDied()) {
              currentDistance = (entity.transform.position - this.transform.position).magnitude;

              if (currentDistance < lastDistance || lastDistance == 0) {
                nearestEntity = entity;
                lastDistance = currentDistance;
              }
            }
          }
        }

        if (nearestEntity != null) {
          return nearestEntity;
        }
      }
    } 
    
    if (contactTeam == AI_Team.PLAYER) {
      return GameObject.FindGameObjectWithTag("Player");
    }
    
    return null;
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    currentExecutionStep += 1 * Time.deltaTime;
    currentRecheckInterval += 1 * Time.deltaTime;
    
    if (recheckAIContact) {
      if (currentRecheckInterval > recheckInterval) {
        currentRecheckInterval = 0;
        updateContact();
      }
    }
     
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
        } else if (behavior.type == AI_Behavior.RUNAWAY) {
          AI_Controller_RunAway controller = this.gameObject.GetComponent<AI_Controller_RunAway>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_RunAway>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }
          
          check = check_for_contact(behavior);
          controller.contact = contactRepresentation;
          controller.movementSpeed = behavior.movementSpeed;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.PULL) {
          AI_Controller_Pull controller = this.gameObject.GetComponent<AI_Controller_Pull>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_Pull>();
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
          controller.distance = behavior.distance;

          if (controller.doBehavior(check)) {
            reachedMaximumPriority = true;
          }
        } else if (behavior.type == AI_Behavior.REMOVE_ABILITIES) {
          AI_Controller_RemoveAbilities controller = this.gameObject.GetComponent<AI_Controller_RemoveAbilities>();

          if (controller == null) {
            this.gameObject.AddComponent<AI_Controller_RemoveAbilities>();
            continue;
          }

          if (behavior.priority != p) {
            continue;
          }

          if (ManagesGame.getInstance().noHostileAI) {
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

    if (deathController != null && deathController.hasDied()) {
      return false;
    }

    if (contact == AI_Contact.RANDOM_FROM_TEAM) {
      if (contactAIController != null && !contactAIController.off && contactAIController.deathController != null &&
          contactAIController.deathController.hasDied()) {
        updateContact();
      } else if (contactRepresentation == null) {
        updateContact();
      }
    }

    if (contact == AI_Contact.NEAREST_FROM_TEAM) {
      if (contactAIController != null && !contactAIController.off && contactAIController.deathController != null &&
          contactAIController.deathController.hasDied()) {
        updateContact();
      } else if (contactRepresentation == null) {
        updateContact();
      }
    }

    if (contactRepresentation == null) {
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
        } else if (contact == AI_Contact.PREDEFINED || contact == AI_Contact.TAG ||
                   contact == AI_Contact.RANDOM_FROM_TEAM || contact == AI_Contact.NEAREST_FROM_TEAM) {
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
