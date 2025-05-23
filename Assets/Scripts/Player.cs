using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance{get;set;}
	public PlayerMovement PM;
	PlayerLedgeGrab PLG;
	public bool BlockInput = false;
	public bool CanGoThroughDoors = true;
	public bool test;
	Animator anim;
	Rigidbody2D rb;
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
		anim = GetComponent<Animator>();
		PM = GetComponent<PlayerMovement>();
		rb = GetComponent<Rigidbody2D>();
		PLG = GetComponent<PlayerLedgeGrab>();
	}
	private void Update()
	{
		AnimatePlayer();
	}
	void AnimatePlayer()
	{
		anim.SetBool("IsGrounded",PM.isGrounded);
		anim.SetFloat("yVelocity",rb.linearVelocityY);
		anim.SetBool("IsMoving", PM.movement != 0);
		anim.SetBool("isLedgeGrab", PLG.isGrab);
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
