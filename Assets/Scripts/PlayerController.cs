using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D coll;
    private enum State {Idle, running, Jumping, Falling, Hurt}
    private State state = State.Idle;
    [SerializeField] private LayerMask foreground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private TextMeshProUGUI cherrytext;
    [SerializeField] private float hurtforce = 10f;
    [SerializeField]private AudioSource cherry;
    [SerializeField]private AudioSource footstep;
    [SerializeField]private AudioSource jumpsound;
    [SerializeField]private AudioSource hurtsound;
    [SerializeField]private AudioSource HeartSound;
    [SerializeField]private AudioSource InvicSound;
    [SerializeField] private int Health;
    [SerializeField] private TextMeshProUGUI healthamount;
    [SerializeField] private int Score;
    [SerializeField] private TextMeshProUGUI scoreamount;
    [SerializeField] public static int Life = 3;
    [SerializeField] private TextMeshProUGUI lifeamount;
    private int previousScoreThreshold = 0;
    private bool isInvincible = false;
    private float invincibilityDuration = 10f; // Duration of invincibility in seconds
    private float invincibilityTimer = 0f; // Timer to keep track of invincibility duration




    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
        healthamount.text = Health.ToString();
        scoreamount.text = Score.ToString();
        lifeamount.text = Life.ToString();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(state != State.Hurt){
        Movement();
        }
        AnimationState();
        anim.SetInteger("State",(int)state);
        if (isInvincible)
        {
            // Update the invincibility timer
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                // Turn off invincibility after the duration
                isInvincible = false;
                // Restore the player's color to white
                GetComponent<Renderer>().material.color = Color.white;
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Collectible"){
            cherry.Play();
            Destroy(collision.gameObject);
            cherries += 1;
            cherrytext.text = cherries.ToString();
            Score += 100;
            scoreamount.text = Score.ToString();
             if (Score % 1000 == 0)
        {
            LifeAdd(); // Call the method to add an extra life
        }
        }
        if(collision.tag == "Powerup")
        {
            Destroy(collision.gameObject);
            jumpforce = 20;
            GetComponent<SpriteRenderer>().color = Color.cyan;
            Score += 100;
            scoreamount.text = Score.ToString();
            StartCoroutine(Resetpower());
             if (Score % 1000 == 0)
        {
            LifeAdd(); // Call the method to add an extra life
        }
        }
        if(collision.tag == "Heart"){
            HeartSound.Play();
            Destroy(collision.gameObject);
            Health += 1;
            healthamount.text = Health.ToString();
        }
        if (collision.tag == "Inviciblity")
        {
            InvicSound.Play();
            // Make the player invincible
            isInvincible = true;
            invincibilityTimer = invincibilityDuration; // Reset the timer
            GetComponent<Renderer>().material.color = Color.red; // Change player's color to red
            // Destroy the acorn
            Destroy(collision.gameObject);
        }
    }
    private void LifeAdd(){
        int scoreThreshold = Score / 1000; // Calculate the current score threshold

    // Check if the current score threshold is greater than the previous one
    if (scoreThreshold > previousScoreThreshold)
    {
        int extraLives = scoreThreshold - previousScoreThreshold; // Calculate the number of extra lives to add
        Life += extraLives; // Increment the life count
        lifeamount.text = Life.ToString(); // Update the UI to display the new life count
        previousScoreThreshold = scoreThreshold; // Update the previous score threshold
    }
    }
    private void OnCollisionEnter2D(Collision2D other) {
     if (other.gameObject.CompareTag("Enemy")) {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        // Check if the player is invincible
        if (isInvincible) {
            if (state == State.Falling) {
                enemy.JumpedOn();
                Score += 200;
                scoreamount.text = Score.ToString();
                if (Score % 1000 == 0) {
                    LifeAdd(); // Call the method to add an extra life
                }
                Jump();
            }
            // Destroy the enemy if the player is invincible

            Destroy(other.gameObject);
            Score += 200;
                scoreamount.text = Score.ToString();
                if (Score % 1000 == 0) {
                    LifeAdd(); // Call the method to add an extra life
                }
        } else {
            // Jump on the enemy if the player is not invincible
            if (state == State.Falling) {
                enemy.JumpedOn();
                Score += 200;
                scoreamount.text = Score.ToString();
                if (Score % 1000 == 0) {
                    LifeAdd(); // Call the method to add an extra life
                }
                Jump();
            } else {
                state = State.Hurt;
                hurtsound.Play();
                HealthHandle();
                if (other.gameObject.transform.position.x > transform.position.x) {
                    // Enemy is on my right so when I get hit I move to the left
                    rb.velocity = new Vector2(-hurtforce, rb.velocity.y);
                } else {
                    // Enemy is on my left so when I get hit I move to the right
                    rb.velocity = new Vector2(hurtforce, rb.velocity.y);
                }
            }
        }
    }
}

    private void HealthHandle()
    {
        if (!isInvincible)
    {
        Health -= 1;
        healthamount.text = Health.ToString();
        if (Health == 0)
        {
            Life -= 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    }

    private void Movement(){
        float hdirection = Input.GetAxis("Horizontal");
        //moving left
        if(hdirection < 0){ 
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1,1);
           
            
        
        }
        //moving right
        else if(hdirection > 0){
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1,1);
            
        }
        //Jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(foreground)){ 
            Jump();
        }
    }
    private void Jump(){
        jumpsound.Play();
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            state = State.Jumping;
    }
   private void AnimationState()
{
    if (state == State.Jumping)
    {
        if (rb.velocity.y < -0.1f)
        {
            state = State.Falling;
        }
    }
    else if (state == State.Falling)
    {
        if (coll.IsTouchingLayers(foreground))
        {
            state = State.Idle;
        }
    }
    else if (state == State.Hurt)
    {
        if(Mathf.Abs(rb.velocity.x) < .1f)
        {
            state = State.Idle;
        }
    }
    else if (Mathf.Abs(rb.velocity.x) > 0.1f)
    {
        // running
        state = State.running;
    }
    else
    {
        state = State.Idle;
    }
}
private void Footstep(){
    footstep.Play();
}
private IEnumerator Resetpower()
{
yield return new WaitForSeconds(10);
jumpforce = 10;
GetComponent<SpriteRenderer>().color = Color.white;
}
public void ApplyInvincibility(float duration)
{
    StartCoroutine(InvincibilityCoroutine(duration));
}

// Coroutine for handling invincibility duration
private IEnumerator InvincibilityCoroutine(float duration)
{
    isInvincible = true;
    // Add any visual feedback here, like changing color or adding effects
    GetComponent<SpriteRenderer>().color = Color.red;

    yield return new WaitForSeconds(duration);

    // Restore normal state after duration
    isInvincible = false;
    GetComponent<SpriteRenderer>().color = Color.white;
}

    public bool IsInvincible()
    {
        return isInvincible;
    }
}

    
