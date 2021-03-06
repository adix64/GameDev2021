﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroy(5f));
    }
    IEnumerator AutoDestroy(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * projectileSpeed;
    }
}
