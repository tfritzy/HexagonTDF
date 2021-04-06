﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Source : ResourceCollector
{
    public override BuildingType Type => BuildingType.Source;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override HashSet<HexagonType> CollectionTypes => collectionTypes;
    public override int CollectionRatePerHex => 2;
    public override List<ResourceType> ResourceTypes => resourceTypes;
    public override int CollectionRange => 0;
    public override int StartingHealth => 50;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 1;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Stone, 1f},
    };

    private HashSet<HexagonType> collectionTypes = new HashSet<HexagonType>() { HexagonType.Grass, HexagonType.Stone };
    private List<ResourceType> resourceTypes = new List<ResourceType>() { ResourceType.Wood };
    private Text healthText;

    protected override void Setup()
    {
        base.Setup();
        this.healthText = Managers.Canvas.Find("HealthUI").Find("Text").GetComponent<Text>();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        healthText.text = this.Health.ToString();
    }
}
