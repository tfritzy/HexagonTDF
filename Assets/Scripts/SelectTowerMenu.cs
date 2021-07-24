using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTowerMenu : MonoBehaviour
{
    public AttackTower TargetTower { get; private set; }
    private float setActiveTime;

    void Start()
    {
        InputManager.InputWasMade += InformOfClickElsewhere;
    }

    void Update()
    {
        Follow();
    }

    public void SetTargetTower(AttackTower targetTower)
    {
        this.TargetTower = targetTower;
        this.setActiveTime = Time.time;
    }

    private void Follow()
    {
        if (TargetTower == null)
        {
            this.gameObject.SetActive(false);
        }

        this.transform.position = Managers.Camera.WorldToScreenPoint(TargetTower.transform.position);
    }

    public void InformOfClickElsewhere(object sender, EventArgs e)
    {
        if (Time.time > setActiveTime + .25f && this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        InputManager.InputWasMade -= InformOfClickElsewhere;
    }
}
