using UnityEngine;
using PrimeTween;

public class Player : MonoBehaviour
{
	private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
	private static readonly int YVelocity = Animator.StringToHash("yVelocity");
	private static readonly int IsMoving = Animator.StringToHash("IsMoving");
	private static readonly int IsLedgeGrab = Animator.StringToHash("isLedgeGrab");
	
	private static readonly int isDashing = Animator.StringToHash("isDashing");
	private static readonly int isAttacking = Animator.StringToHash("IsAttacking");
	public static Player Instance{get;set;}
	public PlayerMovement PM;
	public PlayerLedgeGrab PLG;
	public bool BlockInput = false;
	public bool CanGoThroughDoors = true;
	public bool test;
	Animator anim;
	Rigidbody2D rb;
	
	[HideInInspector] public bool IsAttacking;
	
	[Header("Animations")]
	[SerializeField] RuntimeAnimatorController NoSword;
	[SerializeField] RuntimeAnimatorController Sword;
	
	

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
		anim = GetComponent<Animator>();
		PM = GetComponent<PlayerMovement>();
		rb = GetComponent<Rigidbody2D>();
		PLG = GetComponent<PlayerLedgeGrab>();
	}
	private void Start()
	{
		//anim.runtimeAnimatorController = Sword;
	}
	private void Update()
	{
		AnimatePlayer();
	}
	public void SwordAnimation()
	{
		anim.runtimeAnimatorController = Sword;
	}
	void AnimatePlayer()
	{
		anim.SetBool(IsGrounded,PM.isGrounded);
		anim.SetFloat(YVelocity,rb.linearVelocityY);
		anim.SetBool(IsMoving, PM.movement != 0 || (BlockInput && rb.linearVelocity.x != 0));
		anim.SetBool(IsLedgeGrab, PLG.isGrab);
		anim.SetBool(isDashing, PM.isDashing);
		anim.SetBool(isAttacking, IsAttacking);
	}
	
	public void Slash1()
	{
		anim.SetTrigger("Slash1");
	}
	public void Slash2()
	{
		anim.SetTrigger("Slash2");
	}
	public void SlashUp()
	{
		anim.SetTrigger("SlashUp");
	}
	public void SlashDown()
	{
		anim.SetTrigger("SlashDown");
	}
	
	
	
	
	
	
	
	
	public void startTransition(bool HorizontalDooor, bool UpDoor)
	{
		if (PM.isDashing)
		{
			PM.isDashing = false;
		}
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
