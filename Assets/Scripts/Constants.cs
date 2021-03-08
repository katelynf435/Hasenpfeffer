using System.ComponentModel;
using UnityEngine;

namespace Hasenpfeffer
{
    public static class Constants
    {
        public const float  PLAYER_CARD_POSITION_OFFSET = 1.5f;
        public const float  PLAYER_CARD_POSITION_OFFSET_DOWN = 1f;
        public const float  PLAYER_CARD_Z_POSITION_OFFSET = -0.2f;
        public const float  DECK_CARD_POSITION_OFFSET = 0.05f;
        public const string CARD_BACK_SPRITE = "cardBack_blue4";
        public const float  CARD_SELECTED_OFFSET = 1f;
        public const int    PLAYER_INITIAL_CARDS = 12;
        public const float  CARD_MOVEMENT_SPEED = 200.0f;
        public const float  CARD_SNAP_DISTANCE = 0.01f;
        public const float  CARD_ROTATION_SPEED = 8f;
    }

    public enum Suits
    {
        NoTrump = -1,
        Hearts = 0,
        Clubs = 1,
        Diamonds = 2,
        Spades = 3,
    }

    public enum Ranks
    {
        [Description("No Ranks")]
        NoRanks = -1,
        [Description("9")]
        Nine = 1,
        [Description("10")]
        Ten = 2,
        [Description("J")]
        Jack = 3,
        [Description("Q")]
        Queen = 4,
        [Description("K")]
        King = 5,
        [Description("A")]
        Ace = 6,
    }
}
