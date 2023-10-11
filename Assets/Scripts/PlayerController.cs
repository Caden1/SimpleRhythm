using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BeatMoveController : MonoBehaviour
{
	private float moveDistance = 1f;
	private float jumpForce = 5f;
	private float beatInterval = 1f; // Time between each beat for an 60 BPM song.
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private float startRotation;
	private float targetRotation;
	private float wallCheckDistance = 1f;
	private bool isNearWall = false;
	private int moveDirection = 1;
	private float nextJumpTime = 0f;
	private float groundCheckDistance = 1f;
	private bool isGrounded = false;
	private bool hasJumped = false;

	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private LayerMask ignorePlayerMask; // Targets everything except Player layer

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		ignorePlayerMask = ~LayerMask.GetMask("Player");
		nextJumpTime = Time.time + beatInterval / 2;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			moveDirection = -1;
		} else if (Input.GetKeyDown(KeyCode.D)) {
			moveDirection = 1;
		}

		Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
		isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, ignorePlayerMask);

		if (isNearWall) {
			moveDirection *= -1;
			return;
		}

		isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, ignorePlayerMask);

		// Handle horizontal movement
		if (Time.time >= nextMoveTime) {
			float moveX = moveDistance * moveDirection;
			currentVelocity.x = moveX / beatInterval;
			startRotation = transform.eulerAngles.z;
			targetRotation = startRotation + (-90.0f * moveDirection);

			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;
		}

		// Handle jump
		if (Time.time >= nextJumpTime && isGrounded && !hasJumped) {
			rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			hasJumped = true;
			nextJumpTime = Time.time + beatInterval;
		}

		if (isGrounded) {
			hasJumped = false;
		}

		// Apply horizontal velocity
		rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);

		// Handle rotations
		float t = (Time.time - moveStartTime) / beatInterval;
		float newRotation = Mathf.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));
		transform.rotation = Quaternion.Euler(0, 0, newRotation);

		if (t > 0.95f) {
			float snappedRotation = SnapToNearest90(transform.eulerAngles.z);
			transform.rotation = Quaternion.Euler(0, 0, snappedRotation);
		}
	}

	private float SnapToNearest90(float angle) {
		return Mathf.Round(angle / 90.0f) * 90.0f;
	}
}
