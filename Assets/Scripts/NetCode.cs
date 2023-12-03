using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.Events;
using System;

namespace Hasenpfeffer
{
    [Serializable]
    public class GameDataEvent : UnityEvent<EncryptedData>
    {

    }

    [Serializable]
    public class TrumpSelectedEvent : UnityEvent<int>
    {

    }

    [Serializable]
    public class BidSelectedEvent : UnityEvent<int>
    {

    }

    [Serializable]
    public class CardPlayedEvent : UnityEvent<byte>
    {

    }

    [Serializable]
    public class ThreeCardsSelectedEvent : UnityEvent<byte,byte,byte>
    {

    }

    public class NetCode : MonoBehaviour
    {
        public GameDataEvent OnGameDataReadyEvent = new GameDataEvent();
        public GameDataEvent OnGameDataChangedEvent = new GameDataEvent();

        public UnityEvent OnGameStateChangedEvent = new UnityEvent();

        public BidSelectedEvent OnBidSelected = new BidSelectedEvent();

        public TrumpSelectedEvent OnTrumpSelected = new TrumpSelectedEvent();

        public CardPlayedEvent OnCardPlayed = new CardPlayedEvent();

        public ThreeCardsSelectedEvent On3CardsSelected = new ThreeCardsSelectedEvent();

        public UnityEvent OnLeftRoom = new UnityEvent();

        public UnityEvent OnPlayerReady = new UnityEvent();

        RoomPropertyAgent roomPropertyAgent;
        RoomRemoteEventAgent roomRemoteEventAgent;

        const string ENCRYPTED_DATA = "EncryptedData";
        const string GAME_STATE_CHANGED = "GameStateChanged";
        const string BID_SELECTED = "BidSelected";
        const string TRUMP_SELECTED = "TrumpSelected";
        const string CARD_PLAYED = "CardPlayed";
        const string THREE_CARDS_SELECTED = "ThreeCardsSelected";
        const string PLAYER_READY = "PlayerReady";

        public void ModifyGameData(EncryptedData encryptedData)
        {
            roomPropertyAgent.Modify(ENCRYPTED_DATA, encryptedData);
        }

        public void NotifyOtherPlayersGameStateChanged()
        {
            roomRemoteEventAgent.Invoke(GAME_STATE_CHANGED);
        }

        public void NotifyHostPlayerBidSelected(int selectedBid)
        {
            SWNetworkMessage message = new SWNetworkMessage();
            message.Push(selectedBid);
            roomRemoteEventAgent.Invoke(BID_SELECTED, message);
        }

        public void NotifyHostPlayerTrumpSelected(int trump)
        {
        	SWNetworkMessage message = new SWNetworkMessage();
            message.Push(trump);
            roomRemoteEventAgent.Invoke(TRUMP_SELECTED, message);
        }

        public void NotifyHostPlayerCardPlayed(byte value)
        {
        	SWNetworkMessage message = new SWNetworkMessage();
        	message.Push(value);
        	roomRemoteEventAgent.Invoke(CARD_PLAYED, message);
        }

        public void NotifyHostPlayer3CardsSelected(byte card1, byte card2, byte card3)
        {
            SWNetworkMessage message = new SWNetworkMessage();
            /*    message.Push(card1rank);
                message.Push(card1suit);
                message.Push(card2rank);
                message.Push(card2suit);
                message.Push(card3rank);
                message.Push(card3suit);  */
            message.Push(card1);
            message.Push(card2);
            message.Push(card3);
            roomRemoteEventAgent.Invoke(THREE_CARDS_SELECTED, message);
        }

        public void NotifyHostPlayerReady()
        {
            roomRemoteEventAgent.Invoke(PLAYER_READY);
        }

        public void EnableRoomPropertyAgent()
        {
            bool auto = roomPropertyAgent.AutoInitialization;
            Debug.Log("auto" + auto);
            roomPropertyAgent.Initialize();
        }

        public void LeaveRoom()
        {
            NetworkClient.Instance.DisconnectFromRoom();
            NetworkClient.Lobby.LeaveRoom((successful, error) => {

                if (successful)
                {
                    Debug.Log("Left room");
                }
                else
                {
                    Debug.Log($"Failed to leave room {error}");
                }

                OnLeftRoom.Invoke();
            });
        }

        private void Awake()
        {
            roomPropertyAgent = FindObjectOfType<RoomPropertyAgent>();
            roomRemoteEventAgent = FindObjectOfType<RoomRemoteEventAgent>();
        }

        //****************** Room Property Events *********************//
        public void OnEncryptedDataReady()
        {
            Debug.Log("OnEncryptedDataReady");
            EncryptedData encryptedData = roomPropertyAgent.GetPropertyWithName(ENCRYPTED_DATA).GetValue<EncryptedData>();
            OnGameDataReadyEvent.Invoke(encryptedData);
        }

        public void OnEncryptedDataChanged()
        {
            Debug.Log("OnEncryptedDataChanged");
            EncryptedData encryptedData = roomPropertyAgent.GetPropertyWithName(ENCRYPTED_DATA).GetValue<EncryptedData>();
            OnGameDataChangedEvent.Invoke(encryptedData);
        }

        //****************** Room Remote Events *********************//
        public void OnGameStateChangedRemoteEvent()
        {
            OnGameStateChangedEvent.Invoke();
        }

        public void OnBidSelectedRemoteEvent(SWNetworkMessage message)
        {
        	int intBid = message.PopInt32();
        	OnBidSelected.Invoke(intBid);
        }

        public void OnTrumpSelectedRemoteEvent(SWNetworkMessage message)
        {
        	int intTrump = message.PopInt32();
        	OnTrumpSelected.Invoke(intTrump);
        }

        public void OnCardPlayedRemoteEvent(SWNetworkMessage message)
        {
        	byte value = message.PopByte();
        	OnCardPlayed.Invoke(value);
        }

        public void On3CardsSelectedRemoteEvent(SWNetworkMessage message)
        {
            byte card1 = message.PopByte();
            byte card2 = message.PopByte();
            byte card3 = message.PopByte();
            On3CardsSelected.Invoke(card1, card2, card3);
        }

        public void OnPlayerReadyEvent()
        {
            OnPlayerReady.Invoke();
        }
    }
}