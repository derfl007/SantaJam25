using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.scenes;
using SantaJam25.scripts.util;
using Range = Godot.Range;

namespace SantaJam25.scripts.levels;

public partial class Factory : NodeOverlay
{
    [Export]
    private int _baseCustomers = 100;

    private Cards _cards;

    private Control _cardsContainer;

    private RichTextLabel _costLabel;

    private GlobalGameState _globalGameState;

    private bool _isDesignReady;
    private bool _isMinigameReady;
    private bool _isOverviewReady;

    private RichTextLabel _messageLabel;

    private Control _minigameContainer;
    private RichTextLabel _minigameLabel;
    private Range _minigameRange;
    private int _minigameRangeTarget;

    [Export]
    private float _minigameSpeed = 350f;

    private PlayerStats[] _playerStats;

    private float _quality;
    private RichTextLabel _qualityLabel;
    private Control _resultsContainer;

    [Export]
    private float _targetShare = 0.2f;

    [Export]
    private float _volatility = 0.5f;

    private WeaponScene _weaponScene;


    public override void _Ready()
    {
        _globalGameState = this.GetGlobalGameState();
        _weaponScene = GetNode<WeaponScene>("%Weapon");
        _cards = GetNode<Cards>("%Cards");
        _messageLabel = GetNode<RichTextLabel>("%MessageLabel");
        _costLabel = GetNode<RichTextLabel>("%CostLabel");
        _qualityLabel = GetNode<RichTextLabel>("%QualityLabel");
        _minigameLabel = GetNode<RichTextLabel>("%MinigameLabel");

        _minigameRange = GetNode<Range>("%MinigameRange");

        _cardsContainer = GetNode<Control>("%CardsContainer");
        _minigameContainer = GetNode<Control>("%MinigameContainer");
        _resultsContainer = GetNode<Control>("%ResultsContainer");


        PopulateCards();

        _cards.CardSelected += card =>
        {
            switch (card.Component.ComponentType)
            {
                case ComponentType.Hilt:
                    _weaponScene.Weapon.Hilt = card.Component;
                    break;
                case ComponentType.Blade:
                    _weaponScene.Weapon.Blade = card.Component;
                    break;
                case ComponentType.Quench:
                    _weaponScene.Weapon.Quench = card.Component;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _weaponScene.UpdateTextures();

            _costLabel.Text = $"Cost: [color=gold]{_weaponScene.Weapon.GetTotalCost()} Gold";

            _isDesignReady = _weaponScene.Weapon.Hilt is not null && _weaponScene.Weapon.Blade is not null &&
                             _weaponScene.Weapon.Quench is not null;
        };
    }

    private void PopulateCards()
    {
        // TODO get semi-random selection of cards
        Array<Card> cards = [];

        foreach (var card in cards)
        {
            card.Component.ComponentStats = _globalGameState.CurrentSave.CurrentComponentStats[card.Component.Name];
            _cards.AddCard(card);
        }
    }

    public override void _Process(double delta)
    {
        if (!_isMinigameReady) return;

        _minigameRangeTarget = _minigameRange.Value switch
        {
            >= 98 => 0,
            <= 2 => 100,
            _ => _minigameRangeTarget
        };
        _minigameRange.Value = _minigameRangeTarget < _minigameRange.Value
            ? _minigameRange.Value - delta * _minigameSpeed
            : _minigameRange.Value + delta * _minigameSpeed;
    }


    private void StartMinigame()
    {
        if (_globalGameState.CurrentSave.PlayerStats.Money < _weaponScene.Weapon.GetTotalCost())
        {
            ShowMessage("This weapon is too expensive");
            return;
        }

        _isMinigameReady = true;
        _cardsContainer.Visible = false;
        _minigameContainer.Visible = true;
    }

    private void FinishWeapon()
    {
        _isMinigameReady = false;
        _isOverviewReady = true;
        var playerWeapon = _weaponScene.Weapon;

        playerWeapon.Quality = _minigameRange.Value switch
        {
            >= 48 and <= 52 => 1.5f,
            >= 35 and <= 65 => 1.0f,
            >= 20 and <= 80 => 0.5f,
            _ => 0
        };

        _qualityLabel.Text = "Quality: [color=" + _minigameRange.Value switch
        {
            >= 48 and <= 52 => "cyan] Perfect",
            >= 35 and <= 65 => "green] Good",
            >= 20 and <= 80 => "orange] Okay",
            _ => "red] Bad"
        };

        _minigameLabel.Text = "Press [color=cyan]E[/color] to see this round's results";

        _globalGameState.CurrentSave.PlayerStats.CurrentWeapon = playerWeapon;


        _isMinigameReady = false;
        _isOverviewReady = true;
    }

    private void ShowOverview()
    {
        // get player and enemy stats
        var playerStats = _globalGameState.CurrentSave.PlayerStats;
        var enemy1Stats = _globalGameState.CurrentSave.Enemy1Stats;
        var enemy2Stats = _globalGameState.CurrentSave.Enemy2Stats;
        var enemy3Stats = _globalGameState.CurrentSave.Enemy3Stats;

        // get/set weapons
        var playerWeapon = playerStats.CurrentWeapon;
        var enemy1Weapon = enemy1Stats.CurrentWeapon;
        var enemy2Weapon = enemy2Stats.CurrentWeapon;
        var enemy3Weapon = enemy3Stats.CurrentWeapon;

        // calculate market powers
        var playerMarketPower = playerWeapon.CalculateMarketPower();
        var enemy1MarketPower = enemy1Weapon.CalculateMarketPower();
        var enemy2MarketPower = enemy2Weapon.CalculateMarketPower();
        var enemy3MarketPower = enemy3Weapon.CalculateMarketPower();

        // calculate market share
        var playerShare = (float)playerMarketPower /
                          (playerMarketPower + enemy1MarketPower + enemy2MarketPower + enemy3MarketPower);
        var enemy1Share = (float)enemy1MarketPower / (playerMarketPower + enemy2MarketPower + enemy3MarketPower);
        var enemy2Share = (float)enemy2MarketPower / (playerMarketPower + enemy1MarketPower + enemy3MarketPower);
        var enemy3Share = (float)enemy3MarketPower / (playerMarketPower + enemy1MarketPower + enemy2MarketPower);

        // calculate total sales
        var playerSales = (int)Math.Floor(playerShare * _baseCustomers);
        var enemy1Sales = (int)Math.Floor(enemy1Share * _baseCustomers);
        var enemy2Sales = (int)Math.Floor(enemy2Share * _baseCustomers);
        var enemy3Sales = (int)Math.Floor(enemy3Share * _baseCustomers);


        // calculate profits
        // TODO visualize profits (similar to turmoil?)
        var playerProfit = playerWeapon.CalculateProfit(playerSales);
        var enemy1Profit = enemy1Weapon.CalculateProfit(enemy1Sales);
        var enemy2Profit = enemy2Weapon.CalculateProfit(enemy2Sales);
        var enemy3Profit = enemy3Weapon.CalculateProfit(enemy3Sales);

        // add profits to playerStats
        playerStats.Money += playerProfit;
        enemy1Stats.Money += enemy1Profit;
        enemy2Stats.Money += enemy2Profit;
        enemy3Stats.Money += enemy3Profit;

        // handle demand and nature debt changes
        (Component Component, float Sales)[] componentsUsed =
        [
            (Component: playerWeapon.Blade, Sales: playerSales),
            (Component: playerWeapon.Hilt, Sales: playerSales),
            (Component: playerWeapon.Quench, Sales: playerSales),
            (Component: enemy1Weapon.Blade, Sales: enemy1Sales),
            (Component: enemy1Weapon.Hilt, Sales: enemy1Sales),
            (Component: enemy1Weapon.Quench, Sales: enemy1Sales),
            (Component: enemy2Weapon.Blade, Sales: enemy2Sales),
            (Component: enemy2Weapon.Hilt, Sales: enemy2Sales),
            (Component: enemy2Weapon.Quench, Sales: enemy2Sales),
            (Component: enemy3Weapon.Blade, Sales: enemy3Sales),
            (Component: enemy3Weapon.Hilt, Sales: enemy3Sales),
            (Component: enemy3Weapon.Quench, Sales: enemy3Sales)
        ];

        var totalSales = playerSales + enemy1Sales + enemy2Sales + enemy3Sales;

        var componentUsages = componentsUsed
            .GroupBy(c => c.Component.Name)
            .Select(g => (Name: g.Key, Sales: g.Sum(s => s.Sales)));

        foreach (var (name, sales) in componentUsages)
        {
            var component = componentsUsed.First(t => t.Component.Name == name).Component;
            var natureImpactFactor = component.NatureImpactFactor;
            component.ComponentStats.NatureDebt += (int)Math.Floor(sales * natureImpactFactor);
            component.ComponentStats.Cost =
                (int)Math.Floor(component.BaseCost * (1 + component.ComponentStats.NatureDebt / 100f));
            component.ComponentStats.Demand += (_targetShare - sales / totalSales) * _volatility;
            _globalGameState.CurrentSave.CurrentComponentStats[name] = component.ComponentStats;
        }

        var playerResults = GetNode<PlayerResults>("%PlayerResults");
        var enemy1Results = GetNode<PlayerResults>("%Enemy1Results");
        var enemy2Results = GetNode<PlayerResults>("%Enemy2Results");
        var enemy3Results = GetNode<PlayerResults>("%Enemy3Results");

        var playerResultValues = (playerProfit, playerSales, (int)(playerShare * 100), playerStats.Name);
        var enemy1ResultValues = (enemy1Profit, enemy1Sales, (int)(enemy1Share * 100), enemy1Stats.Name);
        var enemy2ResultValues = (enemy2Profit, enemy2Sales, (int)(enemy2Share * 100), enemy2Stats.Name);
        var enemy3ResultValues = (enemy3Profit, enemy3Sales, (int)(enemy3Share * 100), enemy3Stats.Name);

        playerResults.UpdateValues(playerResultValues);
        enemy1Results.UpdateValues(enemy1ResultValues);
        enemy2Results.UpdateValues(enemy2ResultValues);
        enemy3Results.UpdateValues(enemy3ResultValues);

        var winnerName = ((List<(int gold, int sales, int share, string playerName)>)
        [
            playerResultValues,
            enemy1ResultValues,
            enemy2ResultValues,
            enemy3ResultValues
        ]).MaxBy(v => v.gold).playerName;

        GetNode<Label>("%ResultsLabel").Text =
            $"[color=green]{winnerName}[/color] wins this round! Press [color=cyan]E[/color] to continue";

        // TODO for tomorrow flo: Highest priority are Card dealing and Enemy AI logic
    }

    private void ShowMessage(string message)
    {
        _messageLabel.Text = $"[color=orange]{message}[/color]";
    }

    private void GenerateEnemyWeapons()
    {
        // todo randomly generate 3 types of enemy weapons based on different play styles
        _globalGameState.CurrentSave.Enemy1Stats.CurrentWeapon = new Weapon();
        _globalGameState.CurrentSave.Enemy2Stats.CurrentWeapon = new Weapon();
        _globalGameState.CurrentSave.Enemy3Stats.CurrentWeapon = new Weapon();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                // TODO change this to a pause screen, there should be no way to exit back to the map
                EmitSignalCloseNodeOverlay(false);
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isDesignReady && !_isMinigameReady:
                StartMinigame();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isMinigameReady:
                FinishWeapon();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isOverviewReady:
                ShowOverview();
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}