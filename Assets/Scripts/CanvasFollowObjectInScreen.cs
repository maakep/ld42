﻿using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowObjectInScreen : MonoBehaviour
{

    private GameObject player;
    private Camera mainCam;
    private Transform frame;
    private Canvas parentCanvas;

    float imageSizeOffset = 30;

    bool wasOutside = false;

    public delegate void OutsideScreenToggle(bool isOutside);
    public event OutsideScreenToggle OnOutsideScreenToggle;

    void Start()
    {
        player = GameManager.Player;
        mainCam = Camera.main;
        frame = transform.Find("MiniCamFrame");
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // TODO: Understand this :torkarsvett:
    void Update()
    {
        var screenPos = mainCam.WorldToScreenPoint(player.transform.position);

        if (screenPos.x > 0 && 
            screenPos.x < Screen.width && 
            screenPos.y > 0 && 
            screenPos.y < Screen.height)
        {
            parentCanvas.enabled = false;
            if (wasOutside)
            {
                if (OnOutsideScreenToggle != null)
                    OnOutsideScreenToggle(false);
                wasOutside = false;
            }
        }
        else
        {
            if (!wasOutside)
            {
                if (OnOutsideScreenToggle != null)
                    OnOutsideScreenToggle(true);
                wasOutside = true;
            }

            parentCanvas.enabled = true;
            transform.position = screenPos;
            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            screenPos -= screenCenter;
            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90 * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            screenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            float m = cos / sin;
            Vector3 screenBounds = screenCenter * 1f;

            // TODO: Fix so that x & y aren't fucked up when switching between the two.
            screenPos = cos > 0 ? new Vector3(-screenBounds.y / m, screenCenter.y - imageSizeOffset, 0) : new Vector3(screenBounds.y / m, -screenCenter.y + imageSizeOffset, 0);

            if (screenPos.x + imageSizeOffset > screenBounds.x)
            {
                screenPos = new Vector3(screenBounds.x - imageSizeOffset, -screenBounds.x * m, 0);
            }
            else if (screenPos.x -imageSizeOffset < -screenBounds.x)
            {
                screenPos = new Vector3(-screenBounds.x + imageSizeOffset, screenBounds.x * m, 0);
            }

            screenPos += screenCenter;

            transform.position = screenPos;
            frame.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        }

        transform.position = screenPos;
    }
}
