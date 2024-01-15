using System;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.settings;
using CardsPaperScissors.Game.ui.matchInfo;
using FernandoVmp.GodotUtils.Nodes;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public record BoardContext(
    Deck Deck,
    MatchSettings MatchSettings,
    MoveServiceNode MoveService,
    Action<CardNode> OnPlay,
    PackedScene CardModel,
    MatchInfoControl PlayerInfo,
    MatchInfoControl OpponentInfo);