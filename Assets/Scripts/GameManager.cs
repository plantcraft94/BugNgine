using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	
	[Header("Abilities")]
	public bool HasDash = false;
	public bool HasSword = false;
	
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
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
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Keyboard.current.wKey.wasPressedThisFrame)
		{
			ObtainSword();
		}
	}
	public void ObtainSword()
	{
		Player.Instance.SwordAnimation();
	}
}
