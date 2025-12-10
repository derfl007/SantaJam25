using System;
using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class Component : Resource
{
    private const float RarityCostFactor = 1.0f;
    private const float NatureDebtCostFactor = 1.0f;
    private const float DemandCostFactor = 1.0f;
    private const float RarityDemandFactor = 1.0f;
    private const float RarityNatureDebtFactor = 1.0f;

    [Export]
    public string Name { get; set; }

    [Export]
    public string Description { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public ComponentType ComponentType { get; set; }

    /// <summary>
    ///     Influences how much nature debt is acquired with each usage.
    ///     Is multiplied with the number of sales a weapon with this component gets in a round.
    /// </summary>
    [Export(PropertyHint.Range, "0,2")]
    public float NatureImpactFactor { get; set; }

    [Export]
    public float BaseCost { get; set; }

    [Export]
    public ComponentStats ComponentStats { get; set; }

    /// <summary>
    ///     Increases natureDebt, slightly reduces demand and raises cost for the next round.
    /// </summary>
    public void IncreaseUsage()
    {
        ComponentStats.NatureDebt += Math.Min((int)Math.Floor(RarityNatureDebtFactor * NatureImpactFactor), 100);
        ComponentStats.Demand -= Math.Max(5 - (int)Math.Floor(RarityDemandFactor * NatureImpactFactor), 0);
        ComponentStats.Cost = (int)Math.Floor(RarityCostFactor * NatureImpactFactor +
                                              NatureDebtCostFactor * ComponentStats.NatureDebt +
                                              DemandCostFactor * ComponentStats.Demand);
    }
}