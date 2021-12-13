using System.Collections.Generic;

namespace Hasenpfeffer
{
    public interface IGameDataManager
    {
        void AddCardValueToPlayer(Player player, byte value);
        void ApplyEncrptedData(EncryptedData encryptedData);
        void DealCardValuesToPlayer(Player player, int numberOfCards);
        EncryptedData EncryptedData();
        bool GameFinished();
        int GetActionCount();
        Player GetCurrentPlayer();
        Player GetDealer();
        Game.GameState GetGameState();
        byte GetGivenCard(int cardNum);
        int GetHandCount();
        int GetHighBid();
        Player GetHighBidPlayer();
        int GetLedSuit();
        int GetOppositeTrump();
        byte GetPlayedCard();
        string GetPlayer1();
        string GetPlayer1Name();
        string GetPlayer2();
        string GetPlayer2Name();
        string GetPlayer3();
        string GetPlayer3Name();
        string GetPlayer4();
        string GetPlayer4Name();
        string GetPlayerBid(Player player);
        int GetPlayerTeam(string playerId);
        Player GetPlayerToSkip();
        byte GetRemovedCard(int cardNum);
        Ranks GetSelectedRank();
        int GetTeam1Score();
        int GetTeam1Tricks();
        int GetTeam2Score();
        int GetTeam2Tricks();
        int GetTrump();
        int GetWinnerTeam();
        bool NoCardsLeft();
        List<byte> PlayerCards(Player player);
        void RemoveCardValueFromPlayer(Player player, byte value);
        void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove);
        void SetActionCount(int ac);
        void SetCurrentPlayer(Player player);
        void SetDealer(Player dealer);
        void SetGameState(Game.GameState gameState);
        void SetGivenCard(int cardNum, byte playedCard);
        void SetHandCount(int hc);
        void SetHighBid(int bid);
        void SetHighBidPlayer(Player player);
        void SetLedSuit(int suit);
        void SetOppositeTrump(int oppositeTrump);
        void SetPlayedCard(byte playedCard);
        void SetPlayerBid(Player player, string bid);
        void SetPlayerToSkip(Player player);
        void SetRemovedCard(int cardNum, byte playedCard);
        void SetSelectedBid(int bid);
        void SetTeam1Score(int score);
        void SetTeam1Tricks(int tricks);
        void SetTeam2Score(int score);
        void SetTeam2Tricks(int tricks);
        void SetTrump(int trump);
        void SetWinnerTeam(int team);
        void Shuffle();
        void SortHands();
    }
}