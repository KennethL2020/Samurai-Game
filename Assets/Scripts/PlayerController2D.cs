using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController2D : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb2d;

    [SerializeField]
    private float xVelocity;

    public bool isAttacking = false;
    bool isDashing = false;
    bool isRunning = false;
    bool isAttackFinished = false;

    public Transform attackPos;
    public float attackRangeX;
    public float attackRangeY;
    public LayerMask whatIsEnemy;
    public int damage;

    float Timer, rate;

    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    public float counter;
    public bool isAttackWaiting;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        rate = 1.5f;
        Timer = -1f;
        dashTime = startDashTime;
        counter = 0f;
    }
    //Longer he holds it, the more powerful it gets
    // Update is called once per frame
    void FixedUpdate()
    {
        //Player Attack2
        if (Time.time >= Timer)
        {
            isAttackFinished = false;
        }
        if ((Input.GetButton("Fire2") || dashTime > 0 || isDashing) && !isAttackFinished)        
        {                                                                                  
            PlayerAttack();                                             
        }                                                                

        if (((Input.GetKey("d") || Input.GetKey("a")) && !Input.GetButton("Fire2")) || ((Input.GetKey("d") || Input.GetKey("a")) && Time.time < Timer))
        {
            PlayerMovement();
        }
        else
        {
            if (!Input.GetButton("Fire2") && (!isAttacking || !isAttackWaiting) || ((!isAttacking || !isAttackWaiting) && Time.time < Timer))
            {
                animator.Play("Player_idle");
            }
        }
    }
    void ResetAttack()
    {
        isAttackWaiting = false;
        isAttacking = false;
    }
    void SetTransformX(float prop)
    {
        dashTime -= Time.deltaTime;

        if (transform.localScale.x > 0)
        {
            rb2d.velocity = Vector2.right * prop;
        }
        if (transform.localScale.x < 0)
        {
            rb2d.velocity = Vector2.left * prop;
        }
        /*   if (transform.localScale.x < 0)
                  transform.Translate(Vector2.left * Time.deltaTime * 10f);
              if (transform.localScale.x > 0)
                  transform.Translate(Vector2.right * Time.deltaTime * 10f); */

        Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), 0, whatIsEnemy);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyController2D>().TakeDamage(damage);
        }
    }
    void PlayerAttack()
    {
        if (Time.time < Timer && animator.GetCurrentAnimatorStateInfo(0).IsName("Player_attack1BC") && isAttacking)
        {
            SetTransformX(counter);
            return;
        }

        if (dashTime <= 0)
        {
            animator.Play("Player_idle");
            dashTime = startDashTime;
            rb2d.velocity = Vector2.zero;
            counter = 0f;
            isDashing = false;
            isAttackFinished = true;
        }
        
        if (Input.GetButton("Fire2") && Time.time > Timer)   //attack can happen again during attack
        {
            isAttackWaiting = true;
            animator.Play("Player_attack1A");
            counter += Time.deltaTime * dashSpeed;

            if (counter > dashSpeed * 0.75f)
            {
                counter = dashSpeed * 0.75f;
            }
            else if (counter <= 20)
            {
                counter = 20;
            }
            return;
        }
        if (!(Input.GetButton("Fire2")) && isAttackWaiting)
        {
            isAttacking = true;
            isDashing = true;
            animator.Play("Player_attack1BC");
            Invoke(nameof(ResetAttack), 0.2f);
            Timer = Time.time + rate;
            return;
        }

    }
    void PlayerMovement()
    {
        //Player Movement
        if (Input.GetKey("d") && !isAttacking && !isAttackWaiting)
        {
            isRunning = true;
            transform.Translate(Vector2.right * xVelocity * Time.deltaTime);
            animator.Play("Player_run");

            transform.localScale = new Vector2(3, 3);
        }
        else if (Input.GetKey("a") && !isAttacking && !isAttackWaiting)
        {
            isRunning = true;
            transform.Translate(Vector2.left * xVelocity * Time.deltaTime);
            animator.Play("Player_run");

            transform.localScale = new Vector2(-3, 3);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 1));
    }

}