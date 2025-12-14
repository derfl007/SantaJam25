using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.scenes;
using SantaJam25.scripts.util;

namespace SantaJam25.scripts.levels;

public partial class Shop : NodeOverlay
{
    [Export]
    private int _itemsToShow;

    private Array<ShopItem> _shopItems;
    private RichTextLabel _costLabel;

    public override void _Ready()
    {
        _costLabel = GetNode<RichTextLabel>("%CostLabel");
        var shopItemList = GetNode<VBoxContainer>("%ShopItemList");
        _shopItems = shopItemList.GetChildren().Where(child => child is ShopItem).Cast<ShopItem>()
            .ToGodotArray();
        for (var i = 0; i <= _shopItems.Count - _itemsToShow; i++)
        {
            var itemToRemove = _shopItems.PickRandom();
            _shopItems.Remove(itemToRemove);
            itemToRemove.QueueFree();
        }

        foreach (var shopItem in _shopItems)
        {
            shopItem.CheckBox.Pressed += CalculateCost;
        }
    }

    private void CalculateCost()
    {
        var totalCost = _shopItems.Where(s => s.CheckBox.ButtonPressed).Sum(s => s.PowerUp.Cost);
        _costLabel.Text = $"Total Cost: [color=gold]{totalCost}";
    }

    private void Checkout()
    {
        var selectedItems = _shopItems.Where(s => s.CheckBox.ButtonPressed).ToList();

        if (selectedItems.Sum(s => s.PowerUp.Cost) > GlobalGameState.Instance.CurrentSave.PlayerStats.Money) return;

        foreach (var powerUp in selectedItems.Select(s => s.PowerUp))
        {
            GlobalGameState.Instance.CurrentSave.PowerUps.Add(powerUp);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                Checkout();
                EmitSignalCloseNodeOverlay(true);
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                EmitSignalCloseNodeOverlay(true);
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}