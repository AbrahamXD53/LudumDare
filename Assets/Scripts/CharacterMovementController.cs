using UnityEngine;
using System.Collections;

public class CharacterMovementController : MonoBehaviour
{
    private static float MAX_DAMAGED_TIME = 1.5f;
    private static float DAMAGED_STEP = 2.0f / 60.0f;
    private static float MAX_ATTACK_TIME = 0.3f;
    private static float ATTACKING_STEP = 1.0f / 60.0f;

    public bool facingRight = false;
    [HideInInspector]
    public bool attack = false;
    [HideInInspector]
    public bool carry = false;
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float attackForce = 10f;
    public int playerNumber = 0;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private GameObject bomb = null;
    public PartyController controllers;

    public bool damaged = false;
    private float damagedTime = 0.0f;
    public bool attacking = false;
    private float attackingTime = 0.0f;
    private bool reversed = false;

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
        if (!damaged && controllers != null && controllers.players.Count > playerNumber)
        {
            if (controllers.players[playerNumber].useKeyboard) {
                if(!attacking && Input.GetKeyDown(KeyCode.Space))
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
                if(!attacking && GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.A, controllers.players[playerNumber].controlIndex))
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
        float force = (facingRight) ? attackForce : -attackForce;
        if (damaged)
        {
            if(damagedTime <= 0.0f)
            {
                damagedTime -= 0.0f;
                damaged = false;
                anim.SetBool("damaged", false);
            }
            damagedTime -= DAMAGED_STEP;

        }
        if(attacking)
        {
            force = force * (attackingTime/MAX_ATTACK_TIME);
            force *= ((reversed) ? -1.0f : 1.0f);
            rb2d.AddForce(new Vector2(force, 0.0f));
            if (attackingTime <= 0.0f)
            {
                attackingTime = 0.0f;
                attacking = false;
            }
            attackingTime -= ATTACKING_STEP;
        }

        float h = 0.0f;
        if (!attacking && !damaged && controllers != null && controllers.players.Count > playerNumber)
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
            rb2d.AddForce(new Vector2(force, 0.0f));
            attack = false;
            attacking = true;
            attackingTime = MAX_ATTACK_TIME;
        }

        if (carry)
        {
            if (bomb == null)
            {
                // check if bomb in range
                Vector2 center = transform.position;
                float radius = GetComponent<SpriteRenderer>().size.x * 3.0f;
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

    public void dizzy()
    {
        damaged = true;
        damagedTime = MAX_DAMAGED_TIME;
        anim.SetBool("damaged", true);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            CharacterMovementController cmc = collision.gameObject.GetComponent<CharacterMovementController>();
            if(attacking && !cmc.damaged)
            {
                cmc.dizzy();
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Reverse()
    {
        reversed = (reversed) ? false : true;
    }
}
