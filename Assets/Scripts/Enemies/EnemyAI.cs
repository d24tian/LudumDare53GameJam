using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD53PAPD;

public class EnemyAI : MonoBehaviour
{
    public int hitPoints = 2;
    public bool faceRight = true;
    public bool fireOnNextTick = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 250;
    public Vector2 projFireDirection;
    public float moveSpeed;
    public float shotInterval = 2.3f;

    protected void Shoot()
    {
        GameObject proj;
        proj = GameObject.Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Launch(projFireDirection, projectileSpeed);
    }

    protected void Move()
    {
        Debug.Log("PARENT MOVE CALLED");

    }
}
