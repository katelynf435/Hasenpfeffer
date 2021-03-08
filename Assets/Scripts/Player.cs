using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;

namespace Hasenpfeffer
{
    /// <summary>
    /// Manages the positions of the player's cards
    /// </summary>
    [Serializable]
    public class Player : IEquatable<Player>
    {
        public string PlayerId;
        public string PlayerName;
        public bool IsLocal;
        public Vector3 StartPosition;
        public Vector3 Position;
        public Vector3 PlayPosition;
        public Vector3 RemoveCardPosition;
        public int Team;
        public int Pos;
        public string bid;
        public string text;

        int numberOfDisplayingCards;

        public List<Card> DisplayingCards = new List<Card>();
        public Card PlayedCard;
        public Card SelectedCard1;
        public Card SelectedCard2;
        public Card SelectedCard3;

        public Vector3 NextCardPosition(Player player, int totalCards)
        {
            if (player.Pos == 1)
            {
                if (player.IsLocal)
                {
                    if (totalCards == 12)
                    {
                        StartPosition = Position; 
                    }
                    if (numberOfDisplayingCards == 1)
                    {
                         StartPosition = Position + Vector3.right * 0.60f * (12 - totalCards);
                    }
                    Vector3 nextPos = StartPosition + Vector3.right * Constants.PLAYER_CARD_POSITION_OFFSET * numberOfDisplayingCards;
                    nextPos = nextPos + Vector3.forward * Constants.PLAYER_CARD_Z_POSITION_OFFSET * numberOfDisplayingCards;
                    nextPos = new Vector3(nextPos.x, -7, nextPos.z);
                    return nextPos;
                }
                else
                {
                    Vector3 nextPos = Position + Vector3.right * Constants.PLAYER_CARD_POSITION_OFFSET * numberOfDisplayingCards;
                    nextPos = nextPos + Vector3.forward * Constants.PLAYER_CARD_Z_POSITION_OFFSET * numberOfDisplayingCards;
                    return nextPos;
                }
            }
            else 
            {
            	Vector3 nextPos = Position + Vector3.down * Constants.PLAYER_CARD_POSITION_OFFSET_DOWN * numberOfDisplayingCards;
            	return nextPos;
            }
        }

        public void SetCardValues(List<byte> values)
        {
            if (DisplayingCards.Count != values.Count)
            {
                Debug.LogError($"Displaying cards count {DisplayingCards.Count} is not equal to card values count {values.Count} for {PlayerId}");
                return;
            }

            for (int index = 0; index < values.Count; index++)
            {
                Card card = DisplayingCards[index];
                card.SetCardValue(values[index]);
                card.SetDisplayingOrder(index);
                card.SetByteValue(values[index]);
            }
        }

