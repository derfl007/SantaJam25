using System;
using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class Weapon : Resource
{
    public Component Blade;

    public Component Hilt;

    /// <summary>
    ///     0.5 to 1.5, 1.5 being the best quality
    /// </summary>
    public float Quality;

    public Component Quench;

    /// <summary>
    ///     The total production cost of this weapon
    /// </summary>
    /// <returns></returns>
    public int GetTotalCost()
    {
        return (Blade?.ComponentStats.Cost ?? 0) + (Hilt?.ComponentStats.Cost ?? 0) +
               (Quench?.ComponentStats.Cost ?? 0);
    }

    public void IncreaseComponentUsages()
    {
        Blade.IncreaseUsage();
        Hilt.IncreaseUsage();
        Quench.IncreaseUsage();
    }

    public int CalculateMarketPower()
    {
        var totalDemand = Blade.ComponentStats.Demand +
                          Hilt.ComponentStats.Demand +
                          Quench.ComponentStats.Demand;

        return (int)Math.Floor(totalDemand * Quality);
    }

    public int CalculateProfit(int sales)
    {
        var totalBaseValue = Blade.ComponentStats.BaseValue +
                             Hilt.ComponentStats.BaseValue +
                             Quench.ComponentStats.BaseValue;

        return totalBaseValue * sales - GetTotalCost();
    }
}