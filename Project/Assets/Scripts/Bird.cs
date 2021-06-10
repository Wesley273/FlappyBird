using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    public float upBounce = 300;

    private Animator anim;
    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameController.instance.isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Fly();
        }
        if (GameController.result == "1")
        {
            Fly();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bird Dead, Game Over!");

        Die();
    }

    private void Fly()
    {
        anim.SetTrigger("Fly");

        rb2D.velocity = Vector2.zero;

        Vector2 upForce = Vector2.up * upBounce;
        rb2D.AddForce(upForce);

        SoundManager.instance.PlayFly();
    }

    private void Die()
    {
        anim.SetTrigger("Die");

        rb2D.velocity = Vector2.zero;

        GameController.instance.GameOver();
    }
}
