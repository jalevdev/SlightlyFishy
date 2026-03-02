using UnityEngine;


public class blahajLogic : MonoBehaviour
{
    public GameObject fish;

    public Rigidbody2D rb;
    public float speed = 3f;

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
        //make it only go up and down, if up go down and vice versa
        if(fish.transform.rotation.eulerAngles.z == 90f)
        {
            rb.linearVelocity = Vector2.down * speed;
            fish.transform.rotation = Quaternion.Euler(0, 0, -90f);
        } else
        {
            rb.linearVelocity = Vector2.up * speed;
            fish.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
    }

    void OnEnable()
    {
        //transform.position = startPosition;
        SetRandomDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            SetRandomDirection();
    }
}