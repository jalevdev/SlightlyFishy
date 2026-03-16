using UnityEngine;
using TMPro;

public class movement : MonoBehaviour
{
    public float speed = 1.5f; // Speed of the player movement
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D component

    public TMP_Text speedText;
    public TMP_Text sizeText;
    public int sizex = 2;
    public int speedx = 2;

    public GameObject player;
    public GameObject brain;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speedText.text = speedx + "/5";
        sizeText.text = sizex + "/5";
        player = GameObject.Find("player");
        brain = GameObject.Find("brain");
    }

    // Update is called once per frame
    void Update()
    {
        rb.rotation = 0f; // Reset rotation to prevent unwanted spinning
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        //include time.deltaTime to make the movement frame rate independent
        if (horizontalInput != 0)
        {
            rb.AddForce(new Vector2(horizontalInput * speed * Time.deltaTime, 0f), ForceMode2D.Impulse);
        }

        if (verticalInput != 0)
        {
            rb.AddForce(new Vector2(0f, verticalInput * speed * Time.deltaTime), ForceMode2D.Impulse);
        }
        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f; // Subtract 90 degrees to make the sprite face the correct direction
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "orange")
        {
            if (speedx < 5)
            {
                speedx += 1;
                speedText.text = speedx + "/5";
                speed = speedx - 0.5f;
            }
            collision.gameObject.SetActive(false);
        } else if (collision.gameObject.tag == "blue")
        {
            if (speedx > 0)
            {
                speedx -= 1;
                speedText.text = speedx + "/5";
                speed = speedx - 0.5f;
            }
            collision.gameObject.SetActive(false);
        } else if (collision.gameObject.tag == "green")
        {
            if (sizex < 5)
            {
                sizex += 1;
                sizeText.text = sizex + "/5";
                transform.localScale += new Vector3(0.2f, 0.2f, 0f);
            }
            collision.gameObject.SetActive(false);
        } else if (collision.gameObject.tag == "red")
        {
            if (sizex > 0)
            {
                sizex -= 1;
                sizeText.text = sizex + "/5";
                transform.localScale -= new Vector3(0.2f, 0.2f, 0f);
            }
            collision.gameObject.SetActive(false);
        } else if (collision.gameObject.tag == "blahaj")
        {
            Debug.Log("hit blajah");
            brain.GetComponent<cameraFollow>().dieStart();
        } else if (collision.gameObject.tag == "puff")
        {
            Debug.Log("hit pufferfish");
            brain.GetComponent<cameraFollow>().dieStart();
        } else if (collision.gameObject.tag == "ff")
        {
            Debug.Log("win yay");
            brain.GetComponent<cameraFollow>().winStart();
        }
    }
}
