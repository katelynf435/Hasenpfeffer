using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace Hasenpfeffer
{
	[Serializable]
    public class EncryptedData
    {
        public byte[] data;
    }

    [Serializable]
    public class GameDataManager 
    {
        Player localPlayer;
        Player remotePlayer1;
        Player remotePlayer2;
        Player remotePlayer3;
        Player playerToSkip;

        [SerializeField]
        ProtectedData protectedData;

        public GameDataManager(Player local, Player remote1, Player remote2, Player remote3, Player skip)
        {
            localPlayer = local;
            remotePlayer1 = remote1;
            remotePlayer2 = remote2;
            remotePlayer3 = remote3;
            playerToSkip = skip;

            Debug.Log("*********");
            Debug.Log(localPlayer.Team);
            Debug.Log(remotePlayer1.Team);
            Debug.Log(remotePlayer2.Team);
            Debug.Log(remotePlayer3.Team);

            protectedData = new ProtectedData(localPlayer.PlayerId, localPlayer.PlayerName, localPlayer.Team, remotePlayer1.PlayerId, remotePlayer1.PlayerName, remotePlayer1.Team, remotePlayer2.PlayerId, remotePlayer2.PlayerName, remotePlayer2.Team, remotePlayer3.PlayerId, remotePlayer3.PlayerName, remotePlayer3.Team);
        }

        public void Shuffle()
        {
            Debug.Log("Shuffle");
            List<byte> cardValues = new List<byte>();

            for (byte value = 0; value < 48; value++)
            {
                cardValues.Add(value);
            }

            List<byte> deckOfCards = new List<byte>();

            for (int i = 0; i < 48; i++)
            {
                int valueIndexToAdd = UnityEngine.Random.Range(0, cardValues.Count);

                byte valueToAdd = cardValues[valueIndexToAdd];
                deckOfCards.Add(valueToAdd);
                cardValues.Remove(valueToAdd);
            }

            protectedData.SetDeckOfCards(deckOfCards);
        }

        public void SetDealer(Player dealer)
        {
            protectedData.SetDealer(dealer.PlayerId);
        }

        public Player GetDealer()
        {
            string playerId = protectedData.GetDealer();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else
            {
                return remotePlayer3;
            }
        }

        public void DealCardValuesToPlayer(Player player, int numberOfCards)
        {
            List<byte> deckOfCards = protectedData.GetDeckOfCards();

            int numberOfCardsInTheDeck = deckOfCards.Count;
            int start = numberOfCardsInTheDeck - numberOfCards;

            List<byte> cardValues = deckOfCards.GetRange(start, numberOfCards);
            deckOfCards.RemoveRange(start, numberOfCards);

            protectedData.AddCardValuesToPlayer(player, cardValues);
            protectedData.SetDeckOfCards(deckOfCards);
        }

        public List<byte> PlayerCards(Player player)
        {
            return protectedData.PlayerCards(player);
        }

        public void SetActionCount(int ac)
        {
            protectedData.SetActionCount(ac);
        }

        public int GetActionCount()
        {
            return protectedData.GetActionCount();
        }

        public void SetHandCount(int hc)
        {
            protectedData.SetHandCount(hc);
        }

        public int GetHandCount()
        {
            return protectedData.GetHandCount();
        }

        public bool NoCardsLeft()
        {
            if (protectedData.PlayerCards(localPlayer) == null && protectedData.PlayerCards(remotePlayer1) == null && protectedData.PlayerCards(remotePlayer2) == null && protectedData.PlayerCards(remotePlayer3) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            protectedData.RemoveCardValuesFromPlayer(player, cardValuesToRemove);
        }

        public void RemoveCardValueFromPlayer(Player player, byte value)
        {
            protectedData.RemoveCardValueFromPlayer(player, value);
        }

        public void AddCardValueToPlayer(Player player, byte value)
        {
            protectedData.AddCardValueToPlayer(player, value);
        }

        public void SortHands()
        {
            protectedData.SortHands();
        }

        public void SetWinnerTeam(int team)
        {
            protectedData.SetWinnerTeam(team);
        }

        public int GetWinnerTeam()
        {
            return protectedData.GetWinnerTeam();
        }

        public bool GameFinished()
        {
            return protectedData.GameFinished();
        }

        public void SetCurrentPlayer(Player player)
        {
            protectedData.SetCurrentPlayerId(player.PlayerId);
        }

        public Player GetCurrentPlayer()
        {
            string playerId = protectedData.GetCurrentPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else
            {
                return remotePlayer3;
            }
        }

        public void SetHighBidPlayer(Player player)
        {
            protectedData.SetHighBidPlayerId(player.PlayerId);
        }

        public Player GetHighBidPlayer()
        {
            string playerId = protectedData.GetHighBidPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else
            {
                return remotePlayer3;
            }
        }

        public void SetPlayerBid(Player player, string bid)
        {
            protectedData.SetPlayerBid(player, bid);
        }

        public string GetPlayerBid(Player player)
        {
            return protectedData.GetPlayerBid(player);
        }

        public void SetPlayerToSkip(Player player)
        {
            protectedData.SetPlayerToSkipId(player.PlayerId);
        }

        public Player GetPlayerToSkip()
        {
            string playerId = protectedData.GetPlayerToSkipId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else if (remotePlayer3.PlayerId.Equals(playerId))
            {
                return remotePlayer3;
            }
            else
            {
                return playerToSkip;
            }
        }

        public void SetGameState(Game.GameState gameState)
        {
            protectedData.SetGameState((int)gameState);
        }

        public Game.GameState GetGameState()
        {
            return (Game.GameState)protectedData.GetGameState();
        }

        public string GetPlayer1()
        {
            return (protectedData.GetPlayer1());
        }

        public string GetPlayer2()
        {
            return (protectedData.GetPlayer2());
        }

        public string GetPlayer3()
        {
            return (protectedData.GetPlayer3());
        }

        public string GetPlayer4()
        {
            return (protectedData.GetPlayer4());
        }

        public string GetPlayer1Name()
        {
            return (protectedData.GetPlayer1Name());
        }

        public string GetPlayer2Name()
        {
            return (protectedData.GetPlayer2Name());
        }

        public string GetPlayer3Name()
        {
            return (protectedData.GetPlayer3Name());
        }

        public string GetPlayer4Name()
        {
            return (protectedData.GetPlayer4Name());
        }

        public int GetPlayerTeam(string playerId)
        {
            return (protectedData.GetPlayerTeam(playerId));
        }

        public void SetTeam1Score(int score)
        {
            protectedData.SetTeam1Score(score);
        }

        public int GetTeam1Score()
        {
            return (protectedData.GetTeam1Score());
        }

        public void SetTeam2Score(int score)
        {
            protectedData.SetTeam2Score(score);
        }

        public int GetTeam2Score()
        {
            return (protectedData.GetTeam2Score());
        }

        public void SetTeam1Tricks(int tricks)
        {
            protectedData.SetTeam1Tricks(tricks);
        }

        public int GetTeam1Tricks()
        {
            return (protectedData.GetTeam1Tricks());
        }

        public void SetTeam2Tricks(int tricks)
        {
            protectedData.SetTeam2Tricks(tricks);
        }

        public int GetTeam2Tricks()
        {
            return (protectedData.GetTeam2Tricks());
        }

        public void SetSelectedBid(int bid)
        {
            protectedData.SetSelectedBid(bid);
        }

        public void SetHighBid(int bid)
        {
            protectedData.SetHighBid(bid);
        }

        public int GetHighBid()
        {
            return protectedData.GetHighBid();
        }

        public void SetHighTrickPlayer(Player player)
        {
            protectedData.SetHighTrickPlayer(player.PlayerId);
        }

        public Player GetHighTrickPlayer()
        {
            string playerId = protectedData.GetHighTrickPlayer();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else if (remotePlayer3.PlayerId.Equals(playerId))
            {
                return remotePlayer3;
            }
            else
            {
                return null;
            }
        }

        public void SetTrump(int trump)
        {
            protectedData.SetTrump(trump);
        }

        public int GetTrump()
        {
            return protectedData.GetTrump();
        }

        public void SetOppositeTrump(int oppositeTrump)
        {
            protectedData.SetOppositeTrump(oppositeTrump);
        }

        public int GetOppositeTrump()
        {
            return protectedData.GetOppositeTrump();
        }

        public void SetLedSuit(int suit)
        {
            protectedData.SetLedSuit(suit);
        }

        public int GetLedSuit()
        {
            return protectedData.GetLedSuit();
        }

        public Ranks GetSelectedRank()
        {
            return (Ranks)protectedData.GetSelectedRank();
        }

        public void SetPlayedCard(byte playedCard)
        {
            protectedData.SetPlayedCard(playedCard);
        }

        public byte GetPlayedCard()
        {
            return protectedData.GetPlayedCard();
        }

        public void SetGivenCard(int cardNum, byte playedCard)
        {
            protectedData.SetGivenCard(cardNum, playedCard);
        }

        public byte GetGivenCard(int cardNum)
        {
            return protectedData.GetGivenCard(cardNum);
        }

        public void SetRemovedCard(int cardNum, byte playedCard)
        {
            protectedData.SetRemovedCard(cardNum, playedCard);
        }

        public byte GetRemovedCard(int cardNum)
        {
            return protectedData.GetRemovedCard(cardNum);
        }

        public EncryptedData EncryptedData()
        {
            Byte[] data = protectedData.ToArray();

            EncryptedData encryptedData = new EncryptedData();
            encryptedData.data = data;


            return encryptedData;
        }

        public void ApplyEncrptedData(EncryptedData encryptedData)
        {
            if (encryptedData == null)
            {
                return;
            }

            protectedData.ApplyByteArray(encryptedData.data);
        }
    }
}
