using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TransmitMovementByVelocity : MonoBehaviour
{
	public MovementParameterRepresentations parameters;
	public Animator transmitTarget;

  private Rigidbody myBody;
  private Vector3 horizontalVelocity;

	// Use this for initialization
	void Start ()
	{
    myBody = this.GetComponent<Rigidbody>();
		transmitData ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transmitData ();
	}

	private void transmitData ()
	{ 
    horizontalVelocity = myBody.velocity;
		horizontalVelocity.y = 0;
    
    if (myBody.velocity.magnitude > 0.1f) {
      transmitTarget.SetFloat (parameters.verticalSpeed, myBody.velocity.y);
      transmitTarget.SetFloat (parameters.horizontalSpeed, horizontalVelocity.magnitude);
    } else {
      transmitTarget.SetFloat (parameters.verticalSpeed, 0);
      transmitTarget.SetFloat (parameters.horizontalSpeed, 0);
    }
	}
}
