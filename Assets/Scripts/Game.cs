using SWNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hasenpfeffer
{
    public class Game : MonoBehaviour
    {
        NetCode netCode;
        RoomCustomData roomData_;
        TeamCustomData teamCustomData;

        protected CardAnimator cardAnimator;

        [SerializeField]
        public GameDataManager gameDataManager;

        public Text MessageText;
        public int actionCount;
        public int handCount;
        public Text localPlayerLabel;
        public Text remotePlayer1Label;
        public Text remotePlayer2Label;
        public Text remotePlayer3Label;
        public Text Team1Score;
        public Text Team2Score;
        public Text Team1Tricks;
        public Text Team2Tricks;
        public Text Bids;
        public Text PlayedCards1;
        public Text PlayedCards2;
        public Text LastTrick;
        public Text T1P1;
        public Text T1P2;
        public Text T2P1;
        public Text T2P2;

        public GameObject HeartsButton;
        public GameObject ClubsButton;
        public GameObject DiamondsButton;
        public GameObject SpadesButton;
        public GameObject NoTrumpButton;

        public GameObject SixButton;
        public GameObject SevenButton;
        public GameObject EightButton;
        public GameObject NineButton;
        public GameObject TenButton;
        public GameObject HossButton;
        public GameObject PfefferButton;
        public GameObject PassButton;

        public GameObject RedealButton;

        public Image LocalDealerToken;
        public Image RP1DealerToken;
        public Image RP2DealerToken;
        public Image RP3DealerToken;

        public Image Heart;
        public Image Spade;
        public Image Diamond;
        public Image Club;
        public Image A;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> PlayerPlayPositions = new List<Transform>();
        public Vector3 RemoveCardPosition;
        public Player[] playerAry;
        private EventWaitHandle waitHandle = new ManualResetEvent(false);

        [SerializeField]
        protected Player localPlayer;
        [SerializeField]
        protected Player remotePlayer1;
        [SerializeField]
        protected Player remotePlayer2;
        [SerializeField]
        protected Player remotePlayer3;

        [SerializeField]
        protected Player currentPlayer;
        [SerializeField]
        protected Player highBidPlayer;
        [SerializeField]
        protected Player highTrickPlayer;
        [SerializeField]
        protected Player playerToSkip;
        [SerializeField]
        protected Player dealerPlayer;

        [SerializeField]
        protected int Dealer;
        [SerializeField]
        protected Card selectedCard;
        [SerializeField]
        protected Card givingCard1;
        [SerializeField]
        protected Card givingCard2;
        [SerializeField]
        protected Card givingCard3;
        [SerializeField]
        protected int selectedSuit;
        [SerializeField]
        protected int selectedRank;
        [SerializeField]
        protected int givingCard1Suit;
        [SerializeField]
        protected int givingCard1Rank;
        [SerializeField]
        protected int givingCard2Suit;
        [SerializeField]
        protected int givingCard2Rank;
        [SerializeField]
        protected int givingCard3Suit;
        [SerializeField]
        protected int givingCard3Rank;
        [SerializeField]
        protected int highBid;
        [SerializeField]
        protected int highRank;
        [SerializeField]
        protected int trump;
        [SerializeField]
        protected int oppositeTrump;
        [SerializeField]
        protected int ledSuit;
        [SerializeField]
        protected int team1Score;
        [SerializeField]
        protected int team1Tricks;
        [SerializeField]
        protected int team2Score;
        [SerializeField]
        protected int team2Tricks;
        [SerializeField]
        protected string tempPlayer1Id;
        [SerializeField]
        protected string tempPlayer2Id;
        [SerializeField]
        protected string tempPlayer3Id;
        [SerializeField]
        protected string tempPlayer4Id;
        [SerializeField]
        protected bool player1ready;
        [SerializeField]
        protected bool player2ready;
        [SerializeField]
        protected bool player3ready;
        [SerializeField]
        protected bool AllPlayersReady;
        [SerializeField]
        protected bool alreadyplayed;

        public enum GameState
        {
            Idle,
            GameStarted,
            RoundStarted,
            DealingCards,
            Redeal,
            BidsStarted,
            SelectingBid,
            TurnBidConfirmed,
            SetupHoss,
            SelectCardsToGive,
            SelectCardsToRemove,
            SetupPfeffer,
            StartFirstHand,
            StartNextHand,
            PickTrumpSuit,
            TurnSelectingCard,
            UpdateHossDisplay,
            UpdateCardsDisplayed,
            UpdateDisplayStats,
            GameFinished
        };

        [SerializeField]
        public GameState gameState = GameState.Idle;

        protected void Awake()
        {
            Debug.Log("base awake");
            localPlayer = new Player();
            localPlayer.Position = PlayerPositions[0].position;
            localPlayer.PlayPosition = PlayerPlayPositions[0].position;
            localPlayer.RemoveCardPosition = RemoveCardPosition;
            localPlayer.IsLocal = true;

            remotePlayer1 = new Player();
            remotePlayer1.Position = PlayerPositions[1].position;
            remotePlayer1.PlayPosition = PlayerPlayPositions[1].position;
            remotePlayer1.RemoveCardPosition = RemoveCardPosition;
            remotePlayer1.IsLocal = false;

            remotePlayer2 = new Player();
            remotePlayer2.Position = PlayerPositions[2].position;
            remotePlayer2.PlayPosition = PlayerPlayPositions[2].position;
            remotePlayer2.RemoveCardPosition = RemoveCardPosition;
            remotePlayer2.IsLocal = false;

            remotePlayer3 = new Player();
            remotePlayer3.Position = PlayerPositions[3].position;
            remotePlayer3.PlayPosition = PlayerPlayPositions[3].position;
            remotePlayer3.RemoveCardPosition = RemoveCardPosition;
            remotePlayer3.IsLocal = false;

            playerToSkip = new Player();
            playerToSkip.PlayerId = "";

            LocalDealerToken.gameObject.SetActive(false);
            RP1DealerToken.gameObject.SetActive(false);
            RP2DealerToken.gameObject.SetActive(false);
            RP3DealerToken.gameObject.SetActive(false);

            Heart.gameObject.SetActive(false);
            Diamond.gameObject.SetActive(false);
            Spade.gameObject.SetActive(false);
            Club.gameObject.SetActive(false);
            A.gameObject.SetActive(false);

            netCode = FindObjectOfType<NetCode>();
            cardAnimator = FindObjectOfType<CardAnimator>();

            GetRoomData();
        }

        public void GetRoomData()
        {
            Debug.Log("getroomdata");
            roomData_ = new RoomCustomData();
            roomData_.team1 = new TeamCustomData();
            roomData_.team2 = new TeamCustomData();

            NetworkClient.Lobby.GetRoomCustomData((successful, reply, error) =>
            {
                if (successful)
                {
                    Debug.Log("Got room custom data " + reply);

                    roomData_ = reply.GetCustomData<RoomCustomData>();
                    Debug.Log(roomData_);
                    SetupPlayers();
                }
                else
                {
                    Debug.Log("Failed to get room data " + error);
                    Thread.Sleep(500);
                    GetRoomData();
                }
            });
        }

        public void SetupPlayers()
        {
            Debug.Log("setup playeers");
            foreach (string pID in roomData_.team1.players)
            {
                if (NetworkClient.Instance.PlayerId == pID)
                {
                    localPlayer.Team = 1;
                }
            }
            foreach (string pID in roomData_.team2.players)
            {
                if (NetworkClient.Instance.PlayerId == pID)
                {
                    localPlayer.Team = 2;
                }
            }

            Debug.Log(localPlayer.Team);
            Debug.Log("Get Players in the room");
            NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
            {
                if (successful)
                {
                    foreach (SWPlayer swPlayer in reply.players)
                    {
                        string playerName = swPlayer.GetCustomDataString();
                        string playerId = swPlayer.id;

                        Debug.Log(playerName);
                        Debug.Log(playerId);

                        foreach (string pID in roomData_.team1.players)
                        {
                            Debug.Log(pID);
                            if (pID != playerId)
                            {
                                continue;
                            }
                            if (pID.Equals(NetworkClient.Instance.PlayerId))
                            {
                                localPlayer.PlayerId = playerId;
                                localPlayer.PlayerName = playerName;
                                Debug.Log("in1");
                            }
                            else if (localPlayer.Team == 1)
                            {
                                remotePlayer2.PlayerId = playerId;
                                remotePlayer2.PlayerName = playerName;
                                remotePlayer2.Team = 1;
                                Debug.Log("in2");
                            }
                            else if (remotePlayer1.PlayerId == null)
                            {
                                remotePlayer1.PlayerId = playerId;
                                remotePlayer1.PlayerName = playerName;
                                remotePlayer1.Team = 1;
                                Debug.Log("in3");
                            }
                            else
                            {
                                remotePlayer3.PlayerId = playerId;
                                remotePlayer3.PlayerName = playerName;
                                remotePlayer3.Team = 1;
                                Debug.Log("in4");
                            }
                        }
                        foreach (string pID in roomData_.team2.players)
                        {
                            if (pID != playerId)
                            {
                                continue;
                            }
                            if (pID.Equals(NetworkClient.Instance.PlayerId))
                            {
                                localPlayer.PlayerId = playerId;
                                localPlayer.PlayerName = playerName;
                            }
                            else if (localPlayer.Team == 2)
                            {
                                remotePlayer2.PlayerId = playerId;
                                remotePlayer2.PlayerName = playerName;
                                remotePlayer2.Team = 2;
                            }
                            else if (remotePlayer1.PlayerId == null)
                            {
                                remotePlayer1.PlayerId = playerId;
                                remotePlayer1.PlayerName = playerName;
                                remotePlayer1.Team = 2;
                            }
                            else
                            {
                                remotePlayer3.PlayerId = playerId;
                                remotePlayer3.PlayerName = playerName;
                                remotePlayer3.Team = 2;
                            }
                        }
                    }

                    gameDataManager = new GameDataManager(localPlayer, remotePlayer1, remotePlayer2, remotePlayer3, playerToSkip);
                    netCode.EnableRoomPropertyAgent();

                }
                else
                {
                    Debug.Log("Failed to get players in room.");
                    Thread.Sleep(700);
                    SetupPlayers();
                }
            });
        }

        protected void Start()
        {
            Debug.Log("Multiplayer Game Start");
        }


        //****************** Game Flow *********************//
        public void GameFlow()
        {
            if (gameState > GameState.Redeal && gameState != GameState.UpdateCardsDisplayed)
            {
                ShowPlayerCards();
                ShowAndHidePlayersDisplayingCards();
                SetPlayerTexts();
                SetDealerToken();

                if (gameDataManager.GameFinished())
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                    }
                case GameState.RoundStarted:
                    {
                        Debug.Log("RoundStarted");
                        OnRoundStarted();
                        break;
                    }
                case GameState.DealingCards:
                    {
                        Debug.Log("DealingCards");
                        StartCoroutine(OnDealingCards());
                        break;
                    }
                case GameState.Redeal:
                    {
                        Debug.Log("Redeal");
                        OnRedeal();
                        break;
                    }
                case GameState.BidsStarted:
                    {
                        Debug.Log("BidsStarted");
                        OnBidsStarted();
                        break;
                    }
                case GameState.SelectingBid:
                    {
                        Debug.Log("SelectingBid");
                        OnSelectingBid();
                        break;
                    }
                case GameState.TurnBidConfirmed:
                    {
                        Debug.Log("TurnBidConfirmed");
                        OnBidConfirmed();
                        break;
                    }
                case GameState.PickTrumpSuit:
                    {
                        Debug.Log("PickTrumpSuit");
                        OnPickTrumpSuit();
                        break;
                    }
                case GameState.SetupHoss:
                    {
                        Debug.Log("SetupHoss");
                        OnSetupHoss();
                        break;
                    }
                case GameState.SelectCardsToGive:
                    {
                        Debug.Log("SelectCardsToGive");
                        OnSelectCardsToGive();
                        break;
                    }
                case GameState.SelectCardsToRemove:
                    {
                        Debug.Log("SelectCardsToRemove");
                        OnSelectCardsToRemove();
                        break;
                    }
                case GameState.SetupPfeffer:
                    {
                        Debug.Log("SetupPfeffer");
                        OnSetupPfeffer();
                        break;
                    }
                case GameState.StartFirstHand:
                    {
                        Debug.Log("StartFirstHand");
                        OnStartFirstHand();
                        break;
                    }
                case GameState.StartNextHand:
                    {
                        Debug.Log("StartNextHand");
                        OnStartNextHand();
                        break;
                    }
                case GameState.TurnSelectingCard:
                    {
                        Debug.Log("TurnSelectingCard");
                        OnTurnSelectingCard();
                        break;
                    }
                case GameState.UpdateHossDisplay:
                    {
                        Debug.Log("UpdateHossDisplay");
                        OnUpdateHossDisplay();
                        break;
                    }
                case GameState.UpdateCardsDisplayed:
                    {
                        Debug.Log("UpdateCardsDisplayed");
                        OnUpdateCardsDisplayed();
                        break;
                    }
                case GameState.UpdateDisplayStats:
                    {
                        Debug.Log("DisplayStats");
                        StartCoroutine(OnUpdateDisplayStats());
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        protected void OnGameStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                currentPlayer = new Player();

                Dealer = UnityEngine.Random.Range(1, 4);  //pick a random dealer
                SetDealer();
                currentPlayer = dealerPlayer;
                SwitchToNextPlayer();

                gameState = GameState.RoundStarted;

                gameDataManager.SetCurrentPlayer(dealerPlayer);
                gameDataManager.SetPlayerToSkip(playerToSkip);
                gameDataManager.SetGameState(gameState);
                gameDataManager.SetDealer(dealerPlayer);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        protected void SetDealer()
        {
            Dealer++;

            if (Dealer > 4)
            {
                Dealer = 1;
            }

            if (Dealer == 1)
            {
                dealerPlayer = localPlayer;
            }
            else if (Dealer == 2)
            {
                dealerPlayer = remotePlayer1;
            }
            else if (Dealer == 3)
            {
                dealerPlayer = remotePlayer2;
            }
            else if (Dealer == 4)
            {
                dealerPlayer = remotePlayer3;
            }

            SetDealerToken();
        }

        protected void SyncPlayers()
        {
            tempPlayer1Id = gameDataManager.GetPlayer1();
            tempPlayer2Id = gameDataManager.GetPlayer2();
            tempPlayer3Id = gameDataManager.GetPlayer3();
            tempPlayer4Id = gameDataManager.GetPlayer4();

            if (localPlayer.PlayerId == tempPlayer1Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer2Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                remotePlayer2.PlayerId = tempPlayer3Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer4Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
            }

            else if (localPlayer.PlayerId == tempPlayer2Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer3Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                remotePlayer2.PlayerId = tempPlayer4Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer1Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
            }

            else if (localPlayer.PlayerId == tempPlayer3Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer4Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                remotePlayer2.PlayerId = tempPlayer1Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer2Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
            }

            else if (localPlayer.PlayerId == tempPlayer4Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer1Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                remotePlayer2.PlayerId = tempPlayer2Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer3Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
            }

            SetDealerToken();

            SetPlayerTexts();
        }

        protected void SyncPlayersAgain()
        {
            tempPlayer1Id = gameDataManager.GetPlayer1();
            tempPlayer2Id = gameDataManager.GetPlayer2();
            tempPlayer3Id = gameDataManager.GetPlayer3();
            tempPlayer4Id = gameDataManager.GetPlayer4();

            if (localPlayer.PlayerId == tempPlayer1Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer2Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                remotePlayer2.PlayerId = tempPlayer3Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer4Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
            }

            else if (localPlayer.PlayerId == tempPlayer2Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer3Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                remotePlayer2.PlayerId = tempPlayer4Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer1Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
            }

            else if (localPlayer.PlayerId == tempPlayer3Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer4Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer4Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                remotePlayer2.PlayerId = tempPlayer1Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer2Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
            }

            else if (localPlayer.PlayerId == tempPlayer4Id)
            {
                localPlayer.Team = gameDataManager.GetPlayerTeam(tempPlayer4Id);
                localPlayer.Pos = 1;
                remotePlayer1.PlayerId = tempPlayer1Id;
                remotePlayer1.PlayerName = gameDataManager.GetPlayer1Name();
                remotePlayer1.Team = gameDataManager.GetPlayerTeam(tempPlayer1Id);
                remotePlayer2.PlayerId = tempPlayer2Id;
                remotePlayer2.PlayerName = gameDataManager.GetPlayer2Name();
                remotePlayer2.Team = gameDataManager.GetPlayerTeam(tempPlayer2Id);
                remotePlayer2.Pos = 1;
                remotePlayer3.PlayerId = tempPlayer3Id;
                remotePlayer3.PlayerName = gameDataManager.GetPlayer3Name();
                remotePlayer3.Team = gameDataManager.GetPlayerTeam(tempPlayer3Id);
            }

            SetDealerToken();

            SetPlayerTexts();
        }

        public void OnRoundStarted()
        {
            SyncPlayers();
            localPlayer.ResetAllCards(localPlayer);
            remotePlayer1.ResetAllCards(remotePlayer1);
            remotePlayer2.ResetAllCards(remotePlayer2);
            remotePlayer3.ResetAllCards(remotePlayer3);

            team1Score = gameDataManager.GetTeam1Score();
            team2Score = gameDataManager.GetTeam2Score();

            team1Tricks = gameDataManager.GetTeam1Tricks();
            team2Tricks = gameDataManager.GetTeam2Tricks();

            Team1Score.text = team1Score.ToString();
            Team2Score.text = team2Score.ToString();

            Team1Tricks.text = team1Tricks.ToString();
            Team2Tricks.text = team2Tricks.ToString();

            SetTrumpString();
            UnreadyPlayers();

            if (NetworkClient.Instance.IsHost)
            {
                gameDataManager.Shuffle();
                gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);

                gameState = GameState.DealingCards;

                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public IEnumerator OnDealingCards()
        {
            ClearPlayedCards2();
            ClearBids();
            LastTrick.gameObject.SetActive(false);
            SyncPlayersAgain();

            yield return StartCoroutine(cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS, true));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer1, Constants.PLAYER_INITIAL_CARDS, false));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer2, Constants.PLAYER_INITIAL_CARDS, false));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer3, Constants.PLAYER_INITIAL_CARDS, false));

            if (NetworkClient.Instance.IsHost)
            {
                gameState = GameState.BidsStarted;

                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnRedeal()
        {
            SetMessage("Dealing.");

            localPlayer.ResetAllCards(localPlayer);
            remotePlayer1.ResetAllCards(remotePlayer1);
            remotePlayer2.ResetAllCards(remotePlayer2);
            remotePlayer3.ResetAllCards(remotePlayer3);

            HideAllButtons();
            ClearBids();

            if (NetworkClient.Instance.IsHost)
            {
                gameDataManager.Shuffle();
                gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.SetPlayerBid(localPlayer, "");
                gameDataManager.SetPlayerBid(remotePlayer1, "");
                gameDataManager.SetPlayerBid(remotePlayer2, "");
                gameDataManager.SetPlayerBid(remotePlayer3, "");
                gameDataManager.SetActionCount(0);

                currentPlayer = dealerPlayer;

                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameState = GameState.DealingCards;

                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnBidsStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                SwitchToNextPlayer();

                gameState = GameState.SelectingBid;

                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnSelectingBid()
        {
            highBid = gameDataManager.GetHighBid();
            highBidPlayer = gameDataManager.GetHighBidPlayer();
            dealerPlayer = gameDataManager.GetDealer();
            currentPlayer = gameDataManager.GetCurrentPlayer();

            localPlayer.bid = gameDataManager.GetPlayerBid(localPlayer);
            remotePlayer1.bid = gameDataManager.GetPlayerBid(remotePlayer1);
            remotePlayer2.bid = gameDataManager.GetPlayerBid(remotePlayer2);
            remotePlayer3.bid = gameDataManager.GetPlayerBid(remotePlayer3);

            Player[] playerAry = { localPlayer, remotePlayer1, remotePlayer2, remotePlayer3 };

            if (NetworkClient.Instance.IsHost)
            {
                RedealButton.SetActive(true);
            }
            if (currentPlayer == localPlayer)
            {
                int check = 0;

                foreach (Player player in playerAry)
                {
                    if (player.bid != "")
                    {
                        check++;
                    }
                }

                if (check == 3)
                {
                    SetMessage($"You are last bid.");
                }
                else
                {
                    SetMessage($"Select your Bid.");
                }

                if (highBid < 6)
                {
                    SixButton.SetActive(true);
                }
                if (highBid < 7)
                {
                    SevenButton.SetActive(true);
                }
                if (highBid < 8)
                {
                    EightButton.SetActive(true);
                }
                if (highBid < 9)
                {
                    NineButton.SetActive(true);
                }
                if (highBid < 10)
                {
                    TenButton.SetActive(true);
                }
                if (highBid < 16)
                {
                    HossButton.SetActive(true);
                }
                if (highBid < 32)
                {
                    PfefferButton.SetActive(true);
                }
                if (highBid < 6 && check == 3)
                {
                    PassButton.SetActive(false);
                }
                else
                {
                    PassButton.SetActive(true);
                }
            }
            else
            {
                SetMessage($"{currentPlayer.PlayerName}'s turn.");
                HideAllButtons();
            }

            SetPlayerTexts();
        }

        protected void OnBidConfirmed()
        {
            localPlayer.bid = gameDataManager.GetPlayerBid(localPlayer);
            remotePlayer1.bid = gameDataManager.GetPlayerBid(remotePlayer1);
            remotePlayer2.bid = gameDataManager.GetPlayerBid(remotePlayer2);
            remotePlayer3.bid = gameDataManager.GetPlayerBid(remotePlayer3);
            dealerPlayer = gameDataManager.GetDealer();

            SetPlayerTexts();

            SetBids($"{currentPlayer.PlayerName} bid {currentPlayer.bid}");

            if (NetworkClient.Instance.IsHost)
            {
                actionCount = gameDataManager.GetActionCount();

                /*if (actionCount < 4)
                {
                    gameState = GameState.BidsStarted;
                }*/
                int check = 0;
                Player[] playerAry = { localPlayer, remotePlayer1, remotePlayer2, remotePlayer3 };

                foreach (Player player in playerAry)
                {
                    if (player.bid != "")
                    {
                        check++;
                    }
                }
                if (check < 4)
                {
                    gameState = GameState.BidsStarted;
                }
                else
                {
                    gameState = GameState.StartFirstHand;
                    RedealButton.SetActive(false);
                }
                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        protected void OnSetupHoss()
        {
            SetTrumpString();
            HideAllButtons();

            alreadyplayed = false;

            if (NetworkClient.Instance.IsHost)
            {
                SetPlayerToSkip();

                currentPlayer = playerToSkip;

                gameState = GameState.SelectCardsToGive;

                gameDataManager.SetPlayerToSkip(playerToSkip);
                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        protected void OnSelectCardsToGive()
        {
            HideAllButtons();

            if (currentPlayer == localPlayer)
            {
                SetMessage($"Pick 3 cards to give.");
            }
            else
            {
                SetMessage($"Waiting on {currentPlayer.PlayerName}.");
            }
        }

        protected void OnSelectCardsToRemove()
        {
            if (currentPlayer == localPlayer)
            {
                SetMessage($"Pick 3 cards to remove.");
            }
            else
            {
                SetMessage($"Waiting on {currentPlayer.PlayerName}.");
            }

            highBidPlayer = gameDataManager.GetHighBidPlayer();

            byte givenCard1 = gameDataManager.GetGivenCard(1);
            byte givenCard2 = gameDataManager.GetGivenCard(2);
            byte givenCard3 = gameDataManager.GetGivenCard(3);

            playerToSkip = gameDataManager.GetPlayerToSkip();

            bool isGivingPlayer = localPlayer == playerToSkip;
            bool isReceivingPlayer = localPlayer == highBidPlayer;

            Debug.Log("Highbidplayer");

            playerToSkip.Remove3Cards(cardAnimator, currentPlayer, playerToSkip, givenCard1, givenCard2, givenCard3, isGivingPlayer, isReceivingPlayer);
        }

        protected void OnSetupPfeffer()
        {
            if (NetworkClient.Instance.IsHost)
            {
                SetPlayerToSkip();

                currentPlayer = highBidPlayer;

                gameState = GameState.TurnSelectingCard;

                gameDataManager.SetPlayerToSkip(playerToSkip);
                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        protected void OnStartFirstHand()
        {
            highBidPlayer = gameDataManager.GetHighBidPlayer();
            highBid = gameDataManager.GetHighBid();

            localPlayer.bid = gameDataManager.GetPlayerBid(localPlayer);
            remotePlayer1.bid = gameDataManager.GetPlayerBid(remotePlayer1);
            remotePlayer2.bid = gameDataManager.GetPlayerBid(remotePlayer2);
            remotePlayer3.bid = gameDataManager.GetPlayerBid(remotePlayer3);

            ClearBids();
            SetBids($"{highBidPlayer.PlayerName} bid {highBidPlayer.bid}");
            SetPlayerTexts();
            SetDealerToken();

            if (NetworkClient.Instance.IsHost)
            {
                currentPlayer = highBidPlayer;
                gameState = GameState.PickTrumpSuit;

                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        protected void OnStartNextHand()
        {
            SetPlayerTexts();
            ClearPlayedCards1();

            playerToSkip = gameDataManager.GetPlayerToSkip();

            if (localPlayer != playerToSkip)
            {
                localPlayer.RemovePlayedCard(localPlayer);
            }
            if (remotePlayer1 != playerToSkip)
            {
                remotePlayer1.RemovePlayedCard(remotePlayer1);
            }
            if (remotePlayer2 != playerToSkip)
            {
                remotePlayer2.RemovePlayedCard(remotePlayer2);
            }
            if (remotePlayer3 != playerToSkip)
            {
                remotePlayer3.RemovePlayedCard(remotePlayer3);
            }

            handCount = gameDataManager.GetHandCount();

            if (handCount < 12)
            {
                if (NetworkClient.Instance.IsHost)
                {
                    highTrickPlayer = gameDataManager.GetHighTrickPlayer();
                    currentPlayer = highTrickPlayer;

                    gameState = GameState.TurnSelectingCard;

                    gameDataManager.SetCurrentPlayer(currentPlayer);
                    gameDataManager.SetGameState(gameState);

                    netCode.ModifyGameData(gameDataManager.EncryptedData());
                }
            }
            else
            {
                localPlayer.ResetAllCards(localPlayer);
                remotePlayer1.ResetAllCards(remotePlayer1);
                remotePlayer2.ResetAllCards(remotePlayer2);
                remotePlayer3.ResetAllCards(remotePlayer3);

                team1Score = gameDataManager.GetTeam1Score();
                team2Score = gameDataManager.GetTeam2Score();

                team1Tricks = gameDataManager.GetTeam1Tricks();
                team2Tricks = gameDataManager.GetTeam2Tricks();
                highBid = gameDataManager.GetHighBid();


                if (NetworkClient.Instance.IsHost)
                {
                    if (team1Score > 63 && gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 2 && ((team2Tricks < highBid && highBid < 16) || (team2Tricks < 12 && highBid > 12)))
                    {
                        gameState = GameState.GameFinished;
                        gameDataManager.SetWinnerTeam(1);
                        gameDataManager.SetGameState(gameState);
                        //netCode.ModifyGameData(gameDataManager.EncryptedData());
                    }
                    else if (team2Score > 63 && gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 1 && ((team1Tricks < highBid && highBid < 16) || (team1Tricks < 12 && highBid > 12)))
                    {
                        gameState = GameState.GameFinished;
                        gameDataManager.SetWinnerTeam(2);
                        gameDataManager.SetGameState(gameState);
                        //netCode.ModifyGameData(gameDataManager.EncryptedData());
                    }
                    else
                    {
                        scoreRound();

                        gameDataManager.SetTeam1Score(team1Score);
                        gameDataManager.SetTeam2Score(team2Score);

                        if (team1Score > 63 && gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 1)
                        {
                            gameState = GameState.GameFinished;
                            gameDataManager.SetWinnerTeam(1);
                            gameDataManager.SetGameState(gameState);
                            //netCode.ModifyGameData(gameDataManager.EncryptedData());
                        }
                        else if (team2Score > 63 && gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 2)
                        {
                            gameState = GameState.GameFinished;
                            gameDataManager.SetWinnerTeam(2);
                            gameDataManager.SetGameState(gameState);
                            //netCode.ModifyGameData(gameDataManager.EncryptedData());
                        }
                        else
                        {
                            SwitchToNextDealer();
                            currentPlayer = dealerPlayer;

                            gameState = GameState.RoundStarted;

                            gameDataManager.SetTeam1Tricks(0);
                            gameDataManager.SetTeam2Tricks(0);
                            gameDataManager.SetCurrentPlayer(currentPlayer);
                            gameDataManager.SetGameState(gameState);
                            gameDataManager.SetDealer(dealerPlayer);

                            ResetVariables();

                        }
                    }
                }

                team1Score = gameDataManager.GetTeam1Score();
                team2Score = gameDataManager.GetTeam2Score();

                team1Tricks = gameDataManager.GetTeam1Tricks();
                team2Tricks = gameDataManager.GetTeam2Tricks();

                Team1Score.text = team1Score.ToString();
                Team2Score.text = team2Score.ToString();

                Team1Tricks.text = team1Tricks.ToString();
                Team2Tricks.text = team2Tricks.ToString();

                SetDealerToken();

                if (NetworkClient.Instance.IsHost)
                {
                    netCode.ModifyGameData(gameDataManager.EncryptedData());
                }
            }
        }

        protected void OnPickTrumpSuit()
        {

            if (currentPlayer == localPlayer)
            {
                HideAllButtons();
                SetMessage($"Pick Trump.");
                HeartsButton.SetActive(true);
                ClubsButton.SetActive(true);
                DiamondsButton.SetActive(true);
                SpadesButton.SetActive(true);
                NoTrumpButton.SetActive(true);
            }
            else
            {
                SetMessage($"{currentPlayer.PlayerName}'s turn.");
                HideAllButtons();
            }
        }

        protected void OnTurnSelectingCard()
        {
            handCount = gameDataManager.GetHandCount();

            if (gameDataManager.PlayerCards(currentPlayer).Count == 1)
            {
                alreadyplayed = true;
            }
            else
            {
                alreadyplayed = false;
            }

            HideAllButtons();
            SetTrumpString(); 

            if (handCount == 1)
            {
                LastTrick.gameObject.SetActive(true);
            }

            if (currentPlayer == localPlayer)
            {
                Debug.Log("Update text if");
                SetMessage($"Pick a card to play.");
            }
            else
            {
                Debug.Log("Update text else");
                SetMessage($"{currentPlayer.PlayerName}'s turn.");
            }

            //Auto play last card

            if (NetworkClient.Instance.IsHost)
            {
                if (gameDataManager.PlayerCards(currentPlayer).Count == 1)
                {
                    byte lastCard = gameDataManager.PlayerCards(currentPlayer).ElementAt(0);
                    Debug.Log(lastCard);
                    netCode.NotifyHostPlayerCardPlayed(lastCard);
                }
            }
        }

        public void OnUpdateHossDisplay()
        {
            byte selectedCard1 = gameDataManager.GetRemovedCard(1);
            byte selectedCard2 = gameDataManager.GetRemovedCard(2);
            byte selectedCard3 = gameDataManager.GetRemovedCard(3);

            bool isLocalPlayer = currentPlayer == localPlayer;

            highBidPlayer.RemoveExtraCards(cardAnimator, highBidPlayer, selectedCard1, selectedCard2, selectedCard3, isLocalPlayer);

            if (NetworkClient.Instance.IsHost)
            {
                gameState = GameState.TurnSelectingCard;

                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnUpdateCardsDisplayed()
        {
            Debug.Log("Update Card Display");

            byte playedCard = gameDataManager.GetPlayedCard();
            Suits playedCardSuit = Card.GetSuit(playedCard);
            string stringSuit = Enum.GetName(typeof(Suits), playedCardSuit);
            stringSuit = stringSuit.Substring(0, 1);
            Ranks playedCardRank = Card.GetRank(playedCard);
            string stringRank = Enum.GetName(typeof(Ranks), playedCardRank);
            stringRank = SetStringRank(stringRank);

            bool isLocalPlayer = currentPlayer == localPlayer;

            currentPlayer.SendDisplayingCardToCenter(cardAnimator, playedCard, currentPlayer, isLocalPlayer);
            playerToSkip = gameDataManager.GetPlayerToSkip();

            SetPlayedCards1($"{currentPlayer.PlayerName}: " + stringRank + stringSuit);

            if (NetworkClient.Instance.IsHost)
            {
                actionCount = gameDataManager.GetActionCount();

                if (actionCount < 4 && playerToSkip.PlayerId == "")
                {
                    gameState = GameState.TurnSelectingCard;
                    SwitchToNextPlayer();
                }
                else if (actionCount < 3 && playerToSkip.PlayerId != "")
                {
                    gameState = GameState.TurnSelectingCard;
                    SwitchToNextPlayer();
                }
                else
                {
                    scoreHand();

                    gameState = GameState.UpdateDisplayStats;
                    actionCount = 0;
                }

                gameDataManager.SetTeam1Tricks(team1Tricks);
                gameDataManager.SetTeam2Tricks(team2Tricks);
                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);
                gameDataManager.SetActionCount(actionCount);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public IEnumerator OnUpdateDisplayStats()
        {
            highRank = 0;
            ledSuit = -1;

            team1Score = gameDataManager.GetTeam1Score();
            team2Score = gameDataManager.GetTeam2Score();

            team1Tricks = gameDataManager.GetTeam1Tricks();
            team2Tricks = gameDataManager.GetTeam2Tricks();

            Team1Score.text = team1Score.ToString();
            Team2Score.text = team2Score.ToString();

            Team1Tricks.text = team1Tricks.ToString();
            Team2Tricks.text = team2Tricks.ToString();

            playerToSkip = gameDataManager.GetPlayerToSkip();

            if (PlayedCards2.text != "")
            {
                ClearPlayedCards2();
            }

            handCount = gameDataManager.GetHandCount();

            if (handCount == 1)
            {
                LastTrick.gameObject.SetActive(true);
            }

            SetPlayedCards2(PlayedCards1.text);
            ClearPlayedCards1();

            highTrickPlayer = gameDataManager.GetHighTrickPlayer();

            SetMessage("Waiting");

            Debug.Log("Enter dim");

            yield return StartCoroutine(DimCards());

            if (NetworkClient.Instance.IsHost)
            {
                Debug.Log("wait");
                yield return StartCoroutine(Sleep(1.5f));

                gameState = GameState.StartNextHand;

                IncreaseHandCount();

                //if last hand wait longer
                if (handCount == 12)
                {
                    yield return StartCoroutine(Sleep(2.5f));
                }

                gameDataManager.SetHandCount(handCount);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());

                yield return StartCoroutine(CheckAllPlayersReady());
            }
        }

        public void OnGameFinished()
        {
            team1Score = gameDataManager.GetTeam1Score();
            team2Score = gameDataManager.GetTeam2Score();

            Team1Score.text = team1Score.ToString();
            Team2Score.text = team2Score.ToString();

            if (gameDataManager.GetWinnerTeam() == localPlayer.Team)
            {
                SetMessage($"You WON!");
            }
            else
            {
                SetMessage($"You LOST!");
            }
        }

        //****************** Helper Methods *********************//

        public void UnreadyPlayers()
        {
            Debug.Log("unready");
            player1ready = false;
            player2ready = false;
            player3ready = false;
            AllPlayersReady = false;
        }

        public IEnumerator CheckAllPlayersReady()
        {
            Debug.Log("CheckingPlayerStatus");
            while (player1ready == false && player2ready == false && player3ready == false)
            {
                yield return null;
            }

            AllPlayersReady = true;
        }

        public IEnumerator Sleep(float time)
        {
            yield return new WaitForSeconds(time);
        }

        public IEnumerator DimCards()
        {
            Player[] playerAry = { localPlayer, remotePlayer1, remotePlayer2, remotePlayer3 };

            foreach (Player player in playerAry)
            {
                if (player != highTrickPlayer && player != playerToSkip)
                {
                    Card playedCard = player.GetPlayedCard();
                    playedCard.DimSprite();
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public void ResetSelectedCard()
        {
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
            }
        }

        protected void SetMessage(string message)
        {
            MessageText.text = message;
        }

        protected void SetTrumpString()
        {
            trump = gameDataManager.GetTrump();

            Heart.gameObject.SetActive(false);
            Club.gameObject.SetActive(false);
            Diamond.gameObject.SetActive(false);
            Spade.gameObject.SetActive(false);
            A.gameObject.SetActive(false);

            //string trumpString = Enum.GetName(typeof(Suits), trump);
            //Trump.text = trumpString + $" is Trump";

            if (trump == 0)
            {
                Heart.gameObject.SetActive(true);
            }
            else if (trump == 1)
            {
                Club.gameObject.SetActive(true);
            }
            else if (trump == 2)
            {
                Diamond.gameObject.SetActive(true);
            }
            else if (trump == 3)
            {
                Spade.gameObject.SetActive(true);
            }
            else if (trump == -1)
            {
                A.gameObject.SetActive(true);
            }
        }

        public string SetStringRank(string rank)
        {
            if (rank == "Nine")
            {
                return "9";
            }
            else if (rank == "Ten")
            {
                return "10";
            }
            else if (rank == "Jack")
            {
                return "J";
            }
            else if (rank == "Queen")
            {
                return "Q";
            }
            else if (rank == "King")
            {
                return "K";
            }
            else if (rank == "Ace")
            {
                return "A";
            }
            else
            {
                return "";
            }
        }

        protected void SetPlayerTexts()
        {
            Player[] playerAry = { localPlayer, remotePlayer1, remotePlayer2, remotePlayer3 };

            foreach (Player player in playerAry)
            {
                if (player == localPlayer)
                {
                    player.text = "";
                }
                else
                {
                    player.text = player.PlayerName;
                }
            }

            localPlayerLabel.text = localPlayer.text;
            remotePlayer1Label.text = remotePlayer1.text;
            remotePlayer2Label.text = remotePlayer2.text;
            remotePlayer3Label.text = remotePlayer3.text;

            foreach (Player player in playerAry)
            {
                if (player.Team == 1)
                {
                    if (T1P1.text == "")
                    {
                        T1P1.text = player.PlayerName;
                    }
                    else
                    {
                        T1P2.text = player.PlayerName;
                    }
                }
                if (player.Team == 2)
                {
                    if (T2P1.text == "")
                    {
                        T2P1.text = player.PlayerName;
                    }
                    else
                    {
                        T2P2.text = player.PlayerName;
                    }
                }
            }
        }

        protected void SetDealerToken()
        {
            dealerPlayer = gameDataManager.GetDealer();

            LocalDealerToken.gameObject.SetActive(false);
            RP1DealerToken.gameObject.SetActive(false);
            RP2DealerToken.gameObject.SetActive(false);
            RP3DealerToken.gameObject.SetActive(false);

            if (dealerPlayer == localPlayer)
            {
                LocalDealerToken.gameObject.SetActive(true);
            }
            else if (dealerPlayer == remotePlayer1)
            {
                RP1DealerToken.gameObject.SetActive(true);
            }
            else if (dealerPlayer == remotePlayer2)
            {
                RP2DealerToken.gameObject.SetActive(true);
            }
            else
            {
                RP3DealerToken.gameObject.SetActive(true);
            }
        }

        public void SetBids(string toAdd)
        {
            Bids.text += "\n" + toAdd;
        }

        public void ClearBids()
        {
            Bids.text = "";
        }

        public void SetPlayedCards1(string toAdd)
        {
            PlayedCards1.text += "\n" + toAdd;
        }

        public void ClearPlayedCards1()
        {
            PlayedCards1.text = "";
        }

        public void SetPlayedCards2(string toAdd)
        {
            PlayedCards2.text = toAdd;
        }

        public void ClearPlayedCards2()
        {
            PlayedCards2.text = "";
        }

        public void SwitchToNextPlayer()
        {
            if (currentPlayer == localPlayer)
            {
                if (remotePlayer1 == playerToSkip)
                {
                    currentPlayer = remotePlayer2;
                }
                else
                {
                    currentPlayer = remotePlayer1;
                }
            }
            else if (currentPlayer == remotePlayer1)
            {
                if (remotePlayer2 == playerToSkip)
                {
                    currentPlayer = remotePlayer3;
                }
                else
                {
                    currentPlayer = remotePlayer2;
                }
            }
            else if (currentPlayer == remotePlayer2)
            {
                if (remotePlayer3 == playerToSkip)
                {
                    currentPlayer = localPlayer;
                }
                else
                {
                    currentPlayer = remotePlayer3;
                }
            }
            else if (currentPlayer == remotePlayer3)
            {
                if (localPlayer == playerToSkip)
                {
                    currentPlayer = remotePlayer1;
                }
                else
                {
                    currentPlayer = localPlayer;
                }
            }
        }

        public void SwitchToNextDealer()
        {
            if (dealerPlayer == localPlayer)
            {
                dealerPlayer = remotePlayer1;
            }
            else if (dealerPlayer == remotePlayer1)
            {
                dealerPlayer = remotePlayer2;
            }
            else if (dealerPlayer == remotePlayer2)
            {
                dealerPlayer = remotePlayer3;
            }
            else if (dealerPlayer == remotePlayer3)
            {
                dealerPlayer = localPlayer;
            }
        }

        public void ShowPlayerCards()
        {
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer1);
            remotePlayer1.SetCardValues(playerCardValues);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer2);
            remotePlayer2.SetCardValues(playerCardValues);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer3);
            remotePlayer3.SetCardValues(playerCardValues);
        }

        public bool CheckPlayerHand(Player player, int suit, int trump, int oppositeSuit)
        {
            List<byte> playerHand = gameDataManager.PlayerCards(player);

            foreach (byte card in playerHand)
            {
                if ((card / 12) == suit)
                {
                    if (suit == oppositeSuit && (card % 12 / 2 + 1) == 3)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (suit == trump && (card / 12) == oppositeSuit && (card % 12 / 2 + 1) == 3)
                {
                    return false;
                }
            }
            return true;
        }

        public void ShowAndHidePlayersDisplayingCards()
        {
            localPlayer.ShowCardValues();
            remotePlayer1.HideCardValues();
            remotePlayer2.HideCardValues();
            remotePlayer3.HideCardValues();
        }

        public int SetOppositeTrump(int trump)
        {
            if (trump == 0)
            {
                return 2;
            }
            else if (trump == 1)
            {
                return 3;
            }
            else if (trump == 2)
            {
                return 0;
            }
            else if (trump == 3)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public void SetPlayerToSkip()
        {
            if (highBidPlayer == localPlayer)
            {
                playerToSkip = remotePlayer2;
            }
            else if (highBidPlayer == remotePlayer1)
            {
                playerToSkip = remotePlayer3;
            }
            else if (highBidPlayer == remotePlayer2)
            {
                playerToSkip = localPlayer;
            }
            else if (highBidPlayer == remotePlayer3)
            {
                playerToSkip = remotePlayer1;
            }
        }

        public void SetHighValues(int rank, int suit)
        {
            ledSuit = gameDataManager.GetLedSuit();
            oppositeTrump = gameDataManager.GetOppositeTrump();

            if (suit == trump)
            {
                if (rank == 3)
                {
                    rank = rank + 17;
                }
                else
                {
                    rank = rank + 12;
                }
            }
            else if (suit == oppositeTrump && rank == 3)
            {
                rank = rank + 16;
            }
            else if (suit == ledSuit)
            {
                rank = rank + 6;
            }

            if (rank > highRank)
            {
                highRank = rank;
                highTrickPlayer = currentPlayer;
            }

            gameDataManager.SetHighTrickPlayer(highTrickPlayer);
        }

        public void scoreHand()
        {
            if (gameDataManager.GetPlayerTeam(highTrickPlayer.PlayerId) == 1)
            {
                team1Tricks++;
            }
            else
            {
                team2Tricks++;
            }
        }

        public void scoreRound()
        {
            if (gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 1)
            {
                if (team1Tricks >= highBid)
                {
                    team1Score = team1Score + team1Tricks;
                    team2Score = team2Score + team2Tricks;
                }
                else if (team1Tricks == 12)
                {
                    team1Score = team1Score + highBid;
                    team2Score = team2Score + team2Tricks;
                }
                else
                {
                    team1Score = team1Score - highBid;
                    team2Score = team2Score + team2Tricks;
                }
            }
            else if (gameDataManager.GetPlayerTeam(highBidPlayer.PlayerId) == 2)
            {
                if (team2Tricks >= highBid)
                {
                    team2Score = team2Score + team2Tricks;
                    team1Score = team1Score + team1Tricks;
                }
                else if (team2Tricks == 12)
                {
                    team2Score = team2Score + highBid;
                    team1Score = team1Score + team1Tricks;
                }
                else
                {
                    team2Score = team2Score - highBid;
                    team1Score = team1Score + team1Tricks;
                }
            }

        }

        public void ResetVariables()
        {
            highRank = 0;
            ledSuit = -1;
            highBid = 0;
            trump = 5;
            oppositeTrump = 0;
            team1Tricks = 0;
            team2Tricks = 0;
            playerToSkip = new Player();
            playerToSkip.PlayerId = "";
            localPlayer.bid = "";
            remotePlayer1.bid = "";
            remotePlayer2.bid = "";
            remotePlayer3.bid = "";

            List<byte> lp = gameDataManager.PlayerCards(localPlayer);
            List<byte> rp1 = gameDataManager.PlayerCards(remotePlayer1);
            List<byte> rp2 = gameDataManager.PlayerCards(remotePlayer2);
            List<byte> rp3 = gameDataManager.PlayerCards(remotePlayer3);

            if (lp.Count > 0)
            {
                gameDataManager.RemoveCardValuesFromPlayer(localPlayer, lp);
            }
            if (rp1.Count > 0)
            {
                gameDataManager.RemoveCardValuesFromPlayer(remotePlayer1, rp1);
            }
            if (rp2.Count > 0)
            {
                gameDataManager.RemoveCardValuesFromPlayer(remotePlayer2, rp2);
            }
            if (rp3.Count > 0)
            {
                gameDataManager.RemoveCardValuesFromPlayer(remotePlayer3, rp3);
            }

            gameDataManager.SetPlayerToSkip(playerToSkip);
            gameDataManager.SetHighBid(highBid);
            gameDataManager.SetTeam1Tricks(team1Tricks);
            gameDataManager.SetTeam2Tricks(team2Tricks);
            gameDataManager.SetTrump(trump);
            gameDataManager.SetPlayerBid(localPlayer, "");
            gameDataManager.SetPlayerBid(remotePlayer1, "");
            gameDataManager.SetPlayerBid(remotePlayer2, "");
            gameDataManager.SetPlayerBid(remotePlayer3, "");
        }

        /*        public byte GetCardByteOption1(int rank, int suit)
                {
                    int result;
                    result = suit * 12;
                    result = result + (rank * 2 - 1);
                    byte result2 = Convert.ToByte(result);
                    return result2;
                }

                public byte GetCardByteOption2(int rank, int suit)
                {
                    int result;
                    result = suit * 12;
                    result = result + (rank * 2 - 2);
                    byte result2 = Convert.ToByte(result);
                    return result2;
                }*/

        public void IncreaseActionCount()
        {
            if (actionCount < 4)
            {
                Debug.Log("Increaseaction");
                actionCount++;
            }
            else
            {
                Debug.Log("Setto1");
                actionCount = 1;
            }
        }

        public void IncreaseHandCount()
        {
            if (handCount < 12)
            {
                handCount++;
            }
            else
            {
                handCount = 1;
            }
        }

        //****************** User Interaction *********************//

        public void OnSixSelected()
        {
            netCode.NotifyHostPlayerBidSelected(6);
            HideAllButtons();
        }

        public void OnSevenSelected()
        {
            netCode.NotifyHostPlayerBidSelected(7);
            HideAllButtons();
        }

        public void OnEightSelected()
        {
            netCode.NotifyHostPlayerBidSelected(8);
            HideAllButtons();
        }

        public void OnNineSelected()
        {
            netCode.NotifyHostPlayerBidSelected(9);
            HideAllButtons();
        }

        public void OnTenSelected()
        {
            netCode.NotifyHostPlayerBidSelected(10);
            HideAllButtons();
        }

        public void OnHossSelected()
        {
            netCode.NotifyHostPlayerBidSelected(16);
            HideAllButtons();
        }

        public void OnPfefferSelected()
        {
            netCode.NotifyHostPlayerBidSelected(32);
            HideAllButtons();
        }

        public void OnPassSelected()
        {
            netCode.NotifyHostPlayerBidSelected(0);
            HideAllButtons();
        }

        public void OnHeartsSelected()
        {
            trump = (int)Suits.Hearts;
            netCode.NotifyHostPlayerTrumpSelected(trump);
            HideAllButtons();
        }

        public void OnClubsSelected()
        {
            trump = (int)Suits.Clubs;
            netCode.NotifyHostPlayerTrumpSelected(trump);
            HideAllButtons();
        }

        public void OnDiamondsSelected()
        {
            trump = (int)Suits.Diamonds;
            netCode.NotifyHostPlayerTrumpSelected(trump);
            HideAllButtons();
        }

        public void OnSpadesSelected()
        {
            trump = (int)Suits.Spades;
            netCode.NotifyHostPlayerTrumpSelected(trump);
            HideAllButtons();
        }

        public void OnNoTrumpSelected()
        {
            trump = (int)Suits.NoTrump;
            netCode.NotifyHostPlayerTrumpSelected(trump);
            HideAllButtons();
        }

        public void OnRedealSelected()
        {
            ResetVariables();
            gameState = GameState.Redeal;
            gameDataManager.SetGameState(gameState);

            netCode.ModifyGameData(gameDataManager.EncryptedData());
        }

        public void OnCardSelected(Card card)
        {
            if (alreadyplayed == false)
            {
                if (gameState == GameState.SelectCardsToGive)
                {
                    if (card == givingCard1)
                    {
                        givingCard1.OnSelected(false);
                        givingCard1 = null;
                        givingCard1Rank = 0;
                        givingCard1Suit = 0;
                    }
                    else if (card == givingCard2)
                    {
                        givingCard2.OnSelected(false);
                        givingCard2 = null;
                        givingCard2Rank = 0;
                        givingCard2Suit = 0;
                    }
                    else if (givingCard1 == null)
                    {
                        givingCard1 = card;
                        givingCard1Suit = (int)givingCard1.Suit;
                        givingCard1Rank = (int)givingCard1.Rank;
                        givingCard1.OnSelected(true);
                    }
                    else if (givingCard2 == null)
                    {
                        givingCard2 = card;
                        givingCard2Suit = (int)givingCard2.Suit;
                        givingCard2Rank = (int)givingCard2.Rank;
                        givingCard2.OnSelected(true);
                    }
                    else
                    {
                        givingCard3 = card;
                        givingCard3Suit = (int)givingCard3.Suit;
                        givingCard3Rank = (int)givingCard3.Rank;
                        givingCard3.OnSelected(true);
                        netCode.NotifyHostPlayer3CardsSelected(givingCard1.byteValue, givingCard2.byteValue, givingCard3.byteValue);
                        givingCard1 = null;
                        givingCard2 = null;
                        givingCard3 = null;
                    }
                }
                else if (gameState == GameState.SelectCardsToRemove)
                {
                    if (card == givingCard1)
                    {
                        givingCard1.OnSelected(false);
                        givingCard1 = null;
                        givingCard1Rank = 0;
                        givingCard1Suit = 0;

                    }
                    else if (card == givingCard2)
                    {
                        givingCard2.OnSelected(false);
                        givingCard2 = null;
                        givingCard2Rank = 0;
                        givingCard2Suit = 0;
                    }
                    else if (givingCard1 == null)
                    {
                        givingCard1 = card;
                        givingCard1Suit = (int)givingCard1.Suit;
                        givingCard1Rank = (int)givingCard1.Rank;
                        givingCard1.OnSelected(true);
                    }
                    else if (givingCard2 == null)
                    {
                        givingCard2 = card;
                        givingCard2Suit = (int)givingCard2.Suit;
                        givingCard2Rank = (int)givingCard2.Rank;
                        givingCard2.OnSelected(true);
                    }
                    else
                    {
                        givingCard3 = card;
                        givingCard3Suit = (int)givingCard3.Suit;
                        givingCard3Rank = (int)givingCard3.Rank;
                        givingCard3.OnSelected(true);
                        netCode.NotifyHostPlayer3CardsSelected(givingCard1.byteValue, givingCard2.byteValue, givingCard3.byteValue);
                        givingCard1 = null;
                        givingCard2 = null;
                        givingCard3 = null;
                    }
                }
                else if (gameState == GameState.TurnSelectingCard)
                {
                    actionCount = gameDataManager.GetActionCount();

                    if (card.OwnerId == currentPlayer.PlayerId)
                    {
                        if (actionCount == 4 || actionCount == 0)
                        {
                            if (selectedCard != null)
                            {
                                selectedCard.OnSelected(false);
                                selectedSuit = 0;
                                selectedRank = 0;
                            }

                            selectedCard = card;
                            selectedSuit = (int)selectedCard.Suit;
                            selectedRank = (int)selectedCard.Rank;
                            alreadyplayed = true;
                            netCode.NotifyHostPlayerCardPlayed(selectedCard.byteValue);
                            selectedCard = null;
                            SetMessage($"");
                        }
                        else
                        {
                            Debug.Log("not leading");
                            ledSuit = gameDataManager.GetLedSuit();
                            trump = gameDataManager.GetTrump();
                            oppositeTrump = gameDataManager.GetOppositeTrump();

                            bool playerHasNoLedSuit = CheckPlayerHand(currentPlayer, ledSuit, trump, oppositeTrump);

                            Card checkCard = card;

                            if (playerHasNoLedSuit)
                            {
                                Debug.Log("FS2");
                                if (selectedCard != null)
                                {

                                    selectedCard.OnSelected(false);
                                    selectedSuit = 0;
                                    selectedRank = 0;
                                }

                                selectedCard = card;
                                selectedSuit = (int)selectedCard.Suit;
                                selectedRank = (int)selectedCard.Rank;
                                alreadyplayed = true;
                                netCode.NotifyHostPlayerCardPlayed(selectedCard.byteValue);
                                selectedCard = null;
                                SetMessage($"");
                            }
                            else if ((int)checkCard.Suit == ledSuit || (ledSuit == trump && (int)checkCard.Suit == oppositeTrump && (int)checkCard.Rank == 3))
                            {
                                if (ledSuit == oppositeTrump && (int)checkCard.Suit == oppositeTrump && (int)checkCard.Rank == 3)
                                {
                                    SetMessage($"You must follow suit.");
                                }
                                else
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.OnSelected(false);
                                        selectedSuit = 0;
                                        selectedRank = 0;
                                    }

                                    selectedCard = card;
                                    selectedSuit = (int)selectedCard.Suit;
                                    selectedRank = (int)selectedCard.Rank;
                                    alreadyplayed = true;
                                    netCode.NotifyHostPlayerCardPlayed(selectedCard.byteValue);
                                    selectedCard = null;
                                    SetMessage($"");
                                }
                            }
                            else
                            {
                                if (selectedCard != null)
                                {
                                    selectedCard.OnSelected(false);
                                    selectedSuit = 0;
                                    selectedRank = 0;
                                    selectedCard = null;
                                }

                                SetMessage($"You must follow suit.");
                            }
                        }
                    }
                }
            }
        }

        void HideAllButtons()
        {
            HeartsButton.SetActive(false);
            ClubsButton.SetActive(false);
            DiamondsButton.SetActive(false);
            SpadesButton.SetActive(false);
            NoTrumpButton.SetActive(false);
            SixButton.SetActive(false);
            SevenButton.SetActive(false);
            EightButton.SetActive(false);
            NineButton.SetActive(false);
            TenButton.SetActive(false);
            HossButton.SetActive(false);
            PfefferButton.SetActive(false);
            PassButton.SetActive(false);
        }

        public void OnOkSelected()
        {
            if ((gameState == GameState.SelectCardsToGive || gameState == GameState.SelectCardsToRemove) && localPlayer == currentPlayer)
            {
                if (givingCard1 != null && givingCard2 != null && givingCard3 != null)
                {
                    netCode.NotifyHostPlayer3CardsSelected(givingCard1.byteValue, givingCard2.byteValue, givingCard3.byteValue);
                }
            }
            else if (gameState == GameState.TurnSelectingCard && localPlayer == currentPlayer)
            {
                if (selectedCard != null)
                {
                    netCode.NotifyHostPlayerCardPlayed(selectedCard.byteValue);
                    selectedCard = null;
                }
            }
        }

        //****************** Animator Event *********************//
        public void AllAnimationsFinished()
        {
            if (NetworkClient.Instance.IsHost && gameState == GameState.BidsStarted)
            {
                netCode.NotifyOtherPlayersGameStateChanged();
            }
            else if (NetworkClient.Instance.IsHost && gameState == GameState.SelectingBid)
            {
                netCode.NotifyOtherPlayersGameStateChanged();
            }
        }

        //****************** NetCode Events *********************//
        public void OnGameDataReady(EncryptedData encryptedData)
        {
            Debug.Log(encryptedData);
            if (encryptedData == null)
            {
                Debug.Log("New game");

                if (NetworkClient.Instance.IsHost)
                {
                    gameState = GameState.GameStarted;
                    gameDataManager.SetGameState(gameState);

                    netCode.ModifyGameData(gameDataManager.EncryptedData());
                }
            }
            else
            {
                gameDataManager.ApplyEncrptedData(encryptedData);
                SyncPlayers();
                ResetVariables();
                gameState = gameDataManager.GetGameState();
                currentPlayer = gameDataManager.GetCurrentPlayer();

                if (gameState > GameState.GameStarted)
                {
                    Debug.Log("Restore the game state");

                    //restore player's cards

                    localPlayer.ResetAllCards(localPlayer);
                    remotePlayer1.ResetAllCards(remotePlayer1);
                    remotePlayer2.ResetAllCards(remotePlayer2);
                    remotePlayer3.ResetAllCards(remotePlayer3);


                    StartCoroutine(DealCards());
                    Debug.Log("Going to gameflow");

                    GameFlow();
                    netCode.NotifyHostPlayerReady();
                }
            }
        }

        public IEnumerator DealCards()
        {
            Debug.Log("StartDealing");
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(localPlayer, gameDataManager.PlayerCards(localPlayer).Count, false));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer1, gameDataManager.PlayerCards(remotePlayer1).Count, false));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer2, gameDataManager.PlayerCards(remotePlayer2).Count, false));
            yield return StartCoroutine(cardAnimator.DealDisplayingCards(remotePlayer3, gameDataManager.PlayerCards(remotePlayer3).Count, false));
            Debug.Log("EndDealing");
        }

        public void OnGameDataChanged(EncryptedData encryptedData)
        {
            StartCoroutine(OnGameDataChangedCoroutine(encryptedData));
        }

        public IEnumerator OnGameDataChangedCoroutine(EncryptedData encryptedData)
        {
            gameDataManager.ApplyEncrptedData(encryptedData);
            gameState = gameDataManager.GetGameState();
            currentPlayer = gameDataManager.GetCurrentPlayer();
            if (NetworkClient.Instance.IsHost)
            {
                Debug.Log(System.DateTime.Now);
                yield return StartCoroutine(CheckAllPlayersReady());
                Debug.Log(System.DateTime.Now);
                UnreadyPlayers();
                GameFlow();
            }
            else
            {
                Debug.Log("Notify");
                GameFlow();
                netCode.NotifyHostPlayerReady();
            }
        }

        public void OnGameStateChanged()
        {
            GameFlow();
        }

        public void OnBidSelected(int bid)
        {
            if (NetworkClient.Instance.IsHost)
            {
                if (bid > highBid)
                {
                    highBid = bid;
                    highBidPlayer = currentPlayer;
                }

                string stringbid;

                actionCount = gameDataManager.GetActionCount();

                IncreaseActionCount();

                if (bid == 0)
                {
                    stringbid = "Pass";
                }
                else if (bid == 16)
                {
                    stringbid = "Hoss";
                }
                else if (bid == 32)
                {
                    stringbid = "Pfeffer";
                }
                else
                {
                    stringbid = bid.ToString();
                }

                gameState = GameState.TurnBidConfirmed;

                gameDataManager.SetHighBid(highBid);
                gameDataManager.SetHighBidPlayer(highBidPlayer);
                gameDataManager.SetSelectedBid(bid);
                gameDataManager.SetGameState(gameState);
                gameDataManager.SetPlayerBid(currentPlayer, stringbid);
                gameDataManager.SetActionCount(actionCount);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnTrumpSelected(int TrumpInt)
        {
            if (NetworkClient.Instance.IsHost)
            {
                trump = TrumpInt;
                oppositeTrump = SetOppositeTrump(trump);

                if (highBid == 16)
                {
                    gameState = GameState.SetupHoss;
                }
                else if (highBid == 32)
                {
                    gameState = GameState.SetupPfeffer;
                }
                else
                {
                    gameState = GameState.TurnSelectingCard;
                }

                gameDataManager.SetTrump(trump);
                gameDataManager.SetOppositeTrump(oppositeTrump);
                gameDataManager.SetGameState(gameState);
                gameDataManager.SortHands();

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnCardPlayed(byte value)
        {
            if (NetworkClient.Instance.IsHost)
            {
                Debug.Log("OnCardPlayed");

                int suit = Convert.ToInt32(value / 12);
                int rank = Convert.ToInt32(value % 12 / 2 + 1);

                actionCount = gameDataManager.GetActionCount();
                IncreaseActionCount();

                if (actionCount == 1)
                {
                    if (suit == oppositeTrump && rank == 3)
                    {
                        ledSuit = trump;
                    }
                    else
                    {
                        ledSuit = suit;
                    }
                    gameDataManager.SetLedSuit(ledSuit);
                }

                SetHighValues(rank, suit);

                Debug.Log("SetnewGS");
                gameState = GameState.UpdateCardsDisplayed;

                gameDataManager.RemoveCardValueFromPlayer(currentPlayer, value);

                gameDataManager.SetPlayedCard(value);
                gameDataManager.SetGameState(gameState);
                gameDataManager.SetActionCount(actionCount);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void On3CardsSelected(byte card1, byte card2, byte card3)
        {
            if (NetworkClient.Instance.IsHost)
            {
                if (gameState == GameState.SelectCardsToGive)
                {
                    gameDataManager.AddCardValueToPlayer(highBidPlayer, card1);
                    gameDataManager.AddCardValueToPlayer(highBidPlayer, card2);
                    gameDataManager.AddCardValueToPlayer(highBidPlayer, card3);

                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card1);
                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card2);
                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card3);

                    gameDataManager.SetGivenCard(1, card1);
                    gameDataManager.SetGivenCard(2, card2);
                    gameDataManager.SetGivenCard(3, card3);

                    gameState = GameState.SelectCardsToRemove;
                    currentPlayer = highBidPlayer;
                }
                else
                {
                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card1);
                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card2);
                    gameDataManager.RemoveCardValueFromPlayer(currentPlayer, card3);

                    gameDataManager.SetRemovedCard(1, card1);
                    gameDataManager.SetRemovedCard(2, card2);
                    gameDataManager.SetRemovedCard(3, card3);

                    gameState = GameState.UpdateHossDisplay;
                }

                gameDataManager.SetCurrentPlayer(currentPlayer);
                gameDataManager.SetGameState(gameState);
                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }
        }

        public void OnPlayerReady()
        { 
            if (NetworkClient.Instance.IsHost)
            {
                if (player1ready == false)
                {
                    player1ready = true;
                }
                else if (player2ready == false)
                {
                    player2ready = true;
                }
                else if (player3ready == false)
                {
                    player3ready = true;
                }

                if (player1ready && player2ready && player3ready)
                {
                    AllPlayersReady = true;
                }
            }
        }

        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            NetworkClient.Instance.DisconnectFromRoom();
            NetworkClient.Lobby.LeaveRoom(HandleLeaveRoom);
        }

        void HandleLeaveRoom(bool okay, SWLobbyError error)
        {
            if (!okay)
            {
                Debug.LogError(error);
            }

            Debug.Log("Left room");
        }
    }
}
