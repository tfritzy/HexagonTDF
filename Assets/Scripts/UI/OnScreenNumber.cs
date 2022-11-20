using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OnScreenNumber : MonoBehaviour
{
    public int Value;
    private const float LifeSpan = 1f;
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
    private Outline _outline;
    private Outline outline
    {
        get
        {
            if (_outline == null)
            {
                _outline = transform.Find("Text").GetComponent<Outline>();
            }
            return _outline;
        }
    }
    private float birthTime;
    private Vector3 rootPosition;
    private Vector3 offset;
    private Vector3 velocity;
    protected abstract Vector3 InitialVelocity { get; }
    protected abstract float GravityForce { get; }
    protected bool isStatic = false;

    private void Start()
    {
        birthTime = Time.time;
        this.rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();

        if (isStatic == false)
        {
            if (Time.time > birthTime + LifeSpan)
            {
                Delete();
            }

            LessenColorOverTime();
        }
    }

    private void LessenColorOverTime()
    {
        float alpha = (LifeSpan - (Time.time - birthTime)) / LifeSpan;
        text.color = ColorExtensions.WithAlpha(text.color, alpha);
        outline.effectColor = ColorExtensions.WithAlpha(outline.effectColor, alpha);
    }

    public void SetValue(int value, GameObject owner)
    {
        this.Value = value;
        text.text = value.ToString();
        this.rootPosition = owner.transform.position;
        this.offset = (Vector3)Random.insideUnitCircle / 5;
        this.velocity = InitialVelocity;
        Move();
    }

    private void Move()
    {
        if (isStatic == false)
        {
            this.velocity += Vector3.down * GravityForce * Time.deltaTime;
            this.offset += this.velocity * Time.deltaTime;
        }

        this.transform.position = Managers.Camera.WorldToScreenPoint(rootPosition) + offset;
    }

    public void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }
}
