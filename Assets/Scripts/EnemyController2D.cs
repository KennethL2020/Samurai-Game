using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float agroRange;

    bool isColliding = false;
    float xVelocity;

    public Animator animator;

    public int health;
    public float speed;

    public Rigidbody2D rb2d;

    private float dazeTime;
    public float startDazeTime;

    GameObject myPlayer;
    // Aggregation of mass when enemies are close together
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        dazeTime = 0;
        xVelocity = 3;
        myPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(dazeTime <= 0)
        {
            xVelocity = 3;
            
        }
        else
        {
            xVelocity = 0;
            dazeTime -= Time.deltaTime;
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        EnemyAgro();
    }
    void EnemyAgro()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer <= agroRange && dazeTime <= 0)
        {
            if (transform.position.x > player.position.x)
            {
                transform.localScale = new Vector2(3, 3);
                transform.Translate(Vector2.left * xVelocity * Time.deltaTime);
                animator.Play("Enemy_run");
            }
            else
            {
                transform.localScale = new Vector2(-3, 3);
                transform.Translate(Vector2.right * xVelocity * Time.deltaTime);
                animator.Play("Enemy_run");
            }
        }
        else if (isColliding && dazeTime > 0)
        {
            animator.Play("Enemy_hurt");
        }
        else
        {
            animator.Play("Enemy_idle");
            rb2d.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && myPlayer.GetComponent<PlayerController2D>().isAttacking && !myPlayer.GetComponent<PlayerController2D>().animator.GetCurrentAnimatorStateInfo(0).IsName("Player_attack1A"))
        {
            dazeTime = myPlayer.GetComponent<PlayerController2D>().counter / myPlayer.GetComponent<PlayerController2D>().dashSpeed;
            isColliding = true;
        }
    }
}
