using UnityEngine;
#if UNITY_EDITOR
using Physics2D = Nomnom.RaycastVisualization.VisualPhysics2D;
#else
using Physics2D = UnityEngine.Physics2D;
#endif

public class Door : MonoBehaviour
{
	[SerializeField] DoorData data;
	[SerializeField] Transform TpLocation;
	[Header("Type")]
	[SerializeField] bool HorizontalDoor;
	[SerializeField] bool UpDoor;
	[SerializeField] LayerMask groundLayer;
	private void Awake()
	{
		gameObject.name = data.DoorID;
		if (HorizontalDoor)
		{
			var position = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer).point;
			TpLocation.position = new Vector2(position.x, position.y + 1.176f);
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player") && Player.Instance.CanGoThroughDoors)
		{
			Player.Instance.BlockInput = true;
			Player.Instance.PLG.canGrab = false;
			Player.Instance.CanGoThroughDoors = false;
			TransitionManager.Instance.StartTransition(data.DoorID, data.scene1, data.scene2);
			Player.Instance.startTransition(HorizontalDoor, UpDoor);
		}
	}
	public void TpPlayer(GameObject player)
	{
		player.transform.position = TpLocation.position;
	}
}
