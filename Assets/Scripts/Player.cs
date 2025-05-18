using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance{get;set;}
	public PlayerMovement PM;
	public bool BlockInput = false;
	public bool CanGoThroughDoors = true;
	public bool test;
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
	}
	private void Start()
	{
		PM = GetComponent<PlayerMovement>();
	}
	private void Update()
	{
		if(test)
		{
			BlockInput = true;
			startTransition(true, false);
		}
	}
	public void startTransition(bool HorizontalDooor, bool UpDoor)
	{
		if(HorizontalDooor && !UpDoor)
		{
			StartCoroutine(PM.TransitionMove());
		}
		else if(!HorizontalDooor && UpDoor)
		{
			StartCoroutine(PM.TransitionJump());
		}
		else if (!HorizontalDooor && !UpDoor)
		{
			StartCoroutine(PM.TransitionFall());
		}
	}
}
