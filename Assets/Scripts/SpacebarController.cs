﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacebarController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float chargePower;
    public float turnSpeed;
    public float dischargeMaxTime;
    [Range(1, 10)] public int startingFuel = 3;

    private float dischargeTime;
    private float spaceStartTime;
    private float chargeUpTime = 2f;
    private float chargeFactor;
    private bool charging = false;

    private float chargeTrailTimestamp;

    private float discStartScale = 0.3f;
    private float discEndScale = 1.4f;

    private Transform engineDisc1;
    private ParticleSystem ps1;
    private Transform engineDisc2;
    private ParticleSystem ps2;

    private ParticleSystem ecp1;
    private ParticleSystem ecp2;

    public int FuelCanisters { get; internal set; }
    public delegate void FuelCanisterPickup();
    public event FuelCanisterPickup OnFuelPickup;
    public delegate void FuelCanisterUse();
    public event FuelCanisterPickup OnFuelUse;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        engineDisc1 = transform.Find("EngineDisc1");
        ps1 = transform.Find("EngineParticles1").GetComponent<ParticleSystem>();
        engineDisc2 = transform.Find("EngineDisc2");
        ps2 = transform.Find("EngineParticles2").GetComponent<ParticleSystem>();

        ecp1 = transform.Find("EngineChargeParticles1").GetComponent<ParticleSystem>();
        ecp2 = transform.Find("EngineChargeParticles2").GetComponent<ParticleSystem>();

        GainFuel(startingFuel);
    }

    void Boost(float power)
    {
        rb.AddForce(-transform.up * power);
        ecp1.gameObject.SetActive(true);
        ecp2.gameObject.SetActive(true);
        ecp1.Play();
        ecp2.Play();
        DisableStuff();
        chargeTrailTimestamp = Time.time;
        dischargeTime = dischargeMaxTime * chargeFactor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && FuelCanisters > 0)
        {
            spaceStartTime = Time.time;
            charging = true;
            FuelCanisters--;
            if (OnFuelUse != null)
                OnFuelUse();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && charging)
        {
            charging = false;
            Boost(chargePower * chargeFactor);
        }
        else if (Input.GetKey(KeyCode.Space) && charging)
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
        
        Camera.main.GetComponent<CameraShake>().Shake(chargeFactor, chargeFactor);
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Fuel")
        {
            GainFuel(1);
            Destroy(collider.gameObject);
        }
        else if (collider.tag == "SuperFuel")
        {
            GainFuel(1);
            StartCoroutine(SuperBoost());
            Destroy(collider.gameObject);
        }
    }

    private IEnumerator SuperBoost()
    {
        var start = 0f;

        while (start < 3f)
        {
            Boost(10);
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();

        }
    }

    public void GainFuel(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            FuelCanisters++;
            if (OnFuelPickup != null)
                OnFuelPickup();
        }
    }
}
