using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	private float moveDistance = 1f;
	private float dashDistance = 2f;
	private float jumpForce = 5.05f;
	private float bpm = 40f;
	private float beatInterval;
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private float startRotation;
	private float targetRotation;
	private float wallCheckDistance = 1f;
	private bool isNearWall = false;
	private int moveDirection = 1;
	private float groundCheckDistance = 1f;
	private bool isGrounded = false;
	private float moveDuration;
	private float moveTimer = 0f;
	private bool queueJump = false;
	private bool queueDash = false;
	private bool queueShield = false;
	private float snappedRotation;
	private float startGravity;
	private bool addForceMovement = false;
	private int beatCounter = 0;

	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private LayerMask ignorePlayerMask; // Targets everything except Player layer
	private AudioManager40bpm audioManager40bpm;
	private Rigidbody2D rb;
	private Animator animator;

	private void Start() {
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		beatInterval = 60f / bpm;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		ignorePlayerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Triggers"));
		moveDuration = beatInterval * 0.5f;
		startGravity = rb.gravityScale;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) {
			queueJump = true;
			queueDash = false;
			queueShield = false;
		} else if (Input.GetKeyDown(KeyCode.D)) {
			queueDash = true;
			queueJump = false;
			queueShield = false;
		} else if (Input.GetKeyDown(KeyCode.S)) {
			queueShield = true;
			queueDash = false;
			queueJump = false;
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
			rb.gravityScale = startGravity;

			if (!isGrounded) {
				addForceMovement = true;
			}

			startRotation = transform.eulerAngles.z;

			if (queueDash) {
				float dashXOffset = 0.8f;
				currentVelocity.x = (dashDistance + dashXOffset) * moveDirection;
				targetRotation = startRotation + (-180.0f * moveDirection);
			} else {
				float moveXOffset = 0.4f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				targetRotation = startRotation + (-90.0f * moveDirection);
			}

			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			if (beatCounter == 0) {
				audioManager40bpm.PlayTexture();
			}

			beatCounter = (beatCounter + 1) % 4;

			moveTimer = moveDuration;

			if (!queueShield) {
				animator.Play("EmptyState");
			}

			// Handle jump
			if (queueJump && isGrounded) {
				rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
				audioManager40bpm.PlayKickWithSnare();
				queueJump = false;
			} else if (queueDash) {
				audioManager40bpm.PlayKickWithSnare();
				queueDash = false;
			} else if (queueShield) {
				audioManager40bpm.PlayKickWithSnare();
				animator.Play("Shield");
				queueShield = false;
			} else {
				audioManager40bpm.PlayKickNoSnare();
			}
		}

		// Perform horizontal movement
		if (addForceMovement) {
			addForceMovement = false;
			rb.AddForce(new Vector2(1f * moveDirection, 4f), ForceMode2D.Impulse);
		} else {
			if (moveTimer > 0) {
				rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
				moveTimer -= Time.deltaTime;
			} else {
				rb.velocity = new Vector2(0f, rb.velocity.y);
			}
		}

		float t = (Time.time - moveStartTime) / moveDuration;

		// Handle rotations
		float newRotation = Mathf.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));
		transform.rotation = Quaternion.Euler(0, 0, newRotation);

		// Handle snapping to grid
		if (t > 0.95f) {
			StartCoroutine(HandleRotateSnap());
			transform.rotation = Quaternion.Euler(0, 0, snappedRotation);
			StartCoroutine(HandlePositionSnap());
			transform.position = snapToPosition;
			if (!isGrounded) {
				rb.gravityScale = 0f;
			}
		}
	}

	private IEnumerator HandleRotateSnap() {
		snappedRotation = SnapToNearest90(transform.eulerAngles.z);
		yield return null;
	}

	private IEnumerator HandlePositionSnap() {
		snapToPosition = SnapToGrid(transform.position, 1f);
		yield return null;
	}

	private float SnapToNearest90(float angle) {
		return Mathf.Round(angle / 90.0f) * 90.0f;
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = Mathf.Round(position.y / gridSize ) * gridSize;
		return new Vector2(x, y);
	}
}
