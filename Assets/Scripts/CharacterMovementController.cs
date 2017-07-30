using UnityEngine;
using System.Collections;

public class CharacterMovementController : MonoBehaviour
{

    public bool facingRight = false;
    [HideInInspector]
    public bool attack = false;
    [HideInInspector]
    public bool carry = false;
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float attackForce = 10f;
    //public Transform groundCheck;
    public int playerNumber = 0;


    //private bool grounded = false;
    //private Animator anim;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private GameObject bomb = null;
    public PartyController controllers;
    //private float ypad = 0.12972f;


    // Use this for initialization
    void Awake()
    {
        //anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        controllers = null;
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit2D hit = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        //float distance = Mathf.Abs(hit.point.y - transform.position.y);

        //Debug.Log("distance: " + (distance-ypad) + " height: " + boxCollider.size.y + " grounded: " + grounded);
        //if ((distance-ypad) < boxCollider.size.y)
        //{
        //    grounded = true;
        //}
        //else
        //{
        //    grounded = false;
        //}

        if (controllers != null && controllers.players.Count > playerNumber)
        {
            if (controllers.players[playerNumber].useKeyboard) {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    attack = true;
                }
                if(Input.GetKeyDown(KeyCode.LeftControl))
                {
                    carry = true;
                }
            }
            else
            {
                if(GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.A, controllers.players[playerNumber].controlIndex))
                {
                    attack = true;
                }
                if (GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.X, controllers.players[playerNumber].controlIndex))
                {
                    carry = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        float h = 0.0f;
        if (controllers != null && controllers.players.Count > playerNumber)
        {
            if (controllers != null && controllers.players[playerNumber].useKeyboard)
            {
                h = Input.GetAxis("Horizontal");
            }
            else
            {
                h = GamepadInput.GamePad.GetAxis(GamepadInput.GamePad.Axis.LeftStick, controllers.players[playerNumber].controlIndex).x;
            }
        }

        //anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed)
            rb2d.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

        if (rb2d.velocity.x != 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.9f, rb2d.velocity.y);
        }

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();

        if (attack)
        {
            //anim.SetTrigger("Jump");
            float force = (facingRight) ? attackForce : -attackForce;
            rb2d.AddForce(new Vector2(force, 0.0f));
            attack = false;
        }

        if (carry)
        {
            if (bomb == null)
            {
                // check if bonb in range
                Vector2 center = transform.position;
                float radius = GetComponent<SpriteRenderer>().size.x * 4.0f;
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
                foreach (Collider2D col in hitColliders)
                {
                    if (col.tag == "bomb")
                    {
                        col.GetComponent<BombTimer>().owner = this.gameObject;
                        col.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
                        bomb = col.gameObject;
                    }
                }
            }
            else
            {
                float width = GetComponent<SpriteRenderer>().size.x;
                if (facingRight)
                {
                    bomb.transform.position = new Vector3(transform.position.x + width, transform.position.y, 0.0f);
                }
                else
                {
                    bomb.transform.position = new Vector3(transform.position.x - width, transform.position.y, 0.0f);
                }
                bomb.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                bomb.GetComponent<BombTimer>().owner = null;
                bomb = null;
            }
            carry = false;
        }

        anim.SetFloat("bot_speed", Mathf.Abs(rb2d.velocity.x));
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
