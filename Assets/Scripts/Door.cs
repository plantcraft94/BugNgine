using UnityEngine;

public class Door : MonoBehaviour
{
	[SerializeField] DoorData data;
	[SerializeField] Transform TpLocation;
	[Header("Type")]
	[SerializeField] bool HorizontalDooor;
	[SerializeField] bool UpDoor;
	private void Awake()
	{
		gameObject.name = data.DoorID;
	}
	private void Start()
	{

	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player") && Player.Instance.CanGoThroughDoors)
		{
			Player.Instance.BlockInput = true;
			Player.Instance.CanGoThroughDoors = false;
			TransitionManager.Instance.StartTransition(data.DoorID, data.scene1, data.scene2);
			Player.Instance.startTransition(HorizontalDooor, UpDoor);
		}
	}
	public void TpPlayer(GameObject player)
	{
		player.transform.position = TpLocation.position;
	}
}
