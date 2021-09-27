﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObstacleBehavior : MonoBehaviour {
    //GM
    protected GM gm;
    
    //Hp********************
    public float hp;    //Hp of the enemy
    public float collisionVal;  //Base damage done on a collision with the player
    public bool invincible;

    //Camera Shake
    private GameObject camera;

    //Visual FX
    public GameObject explosion;
    public GameObject hitFX;
    private SpriteRenderer sprite;

    //Score
    public int score;
    public GameObject floatingScoreText;

    public bool awake;
    public bool isATurret;
    public bool dontDieOnScreenExit;
    public bool ignorePlayerCollisions;
    public bool ignoreAwakeOnEnterBounds;

    // Events
    public delegate void ObstacleDelegate(ObstacleBehavior thisObstacle);
    public event ObstacleDelegate OnObstacleDeath; 

    protected void Start()
    {
        awake = false;
        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        gm = GM.gameController;

        hp = hp * OmniController.omniController.obstacleHpScale;
        
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            ProjectileBehavior hit = other.gameObject.GetComponent<ProjectileBehavior>();
            Damage(hit.dmgValue);
            if (!hit.perist)
            {
                hit.DestroyProjectile();
            }
            if (hitFX)
                Destroy(Instantiate(hitFX, transform.position, transform.rotation), 1f);
        }
        if (other.tag == "Bounds")
        {
            if(!ignoreAwakeOnEnterBounds)
                awake = true;
        }
    }

    public void Damage(float dmg)
    {
        if (!invincible)
        {
            hp -= dmg * OmniController.omniController.obstacleIncommingDamageScale;
            StartCoroutine(OnHitFx());
        }
    }

    public void Die()
    {
        OmniController.omniController.enemiesKilled++;
        Destroy(gameObject);
        if(explosion)
            Destroy(
                Instantiate(explosion, transform.position, transform.rotation), 
                5f);
        DisplayScore();
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 60.0f);
        gm.AddScore(score);
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
        OnObstacleDeath?.Invoke(this);
    }

    private void DisplayScore()
    {
        if (score != 0 && floatingScoreText)
        {
            GameObject scoreText = Instantiate(floatingScoreText, 
                transform.position, 
                Quaternion.Euler(0f, 0f, Random.Range(-30f,30f))
                ) as GameObject;
            scoreText.GetComponent<TextMeshPro>().text = (score * gm.scoreMultiplier).ToString();
            scoreText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere, ForceMode2D.Impulse);
            Destroy(scoreText, 1);
        }
    }

    public void SleepOnScreenExit()
    {
        awake = false;
    }

    public void SetAwake(bool setAwake)
    {
        awake = setAwake;
    }

    private IEnumerator OnHitFx()
    {
        sprite?.material?.SetFloat("Flash", 1);

        yield return new WaitForSeconds(.05f);

        sprite?.material?.SetFloat("Flash", 0);
    }
}
