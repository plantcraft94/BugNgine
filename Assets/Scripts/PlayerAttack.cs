using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
	public InputAction AttackAction;
	PlayerMovement PM;
	[SerializeField] float ComboTimer;
	int slash = 0;
	
	public float Cooldown;
	
	PlayerLedgeGrab PLG;
	
	bool BlockAttack;
	private void Awake()
	{
		AttackAction = InputSystem.actions.FindAction("Attack");
		PLG = GetComponent<PlayerLedgeGrab>();
		PM = GetComponent<PlayerMovement>();
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		ComboTimer -= Time.deltaTime;
		if(ComboTimer <= 0)
		{
			slash = 0;
		}
		if(PLG.isGrab)
		{
			return;
		}
		if(PM.LookUpDown > 0)
		{
			if(AttackAction.WasPressedThisFrame())
			{
				UpSlash();
			}
		}
		else if(PM.LookUpDown < 0)
		{
			if (AttackAction.WasPressedThisFrame())
			{
				if(PM.isGrounded)
				{
					NormalSlash();
				}
				else
				{
					DownSlash();
				}
			}
		}
		else if(AttackAction.WasPressedThisFrame())
		{
			NormalSlash();
		}
		
	}
	
	void NormalSlash()
	{
		if(BlockAttack)
		{
			return;
		}
		ComboTimer = 1f;
		slash++;
		if(slash > 2)
		{
			slash = 1;
		}
		if(slash == 1)
		{
			Player.Instance.Slash1();
		}
		else if(slash == 2)
		{
			Player.Instance.Slash2();
		}
		BlockAttack = true;
		Player.Instance.IsAttacking = true;
	}
	void UpSlash()
	{
		if(BlockAttack)
		{
			return;
		}
		Player.Instance.SlashUp();
		BlockAttack = true;
		Player.Instance.IsAttacking = true;
	}
	void DownSlash()
	{
		if(BlockAttack)
		{
			return;
		}
		Player.Instance.SlashDown();
		BlockAttack = true;
		Player.Instance.IsAttacking = true;
	}
	public void StartAttackCooldownTimer(float time)
	{
		Tween.Delay(duration: time, () => BlockAttack = false);
	}
}
