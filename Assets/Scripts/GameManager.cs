using UnityEngine;
using UnityEngine.SceneManagement;
using PrimeTween;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; set; }

	string TransitionDoorID;
	string currentSceneName;
	GameObject player;
	public bool LoadedLevel;

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
		player = GameObject.FindGameObjectWithTag("Player");
	}
	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		currentSceneName = scene.name;
		Debug.Log($"{currentSceneName} finish loaded");
		CameraManager.instance.RelocateBound();
		CameraManager.instance.ResetCam();
		LoadedLevel = true;
		// Only search for the transition door if we have a valid ID
		if (!string.IsNullOrEmpty(TransitionDoorID))
		{
			// Find the specific door directly by name rather than getting all doors first
			GameObject door = GameObject.Find(TransitionDoorID);
			if (door != null && door.CompareTag("Door"))
			{
				door.GetComponent<Door>().TpPlayer(player);
			}

			// Reset the ID after handling the transition
			TransitionDoorID = null;
		}
		Tween.Delay(0.5f, () => { Player.Instance.CanGoThroughDoors = true; Player.Instance.BlockInput = false; });

	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void LoadNextRoom(string DoorID, Object Scene1, Object Scene2)
	{
		Debug.Log("test");
		TransitionDoorID = DoorID;
		if (Scene1.name == currentSceneName)
		{
			SceneManager.LoadScene(Scene2.name);
		}
		else if (Scene2.name == currentSceneName)
		{
			SceneManager.LoadScene(Scene1.name);
		}
	}
}
