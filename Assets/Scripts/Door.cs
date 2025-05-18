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
	[SerializeField] bool HorizontalDooor;
	[SerializeField] bool UpDoor;
	[SerializeField] LayerMask groundLayer;
	private void Awake()
	{
		gameObject.name = data.DoorID;
		TpLocation.position = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer).point;
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
