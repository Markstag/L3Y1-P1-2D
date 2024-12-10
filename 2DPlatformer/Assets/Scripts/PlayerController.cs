using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerTxt;
    public float timer;

[Header("Health")]
public Slider healthSlider;
public int  maxHealth;
public int currentHealth;

[Header("Ammo")]
public Slider ammoSlider;
public int  maxAmmo;
public int currentAmmo;


[Header("Doors & Keys")]
public bool GotKey;

[Header("Shooting")]
public Transform shootingPoint;
public GameObject bullet;
bool isFacingRight;

    [Header("Main")]
    public float moveSpeed;
    public float jumpForce;
    float inputs;
    public Rigidbody2D rb;
    public float groundDistance;
    public LayerMask layerMask;

    RaycastHit2D hit;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = maxHealth;

        startPos = transform.position;

        currentHealth=maxHealth;
        currentAmmo=maxAmmo;
        isFacingRight = true;
        GotKey = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timerTxt.text = timer.ToString("F2");
        
        Movement();
        Health();
        Ammo();
        Shoot();
        MovementDirection();
        UpdateAnimations();
    }

    void Movement()
    {
        inputs = Input.GetAxisRaw("Horizontal");
        rb.velocity = new UnityEngine.Vector2(inputs * moveSpeed, rb.velocity.y);

        hit = Physics2D.Raycast(transform.position, -transform.up, groundDistance, layerMask);
        Debug.DrawRay(transform.position, -transform.up * groundDistance, Color.yellow);

        if (hit.collider)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
void Health()
{
    healthSlider.value = currentHealth;

    if (currentHealth <= 0)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

void Ammo()
{
    ammoSlider.value = currentAmmo;

}

void Shoot()
{
    if (Input.GetKeyDown(KeyCode.S) && currentAmmo > 0)
    {
        Instantiate(bullet, shootingPoint.position, shootingPoint.rotation);
        currentAmmo--;
    }
}

void MovementDirection()
{
if (isFacingRight && inputs < -.1f)
{
    Flip();
}
else if (!isFacingRight && inputs > .1f)
{
     Flip();
}
}

void Flip()
{
isFacingRight = !isFacingRight;
transform.Rotate(0f, 180f, 0f);
}

void UpdateAnimations()
{
    Animator anim = transform.GetChild(0).transform.GetComponent<Animator>();

    anim.SetBool("isGrounded", hit.collider);

     if(inputs != 0)
    {
        anim.SetBool("isMoving", true);
    }
    else
    {
        anim.SetBool("isMoving", false);
    }

    if (Input.GetKeyDown(KeyCode.S) && currentAmmo > 0)
    {
        anim.SetTrigger("Shoot");
    }

}
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            transform.position = startPos;
        }
        
         if (other.gameObject.CompareTag("Enemy"))
        {
            currentHealth--;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Exit") && GotKey == true )
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (other.gameObject.CompareTag("Key"))
        {
             GotKey = true;
             Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("AmmoRelode"))
        {
            currentAmmo++;
             Destroy(other.gameObject);
        }
         if (other.gameObject.CompareTag("HealthPack"))
        {
            currentHealth++;
             Destroy(other.gameObject);
        }
    }
}