        public void HideCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(false);
            }
        }

        public void ShowCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(true);
            }
        }

        public void ReceiveDisplayingCard(Card card)
        {
            DisplayingCards.Add(card);
            card.OwnerId = PlayerId;
            numberOfDisplayingCards++;
        }

        public void RemoveDisplayingCard(Card card)
        {
            DisplayingCards.Remove(card);
            card.OwnerId = null;
            numberOfDisplayingCards--;
        }

        public void ResetAllCards(Player player)
        {
            if (DisplayingCards.Count > 0)
            {
                for (int i = (DisplayingCards.Count - 1); i > -1; i--)
                {
                    Card c = DisplayingCards[i];
                    DisplayingCards.Remove(c);
                    numberOfDisplayingCards--;
                    c.transform.position = player.RemoveCardPosition;
                }
            }
        }

        public void RepositionDisplayingCards(CardAnimator cardAnimator, Player player)
        {
            numberOfDisplayingCards = 0;
            int totalCards = DisplayingCards.Count;

            foreach (Card card in DisplayingCards)
            {
                numberOfDisplayingCards++;
                card.SetDisplayingOrder(numberOfDisplayingCards);
                cardAnimator.AddCardAnimation(card, NextCardPosition(player, totalCards));
            }
        }

        public void SendDisplayingCardToCenter(CardAnimator cardAnimator, byte cardValue, Player player, bool isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                foreach (Card c in DisplayingCards)
                {
                    if (c.byteValue == cardValue)
                    {
                        PlayedCard = c;
 //                       c.Rank == Card.GetRank(cardValue) && c.Suit == Card.GetSuit(cardValue)
                        break;
                    }
                }
            }
            else
            {
                PlayedCard = DisplayingCards[DisplayingCards.Count - 1];
                PlayedCard.SetCardValue(cardValue);
                PlayedCard.SetFaceUp(true);
            }

            if (PlayedCard !=null)
            {
                PlayedCard.transform.position = player.PlayPosition;
                PlayedCard.transform.localScale = new Vector3(2.5F, 2.5F, 2.5f);
                DisplayingCards.Remove(PlayedCard);
                numberOfDisplayingCards--;
                RepositionDisplayingCards(cardAnimator, player);
            }
            else
            {
                Debug.Log("unable to find displaying card");
            }
        }

        public void RemovePlayedCard(Player player)
        {
            PlayedCard.SetFaceUp(false);
            PlayedCard.transform.position = player.RemoveCardPosition;
        }

        public Card GetPlayedCard()
        {
            if (PlayedCard != null)
            {
                return PlayedCard;
            }
            else
            {
                return null;
            }
        }

        public void RemoveExtraCards(CardAnimator cardAnimator, Player player, byte selectedCard1, byte selectedCard2, byte selectedCard3, bool isLocalPlayer)
        {
            Card CardToRemove1 = null;
            Card CardToRemove2 = null;
            Card CardToRemove3 = null;

            if (isLocalPlayer)
            {
                foreach (Card c in DisplayingCards)
                {
                    if (c.Rank == Card.GetRank(selectedCard1) && c.Suit == Card.GetSuit(selectedCard1))
                    {
                        CardToRemove1 = c;
                        break;
                    }
                }
                foreach (Card c in DisplayingCards)
                {
                    if (c.Rank == Card.GetRank(selectedCard2) && c.Suit == Card.GetSuit(selectedCard2))
                    {
                        CardToRemove2 = c;

                        if (CardToRemove1 != CardToRemove2)
                        {
                            break;
                        }
                    }
                }
                foreach (Card c in DisplayingCards)
                { 
                    if (c.Rank == Card.GetRank(selectedCard3) && c.Suit == Card.GetSuit(selectedCard3))
                    {
                        CardToRemove3 = c;

                        if (CardToRemove3 != CardToRemove2 && CardToRemove3 != CardToRemove1)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                CardToRemove1 = DisplayingCards[DisplayingCards.Count - 1];
                CardToRemove2 = DisplayingCards[DisplayingCards.Count - 2];
                CardToRemove3 = DisplayingCards[DisplayingCards.Count - 3];
            }

            DisplayingCards.Remove(CardToRemove1);
            DisplayingCards.Remove(CardToRemove2);
            DisplayingCards.Remove(CardToRemove3);
            numberOfDisplayingCards = numberOfDisplayingCards - 3;


            CardToRemove1.SetFaceUp(false);
            CardToRemove1.transform.position = player.RemoveCardPosition;
            CardToRemove2.SetFaceUp(false);
            CardToRemove2.transform.position = player.RemoveCardPosition;
            CardToRemove3.SetFaceUp(false);
            CardToRemove3.transform.position = player.RemoveCardPosition;

            RepositionDisplayingCards(cardAnimator, player);
        }

        public void Remove3Cards(CardAnimator cardAnimator, Player receivingPlayer, Player playerToSkip, byte Card1, byte Card2, byte Card3, bool isGivingPlayer, bool isReceivingPlayer)
        {
            Card CardToAdd1 = null;
            Card CardToAdd2 = null;
            Card CardToAdd3 = null;

            if (isReceivingPlayer)
            {
                int playerDisplayingCardsCount = DisplayingCards.Count;

                CardToAdd1 = DisplayingCards[playerDisplayingCardsCount - 1];
                CardToAdd1.SetCardValue(Card1);
                CardToAdd1.SetFaceUp(true);

                CardToAdd2 = DisplayingCards[playerDisplayingCardsCount - 2];
                CardToAdd2.SetCardValue(Card2);
                CardToAdd2.SetFaceUp(true);

                CardToAdd3 = DisplayingCards[playerDisplayingCardsCount - 3];
                CardToAdd3.SetCardValue(Card3);
                CardToAdd3.SetFaceUp(true);
            }
            else if (isGivingPlayer)
            {
                foreach (Card c in DisplayingCards)
                {
                    if (c.Rank == Card.GetRank(Card1) && c.Suit == Card.GetSuit(Card1))
                    {
                        CardToAdd1 = c;
                    }
                }
                foreach (Card c in DisplayingCards)
                {
                    if (c.Rank == Card.GetRank(Card2) && c.Suit == Card.GetSuit(Card2))
                    {
                        CardToAdd2 = c;

                        if (CardToAdd2 != CardToAdd1)
                        {
                            break;
                        }
                    }
                }
                foreach (Card c in DisplayingCards)
                {
                    if (c.Rank == Card.GetRank(Card3) && c.Suit == Card.GetSuit(Card3))
                    {
                        CardToAdd3 = c;

                        if (CardToAdd3 != CardToAdd1 && CardToAdd3 != CardToAdd2)
                        {
                            break;
                        }
                    }
                }

                CardToAdd1.SetFaceUp(false);
                CardToAdd2.SetFaceUp(false);
                CardToAdd3.SetFaceUp(false);
            }
            else
            {
                CardToAdd1 = DisplayingCards[DisplayingCards.Count - 1];
                CardToAdd2 = DisplayingCards[DisplayingCards.Count - 2];
                CardToAdd3 = DisplayingCards[DisplayingCards.Count - 3];
            }

            DisplayingCards.Remove(CardToAdd1);
            DisplayingCards.Remove(CardToAdd2);
            DisplayingCards.Remove(CardToAdd3);

            receivingPlayer.ReceiveDisplayingCard(CardToAdd1);
            CardToAdd1.SetDisplayingOrder(13);
            CardToAdd1.transform.position = receivingPlayer.NextCardPosition(receivingPlayer, 13);
            receivingPlayer.ReceiveDisplayingCard(CardToAdd2);
            CardToAdd2.SetDisplayingOrder(14);
            CardToAdd2.transform.position = receivingPlayer.NextCardPosition(receivingPlayer, 14);
            receivingPlayer.ReceiveDisplayingCard(CardToAdd3);
            CardToAdd3.SetDisplayingOrder(15);
            CardToAdd3.transform.position = receivingPlayer.NextCardPosition(receivingPlayer, 15);

            numberOfDisplayingCards = numberOfDisplayingCards - 3;

            RepositionDisplayingCards(cardAnimator, receivingPlayer);
            RepositionDisplayingCards(cardAnimator, playerToSkip);
        }

        public bool Equals(Player other)
        {
            if (PlayerId.Equals(other.PlayerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
