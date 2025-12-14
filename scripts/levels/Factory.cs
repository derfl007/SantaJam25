using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.scenes;
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

    private bool _isDesignReady;
    private bool _isMinigameReady;
    private bool _isOverviewReady;

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

    private VBoxContainer _powerUpList;

    private float[] _qualityBuffs = [0, 0, 0];


    public override void _Ready()
    {
        _weaponScene = GetNode<WeaponScene>("%Weapon");
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

        _powerUpList = GetNode<VBoxContainer>("%PowerUpList");

        PopulatePowerUps();

        PopulateCards();

        _hiltCards.CardSelected += OnCardSelected;
        _bladeCards.CardSelected += OnCardSelected;
        _quenchCards.CardSelected += OnCardSelected;
    }

    private void PopulatePowerUps()
    {
        foreach (var child in _powerUpList.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var powerUp in GlobalGameState.Instance.CurrentSave.PowerUps)
        {
            var button = new Button();
            button.Text = $"{powerUp.Name} ({powerUp.Usages})";
            _powerUpList.AddChild(button);
            button.Pressed += () => UsePowerUp(button, powerUp);
        }
    }

    private void UsePowerUp(Button button, PowerUp powerUp)
    {
        button.QueueFree();

        switch (powerUp.Type)
        {
            case PowerUp.PowerUpType.RevealHighDemand when _hiltCardsContainer.Visible:
                _hiltCards.HighlightCard(_hiltCards.CardArray.MaxBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealHighDemand when _bladeCardsContainer.Visible:
                _bladeCards.HighlightCard(_bladeCards.CardArray.MaxBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealHighDemand when _quenchCardsContainer.Visible:
                _quenchCards.HighlightCard(
                    _quenchCards.CardArray.MaxBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealLowDemand when _hiltCardsContainer.Visible:
                _hiltCards.HighlightCard(_hiltCards.CardArray.MinBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealLowDemand when _bladeCardsContainer.Visible:
                _bladeCards.HighlightCard(_bladeCards.CardArray.MinBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealLowDemand when _quenchCardsContainer.Visible:
                _quenchCards.HighlightCard(
                    _quenchCards.CardArray.MinBy(card1 => card1.Component.ComponentStats.Demand));
                break;
            case PowerUp.PowerUpType.RevealHighNatureDebt when _hiltCardsContainer.Visible:
                _hiltCards.HighlightCard(_hiltCards.CardArray.MaxBy(card1 =>
                    card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.RevealHighNatureDebt when _bladeCardsContainer.Visible:
                _bladeCards.HighlightCard(
                    _bladeCards.CardArray.MaxBy(card1 => card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.RevealHighNatureDebt when _quenchCardsContainer.Visible:
                _quenchCards.HighlightCard(
                    _quenchCards.CardArray.MaxBy(card1 => card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.RevealLowNatureDamage when _hiltCardsContainer.Visible:
                _hiltCards.HighlightCard(_hiltCards.CardArray.MinBy(card1 =>
                    card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.RevealLowNatureDamage when _bladeCardsContainer.Visible:
                _bladeCards.HighlightCard(
                    _bladeCards.CardArray.MinBy(card1 => card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.RevealLowNatureDamage when _quenchCardsContainer.Visible:
                _quenchCards.HighlightCard(
                    _quenchCards.CardArray.MinBy(card1 => card1.Component.ComponentStats.NatureDebt));
                break;
            case PowerUp.PowerUpType.SabotageQuality:
                var enemyToSabotage = new RandomNumberGenerator().RandiRange(0, 2);
                _qualityBuffs[enemyToSabotage] += 0.2f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        powerUp.Usages -= 1;
        if (powerUp.Usages <= 0)
        {
            GlobalGameState.Instance.CurrentSave.PowerUps.Remove(powerUp);
        }

        PopulatePowerUps();
        _powerUpList.Visible = false;
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

        GlobalGameState.Instance.CurrentSave.PlayerStats.CurrentWeapon = playerWeapon;


        _isMinigameReady = false;
        _isOverviewReady = true;
    }

    private void ShowOverview()
    {
        GenerateEnemyWeapons();

        // get player and enemy stats
        var playerStats = GlobalGameState.Instance.CurrentSave.PlayerStats;
        var enemy1Stats = GlobalGameState.Instance.CurrentSave.Enemy1Stats;
        var enemy2Stats = GlobalGameState.Instance.CurrentSave.Enemy2Stats;
        var enemy3Stats = GlobalGameState.Instance.CurrentSave.Enemy3Stats;

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

        var playerResultWeapon = GetNode<WeaponResultScene>("%ResultsContainer/WeaponResult1");
        playerResultWeapon.Weapon = weapons[0];
        playerResultWeapon.Player = playerStats;
        playerResultWeapon.UpdateTextures();

        var enemy1ResultWeapon = GetNode<WeaponResultScene>("%ResultsContainer/WeaponResult2");
        enemy1ResultWeapon.Weapon = weapons[1];
        enemy1ResultWeapon.Player = enemy1Stats;
        enemy1ResultWeapon.UpdateTextures();

        var enemy2ResultWeapon = GetNode<WeaponResultScene>("%ResultsContainer/WeaponResult3");
        enemy2ResultWeapon.Weapon = weapons[2];
        enemy2ResultWeapon.Player = enemy2Stats;
        enemy2ResultWeapon.UpdateTextures();

        var enemy3ResultWeapon = GetNode<WeaponResultScene>("%ResultsContainer/WeaponResult4");
        enemy3ResultWeapon.Weapon = weapons[3];
        enemy3ResultWeapon.Player = enemy3Stats;
        enemy3ResultWeapon.UpdateTextures();

        var winnerName = resultValues.MaxBy(v => v.profit).name;

        GetNode<RichTextLabel>("%ResultsLabel").Text =
            $"[color=green]{winnerName}[/color] wins this round! Press [color=cyan]E[/color] to continue";

        GD.Print("START COMPONENT UPDATE");
        GD.PrintT("Name", "NatureDebt", "Cost", "Demand", "Sales", "Share");

        foreach (var (name, componentSales) in componentUsages)
        {
            var component = componentsUsed.First(t => t.component.Name == name).component;

            GD.PrintT(component.Name, component.ComponentStats.NatureDebt, component.ComponentStats.Cost,
                component.ComponentStats.Demand, componentSales, componentSales / (float)totalSales);

            component.ComponentStats.NatureDebt += (int)Math.Floor(componentSales / component.MaxLifetimeSales * 100);

            component.ComponentStats.Cost =
                (int)Math.Floor(component.BaseCost * (1 + component.ComponentStats.NatureDebt / 100f));

            component.ComponentStats.Demand =
                Math.Clamp(
                    component.ComponentStats.Demand + (_targetShare - componentSales / (float)totalSales) * _volatility,
                    1f, 10f);

            GD.PrintT(component.Name, component.ComponentStats.NatureDebt, component.ComponentStats.Cost,
                component.ComponentStats.Demand, componentSales, componentSales / (float)totalSales);
            GlobalGameState.Instance.CurrentSave.CurrentComponentStats[name] = component.ComponentStats;
        }

        GD.Print("END COMPONENT UPDATE");

        _minigameContainer.Visible = false;
        _resultsContainer.Visible = true;
    }

    private void GenerateEnemyWeapons()
    {
        // todo improve enemy strategies maybe

        // enemy 1: cheap
        var enemy1Weapon = new Weapon();
        enemy1Weapon.Blade = _bladeComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Hilt = _hiltComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Quench = _quenchComponents.MinBy(c => c.BaseCost);
        enemy1Weapon.Quality = (int)((new RandomNumberGenerator().RandfRange(0.6f, 1f) - _qualityBuffs[0]) * 10) / 10f;
        GlobalGameState.Instance.CurrentSave.Enemy1Stats.CurrentWeapon = enemy1Weapon;

        GD.Print($"AI 1 Weapon: {enemy1Weapon}");

        // enemy 2: random
        var enemy2Weapon = new Weapon();
        enemy2Weapon.Blade = _bladeComponents.PickRandom();
        enemy2Weapon.Hilt = _hiltComponents.PickRandom();
        enemy2Weapon.Quench = _quenchComponents.PickRandom();
        enemy2Weapon.Quality =
            (int)((new RandomNumberGenerator().RandfRange(0.6f, 1.2f) - _qualityBuffs[1]) * 10) / 10f;
        GlobalGameState.Instance.CurrentSave.Enemy2Stats.CurrentWeapon = enemy2Weapon;

        GD.Print($"AI 2 Weapon: {enemy2Weapon}");


        // enemy 3: most expensive
        var enemy3Weapon = new Weapon();
        enemy3Weapon.Blade = _bladeComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Hilt = _hiltComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Quench = _quenchComponents.MaxBy(c => c.BaseCost);
        enemy3Weapon.Quality =
            (int)((new RandomNumberGenerator().RandfRange(0.8f, 1.2f) - _qualityBuffs[2]) * 10) / 10f;
        GlobalGameState.Instance.CurrentSave.Enemy3Stats.CurrentWeapon = enemy3Weapon;

        GD.Print($"AI 3 Weapon: {enemy3Weapon}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            // case InputEventKey { Pressed: true, Keycode: Key.Escape }:
            //     // TODO change this to a pause screen, there should be no way to exit back to the map
            //     EmitSignalCloseNodeOverlay(false);
            //     break;
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _hiltCardsContainer.Visible && _weaponScene.Weapon.Hilt is not null:
                ShowBladeCardsContainer();
                _powerUpList.Visible = true;
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _bladeCardsContainer.Visible && _weaponScene.Weapon.Blade is not null:
                _powerUpList.Visible = true;
                ShowQuenchCardsContainer();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }
                when _quenchCardsContainer.Visible && _weaponScene.Weapon.Quench is not null:
                StartMinigame();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isMinigameReady:
                FinishWeapon();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } when _isOverviewReady && !_resultsContainer.Visible:
                ShowOverview();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                EmitSignalCloseNodeOverlay(true);
                break;
            default:
                return;
        }

        GetViewport().SetInputAsHandled();
    }
}