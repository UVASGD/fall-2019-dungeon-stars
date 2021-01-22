﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour {

    public enum PowerUps { None, LevelUp, Repair, SpeedUp, FireUp, Ammo, BoltDrone };

    public PowerUps type;

    public float speed;
    public float duration;

    public GameObject summon;

    private bool awake;

    public GameObject marker;  // Used to identify in scene in edit mode
    Rigidbody2D rb;

    private void Start()
    {
        awake = false;

        rb = GetComponent<Rigidbody2D>();

        marker.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (GM.gameController.gameStart)
        {
            if (awake)
            {
               rb.velocity = (Vector3.down * speed) + Vector3.down;
            }
            else
            {
                rb.velocity = Vector3.down;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }

    public void Summon(bool byPlayerTwo)
    {
        GameObject summonedObject = Instantiate(summon, transform.position, transform.rotation);
        DroneBehavior summonedDrone = summonedObject.GetComponent<DroneBehavior>();
        summonedDrone.lifetime = duration;
        summonedDrone.followPlayer2 = byPlayerTwo;
    }
}
