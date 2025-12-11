using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.scenes;
using SantaJam25.scripts.util;
using Array = Godot.Collections.Array;
using Range = Godot.Range;

namespace SantaJam25.scripts.levels;

public partial class Factory : NodeOverlay
{
    [Export]
    private int _baseCustomers = 100;

    [Export]
    private Array<Component> _bladeComponents;

    [Export]
    private Array<Component> _hiltComponents;

    [Export]
    private Array<Component> _quenchComponents;

    private Cards _bladeCards;
    private Cards _hiltCards;
    private Cards _quenchCards;

    [Export]
    private PackedScene _cardScene;

    private Control _bladeCardsContainer;
    private Control _hiltCardsContainer;
    private Control _quenchCardsContainer;

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
    private float _volatility = 2f;

    private WeaponScene _weaponScene;


    public override void _Ready()
    {
        _globalGameState = this.GetGlobalGameState();
        _weaponScene = GetNode<WeaponScene>("%Weapon");
        _messageLabel = GetNode<RichTextLabel>("%MessageLabel");
        _costLabel = GetNode<RichTextLabel>("%CostLabel");
        _qualityLabel = GetNode<RichTextLabel>("%QualityLabel");
        _minigameLabel = GetNode<RichTextLabel>("%MinigameLabel");

        _minigameRange = GetNode<Range>("%MinigameRange");

        _hiltCardsContainer = GetNode<Control>("%HiltCardsContainer");
        _bladeCardsContainer = GetNode<Control>("%BladeCardsContainer");
        _quenchCardsContainer = GetNode<Control>("%QuenchCardsContainer");

        _hiltCards = _hiltCardsContainer.GetNode<Cards>("%HiltCardsContainer/Control/Cards");
        _bladeCards = _hiltCardsContainer.GetNode<Cards>("%BladeCardsContainer/Control/Cards");
        _quenchCards = _hiltCardsContainer.GetNode<Cards>("%QuenchCardsContainer/Control/Cards");
        _minigameContainer = GetNode<Control>("%MinigameContainer");
        _resultsContainer = GetNode<Control>("%ResultsContainer");


        PopulateCards();

        _hiltCards.CardSelected += OnCardSelected;
        _bladeCards.CardSelected += OnCardSelected;
        _quenchCards.CardSelected += OnCardSelected;
    }

