using System;
using Godot;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.scenes;
using SantaJam25.scripts.util;

namespace SantaJam25.scripts.levels;

public partial class FactoryNodeOverlay : NodeOverlay
{
    private Cards _cards;

    private GlobalGameState _globalGameState;

    private RichTextLabel _messageLabel;

    private Weapon _weapon;


    public override void _Ready()
    {
        _globalGameState = this.GetGlobalGameState();
        _weapon = GetNode<Weapon>("%Weapon");
        _cards = GetNode<Cards>("%Cards");
        _messageLabel = GetNode<RichTextLabel>("%MessageLabel");

        _cards.CardSelected += card =>
        {
            switch (card.Component.ComponentType)
            {
                case ComponentType.Hilt:
                    _weapon.Hilt = card.Component;
                    break;
                case ComponentType.Blade:
                    _weapon.Blade = card.Component;
                    break;
                case ComponentType.Quench:
                    _weapon.Quench = card.Component;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };
    }

    private void FinishWeapon()
    {
        if (_weapon.Blade is null)
        {
            ShowMessage("You have not selected a blade");
            return;
        }

        if (_weapon.Hilt is null)
        {
            ShowMessage("You have not selected a hilt");
            return;
        }

        if (_weapon.Quench is null)
        {
            ShowMessage("You have not selected a quench type");
            return;
        }

        if (_globalGameState.CurrentSave.Gold < _weapon.GetTotalCost())
        {
            ShowMessage("This weapon is too expensive");
            return;
        }

        var profit = _weapon.GetTotalPrice() - _weapon.GetTotalCost();

        _globalGameState.CurrentSave.Gold += profit;

        _weapon.IncreaseComponentUsages();

        EmitSignalCloseNodeOverlay(true);
    }

    private void ShowMessage(string message)
    {
        _messageLabel.Text = $"[color=orange]{message}[/color]";
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                EmitSignalCloseNodeOverlay(false);
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                FinishWeapon();
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}