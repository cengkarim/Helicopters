using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {

	public GameObject playerGameObject;
	public Texture altimeterTexture;
	public Texture[] throttleTexture;

	private float helicopterThrottle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnGUI ( )
	{
		RaycastHit groundHit;
		Physics.Raycast (playerGameObject.transform.position - Vector3.up, -Vector3.up, out groundHit);

		helicopterThrottle = (playerGameObject.GetComponent<HelicopterController> ()).rotorVelocity;

		GUI.Label (new Rect (0, 0, 128, 128), altimeterTexture);
		GUI.Label (new Rect (0, 128, 128, 128), throttleTexture [(int)(helicopterThrottle * 10)]);

		GUI.Label(new Rect(40,40,256,256),Mathf.Round(groundHit.distance) + "m");
		GUI.Label (new Rect (20, 182, 256, 256), "ENG");

	}
}
