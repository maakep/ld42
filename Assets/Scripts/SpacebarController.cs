﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacebarController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float chargePower;
    public float turnSpeed;
    public float dischargeMaxTime;

    private float dischargeTime;
    private float spaceStartTime;
    private float chargeUpTime = 2f;
    private float chargeFactor;

    private float chargeTrailTimestamp;

    private float discStartScale = 0.3f;
    private float discEndScale = 1.4f;

    private Transform engineDisc1;
    private ParticleSystem ps1;
    private Transform engineDisc2;
    private ParticleSystem ps2;

    private ParticleSystem ecp1;
    private ParticleSystem ecp2;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        engineDisc1 = transform.Find("EngineDisc1");
        ps1 = transform.Find("EngineParticles1").GetComponent<ParticleSystem>();
        engineDisc2 = transform.Find("EngineDisc2");
        ps2 = transform.Find("EngineParticles2").GetComponent<ParticleSystem>();

        ecp1 = transform.Find("EngineChargeParticles1").GetComponent<ParticleSystem>();
        ecp2 = transform.Find("EngineChargeParticles2").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceStartTime = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.AddForce(-transform.up * chargePower * chargeFactor);
            ecp1.gameObject.SetActive(true);
            ecp2.gameObject.SetActive(true);
            ecp1.Play();
            ecp2.Play();
            DisableStuff();
            chargeTrailTimestamp = Time.time;
            dischargeTime = dischargeMaxTime * chargeFactor;
            Debug.Log("DISCHARGE TIME = " + dischargeTime);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            if (!engineDisc1.gameObject.activeSelf)
                EnableStuff();
            ChargeUp();
        }

        if (Input.GetKey(KeyCode.A))
        {
            float t = turnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + Mathf.LerpAngle(0, 5, t));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float t = turnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + Mathf.LerpAngle(0, -5, t));
        }

        if (Time.time - chargeTrailTimestamp > dischargeTime) {
            ecp1.Stop();
            ecp2.Stop();
        }
    }

    void ChargeUp()
    {
        chargeFactor = Mathf.Min((Time.time - spaceStartTime) / chargeUpTime, 1f);
        float scale = discStartScale + (discEndScale - discStartScale) * chargeFactor;
        engineDisc1.transform.localScale = new Vector3(scale, scale, scale);
        engineDisc2.transform.localScale = new Vector3(scale, scale, scale);
    }

    void DisableStuff()
    {
        engineDisc1.gameObject.SetActive(false);
        engineDisc2.gameObject.SetActive(false);
        ps1.gameObject.SetActive(false);
        ps2.gameObject.SetActive(false);
        ps1.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ps2.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void EnableStuff()
    {
        engineDisc1.gameObject.SetActive(true);
        engineDisc2.gameObject.SetActive(true);
        ps1.gameObject.SetActive(true);
        ps2.gameObject.SetActive(true);
        ps1.Play();
        ps2.Play();
    }
}
