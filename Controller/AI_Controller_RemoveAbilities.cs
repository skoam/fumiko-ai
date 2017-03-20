using UnityEngine;
using System.Collections;

public class AI_Controller_RemoveAbilities : MonoBehaviour {
  public bool active;
  
  private AI_Entity entity;

  void Start () {
    entity = this.gameObject.GetComponent<AI_Entity>();
  }

  public bool doBehavior (bool condition) {
    if (condition && !active) {
      if (entity == null) {
        entity = this.gameObject.GetComponent<AI_Entity>();
      }

      if (entity.pushOneShotParticles != null) {
        entity.pushOneShotParticles.Emit(entity.pushOneShotAmount);
      }

      if (!ManagesPostProcessing.getInstance().hackingEntities.Contains(this.gameObject)) {
        ManagesPostProcessing.getInstance().hackingEntities.Add(this.gameObject);
      }
      
      if (ManagesPlayer.getInstance().playerValues.canUseAbilities) {
        if (entity.playRemoveAbilitiesSound) {
          entity.playSound();
        }
      }

      ManagesPlayer.getInstance().canUseAbilities(false);

      ManagesGame.getInstance().utilityFunctions.particleEmission(
        ManagesPlayer.getInstance().noAbilityParticleSystem, true);
      
      active = true;
    } else if (condition && active) {
      if (entity.pushOneShotParticles != null) {
        entity.pushOneShotParticles.Emit(entity.pushOneShotAmount);
      }
    } else if(!condition && active) {
      ManagesPlayer.getInstance().canUseAbilities(true);

      if (ManagesPostProcessing.getInstance().hackingEntities.Contains(this.gameObject)) {
        ManagesPostProcessing.getInstance().hackingEntities.Remove(this.gameObject);
      }

      ManagesGame.getInstance().utilityFunctions.particleEmission(
        ManagesPlayer.getInstance().noAbilityParticleSystem, false);

      active = false;
    }

    return condition;
  }
}
