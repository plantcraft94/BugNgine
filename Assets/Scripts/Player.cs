using UnityEngine;
using PrimeTween;

public class Player : MonoBehaviour
{
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsLedgeGrab = Animator.StringToHash("isLedgeGrab");
    public static Player Instance{get;set;}
	public PlayerMovement PM;
	PlayerLedgeGrab PLG;
	public bool BlockInput = false;
	public bool CanGoThroughDoors = true;
	public bool test;
	Animator anim;
	Rigidbody2D rb;
	[Header("Abilities")]
	public bool HasDash;

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
		anim.SetBool(IsGrounded,PM.isGrounded);
		anim.SetFloat(YVelocity,rb.linearVelocityY);
		anim.SetBool(IsMoving, PM.movement != 0);
		anim.SetBool(IsLedgeGrab, PLG.isGrab);
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
