using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveBall : MonoBehaviour
{
    Vector3 ballStartPosition;
    Rigidbody2D rb;
    [SerializeField] float speed = 400;
    [SerializeField] AudioSource blip;
    [SerializeField] AudioSource blop;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballStartPosition = transform.position;
        ResetBall();
    }

    void Update() {
        if(Input.GetKeyDown("space"))
        {
            ResetBall();
        }
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 dir = new Vector3(Random.Range(100,300), Random.Range(-100, 100), 0).normalized;
        rb.AddForce(dir*speed);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("backwall"))
        {
            blop.Play();
        }
        else
        {
            blip.Play();
        }
    }

}
