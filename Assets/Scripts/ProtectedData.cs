using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hasenpfeffer;
using SWNetwork;
using UnityEngine;

namespace Hasenpfeffer
{
    /// <summary>
    /// Stores the important data of the game
    /// We will encypt the fields in a multiplayer game.
    /// </summary>
    /// 
    public class NoTrumpCompare : IComparer<byte>
    {
        public int Compare(byte left, byte right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (left < right)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
     
    public class HeartsTrumpCompare : IComparer<byte>
    {
        public int Compare(byte left, byte right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (left == 28 || left == 29)
            {
                if (right > 11)
                {
                    return -1;
                }
                else if (right == 4 || right == 5)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (left == 4 || left == 5)
            {
                if (right < 12)
                {
                    return 1;
                }
                else if (right == 28 || right == 29)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (right == 28 || right == 29)
            {
                if (left > 11)
                {
                    return 1;
                }
                else if (left == 4 || left == 5)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (right == 4 || right == 5)
            {
                if (left < 12)
                {
                    return -1;
                }
                else if (left == 28 || left == 29)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (left < right)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    public class ClubsTrumpCompare : IComparer<byte>
    {
        public int Compare(byte left, byte right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (left == 40 || left == 41)
            {
                if (right < 12)
                {
                    return 1;
                }
                else if (right > 23)
                {
                    return -1;
                }
                else if (right == 16 || right == 17)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (left == 16 || left == 17)
            {
                if (right < 16)
                {
                    return 1;
                }
                else if (right == 40 || right == 41)
                {
                    return 1;
                }
                else if (right > 23)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (right == 40 || right == 41)
            {
                if (left < 12)
                {
                    return -1;
                }
                else if (left > 23)
                {
                    return 1;
                }
                else if (left == 16 || left == 17)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (right == 16 || right == 17)
            {
                if (left < 16)
                {
                    return -1;
                }
                else if (left == 40 || left == 41)
                {
                    return -1;
                }
                else if (left > 23)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (left < right)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    public class DiamondsTrumpCompare : IComparer<byte>
    {
        public int Compare(byte left, byte right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (left == 4 || left == 5)
            {
                if (right < 24)
                {
                    return 1;
                }
                else if (right > 35)
                {
                    return -1;
                }
                else if (right == 28 || right == 29)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (left == 28 || left == 29)
            {
                if (right < 28)
                {
                    return 1;
                }
                else if (right > 35)
                {
                    return -1;
                }
                else if (right == 4 || right == 5)
                {
                    return 1;
                }
                else
                {
                    return 1;
                }
            }
            else if (right == 4 || right == 5)
            {
                if (left < 24)
                {
                    return -1;
                }
                else if (left > 35)
                {
                    return 1;
                }
                else if (left == 28 || left == 29)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (right == 28 || right == 29)
            {
                if (left < 28)
                {
                    return -1;
                }
                else if (left > 35)
                {
                    return 1;
                }
                else if (left == 4 || left == 5)
                {
                    return -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (left < right)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    public class SpadesTrumpCompare : IComparer<byte>
    {
        public int Compare(byte left, byte right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (left == 16 || left == 17)
            {
                if (right == 40 || right == 41)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (left == 40 || left == 41)
            {
                return 1;
            }
            else if (right == 16 || right == 17)
            {
                if (left == 40 || left == 41)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (right == 40 || right == 41)
            {
                return -1;
            }
            else if (left < right)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    [Serializable]
    public class ProtectedData
    {
        NoTrumpCompare ntc = new NoTrumpCompare();
        HeartsTrumpCompare htc = new HeartsTrumpCompare();
        ClubsTrumpCompare ctc = new ClubsTrumpCompare();
        DiamondsTrumpCompare dtc = new DiamondsTrumpCompare();
        SpadesTrumpCompare stc = new SpadesTrumpCompare();

        [SerializeField]
        List<byte> deckOfCards = new List<byte>();
        [SerializeField]
        List<byte> player1Hand = new List<byte>();
        [SerializeField]
        List<byte> player2Hand = new List<byte>();
        [SerializeField]
        List<byte> player3Hand = new List<byte>();
        [SerializeField]
        List<byte> player4Hand = new List<byte>();
        [SerializeField]
        int player1Team;
        [SerializeField]
        int player2Team;
        [SerializeField]
        int player3Team;
        [SerializeField]
        int player4Team;
        [SerializeField]
        string player1Id;
        [SerializeField]
        string player2Id;
        [SerializeField]
        string player3Id;
        [SerializeField]
        string player4Id;
        [SerializeField]
        string player1Name;
        [SerializeField]
        string player2Name;
        [SerializeField]
        string player3Name;
        [SerializeField]
        string player4Name;
        [SerializeField]
        string player1Bid;
        [SerializeField]
        string player2Bid;
        [SerializeField]
        string player3Bid;
        [SerializeField]
        string player4Bid;
        [SerializeField]
        string currentPlayerId;
        [SerializeField]
        string playerToSkipId;
        [SerializeField]
        string highBidPlayerId;
        [SerializeField]
        string highTrickPlayerId;
        [SerializeField]
        int currentGameState;
        [SerializeField]
        int selectedBid;
        [SerializeField]
        int highBid;
        [SerializeField]
        int trump;
        [SerializeField]
        int oppositeTrump;
        [SerializeField]
        int ledSuit;
        [SerializeField]
        int team1Score;
        [SerializeField]
        int team2Score;
        [SerializeField]
        string dealer;
        [SerializeField]
        int team1Tricks;
        [SerializeField]
        int team2Tricks;
        [SerializeField]
        int winnerTeam;
        [SerializeField]
        byte playedCard;
        [SerializeField]
        byte givenCard1;
        [SerializeField]
        byte givenCard2;
        [SerializeField]
        byte givenCard3;
        [SerializeField]
        byte removedCard1;
        [SerializeField]
        byte removedCard2;
        [SerializeField]
        byte removedCard3;
        [SerializeField]
        int actionCount;
        [SerializeField]
        int handCount;


        byte[] encryptionKey;
        byte[] safeData;

        void Awake()
        {
            string roomId = "12345678910111213141516";
            CalculateKey(roomId);
        }


        public ProtectedData(string p1Id, string p1Nam, int p1Team, string p2Id, string p2Nam, int p2Team, string p3Id, string p3Nam, int p3Team, string p4Id, string p4Nam, int p4Team)
        {
            player1Id = p1Id;
            player2Id = p2Id;
            player3Id = p3Id;
            player4Id = p4Id;
            player1Name = p1Nam;
            player2Name = p2Nam;
            player3Name = p3Nam;
            player4Name = p4Nam;
            player1Bid = "";
            player2Bid = "";
            player3Bid = "";
            player4Bid = "";
            player1Team = p1Team;
            player2Team = p2Team;
            player3Team = p3Team;
            player4Team = p4Team;
            currentPlayerId = "";
            playerToSkipId = "";
            highBidPlayerId = "";
            highTrickPlayerId = "";
            actionCount = 0;
            handCount = 0;
            highBid = 0;
            dealer = "";
            winnerTeam = 0;
            selectedBid = 0;
            trump = 5;
            string roomId = "12345678910111213141516";
            CalculateKey(roomId);
            Encrypt();
        }

        public void SetupVariables()
        {
            player1Id = "";
            player2Id = "";
            player3Id = "";
            player4Id = "";
            player1Name = "";
            player2Name = "";
            player3Name = "";
            player4Name = "";
            player1Bid = "";
            player2Bid = "";
            player3Bid = "";
            player4Bid = "";
            player1Team = 0;
            player2Team = 0;
            player3Team = 0;
            player4Team = 0;
            currentPlayerId = "";
            playerToSkipId = "";
            highBidPlayerId = "";
            highTrickPlayerId = "";
            actionCount = 0;
            handCount = 0;
            highBid = 0;
            dealer = "";
            winnerTeam = 0;
            selectedBid = 0;
            trump = 5;
            string roomId = "12345678910111213141516";
            CalculateKey(roomId);
        }

        public void SetDealer(string d)
        {
            Decrypt();
            dealer = d;
            Encrypt();
        }

        public string GetDealer()
        {
            string result;
            Decrypt();
            result = dealer;
            Encrypt();
            return result;
        }

        public string GetPlayer1()
        {
        	string result;
        	Decrypt();
        	result = player1Id;
        	Encrypt();
        	return result;
        }

        public string GetPlayer2()
        {
        	string result;
        	Decrypt();
        	result = player2Id;
        	Encrypt();
        	return result;
        }

        public string GetPlayer3()
        {
        	string result;
        	Decrypt();
        	result = player3Id;
        	Encrypt();
        	return result;
        }

        public string GetPlayer4()
        {
        	string result;
        	Decrypt();
        	result = player4Id;
        	Encrypt();
        	return result;
        }

        public string GetPlayer1Name()
        {
        	string result;
        	Decrypt();
        	result = player1Name;
        	Encrypt();
        	return result;
        }

        public string GetPlayer2Name()
        {
        	string result;
        	Decrypt();
        	result = player2Name;
        	Encrypt();
        	return result;
        }

        public string GetPlayer3Name()
        {
        	string result;
        	Decrypt();
        	result = player3Name;
        	Encrypt();
        	return result;
        }

        public string GetPlayer4Name()
        {
        	string result;
        	Decrypt();
        	result = player4Name;
        	Encrypt();
        	return result;
        }

        public void SetPlayerBid(Player player, string bid)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Bid = bid;
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Bid = bid;
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Bid = bid;
            }
            else
            {
                player4Bid = bid;
            }
            Encrypt();
        }

        public string GetPlayerBid(Player player)
        {
            string result;
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                result = player1Bid;
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                result = player2Bid;
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                result = player3Bid;
            }
            else
            {
                result = player4Bid;
            }
            Encrypt();
            return result;
        }

        public int GetPlayerTeam(string playerId)
        {
        	int result;
           	Decrypt();
            if (playerId == player1Id)
            {
                result = player1Team;
            }
            else if (playerId == player2Id)
            {
                result = player2Team;
            }
            else if (playerId == player3Id)
            {
                result = player3Team;
            }
            else if (playerId == player4Id)
            {
                result = player4Team;
            }
            else
            {
                result = 3;
            }
            Encrypt();
        	return result;
        }

        public void SetTeam1Score(int score)
        {
            Decrypt();
            team1Score = score;
            Encrypt();
        }

        public int GetTeam1Score()
        {
            int result;
            Decrypt();
            result = team1Score;
            Encrypt();
            return result;
        }

        public void SetTeam2Score(int score)
        {
            Decrypt();
            team2Score = score;
            Encrypt();
        }

        public int GetTeam2Score()
        {
            int result;
            Decrypt();
            result = team2Score;
            Encrypt();
            return result;
        }

        public void SetTeam1Tricks(int tricks)
        {
            Decrypt();
            team1Tricks = tricks;
            Encrypt();
        }

        public int GetTeam1Tricks()
        {
            int result;
            Decrypt();
            result = team1Tricks;
            Encrypt();
            return result;
        }

        public void SetTeam2Tricks(int tricks)
        {
            Decrypt();
            team2Tricks = tricks;
            Encrypt();
        }

        public int GetTeam2Tricks()
        {
            int result;
            Decrypt();
            result = team2Tricks;
            Encrypt();
            return result;
        }

        public void SetDeckOfCards(List<byte> cardValues)
        {
            Decrypt();
            deckOfCards = cardValues;
            Encrypt();
        }

        public List<byte> GetDeckOfCards()
        {
            List<byte> result;
            Decrypt();
            result = deckOfCards;
            Encrypt();
            return result;
        }

        public List<byte> PlayerCards(Player player)
        {
            List<byte> result;
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                result = player1Hand;
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                result = player2Hand;
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                result = player3Hand;
            }
            else
            {
                result = player4Hand;
            }
            Encrypt();
            return result;
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            Decrypt();
            trump = GetTrump();

            if (player.PlayerId.Equals(player1Id))
            {
                player1Hand.AddRange(cardValues);

                if (trump == 0)
                {
                    player1Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player1Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player1Hand.Sort(dtc);
                }
                else if (trump ==3)
                {
                    player1Hand.Sort(stc);
                }
                else
                {
                    player1Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Hand.AddRange(cardValues);

                if (trump == 0)
                {
                    player2Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player2Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player2Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player2Hand.Sort(stc);
                }
                else
                {
                    player2Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Hand.AddRange(cardValues);

                if (trump == 0)
                {
                    player3Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player3Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player3Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player3Hand.Sort(stc);
                }
                else
                {
                    player3Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player4Id))
            {
                player4Hand.AddRange(cardValues);

                if (trump == 0)
                {
                    player4Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player4Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player4Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player4Hand.Sort(stc);
                }
                else
                {
                    player4Hand.Sort(ntc);
                }
            }
            Encrypt();
        }             

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Hand.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Hand.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Hand.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else if (player.PlayerId.Equals(player4Id))
            {
                player4Hand.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            Encrypt();
        }

        public void RemoveCardValueFromPlayer(Player player, byte value)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Hand.Remove(value);

                if (trump == 0)
                {
                    player1Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player1Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player1Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player1Hand.Sort(stc);
                }
                else
                {
                    player1Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Hand.Remove(value);

                if (trump == 0)
                {
                    player2Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player2Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player2Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player2Hand.Sort(stc);
                }
                else
                {
                    player2Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Hand.Remove(value);

                if (trump == 0)
                {
                    player3Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player3Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player3Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player3Hand.Sort(stc);
                }
                else
                {
                    player3Hand.Sort(ntc);
                }
            }
            else if (player.PlayerId.Equals(player4Id))
            {
                player4Hand.Remove(value);

                if (trump == 0)
                {
                    player4Hand.Sort(htc);
                }
                else if (trump == 1)
                {
                    player4Hand.Sort(ctc);
                }
                else if (trump == 2)
                {
                    player4Hand.Sort(dtc);
                }
                else if (trump == 3)
                {
                    player4Hand.Sort(stc);
                }
                else
                {
                    player4Hand.Sort(ntc);
                }
            }
            Encrypt();
        }

        public void AddCardValueToPlayer(Player player, byte value)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Hand.Add(value);
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Hand.Add(value);
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Hand.Add(value);
            }
            else if (player.PlayerId.Equals(player4Id))
            {
                player4Hand.Add(value);
            }
            Encrypt();
        }

        public void SortHands()
        {
            Decrypt();
            trump = GetTrump();

            if (trump == 0)
            {
                player1Hand.Sort(htc);
                player2Hand.Sort(htc);
                player3Hand.Sort(htc);
                player4Hand.Sort(htc);
            }
            else if (trump == 1)
            {
                player1Hand.Sort(ctc);
                player2Hand.Sort(ctc);
                player3Hand.Sort(ctc);
                player4Hand.Sort(ctc);
            }
            else if (trump == 2)
            {
                player1Hand.Sort(dtc);
                player2Hand.Sort(dtc);
                player3Hand.Sort(dtc);
                player4Hand.Sort(dtc);
            }
            else if (trump == 3)
            {
                player1Hand.Sort(stc);
                player2Hand.Sort(stc);
                player3Hand.Sort(stc);
                player4Hand.Sort(stc);
            }
            else
            {
                player1Hand.Sort(ntc);
                player2Hand.Sort(ntc);
                player3Hand.Sort(ntc);
                player4Hand.Sort(ntc);
            }
            
            Encrypt();
        }

        public bool GameFinished()
        {
            bool result = false;
            Decrypt();
            Encrypt();

            return result;
        }

        public void SetWinnerTeam(int team)
        {
            Decrypt();
            winnerTeam = team;
            Encrypt();
        }

        public int GetWinnerTeam()
        {
            int result;
            Decrypt();
            result = winnerTeam;
            Encrypt();
            return result;
        }

        public void SetCurrentPlayerId(string playerId)
        {
            Decrypt();
            currentPlayerId = playerId;
            Encrypt();
        }

        public string GetCurrentPlayerId()
        {
            string result;
            Decrypt();
            result = currentPlayerId;
            Encrypt();
            return result;
        }

        public void SetHighBidPlayerId(string playerId)
        {
            Decrypt();
            highBidPlayerId = playerId;
            Encrypt();
        }

        public string GetHighBidPlayerId()
        {
            string result;
            Decrypt();
            result = highBidPlayerId;
            Encrypt();
            return result;
        }

        public void SetPlayerToSkipId(string playerId)
        {
            Decrypt();
            playerToSkipId = playerId;
            Encrypt();
        }

        public string GetPlayerToSkipId()
        {
            string result;
            Decrypt();
            result = playerToSkipId;
            Encrypt();
            return result;
        }

        public void SetGameState(int gameState)
        {
            Decrypt();
            currentGameState = gameState;
            Encrypt();
        }
        public int GetGameState()
        {
            int result;
            Decrypt();
            result = currentGameState;
            Encrypt();
            return result;
        }

        public void SetSelectedBid(int bid)
        {
            Decrypt();
            selectedBid = bid;
            Encrypt();
        }

        public void SetHighBid(int bid)
        {
            Decrypt();
            highBid = bid;
            Encrypt();
        }

        public int GetHighBid()
        {
            int result;
            Decrypt();
            result = highBid;
            Encrypt();
            return result;
        }

        public void SetTrump(int Trump)
        {
            Decrypt();
            trump = Trump;
            Encrypt();
        }

        public int GetTrump()
        {
            int result;
            Decrypt();
            result = trump;
            Encrypt();
            return result;
        }

        public void SetOppositeTrump(int oTrump)
        {
            Decrypt();
            oppositeTrump = oTrump;
            Encrypt();
        }

        public int GetOppositeTrump()
        {
            int result;
            Decrypt();
            result = oppositeTrump;
            Encrypt();
            return result;
        }

        public void SetLedSuit(int suit)
        {
            Decrypt();
            ledSuit = suit;
            Encrypt();
        }

        public int GetLedSuit()
        {
            int result;
            Decrypt();
            result = ledSuit;
            Encrypt();
            return result;
        }

        public int GetSelectedRank()
        {
            int result;
            Decrypt();
            result = selectedBid;
            Encrypt();
            return result;
        }

        public void SetPlayedCard(byte pCard)
        {
            Decrypt();
            playedCard = pCard;
            Encrypt();
        }

        public byte GetPlayedCard()
        {
            byte result;
            Decrypt();
            result = playedCard;
            Encrypt();
            return result;
        }

        public void SetActionCount(int ac)
        {
            Decrypt();
            actionCount = ac;
            Encrypt();
        }

        public int GetActionCount()
        {
            int result;
            Decrypt();
            result = actionCount;
            Encrypt();
            return result;
        }

        public void SetHandCount(int hc)
        {
            Decrypt();
            handCount = hc;
            Encrypt();
        }

        public int GetHandCount()
        {
            int result;
            Decrypt();
            result = handCount;
            Encrypt();
            return result;
        }

        public void SetGivenCard(int cardNum, byte givenCard)
        {
            Decrypt();
            if (cardNum == 1)
            {
                givenCard1 = givenCard;
            }
            else if (cardNum == 2)
            {
                givenCard2 = givenCard;
            }
            else if (cardNum == 3)
            {
                givenCard3 = givenCard;
            }
            Encrypt();
        }

        public byte GetGivenCard(int cardNum)
        {
            byte result;
            Decrypt();
            if (cardNum == 1)
            {
                result = givenCard1;
            }
            else if (cardNum == 2)
            {
                result = givenCard2;
            }
            else if (cardNum == 3)
            {
                result = givenCard3;
            }
            else
            {
                result = 49;
            }
            Encrypt();
            return result;
        }

        public void SetRemovedCard(int cardNum, byte removedCard)
        {
            Decrypt();
            if (cardNum == 1)
            {
                removedCard1 = removedCard;
            }
            else if (cardNum == 2)
            {
                removedCard2 = removedCard;
            }
            else if (cardNum == 3)
            {
                removedCard3 = removedCard;
            }
            Encrypt();
        }

        public byte GetRemovedCard(int cardNum)
        {
            byte result;
            Decrypt();
            if (cardNum == 1)
            {
                result = removedCard1;
            }
            else if (cardNum == 2)
            {
                result = removedCard2;
            }
            else if (cardNum == 3)
            {
                result = removedCard3;
            }
            else
            {
                result = 49;
            }
            Encrypt();
            return result;
        }

        public void SetHighTrickPlayer(string player)
        {
            Decrypt();
            highTrickPlayerId = player;
            Encrypt();

        }

        public string GetHighTrickPlayer()
        {
            Decrypt();
            string result = highTrickPlayerId;
            Encrypt();
            return result;
        }

        public Byte[] ToArray()
        {
            return safeData;
        }

        public void ApplyByteArray(Byte[] byteArray)
        {
            safeData = byteArray;
        }

        void CalculateKey(string roomId)
        {
            string roomIdSubString = roomId.Substring(0, 16);
            encryptionKey = Encoding.UTF8.GetBytes(roomIdSubString);
        }

        void Encrypt()
        {
            SWNetworkMessage message = new SWNetworkMessage();
            message.Push((Byte)deckOfCards.Count);
            message.PushByteArray(deckOfCards.ToArray());

            message.Push((Byte)player1Hand.Count);
            message.PushByteArray(player1Hand.ToArray());

            message.Push((Byte)player2Hand.Count);
            message.PushByteArray(player2Hand.ToArray());

            message.Push((Byte)player3Hand.Count);
            message.PushByteArray(player3Hand.ToArray());

            message.Push((Byte)player4Hand.Count);
            message.PushByteArray(player4Hand.ToArray());

            message.PushUTF8ShortString(player1Id);
            message.PushUTF8ShortString(player2Id);
            message.PushUTF8ShortString(player3Id);
            message.PushUTF8ShortString(player4Id);

            message.PushUTF8ShortString(player1Name);
            message.PushUTF8ShortString(player2Name);
            message.PushUTF8ShortString(player3Name);
            message.PushUTF8ShortString(player4Name);

            message.Push(player1Team);
            message.Push(player2Team);
            message.Push(player3Team);
            message.Push(player4Team);

            message.PushUTF8ShortString(player1Bid);
            message.PushUTF8ShortString(player2Bid);
            message.PushUTF8ShortString(player3Bid);
            message.PushUTF8ShortString(player4Bid);

            message.PushUTF8ShortString(currentPlayerId);
            message.PushUTF8ShortString(playerToSkipId);
            message.PushUTF8ShortString(highBidPlayerId);
            message.PushUTF8ShortString(highTrickPlayerId);
            message.Push(currentGameState);
            message.PushUTF8ShortString(dealer);
            message.Push(trump);
            message.Push(oppositeTrump);
            message.Push(ledSuit);
            message.Push(highBid);
            message.Push(team1Score);
            message.Push(team2Score);
            message.Push(team1Tricks);
            message.Push(team2Tricks);
            message.Push(winnerTeam);
            message.Push(playedCard);
            message.Push(givenCard1);
            message.Push(givenCard2);
            message.Push(givenCard3);
            message.Push(removedCard1);
            message.Push(removedCard2);
            message.Push(removedCard3);
            message.Push(actionCount);
            message.Push(handCount);

            //safeData = AES.EncryptAES128(message.ToArray(), encryptionKey);
            safeData = message.ToArray();
        }

        void Decrypt()
        {
            //int retries = 0;
            byte[] byteArray;

            /*while (true)
            {
                try
                {
                    byteArray = AES.DecryptAES128(safeData, encryptionKey);
                    break;
                }
                catch (ArgumentNullException e)
                {
                    if (retries < 2)
                    {
                        retries++;
                        SetupVariables();
                        byteArray = AES.DecryptAES128(safeData, encryptionKey);
                    }
                    else
                        throw;
                }
            }*/

            byteArray = safeData;

            SWNetworkMessage message = new SWNetworkMessage(byteArray);
            byte deckOfCardsCount = message.PopByte();
            deckOfCards = message.PopByteArray(deckOfCardsCount).ToList();

            byte player1HandCount = message.PopByte();
            player1Hand = message.PopByteArray(player1HandCount).ToList();

            byte player2HandCount = message.PopByte();
            player2Hand = message.PopByteArray(player2HandCount).ToList();

            byte player3HandCount = message.PopByte();
            player3Hand = message.PopByteArray(player3HandCount).ToList();

            byte player4HandCount = message.PopByte();
            player4Hand = message.PopByteArray(player4HandCount).ToList();

            player1Id = message.PopUTF8ShortString();
            player2Id = message.PopUTF8ShortString();
            player3Id = message.PopUTF8ShortString();
            player4Id = message.PopUTF8ShortString();

            player1Name = message.PopUTF8ShortString();
            player2Name = message.PopUTF8ShortString();
            player3Name = message.PopUTF8ShortString();
            player4Name = message.PopUTF8ShortString();

            player1Team = message.PopInt32();
            player2Team = message.PopInt32();
            player3Team = message.PopInt32();
            player4Team = message.PopInt32();

            player1Bid = message.PopUTF8ShortString();
            player2Bid = message.PopUTF8ShortString();
            player3Bid = message.PopUTF8ShortString();
            player4Bid = message.PopUTF8ShortString();

            currentPlayerId = message.PopUTF8ShortString();
            playerToSkipId = message.PopUTF8ShortString();
            highBidPlayerId = message.PopUTF8ShortString();
            highTrickPlayerId = message.PopUTF8ShortString();
            currentGameState = message.PopInt32();
            dealer = message.PopUTF8ShortString();
            trump = message.PopInt32();
            oppositeTrump = message.PopInt32();
            ledSuit = message.PopInt32();
            highBid = message.PopInt32();
            team1Score = message.PopInt32();
            team2Score = message.PopInt32();
            team1Tricks = message.PopInt32();
            team2Tricks = message.PopInt32();
            winnerTeam = message.PopInt32();
            playedCard = message.PopByte();
            givenCard1 = message.PopByte();
            givenCard2 = message.PopByte();
            givenCard3 = message.PopByte();
            removedCard1 = message.PopByte();
            removedCard2 = message.PopByte();
            removedCard3 = message.PopByte();
            actionCount = message.PopInt32();
            handCount = message.PopInt32();
        }
    }
}