using UnityEngine;
using System.Collections;

public class ChecksGrounded : MonoBehaviour
{
	public float radius;
	public float distance;
  
  public bool showGroundHitMarker;
  public GameObject marker;
  public GameObject markerFloaters;

	private bool isGrounded;

  void Start () {
    marker = GameObject.FindGameObjectWithTag ("positionMarker");
    if (marker == null) {
      ManagesGame.getInstance ().gameSettings.showGroundHitMarker = false;
    }
  }
	
	// Update is called once per frame
	void Update () {
    if (showGroundHitMarker && ManagesGame.getInstance().gameSettings.showGroundHitMarker &&
        !isGrounded) {
      ManagesGame.getInstance().utilityFunctions.particleEmission(marker.GetComponent<ParticleSystem> (), true);
      RaycastHit groundHit;
      if (Physics.Raycast(this.transform.position, -Vector3.up, out groundHit, 1000.0f)) {
        marker.transform.position = groundHit.point + new Vector3(0, 0.1f, 0);
      }
    } else {
      ManagesGame.getInstance().utilityFunctions.particleEmission(marker.GetComponent<ParticleSystem> (), false);
    }

		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position + (Vector3.up * radius), radius, Vector3.down, distance + radius);

		foreach (RaycastHit hit in hits) {
			if (hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "AI_Entity") {
				isGrounded = true;
				return;
			}
		}

		isGrounded = false;
	}

	public bool checkGrounded ()
	{
		return isGrounded;
	}
}

