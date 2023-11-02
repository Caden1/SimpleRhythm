using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	[HideInInspector]
	public int moveDirection = 1;

	[SerializeField]
	private GameObject projectilePrefab;

	private float t;
	private float moveStartTime;
	private float startRotation;
	private float targetRotation;
	private float beatInterval;
	private float startGravity;
	private float moveDuration;

	private const float wallCheckDistance = 1f;
	private const float groundCheckDistance = 1f;
	private const float moveDistance = 1f;
	private const float dashDistance = 2f;
	private const float jumpForce = 6.4f;
	private const float bpm = 40f;

	private int beatCounter = 0;
	private float nextMoveTime = 0f;
	private float moveTimer = 0f;
	private bool isNearWall = false;
	private bool isGrounded = false;
	private bool queueMoveRight = false;
	private bool queueMoveLeft = false;
	private bool queueJump = false;
	private bool queueDash = false;
	private bool queueShield = false;
	private bool queueProjectile = false;

	private Vector2 currentVelocity = Vector2.zero;

	private AudioManager40bpm audioManager40bpm;
	private Rigidbody2D rb;
	private Animator animator;
	private LayerMask ignoreMask;

	private void Start() {
		CacheComponents();
		InitializeVariables();
	}

	private void Update() {
		HandleInput();
		PerformEnvironmentChecks();

		if (IsTimeForNextMove()) {
			ProcessMove();
			ProcessActionQueues();
		}

		ApplyMovement();
		ApplyRotation();
		SnapTransform();
	}

	private void CacheComponents() {
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		ignoreMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Triggers") | LayerMask.GetMask("PlayerProjectile"));
	}

	private void InitializeVariables() {
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		startGravity = rb.gravityScale;
	}

	private void HandleInput() {
		if (Input.GetKeyDown(KeyCode.A)) {
			moveDirection = -1;
			ToggleQueue(ref queueMoveRight);
			ResetQueues(ref queueJump, ref queueDash, ref queueShield, ref queueProjectile, ref queueMoveLeft);
		} else if (Input.GetKeyDown(KeyCode.D)) {
			moveDirection = 1;
			ToggleQueue(ref queueMoveLeft);
			ResetQueues(ref queueJump, ref queueDash, ref queueShield, ref queueProjectile, ref queueMoveRight);
		} else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) {
			ToggleQueue(ref queueJump);
			ResetQueues(ref queueDash, ref queueShield, ref queueProjectile, ref queueMoveRight, ref queueMoveLeft);
		} else if (Input.GetKeyDown(KeyCode.E)) {
			ToggleQueue(ref queueDash);
			ResetQueues(ref queueJump, ref queueShield, ref queueProjectile, ref queueMoveRight, ref queueMoveLeft);
		} else if (Input.GetKeyDown(KeyCode.S)) {
			ToggleQueue(ref queueShield);
			ResetQueues(ref queueJump, ref queueDash, ref queueProjectile, ref queueMoveRight, ref queueMoveLeft);
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			ToggleQueue(ref queueProjectile);
			ResetQueues(ref queueJump, ref queueDash, ref queueShield, ref queueMoveRight, ref queueMoveLeft);
		}
	}

	private void ToggleQueue(ref bool queue) {
		queue = !queue;
	}

	private void ResetQueues(ref bool queue1, ref bool queue2, ref bool queue3, ref bool queue4, ref bool queue5) {
		queue1 = queue2 = queue3 = false;
	}

	private void PerformEnvironmentChecks() {
		isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, ignoreMask);
		Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
		isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, ignoreMask);
	}

	private bool IsTimeForNextMove() {
		return Time.time >= nextMoveTime;
	}

	private void ProcessMove() {
		float currentTime = Time.time;
		moveStartTime = currentTime;
		nextMoveTime = currentTime + beatInterval;

		if (isNearWall) {
			moveDirection *= -1;
		}

		rb.gravityScale = startGravity;

		startRotation = transform.eulerAngles.z;

		if (beatCounter == 0) {
			audioManager40bpm.PlayTexture();
		}

		if (beatCounter % 2 == 0) {
			audioManager40bpm.PlayKick();
		} else {
			audioManager40bpm.PlayKickWithSnare();
		}

		beatCounter = (beatCounter + 1) % 4;

		moveTimer = moveDuration;
	}

	private void ProcessActionQueues() {
		if (queueDash) {
			currentVelocity.x = (dashDistance + 0.8f) * moveDirection;
			targetRotation = startRotation + (-180.0f * moveDirection);
		} else {
			currentVelocity.x = (moveDistance + 0.4f) * moveDirection;
			targetRotation = startRotation + (-90.0f * moveDirection);
		}

		if (!queueShield) {
			animator.Play("EmptyState");
		}

		if (queueJump && isGrounded) {
			rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
			audioManager40bpm.PlayPlayerJump();
			queueJump = false;
		} else if (queueDash) {
			audioManager40bpm.PlayPlayerDash();
			queueDash = false;
		} else if (queueShield) {
			animator.Play("Shield");
			audioManager40bpm.PlayPlayerShield();
			queueShield = false;
		} else if (queueProjectile) {
			audioManager40bpm.PlayPlayerProjectile();
			queueProjectile = false;
			if (moveDirection == 1) {
				GameObject projectileClone = Instantiate(
					projectilePrefab, new Vector2(transform.position.x + 1f, transform.position.y + 0.5f), projectilePrefab.transform.rotation);
				projectileClone.GetComponent<SpriteRenderer>().flipX = false;
			} else {
				GameObject projectileClone = Instantiate(
					projectilePrefab, new Vector2(transform.position.x - 1f, transform.position.y + 0.5f), projectilePrefab.transform.rotation);
				projectileClone.GetComponent<SpriteRenderer>().flipX = true;
			}
		}
	}

	private void ApplyMovement() {
		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}
	}

	private void ApplyRotation() {
		t = (Time.time - moveStartTime) / moveDuration;
		float newRotation = Mathf.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));
		transform.rotation = Quaternion.Euler(0, 0, newRotation);
	}

	private void SnapTransform() {
		if (t > 0.95f) {
			float snappedRotation = SnapToNearest90(transform.eulerAngles.z);
			transform.rotation = Quaternion.Euler(0, 0, snappedRotation);
			transform.position = SnapToGrid(transform.position, 1f);
			if (!isGrounded) {
				rb.gravityScale = 0f;
			}
		}
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
