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
    ///     Influences the price of weapons created with this component,
    ///     but also how much nature debt is acquired with each usage.
    /// </summary>
    [Export(PropertyHint.Range, "1,5")]
    public int Rarity { get; set; }

    [Export]
    public ComponentStats ComponentStats { get; set; }

    /// <summary>
    ///     Increases natureDebt, slightly reduces demand and raises cost for the next round.
    /// </summary>
    public void IncreaseUsage()
    {
        ComponentStats.NatureDebt += Math.Min((int)Math.Floor(RarityNatureDebtFactor * Rarity), 100);
        ComponentStats.Demand -= Math.Max(5 - (int)Math.Floor(RarityDemandFactor * Rarity), 0);
        ComponentStats.Cost = (int)Math.Floor(RarityCostFactor * Rarity +
                                              NatureDebtCostFactor * ComponentStats.NatureDebt +
                                              DemandCostFactor * ComponentStats.Demand);
    }
}