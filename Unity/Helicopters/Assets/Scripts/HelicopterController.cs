using UnityEngine;
using System.Collections;

public class HelicopterController : MonoBehaviour {


	public GameObject mainRotor;
	//public GameObject tailRotor;

	public Camera MainCamera;
	public Camera ThirdPersonCamera;

	public float maxRotorForce = 22241.1081f;
	public float maxRotorVelocity = 7200.0f;
	public float rotorVelocity = 0.0f;
	private float rotorRotation = 0.0f;

	public float maxTailRotorForce = 15000.0f;
	public float maxTailRotorVelocity = 2200.0f;
	public float tailRotorVelocity = 0.0f;
	private float tailRotorRotation = 0.0f;

	public float forwardRotorTorqueMultiplier = 0.5f;
	public float sidewaysRotorTorqueMultiplier = 0.5f;

	public bool mainRotorActive = true;
	public bool tailRotorActive = true;

	private Rigidbody _myRigidBody;
	private Transform _myTransform;
	private AudioSource _myAudioSource;

	private Vector3 _initialPosition;
	private Quaternion _initialRotation;

	// Use this for initialization
	void Start () {

		_myRigidBody = this.GetComponent<Rigidbody> ();
		_myTransform = this.GetComponent<Transform> ();
		_myAudioSource = this.GetComponent<AudioSource> ();

		_initialPosition = _myTransform.position;
		_initialRotation = _myTransform.rotation;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (mainRotorActive) {
			mainRotor.transform.rotation = transform.rotation * Quaternion.Euler (270, rotorRotation, 0);
		}

		//if (tailRotorActive) {
			//tailRotor.transform.rotation = transform.rotation * Quaternion.Euler (tailRotorRotation, 0, 0);
		//}

		rotorRotation += maxRotorVelocity * rotorVelocity * Time.deltaTime;
		tailRotorRotation += maxTailRotorVelocity * rotorVelocity * Time.deltaTime;

		var hoverRotorVelocity = _myRigidBody.mass * Mathf.Abs (Physics.gravity.y) / maxRotorForce;
		var hovertailRotorVelocity = maxRotorForce * rotorVelocity / maxTailRotorForce;

		if (Input.GetAxis ("R2") != 0.0f) {
			rotorVelocity += Input.GetAxis ("R2") * 0.001f;
		} else {
			rotorVelocity = Mathf.Lerp (rotorVelocity, hoverRotorVelocity, Time.deltaTime * Time.deltaTime * 5);
		}
		tailRotorVelocity = hovertailRotorVelocity - Input.GetAxis ("HorizontalSteer");

		if (rotorVelocity > 1.0f) {
			rotorVelocity = 1.0f;
		} else if (rotorVelocity < 0.0f) {
			rotorVelocity = 0.0f;
		}

		_myAudioSource.pitch = rotorVelocity;

		if (Input.GetKeyDown (KeyCode.R) || Input.GetButtonDown ("Fire2")) {
			_myTransform.rotation = _initialRotation;
			_myTransform.position = _initialPosition;
		}

		if (Input.GetButtonDown ("Fire1")) {

			MainCamera.enabled = !MainCamera.enabled;
			MainCamera.GetComponent<AudioListener> ().enabled = MainCamera.enabled;

			ThirdPersonCamera.enabled = !MainCamera.enabled;
			ThirdPersonCamera.GetComponent<AudioListener> ().enabled = ThirdPersonCamera.enabled;
		}
	}

	void FixedUpdate ( )
	{
		var torqueValue = Vector3.zero;

		var controlTorque = new Vector3 (
			                    -Input.GetAxis ("Vertical") * forwardRotorTorqueMultiplier,
			                    1.0f,
			                    Input.GetAxis ("Horizontal") * sidewaysRotorTorqueMultiplier
		                    );

		if (mainRotorActive) {
			torqueValue += controlTorque * maxRotorForce * rotorVelocity;
			_myRigidBody.AddRelativeForce (Vector3.up * maxRotorForce * rotorVelocity);
		}

		if (Vector3.Angle (Vector3.up, _myTransform.up) < 80) {
			_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation,
				Quaternion.Euler (0, _myTransform.eulerAngles.y, 0),
				Time.deltaTime * rotorVelocity * 2.0f);
		}

		if (tailRotorActive) {
			torqueValue -= Vector3.up * maxTailRotorForce * tailRotorVelocity;
		}

		_myRigidBody.AddRelativeTorque (torqueValue);
	}
}
