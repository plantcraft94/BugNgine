using System.Collections;
using UnityEngine;
using PrimeTween;

public class CameraFollowObject : MonoBehaviour
{
	public static CameraFollowObject Instance{get;private set;}
	[Header("References")]
	[SerializeField] private Transform _playerTransform;

	[Header("Flip Rotation Stats")]
	[SerializeField] private float _flipYRotationTime = 0.5f;
	
	[SerializeField] private PlayerMovement _player;

	private bool _isFacingRight;

	// Unity Message | References
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		_player = _playerTransform.gameObject.GetComponent<PlayerMovement>();
		_isFacingRight = _player.IsFacingRight;
	}

	// Unity Message | References
	private void Update()
	{
		// Make the cameraFollowObject follow the player's position
		transform.position = _playerTransform.position;
	}

	public void CallTurn()
	{
		Tween.Rotation(transform,endValue: Quaternion.Euler(0,DetermineEndRotation(),0),duration: _flipYRotationTime, ease: Ease.InOutSine);
	}


	private float DetermineEndRotation()
	{
		_isFacingRight = !_isFacingRight;

		if (_isFacingRight)
		{
			return 0;
		}
		else
		{
			return 180f;
		}
	}

}
