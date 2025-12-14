using System;
using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.scenes;
using SantaJam25.scripts.util;

namespace SantaJam25.scripts.levels;

public partial class WorldMap : Node2D
{
    private LevelNode _currentNode;
    private CanvasLayer _hud;

    private bool _isNodeOverlayOpen;
    private TileMapLayer _nodeMapLayer;
    private CanvasLayer _nodeOverlay;

    [Export]
    private Array<LevelNode> _nodes;

    private WorldMapPlayer _player;

    private PauseMenu _pauseMenu;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GlobalGameState.Instance.LoadGame();

        _hud = GetNode<CanvasLayer>("%HUD");

        _hud.GetNode<Label>("%MoneyLabel").Text = GlobalGameState.Instance.CurrentSave.PlayerStats.Money.ToString();

        _pauseMenu = GetNode<PauseMenu>("%PauseMenu");

        GlobalGameState.Instance.SaveUpdate += () =>
        {
            GD.Print($"Current gold: {GlobalGameState.Instance.CurrentSave.PlayerStats.Money}");
            _hud.GetNode<Label>("%MoneyLabel").Text = GlobalGameState.Instance.CurrentSave.PlayerStats.Money.ToString();
        };

        GlobalGameState.Instance.LoadGame();

        _nodeMapLayer = GetNode<TileMapLayer>("%NodeMapLayer");
        _player = GetNode<WorldMapPlayer>("%WorldMapPlayer");
        _nodeOverlay = GetNode<CanvasLayer>("%NodeOverlay");

        _player.NavAgent.NavigationFinished += () =>
        {
            var playerMapPos = _nodeMapLayer.GlobalToMap(_player.GlobalPosition);
            _currentNode = _nodes.First(n => n.TileMapCoords == playerMapPos);
            var isCompleted = _nodeMapLayer.GetCellTileData(playerMapPos).GetCustomData("completed").AsBool();
            if (!isCompleted)
            {
                _player.NinePatchRect.Visible = true;
                _player.IsMoving = false;
            }

            GD.Print($"Player stopped at {playerMapPos}, node is {_currentNode.Name}");
        };

        _player.NavAgent.PathChanged += () =>
        {
            _player.NinePatchRect.Visible = false;
            _player.IsMoving = true;
        };

        foreach (var levelNode in _nodes)
        {
            var isUnlocked = GlobalGameState.Instance.CurrentSave.UnlockedLevels.Contains(levelNode.Name);
            var isCompleted = GlobalGameState.Instance.CurrentSave.CompletedLevels.Contains(levelNode.Name);
            var atlasCoords = levelNode.GetAtlasCoords(isUnlocked, isCompleted);

            _nodeMapLayer.SetCell(levelNode.TileMapCoords, 1, atlasCoords);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isNodeOverlayOpen) return;
        switch (@event)
        {
            case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }:
            {
                var tileCoords = _nodeMapLayer.LocalToMap(_nodeMapLayer.GetLocalMousePosition());
                GD.Print(_nodeMapLayer.GetLocalMousePosition(), tileCoords);
                var tileData = _nodeMapLayer.GetCellTileData(tileCoords);
                if (tileData?.GetCustomData("unlocked").AsBool() ?? false)
                    _player.SetTargetPosition(_nodeMapLayer.MapToGlobal(tileCoords));
                break;
            }
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                OpenNodeOverlay();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                _pauseMenu.Show();
                break;
        }
    }

    public void OpenNodeOverlay()
    {
        if (_isNodeOverlayOpen || _player.IsMoving) return;
        GD.Print($"Open overlay for node: {_currentNode.Name}");

        var scenePath = _currentNode.Type switch
        {
            LevelNode.LevelNodeType.Event => "res://levels/event.tscn",
            LevelNode.LevelNodeType.Shop => "res://levels/shop.tscn",
            LevelNode.LevelNodeType.Factory => "res://levels/factory.tscn",
            _ => throw new ArgumentOutOfRangeException()
        };

        var node = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<NodeOverlay>();
        if (node is EventNodeOverlay eventNodeOverlay) eventNodeOverlay.Description = _currentNode.EventDescription;
        node.CloseNodeOverlay += OnCloseNodeOverlay;
        _nodeOverlay.AddChild(node);
        _isNodeOverlayOpen = true;
    }

    public void CloseNodeOverlay()
    {
        foreach (var child in _nodeOverlay.GetChildren().Where(child => child is NodeOverlay).Cast<NodeOverlay>())
        {
            child.CloseNodeOverlay -= OnCloseNodeOverlay;
            child.QueueFree();
        }

        _isNodeOverlayOpen = false;
    }

    private void OnCloseNodeOverlay(bool success)
    {
        if (success)
        {
            GlobalGameState.Instance.CurrentSave.CompletedLevels.Add(_currentNode.Name);
            _nodeMapLayer.SetCell(_currentNode.TileMapCoords, 1, _currentNode.GetAtlasCoords(true, true));
            foreach (var nextNode in _currentNode.NextNodes)
            {
                GlobalGameState.Instance.CurrentSave.UnlockedLevels.Add(nextNode.Name);
                GlobalGameState.Instance.SaveGame();
                var atlasCoords = nextNode.GetAtlasCoords(true, false);
                _nodeMapLayer.SetCell(nextNode.TileMapCoords, 1, atlasCoords);
            }
        }

        CloseNodeOverlay();
    }
}