using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	private float moveDistance = 2f;
	private float jumpForce = 7f;
	private float bpm = 40f;
	private float beatInterval;
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
	private int beatCounter = 0;
	private float moveDuration;
	private float moveTimer = 0f;

	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private LayerMask ignorePlayerMask; // Targets everything except Player layer
	private AudioManager audioManager;

	private void Start() {
		audioManager = GameObject.Find("AudioObject").GetComponent<AudioManager>();
		beatInterval = 60f / bpm;
		rb = GetComponent<Rigidbody2D>();
		ignorePlayerMask = ~LayerMask.GetMask("Player");
		nextJumpTime = Time.time + beatInterval / 2;
		moveDuration = beatInterval * 0.8f;
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
			// (beatInterval - moveDuration) makes up for the moveDuration reduction in distance
			float moveX = (moveDistance + (beatInterval - moveDuration)) * moveDirection;
			currentVelocity.x = moveX / beatInterval;
			startRotation = transform.eulerAngles.z;
			targetRotation = startRotation + (-90.0f * moveDirection);

			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			audioManager.PlayKick();
			beatCounter = (beatCounter + 1) % 4;

			moveTimer = moveDuration;
		}

		// Handle jump
		if (Time.time >= nextJumpTime && isGrounded && !hasJumped && beatCounter % 2 == 0) {
			rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			audioManager.PlayHiHat();
			hasJumped = true;
			nextJumpTime = Time.time + beatInterval;
		}

		if (isGrounded) {
			hasJumped = false;
		}

		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}


		// Handle rotations
		float t = (Time.time - moveStartTime) / moveDuration;
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
