using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;


    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;



    Rigidbody2D rigidBody;
    // for secure rotations
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
    TrailRenderer trail;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rigidBody.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {

        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }


    void Update()
    {
        // zero = left click
        if (game.GameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            rigidBody.velocity =  Vector2.zero;
            transform.rotation = forwardRotation;
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
            // add tap sound here

        }


        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);

    }


    void OnTriggerEnter2D(Collider2D col)
    {  

        if (col.gameObject.tag == "ScoreZone")
        {
            //register a score
            OnPlayerScored(); //event sent to GameManager
            //play a good sound

        }

        if (col.gameObject.tag == "DeadZone")
        {
            rigidBody.simulated = false;
            //register a dead event
            OnPlayerDied(); //event sent to gamemanager
            //play a bad sound
        }

    }

}
