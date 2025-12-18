using System.Linq;
using Godot;
using Godot.Collections;

namespace SantaJam25.scripts.scenes;

public partial class Cards : Path2D
{
    [Signal]
    public delegate void CardSelectedEventHandler(Card card);

    public Array<Card> CardArray;

    [Export]
    private float _distanceBetweenCards;

    [Export]
    private float _distanceToSelectedCard;

    [Export]
    private ShaderMaterial _highlightMaterial;

    private Array<Card> _hoveredCards = [];

    private Array<Card> _selectedCards = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (var card in GetChildren().Cast<Card>())
        {
            card.Area2D.MouseEntered += () => _hoveredCards.Add(card);
            card.Area2D.MouseExited += () => _hoveredCards.Remove(card);
        }

        CardArray = new Array<Card>(GetChildren().Cast<Card>());
        SpreadCards();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_hoveredCards.Count == 0)
        {
            SpreadCards();
            return;
        }

        var highestSelectedCard = _hoveredCards.MaxBy(card => card.ZIndex);
        SelectCard(highestSelectedCard);
    }

    public void SpreadCards()
    {
        if (Engine.IsEditorHint()) CardArray = new Array<Card>(GetChildren().Cast<Card>());

        var pathLength = Curve.GetBakedLength();
        var requiredLength = _distanceBetweenCards * (CardArray.Count - 1);
        var startPos = (pathLength - requiredLength) / 2;

        for (var index = 0; index < CardArray.Count; index++)
        {
            var card = CardArray[index];
            card.TargetProgress = startPos;
            card.TargetYPosition = -50f;
            card.ZIndex = index;
            startPos += _distanceBetweenCards;
        }
    }

    public void SelectCard(Card highlightedCard)
    {
        var pathLength = Curve.GetBakedLength();
        var requiredLength = _distanceBetweenCards * (CardArray.Count - 3) + _distanceToSelectedCard * 2;
        var startPos = (pathLength - requiredLength) / 2;

        for (var index = 0; index < CardArray.Count; index++)
        {
            var card = CardArray[index];
            if (card == highlightedCard)
            {
                card.TargetProgress = startPos + _distanceToSelectedCard / 2;
                card.TargetYPosition = -150f;
                card.ZIndex = 1000;
                startPos += _distanceToSelectedCard * 1.5f;
            }
            else
            {
                card.TargetProgress = startPos;
                card.TargetYPosition = -50f;
                card.ZIndex = index;
                startPos += _distanceBetweenCards;
            }
        }
    }

    public void AddCard(Card card)
    {
        AddChild(card);
        card.Area2D.MouseEntered += () => _hoveredCards.Add(card);
        card.Area2D.MouseExited += () => _hoveredCards.Remove(card);
        CardArray.Add(card);
    }

    public void RemoveCard(Card card)
    {
        CardArray.Remove(card);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }) return;
        var highlightedCard = _hoveredCards.MaxBy(card => card.ZIndex);

        if (highlightedCard is null) return;
        foreach (var card in CardArray.Where(card =>
                     card.Disabled && card.Component.ComponentType == highlightedCard.Component.ComponentType))
            card.Disabled = false;
        highlightedCard.Disabled = true;
        EmitSignalCardSelected(highlightedCard);
    }

    public void HighlightCard(Card card)
    {
        foreach (var card1 in CardArray)
        {
            card1.Sprite.Material = null;
        }

        card.Sprite.Material = _highlightMaterial;
    }
}