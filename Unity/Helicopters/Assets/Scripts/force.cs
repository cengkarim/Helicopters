using UnityEngine;
using System.Collections;

public class force : MonoBehaviour {

	public float inputMultiplier = 3.0f;
	public float directionMultiplier = 0.9f;
	public float steerMultiplier = 1.0f;
	public float maxForce = 4.0f;
	public float rotationLimit = 20.0f;

	public GameObject frontLeft;
	public GameObject frontRight;
	public GameObject backLeft;
	public GameObject backRight;
	public GameObject RotateLeft;
	public GameObject RotateRight;

	public Camera MainCamera;
	public Camera FirstPersonCamera;

	private Rigidbody _myRigidbody;
	private Transform _myTransform;
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;

	// Use this for initialization
	void Start () {
		_myRigidbody = GetComponent<Rigidbody> ();
		_myTransform = GetComponent<Transform> ();
		_initialPosition = _myTransform.position;
		_initialRotation = _myTransform.rotation;
	}

	void Update ( )
	{
		if (Input.GetKeyDown (KeyCode.R) || Input.GetButtonDown ("Fire2")) {
			_myTransform.rotation = _initialRotation;
			_myTransform.position = _initialPosition;
		}

		if (Input.GetButtonDown ("Fire1")) {
			MainCamera.enabled = !MainCamera.enabled;
			MainCamera.GetComponent<AudioListener> ().enabled = MainCamera.enabled;
			FirstPersonCamera.enabled = !MainCamera.enabled;
			FirstPersonCamera.GetComponent<AudioListener> ().enabled = FirstPersonCamera.enabled;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		var inputVal = Mathf.Clamp ( Input.GetAxisRaw ("R2") * inputMultiplier, 0.0f, maxForce );

		var verticalInput = Input.GetAxis ("Vertical");
		var forwardInput = Mathf.Clamp (verticalInput, 0, 1);
		var forwardMultiplier =  Mathf.Lerp ( 0, directionMultiplier * forwardInput,  forwardInput  );
		var backwardInput = - Mathf.Clamp (verticalInput, -1, 0);
		var backwardMultiplier = Mathf.Lerp ( 0, directionMultiplier * backwardInput, backwardInput );


		var horizontalInput = Input.GetAxis ("Horizontal");
		var rightInput = Mathf.Clamp (horizontalInput, 0, 1);
		var rightMultiplier =  Mathf.Lerp ( 0, directionMultiplier * rightInput,  rightInput  );
		var leftInput = - Mathf.Clamp (horizontalInput, -1, 0);
		var leftMultiplier = Mathf.Lerp ( 0, directionMultiplier * leftInput, leftInput );

		_myRigidbody.AddForceAtPosition ((backwardMultiplier + leftMultiplier + inputVal) * _myTransform.up, frontRight.transform.position);
		_myRigidbody.AddForceAtPosition ((forwardMultiplier + leftMultiplier + inputVal) * _myTransform.up, backRight.transform.position);
		_myRigidbody.AddForceAtPosition ((backwardMultiplier + rightMultiplier + inputVal) * _myTransform.up, frontLeft.transform.position);
		_myRigidbody.AddForceAtPosition ((forwardMultiplier + rightMultiplier + inputVal) * _myTransform.up, backLeft.transform.position);


		var steerInput = Input.GetAxis ("HorizontalSteer");
		var rightSteerInput = Mathf.Clamp (steerInput, 0, 1);
		var rightSteerMultiplier =  Mathf.Lerp ( 0, steerMultiplier * rightSteerInput,  rightSteerInput  );
		var leftSteerInput = - Mathf.Clamp (steerInput, -1, 0);
		var leftSteerMultiplier = Mathf.Lerp ( 0, steerMultiplier * leftSteerInput, leftSteerInput );

		_myRigidbody.AddForceAtPosition (rightSteerMultiplier * _myTransform.right, RotateLeft.transform.position);
		_myRigidbody.AddForceAtPosition (leftSteerMultiplier * - _myTransform.right, RotateRight.transform.position);
	
	}

	void LateUpdate ( )
	{
		_myRigidbody.transform.eulerAngles = new Vector3 (
			ClampAngle360(_myRigidbody.transform.eulerAngles.x, rotationLimit),
			_myRigidbody.transform.eulerAngles.y,
			ClampAngle360(_myRigidbody.transform.eulerAngles.z, rotationLimit)
		);
	}

	public static float ClampAngle360 ( float value, float limit )
	{
		if (value > limit && value <= 180.0f)
			return limit;

		if (value < 360.0f - limit && value > 180.0f)
			return 360.0f - limit;

		return value;
	}
}
