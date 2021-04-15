using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OnScreenNumber : MonoBehaviour
{
    public int Value;
    private const float LifeSpan = .75f;
    private Text _text;
    private Rigidbody rb;
    private Text text
    {
        get
        {
            if (_text == null)
            {
                _text = transform.Find("Text").GetComponent<Text>();
            }

            return _text;
        }
    }
    private float birthTime;
    private Vector3 rootPosition;
    private Vector3 offset;
    private Vector3 velocity;
    protected abstract Vector3 InitialVelocity { get; }
    protected abstract float GravityForce { get; }

    private void Start()
    {
        birthTime = Time.time;
        this.transform.position += (Vector3)(Random.insideUnitCircle / 2);
        this.rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Time.time > birthTime + LifeSpan)
        {
            Delete();
        }
        Move();
        LessenColorOverTime();
    }

    private void LessenColorOverTime()
    {
        float alpha = (LifeSpan - (Time.time - birthTime)) / LifeSpan;

        // Set text alpha
        Color currentTextColor = text.color;
        currentTextColor.a = alpha;
        text.color = currentTextColor;
    }

    public void SetValue(int value, GameObject owner)
    {
        this.Value = value;
        text.text = value.ToString();
        this.rootPosition = owner.transform.position;
        this.offset = (Vector3)Random.insideUnitCircle / 5;
        this.velocity = InitialVelocity;
    }

    private void Move()
    {
        this.velocity += Vector3.down * GravityForce * Time.deltaTime;
        this.offset += this.velocity * Time.deltaTime;
        this.transform.position = Managers.Camera.WorldToScreenPoint(rootPosition) + offset;
    }

    public void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }
}
