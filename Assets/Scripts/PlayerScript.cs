using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerScript : MonoBehaviour
{
    /// <summary>
    /// /////////////////////////////////// OJEJ JAK DUZO ZMIENNYCH
    /// </summary>
    public BoxCollider2D coll2D;
    public LayerMask groundLayer;
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    public Vector2 input;

    [SerializeField]private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    private float chargeTime;

    private bool isGrounded;
    private bool charged = false; 
    private bool isColliding;

    [SerializeField]private float afterJumpCD; //XDD
    [SerializeField]private float lastInput = 1;

    public float jumpPowMultiplX = 5;
    public float jumpPowMultiplY = 15;
    public float bounceMultipl;

    public Vector2 lastVelocity;
    public Vector2 recentJumpVelocity;

    public Color FroggyGreen = new Color(33f/255f, 168f/255f, 40f/255f);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>(); 
    }
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isGrounded = groundcheck();

        Jump();
        Move();
        RotateSprite();

        lastVelocity = rb.velocity;
        afterJumpCD -= Time.deltaTime;
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///PROBABLY DEAD CODE ZONE/////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // if(charged == false && afterJumpCD > 0f)
        //{
        //  afterJumpCD -= Time.deltaTime;
        //if (colliderFixTimer < 0) { recentJumpVelocity = Vector2.zero; colliderFixTimer = 0.2f; }
        //}
        ///////////////////////////////////////////////////////////////////////////////////////////////////
    }
    private void FixedUpdate()
    {
        
    }
    private Color CalculateColor(float time)
    {
        return new Color(FroggyGreen.r * (1 - time), FroggyGreen.g * (1-time), FroggyGreen.b * (1 - time));
    }
    private void Jump()
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                chargeTime += Time.deltaTime;
                if (chargeTime > 1f) chargeTime = 1f;
                charged = true;
                sr.color = CalculateColor(chargeTime);  //Colorchange
            }
            if ((Input.GetKeyUp(KeyCode.Space)) && charged)
            {
                afterJumpCD = 0.2f;
                rb.AddForce(CalculateChargePower(chargeTime) * jumpPower);
                chargeTime = 0;
                charged = false;
                sr.color = FroggyGreen;

            }
        }
        else { charged = false; chargeTime = 0; sr.color = FroggyGreen; }
    }
    private void Move()
    {
        if (isGrounded && !charged && input.x!=0 && afterJumpCD < 0f)
        {
            rb.velocity += new Vector2(input.x, 0f) * Time.deltaTime *speed;
            rb.velocity = Vector3.ClampMagnitude(rb.velocity,maxSpeed);
        }
    }
    private void RotateSprite()
    {
        if (charged == false)
        {
            if (input.x != 0) { lastInput = input.x; }
            if (lastInput != 1) gameObject.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            else gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }
    private bool groundcheck()
    {
        return Physics2D.BoxCast(coll2D.bounds.center, coll2D.bounds.size, 0f, Vector2.down, 0.05f, groundLayer);
    }
    private Vector2 CalculateChargePower(float chargePower)
    {
        float jumpPower = chargePower;

        if (isColliding) return new Vector2(jumpPowMultiplX * -lastInput * 0.5f, jumpPower * jumpPowMultiplY); //0.5f for bounce penalty BOUNCING WHEN COLLIDING IS IMPOSSIBLE or hard
        else { recentJumpVelocity = new Vector2(jumpPowMultiplX * lastInput, jumpPower * jumpPowMultiplY); return recentJumpVelocity; }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(lastVelocity.x == 0f) 
            lastVelocity = recentJumpVelocity; //used for close collisions that engine is not allowed to calculate like 0.01f away from wall, while aiming at it

        if (collision.contacts.Length > 0)
            if (collision.GetContact(0).normal.y < 1 && collision.GetContact(0).normal.y > -1)
            {  
                rb.velocity = new Vector2(-lastVelocity.x * 0.5f, rb.velocity.y + lastVelocity.y *0.2f );//lastVelocity.y         
            }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        isColliding = (collision.GetContact(0).normal.x != 0) ?  true :  false; //simplified if else
    }


}
