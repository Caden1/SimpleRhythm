using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform player;

	// Camera boundaries
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	private void LateUpdate() {
		float x = Mathf.Clamp(player.position.x, minX, maxX);
		float y = Mathf.Clamp(player.position.y, minY, maxY);

		transform.position = new Vector3(x, y, transform.position.z);
	}
}
