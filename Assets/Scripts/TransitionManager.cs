using UnityEngine;

public class TransitionManager : MonoBehaviour
{
	private static readonly int DoTransition = Animator.StringToHash("DoTransition");
	public static TransitionManager Instance {get;set;}
	Animator anim;
	string DoorID; 
	Object Scene1; 
	Object Scene2;
	Canvas canvas;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		canvas = GetComponent<Canvas>();
		canvas.worldCamera = Camera.main;
	}

	private void Start()
	{
		anim = GetComponent<Animator>();
	}
	public void StartTransition(string doorID, Object scene1, Object scene2)
	{
		DoorID = doorID;
		Scene1 = scene1;
		Scene2 = scene2;
		anim.SetTrigger(DoTransition);
		Debug.Log($"{DoorID} {Scene1} {Scene2}");
	}
	public void FinishTransition()
	{
		Debug.Log("Loading");
		RoomChangeManager.Instance.LoadNextRoom(DoorID,Scene1,Scene2);
	}
}
