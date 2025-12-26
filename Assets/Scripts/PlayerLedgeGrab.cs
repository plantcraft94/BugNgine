using UnityEngine;
#if UNITY_EDITOR
using Physics2D = Nomnom.RaycastVisualization.VisualPhysics2D;
#else
using Physics2D = UnityEngine.Physics2D;
#endif

public class PlayerLedgeGrab : MonoBehaviour
{
	public bool canGrab = true;
	public bool isGrab;
	public Transform Check1;
	public Transform Check2;
	public float RayMultiplier;
	public LayerMask groundLayer;
	GameObject LedgeGrabObject;
	public bool SnapPlayer = false;
	PlayerMovement PM;
	Rigidbody2D rb;
	Animator anim;
	public bool GrabInput = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		PM = GetComponent<PlayerMovement>();
		anim = GetComponent<Animator>();
		LedgeGrabObject = transform.GetChild(1).GetChild(2).gameObject;
		LedgeGrabObject.SetActive(false);
	}
	// Update is called once per frame
	void Update()
	{
		RaycastHit2D hit1 = Physics2D.Raycast(Check1.position, Check1.right, 1f * RayMultiplier, groundLayer);
		RaycastHit2D hit2 = Physics2D.Raycast(Check2.position, Check2.right, 1f * RayMultiplier, groundLayer);

		if (!hit1 && hit2  && !PM.isGrounded && canGrab)
		{
			isGrab = true;
			LedgeGrabObject.SetActive(true);


		}
		if (isGrab)
		{
			Player.Instance.BlockInput = true;
			if (rb.linearVelocityY > 0)
			{
				rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.1f);
			}
			if (SnapPlayer)
			{
				if (PM.IsFacingRight)
				{
					rb.linearVelocity = new Vector2(rb.linearVelocityX + 500f * 1, rb.linearVelocityY);
					
				}
				else
				{
					rb.linearVelocity = new Vector2(rb.linearVelocityX + 500f * -1, rb.linearVelocityY);
				}
			}

		}
		if (GrabInput)
		{

			if (Mathf.Approximately(PM.lookUpDownAction.ReadValue<float>(), 1f)||((Mathf.Approximately(PM.moveAction.ReadValue<float>(), 1f) && PM.IsFacingRight) || (Mathf.Approximately(PM.moveAction.ReadValue<float>(), -1f) && !PM.IsFacingRight)))
			{
				
				anim.SetTrigger("Climb");
	   
			}
			else if ((Mathf.Approximately(PM.lookUpDownAction.ReadValue<float>(), -1) && PM.jumpAction.WasPressedThisFrame())||!canGrab)
			{
				isGrab = false;
				LedgeGrabObject.SetActive(false);
				Player.Instance.BlockInput = false;
				GrabInput = false;
			}
		}
	}
	public void SnapPlayerToLedge()
	{
		SnapPlayer = true;
	}
	public void BeginRecieveInput()
	{
		GrabInput = true;
	}
	public void StopSnapPlayerToLedge()
	{
		SnapPlayer = false;
	}
	public void FinishClimb()
	{
		if (PM.IsFacingRight)
		{
			transform.position = new Vector2(transform.position.x + 1f, transform.position.y + 2.25f);
		}
		else
		{
			transform.position = new Vector2(transform.position.x - 1f, transform.position.y + 2.25f);
		}
		UnityEngine.Physics2D.SyncTransforms();
		anim.ResetTrigger("Climb");
		isGrab = false;
		Player.Instance.BlockInput = false;
		LedgeGrabObject.SetActive(false);
		GrabInput = false;

	}
}
