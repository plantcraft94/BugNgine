using Unity.Cinemachine;
using UnityEngine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
	public CustomInspectorObjects customInspectorObjects;
	Collider2D coll;
	private void Start()
	{
		coll = GetComponent<Collider2D>();
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (customInspectorObjects.panCameraOnContact)
			{
				CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
			}
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		
		if (collision.CompareTag("Player"))
		{
			Vector2 exitDir = (collision.transform.position - coll.bounds.center).normalized;
			if (customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
			{
				CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDir);
			}
			
			if (customInspectorObjects.panCameraOnContact)
			{
				CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
			}
		}
	}
}
[System.Serializable]
public class CustomInspectorObjects
{
	public bool swapCameras = false;
	public bool panCameraOnContact = false;

	[HideInInspector] public CinemachineCamera cameraOnLeft;
	[HideInInspector] public CinemachineCamera cameraOnRight;

	[HideInInspector] public PanDirection panDirection;
	[HideInInspector] public float panDistance = 3f;
	[HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{
	Up,
	Down,
	Left,
	Right
}
#if UNITY_EDITOR
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
	public CameraControlTrigger cameraControlTrigger;


	
	void OnEnable()
	{
		cameraControlTrigger = (CameraControlTrigger)target;
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (cameraControlTrigger.customInspectorObjects.swapCameras)
		{
			cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.cameraOnLeft, typeof(CinemachineCamera), true) as CinemachineCamera;

			cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.cameraOnRight, typeof(CinemachineCamera), true) as CinemachineCamera;
		}
		if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
		{
			cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControlTrigger.customInspectorObjects.panDirection);
			cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
			cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
		}
		if (GUI.changed)
		{
			EditorUtility.SetDirty(cameraControlTrigger);
		}
	}
}
#endif