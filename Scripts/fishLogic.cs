using UnityEngine;


public class fishLogic : MonoBehaviour
{
    public GameObject fish;

    public Rigidbody2D rb;
    public float speed = 1f;

    public GameObject player;
    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get current position
        startPosition = transform.position;
        player = GameObject.Find("player");
        // Set fish to the GameObject that this script is attached to
        fish = this.gameObject;
        rb = fish.GetComponent<Rigidbody2D>();

        // Ensure Rigidbody2D is set up for continuous movement
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        SetRandomDirection();
    }

    private void SetRandomDirection()
    {
        // Random direction in 2D space
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * speed;
        //make fish point in direction its moving
        //flip instead of 180 degree turn if more than halfway turned (still need to add this)
        float angle = Mathf.Atan2(randomDir.y, randomDir.x) * Mathf.Rad2Deg;    
        fish.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnEnable()
    {
        //transform.position = startPosition;
        SetRandomDirection();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
            SetRandomDirection();
    }
}