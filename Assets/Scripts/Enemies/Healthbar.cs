using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public Transform Owner;
    public float FillPercentage;
    protected Transform FillBar;
    protected RectTransform DamageBar;
    protected float DamageBarWidth;
    protected float DamageBarDecreaseSpeed;
    private float scale;

    void Update()
    {
        if (Owner == null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        ScaleDamageBar();
        transform.position = Managers.Camera.WorldToScreenPoint(Owner.position) + new Vector3(0, 75 * scale);
    }

    public void SetOwner(Transform owner)
    {
        this.Owner = owner;
        this.scale = owner.localScale.x;
        this.FillBar = this.transform.Find("FillBar").transform;
        this.DamageBar = this.transform.Find("DamageBar").GetComponent<RectTransform>();
        this.FillPercentage = 1;
    }

    public void SetFillScale(float newFillPercentage)
    {
        SetDamageBar(newFillPercentage);
        this.FillPercentage = newFillPercentage;
        this.FillBar.localScale = new Vector3(this.FillPercentage, 1f, 1f);
    }

    private void SetDamageBar(float newFillPercentage)
    {
        this.DamageBarWidth += (this.FillPercentage - newFillPercentage) * 100;
        DamageBarDecreaseSpeed = this.DamageBarWidth * 2;
        DamageBar.anchoredPosition = new Vector2(newFillPercentage * 100, DamageBar.localPosition.y);
    }

    private void ScaleDamageBar()
    {
        if (this.DamageBarWidth <= 0)
        {
            this.DamageBarWidth = 0;
            return;
        }

        this.DamageBarWidth -= Time.deltaTime * DamageBarDecreaseSpeed;
        DamageBar.sizeDelta = new Vector2(this.DamageBarWidth, DamageBar.rect.height);
    }
}