    private void OnCardSelected(Card card)
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
    }

    private void PopulateCards()
    {
        foreach (var bladeComponent in _bladeComponents)
        {
            var card = _cardScene.Instantiate<Card>();
            card.Component = bladeComponent;
            _bladeCards.AddCard(card);
        }

        foreach (var hiltComponent in _hiltComponents)
        {
            var card = _cardScene.Instantiate<Card>();
            card.Component = hiltComponent;
            _hiltCards.AddCard(card);
        }

        foreach (var quenchComponent in _quenchComponents)
        {
            var card = _cardScene.Instantiate<Card>();
            card.Component = quenchComponent;
            _quenchCards.AddCard(card);
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

    private void ShowBladeCardsContainer()
    {
        _hiltCardsContainer.Visible = false;
        _bladeCardsContainer.Visible = true;
    }

    private void ShowQuenchCardsContainer()
    {
        _bladeCardsContainer.Visible = false;
        _quenchCardsContainer.Visible = true;
    }

    private void StartMinigame()
    {
        if (_globalGameState.CurrentSave.PlayerStats.Money < _weaponScene.Weapon.GetTotalCost())
        {
            ShowMessage("This weapon is too expensive");
            return;
        }

        _isMinigameReady = true;
        _quenchCardsContainer.Visible = false;
        _minigameContainer.Visible = true;
    }

    private void FinishWeapon()
    {
        _isMinigameReady = false;
        _isOverviewReady = true;
        var playerWeapon = _weaponScene.Weapon;

        playerWeapon.Quality = _minigameRange.Value switch
        {
            >= 48 and <= 52 => 1.2f,
            >= 35 and <= 65 => 1.0f,
            >= 20 and <= 80 => 0.8f,
            _ => 0.6f
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
        GenerateEnemyWeapons();

        // get player and enemy stats
        var playerStats = _globalGameState.CurrentSave.PlayerStats;
        var enemy1Stats = _globalGameState.CurrentSave.Enemy1Stats;
        var enemy2Stats = _globalGameState.CurrentSave.Enemy2Stats;
        var enemy3Stats = _globalGameState.CurrentSave.Enemy3Stats;

        List<PlayerStats> stats =
        [
            playerStats, enemy1Stats, enemy2Stats, enemy3Stats
        ];

        var weapons = stats.Select(s => s.CurrentWeapon).ToList();

        var marketPowers = weapons.Select(w => w.CalculateMarketPower()).ToList();

        var totalMarketPower = marketPowers.Sum();

        var marketShares = marketPowers.Select(p => p / totalMarketPower).ToList();

        var sales = marketShares.Select(s => (int)Math.Floor(s * _baseCustomers)).ToList();

        var profits = weapons.Select((w, index) => w.CalculateProfit(sales[index])).ToList();

        for (var i = 0; i < stats.Count; i++)
        {
            stats[i].Money += profits[i];
        }

        var componentsUsed =
            weapons
                .SelectMany((w, index) => ((List<Component>)[w.Blade, w.Hilt, w.Quench])
                    .Select(c => (component: c, sales: sales[index])))
                .ToList();

        var totalSales = sales.Sum();

        var highestProfit = profits.Max();

        var componentUsages = componentsUsed
            .GroupBy(c => c.component.Name)
            .Select(g => (Name: g.Key, Sales: g.Sum(s => s.sales)));

        List<PlayerResults> resultBars =
        [
            GetNode<PlayerResults>("%PlayerResults"),
            GetNode<PlayerResults>("%Enemy1Results"),
            GetNode<PlayerResults>("%Enemy2Results"),
            GetNode<PlayerResults>("%Enemy3Results"),
        ];

        var resultValues = profits.Select((p, i) =>
                (profit: p, sales: sales[i], marketShares: (int)((p / (float)highestProfit) * 100),
                    name: stats[i].Name))
            .ToList();

        for (var index = 0; index < resultBars.Count; index++)
        {
            var r = resultBars[index];
            r.UpdateValues(resultValues[index]);
        }

        var playerResultWeapon = GetNode<WeaponScene>("%ResultsContainer/Weapon1");
        playerResultWeapon.Weapon = weapons[0];
        playerResultWeapon.UpdateTextures();
        var enemy1ResultWeapon = GetNode<WeaponScene>("%ResultsContainer/Weapon2");
        enemy1ResultWeapon.Weapon = weapons[1];
        enemy1ResultWeapon.UpdateTextures();

        var enemy2ResultWeapon = GetNode<WeaponScene>("%ResultsContainer/Weapon3");
        enemy2ResultWeapon.Weapon = weapons[2];
        enemy2ResultWeapon.UpdateTextures();

        var enemy3ResultWeapon = GetNode<WeaponScene>("%ResultsContainer/Weapon4");
        enemy3ResultWeapon.Weapon = weapons[3];
        enemy3ResultWeapon.UpdateTextures();

        var winnerName = resultValues.MaxBy(v => v.profit).name;

        GetNode<RichTextLabel>("%ResultsLabel").Text =
            $"[color=green]{winnerName}[/color] wins this round! Press [color=cyan]E[/color] to continue";

        foreach (var (name, componentSales) in componentUsages)
        {
            var component = componentsUsed.First(t => t.component.Name == name).component;
            var natureImpactFactor = component.NatureImpactFactor;
            component.ComponentStats.NatureDebt += (int)Math.Floor(componentSales * natureImpactFactor);
            component.ComponentStats.Cost =
                (int)Math.Floor(component.BaseCost * (1 + component.ComponentStats.NatureDebt / 100f));
            component.ComponentStats.Demand += (_targetShare - componentSales / (float)totalSales) * _volatility;
            _globalGameState.CurrentSave.CurrentComponentStats[name] = component.ComponentStats;
        }

        _minigameContainer.Visible = false;
        _resultsContainer.Visible = true;
    }

    private void ShowMessage(string message)
    {
        _messageLabel.Text = $"[color=orange]{message}[/color]";
    }

    private void GenerateEnemyWeapons()
    {
        // todo improve enemy strategies maybe

        // enemy 1: cheap
        var enemy1Weapon = new Weapon();
        enemy1Weapon.Blade = _bladeComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Hilt = _hiltComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Quench = _quenchComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Quality = new RandomNumberGenerator().RandfRange(0.6f, 1f);
        _globalGameState.CurrentSave.Enemy1Stats.CurrentWeapon = enemy1Weapon;

        GD.Print($"AI 1 Weapon: {enemy1Weapon}");

        // enemy 2: random
        var enemy2Weapon = new Weapon();
        enemy2Weapon.Blade = _bladeComponents.PickRandom();
        enemy2Weapon.Hilt = _hiltComponents.PickRandom();
        enemy2Weapon.Quench = _quenchComponents.PickRandom();
        enemy2Weapon.Quality = new RandomNumberGenerator().RandfRange(0.6f, 1.2f);
        _globalGameState.CurrentSave.Enemy2Stats.CurrentWeapon = enemy2Weapon;

        GD.Print($"AI 2 Weapon: {enemy2Weapon}");


        // enemy 3: most expensive
        var enemy3Weapon = new Weapon();
        enemy3Weapon.Blade = _bladeComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Hilt = _hiltComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Quench = _quenchComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Quality = new RandomNumberGenerator().RandfRange(0.8f, 1.2f);
        _globalGameState.CurrentSave.Enemy3Stats.CurrentWeapon = enemy3Weapon;

        GD.Print($"AI 3 Weapon: {enemy3Weapon}");
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
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _hiltCardsContainer.Visible && _weaponScene.Weapon.Hilt is not null:
                ShowBladeCardsContainer();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _bladeCardsContainer.Visible && _weaponScene.Weapon.Blade is not null:
                ShowQuenchCardsContainer();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _quenchCardsContainer.Visible && _weaponScene.Weapon.Quench is not null:
                StartMinigame();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isMinigameReady:
                FinishWeapon();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isOverviewReady && !_resultsContainer.Visible:
                ShowOverview();
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                EmitSignalCloseNodeOverlay(true);
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}