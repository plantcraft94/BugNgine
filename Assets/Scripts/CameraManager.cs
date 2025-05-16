using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
	public static CameraManager instance { get; set; }

	[SerializeField] private CinemachineCamera[] _allVirtualCameras;

	[Header("Controls for lerping the Y Damping during jump/fall")]
	[SerializeField] private float _fallPanAmount = 0.25f;
	[SerializeField] private float _fallYPanTime = 0.35f;
	public float _fallSpeedYDampingChangeThreshold = -15f;
	private Coroutine _lerpYPanCoroutine;
	private Coroutine _panCameraCoroutine;
	private CinemachineCamera _currentCamera;
	private CinemachinePositionComposer _framingComposer;
	private CinemachineConfiner2D _currentConfiner;
	float _normYPanAmount;
	public bool IsLerpingYDamping { get; private set; }
	public bool LerpedFromPlayerFalling;

	private Vector2 _startingtrackedObjectoffset;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		foreach (CinemachineCamera CinCameras in _allVirtualCameras)
		{
			if (CinCameras.enabled)
			{
				_currentCamera = CinCameras;

				_framingComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
				_currentConfiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
			}
		}

		_normYPanAmount = _framingComposer.Damping.y;

		_startingtrackedObjectoffset = _framingComposer.TargetOffset;
	}

	public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
	{
		_panCameraCoroutine = StartCoroutine(PanCameraCoroutine(panDistance, panTime, panDirection, panToStartingPos));
	}

	private IEnumerator PanCameraCoroutine(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
	{
		Vector2 endPos = Vector2.zero;
		Vector2 startingPos = Vector2.zero;

		// Set the direction and distance if we are panning in the direction indicated by the trigger object
		if (!panToStartingPos)
		{
			// Set the direction and distance
			switch (panDirection)
			{
				case PanDirection.Up:
					endPos = Vector2.up;
					break;
				case PanDirection.Down:
					endPos = Vector2.down;
					break;
				case PanDirection.Left:
					endPos = Vector2.left;
					break;
				case PanDirection.Right:
					endPos = Vector2.right;
					break;
				default:
					break;
			}

			endPos *= panDistance;

			startingPos = _startingtrackedObjectoffset;

			endPos += startingPos;
		}
		else
		{
			// Handle the direction settings when moving back to the starting position
			startingPos = _framingComposer.TargetOffset;
			endPos = _startingtrackedObjectoffset;
		}

		// Handle the actual panning of the camera
		float elapsedTime = 0f;
		while (elapsedTime < panTime)
		{
			elapsedTime += Time.deltaTime;

			Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
			_framingComposer.TargetOffset = panLerp;

			yield return null;
		}
	}

	public void SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
	{
		// If the current camera is the camera on the left and our trigger exit direction was on the right
		if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
		{
			// Activate the new camera
			cameraFromRight.enabled = true;

			// Deactivate the old camera
			cameraFromLeft.enabled = false;

			// Set the new camera as the current camera
			_currentCamera = cameraFromRight;

			// Update our composer variable
			_framingComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
		}
		// If the current camera is the camera on the right and our trigger exit direction was on the left
		else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
		{
			// Activate the new camera
			cameraFromLeft.enabled = true;

			// Deactivate the old camera
			cameraFromRight.enabled = false;

			// Set the new camera as the current camera
			_currentCamera = cameraFromLeft;

			// Update our composer variable
			_framingComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
		}
	}


	public void LerpYDamping(bool isPlayerFalling)
	{
		_lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
	}

	private IEnumerator LerpYAction(bool isPlayerFalling)
	{
		IsLerpingYDamping = true;

		// grab the starting damping amount
		float startDampAmount = _framingComposer.Damping.y;
		float endDampAmount = 6f;

		// determine the end damping amount
		if (isPlayerFalling)
		{
			endDampAmount = _fallPanAmount;
			LerpedFromPlayerFalling = true;
		}
		else
		{
			endDampAmount = _normYPanAmount;
		}

		// lerp the pan amount
		float elapsedTime = 0f;
		while (elapsedTime < _fallYPanTime)
		{
			elapsedTime += Time.deltaTime;

			float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
			_framingComposer.Damping.y = lerpedPanAmount;

			yield return null;
		}

		IsLerpingYDamping = false;
	}
	public void RelocateBound()
	{
		GameObject boundObject = GameObject.FindGameObjectWithTag("Bound");

		CompositeCollider2D collider = boundObject.GetComponent<CompositeCollider2D>();

		_currentConfiner.Damping = 0f;

		_currentConfiner.BoundingShape2D = collider;

		_currentConfiner.InvalidateBoundingShapeCache();
	}
	public void ResetCam()
	{
		StartCoroutine(Resetcam());
	}
	IEnumerator Resetcam()
	{
		
		yield return new WaitForSeconds(1f);
		_currentConfiner.Damping = 0.5f;
	}
}
