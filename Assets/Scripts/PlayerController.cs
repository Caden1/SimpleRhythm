using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BeatMoveController : MonoBehaviour
{
	public float moveDistance = 1.0f;
	private float beatInterval = 0.75f; // Time between each beat for an 80 BPM song.
	private float nextMoveTime = 0.0f;
	private Vector2 startPosition;
	private Vector2 targetPosition;
	private float moveStartTime;
	private Rigidbody2D rb;
	private float startRotation;
	private float targetRotation;
	private float wallCheckDistance = 1f;
	private bool isNearWall = false;
	private LayerMask wallLayerMask; // Targets everything except Player layer
	private int moveDirection = 1;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		wallLayerMask = ~LayerMask.GetMask("Player");
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			moveDirection = -1;
		} else if (Input.GetKeyDown(KeyCode.D)) {
			moveDirection = 1;
		}

		Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
		isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, wallLayerMask);

		if (isNearWall) {
			moveDirection *= -1;
			return;
		}

		if (Time.time >= nextMoveTime) {
			startPosition = rb.position;
			targetPosition = startPosition + new Vector2(moveDistance * moveDirection, 0);
			startRotation = transform.eulerAngles.z;
			targetRotation = startRotation + (-90.0f * moveDirection);
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;
		}

		float t = (Time.time - moveStartTime) / beatInterval; // Calculate the fraction of the movement/rotation completed
		Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
		rb.MovePosition(newPosition);
		float newRotation = Mathf.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));
		transform.rotation = Quaternion.Euler(0, 0, newRotation);

		if (t > 0.95f) { // When movement is close to completion, snap to the nearest 90-degree interval
			float snappedRotation = SnapToNearest90(transform.eulerAngles.z);
			transform.rotation = Quaternion.Euler(0, 0, snappedRotation);
		}
	}

	private float SnapToNearest90(float angle) {
		return Mathf.Round(angle / 90.0f) * 90.0f;
	}
}
