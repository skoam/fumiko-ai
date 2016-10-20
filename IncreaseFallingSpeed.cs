using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class IncreaseFallingSpeed : MonoBehaviour
{
	public ChecksGrounded groundCheck;

	[Range(0.0f, 9999999.9f)]
	public float
		maximumStrength;

	[Range(0.0f, 9999999.9f)]
	public float
		strengthIncrease;

  public float actualStrength = 0.0f;

  private Rigidbody myBody;
  private bool activatedParticles;

  public bool isPlayer;

	// Use this for initialization
	void Start () {
	  myBody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
    if (isPlayer) {
      if (Mathf.Abs(ManagesPlayer.getInstance().playerPhysicsController.velocity.y) > 80 && ! activatedParticles) {
        actualStrength += 50;
        ManagesGame.getInstance().utilityFunctions.particleEmission(ManagesPlayer.getInstance().fallingParticleSystem, true);
        activatedParticles = true;
      } else if (Mathf.Abs(ManagesPlayer.getInstance().playerPhysicsController.velocity.y) < 80) {
        ManagesGame.getInstance().utilityFunctions.particleEmission(ManagesPlayer.getInstance().fallingParticleSystem, false);
        activatedParticles = false;
      }
    }

		if (!groundCheck.checkGrounded ()) {
      if (isPlayer) {
        if (!ManagesPowerUps.getInstance().isPowerUpActive(PowerUpTypes.ANTI_GRAV))
        {
          actualStrength += strengthIncrease * Time.deltaTime;
        } else
        {
          actualStrength = 1.0f;
        }
      } else {
        actualStrength += strengthIncrease * Time.deltaTime;
      }
      
			if (actualStrength > maximumStrength) {
				actualStrength = maximumStrength;
			}

			float effectiveStrength = actualStrength * this.GetComponent<Rigidbody>().mass * Time.deltaTime;
			myBody.AddForce (Physics.gravity * effectiveStrength, ForceMode.Impulse);

		} else {
			resetStrength ();
		}
	}

	public void resetStrength ()
	{
		actualStrength = 0.0f;
	}
}

