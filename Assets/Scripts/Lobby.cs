using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using SWNetwork;
using Unity.Networking.Transport;

namespace Hasenpfeffer
{
	[Serializable]
	public class RoomCustomData
	{
		public TeamCustomData team1;
		public TeamCustomData team2;
	}

	[Serializable]
	public class TeamCustomData
	{
		public List<string> players = new List<string>();
	}

	public class Lobby : MonoBehaviour
	{
		[SerializeField] private GameObject waitingStatusPanel = null;
		[SerializeField] private GameObject RegionPanel = null;
		[SerializeField] private GameObject TeamSelectPanel = null;
		[SerializeField] private GameObject EnterNicknamePanel = null;

		[SerializeField] private Button SelectTeamOne = null;
		[SerializeField] private Button SelectTeamTwo = null;
		[SerializeField] private Button ConnectButton = null;
		[SerializeField] private Button HostButton = null;
		[SerializeField] private Button StartButton = null;

		[SerializeField] private InputField NicknameInputField = null;
		[SerializeField] private Text TeamOneFirstTS = null;
		[SerializeField] private Text TeamOneSecondTS = null;
		[SerializeField] private Text TeamTwoFirstTS = null;
		[SerializeField] private Text TeamTwoSecondTS = null;
		[SerializeField] private Text TeamOneFirstWS = null;
		[SerializeField] private Text TeamOneSecondWS = null;
		[SerializeField] private Text TeamTwoFirstWS = null;
		[SerializeField] private Text TeamTwoSecondWS = null;

		[SerializeField] private Image TeamOneFirstWSI = null;
		[SerializeField] private Image TeamOneSecondWSI = null;
		[SerializeField] private Image TeamTwoFirstWSI = null;
		[SerializeField] private Image TeamTwoSecondWSI = null;

		public Server server;
		public Client client;

		public Dropdown gameRegionDrowDown;

		public string LobbyName = "EURO";

		public SpriteAtlas Atlas;

		Dictionary<string, string> playersDict_;

		RoomCustomData roomData_;

		string playerName_;
		int playerId = -1;
		int myPlayerId;

		TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

		// Start is called before the first frame update
		void Awake()
		{
			//NetworkClient.Lobby.OnLobbyConnectedEvent += OnLobbyConnected;
			//NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += OnNewPlayerJoinRoomEvent;
			//NetworkClient.Lobby.OnPlayerLeaveRoomEvent += OnPlayerLeaveRoomEvent;
			//NetworkClient.Lobby.OnRoomReadyEvent += OnRoomReadyEvent;
			//NetworkClient.Lobby.OnRoomCustomDataChangeEvent += OnRoomCustomDataChangeEvent;
			//NetworkClient.Lobby.OnRoomMessageEvent += OnRoomMessageEvent;

			RegisterEvents();

			NicknameInputField.Select();
			NicknameInputField.ActivateInputField();

			roomData_ = new RoomCustomData();
			roomData_.team1 = new TeamCustomData();
			roomData_.team2 = new TeamCustomData();
		}

		void onDestroy()
		{
			NetworkClient.Lobby.OnLobbyConnectedEvent -= OnLobbyConnected;
			NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= OnNewPlayerJoinRoomEvent;
			NetworkClient.Lobby.OnPlayerLeaveRoomEvent -= OnPlayerLeaveRoomEvent;
			NetworkClient.Lobby.OnRoomReadyEvent -= OnRoomReadyEvent;
			NetworkClient.Lobby.OnRoomCustomDataChangeEvent -= OnRoomCustomDataChangeEvent;
			NetworkClient.Lobby.OnRoomMessageEvent -= OnRoomMessageEvent;

			server.Shutdown();
			client.Shutdown();
		}

		private void Update()
		{
			if (NicknameInputField.text != "")
			{
				ConnectButton.interactable = true;
				HostButton.interactable = true;

				if (Input.GetKeyDown(KeyCode.Return))
				{
					ConnectButton.onClick.Invoke();
				}
			}

			//if (NetworkClient.Lobby.IsOwner)
			//{
			//	if (roomData_.team1.players.Count == 2 && roomData_.team2.players.Count == 2)
			//	{
			//		StartButton.interactable = true;
			//	}
			//	else
			//	{
			//		StartButton.interactable = false;
			//	}
			//}
		}

		private void RegisterEvents()
		{
			NetUtility.S_WELCOME += OnWelcomeServer;
			NetUtility.S_SEND_NAME += OnSendNameServer;

			NetUtility.C_WELCOME += OnWelcomeClient;
			NetUtility.C_SEND_NAME += OnSendNameClient;
		}
		private void UnregisterEvents()
		{
			NetUtility.S_WELCOME -= OnWelcomeServer;
			NetUtility.S_SEND_NAME -= OnSendNameServer;
		}
		private void OnSendNameServer(NetMessage msg, NetworkConnection cnn)
		{
			NetSendName sn = msg as NetSendName;

			Server.Instance.SendToClient(cnn, sn);
		}

		private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
		{
			NetWelcome nw = msg as NetWelcome;

			Debug.Log("Player joined room");

			nw.PlayerId = ++playerId;
			
			Server.Instance.SendToClient(cnn, nw);
			Debug.Log("Player id is " + playerId);
		}

		//Client
		private void OnWelcomeClient(NetMessage msg)
		{
			NetWelcome nw = msg as NetWelcome;

			myPlayerId = nw.PlayerId;

			Debug.Log($"My playerid is {nw.PlayerId}");
		}
		private void OnSendNameClient(NetMessage msg)
		{
			Debug.Log($"In name");
		}

		//****************** Matchmaking *********************//
		void Checkin()
		{
			Debug.Log("Checkin");
			NetworkClient.Instance.CheckIn(playerName_, (bool successful, string error) =>
			{
				if (!successful)
				{
					Debug.LogError(error);
					EnterNicknamePanel.SetActive(true);
				}

				UpdateGameRegionDropdownOptions();
			});
		}

		public void JoinOrCreateRoom()
		{
			NetworkClient.Lobby.JoinOrCreateRoom(false, 4, 1, (successful, reply, error) =>
			{
				if (successful)
				{
					Debug.Log("Joined or created room " + reply);

					if (NetworkClient.Lobby.IsOwner)
					{
						NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool success, SWLobbyError err) =>
						{
							if (success)
							{
								Debug.Log("ChangeRoomCustomData successful");
								UpdateTeamSelectPanel();
								UpdateWaitingPanel();
							}
							else
							{
								Debug.Log("ChangeRoomCustomData failed: " + err);
							}
						});
					}

					RefreshPlayerList();
					TeamSelectPanel.SetActive(true);
				}
				else
				{
					Debug.Log("Failed to join or create room " + error);
					RegionPanel.SetActive(true);
				}
			});
		}

		public void GameRegionChanged(int value)
		{
			NodeRegion nodeRegion = NetworkClient.Instance.AvailableNodeRegions[value];
			NetworkClient.Instance.NodeRegion = nodeRegion.name;

			OnLobbyConnected();
		}

		void UpdateGameRegionDropdownOptions()
		{
			if (NetworkClient.Instance == null)
			{
				return;
			}

			gameRegionDrowDown.ClearOptions();

			int currentValue = 0;

			for (int i = 0; i < NetworkClient.Instance.AvailableNodeRegions.Length; i++)
			{
				NodeRegion nodeRegion1 = NetworkClient.Instance.AvailableNodeRegions[i];

				if (nodeRegion1.name.Equals(NetworkClient.Instance.NodeRegion))
				{
					currentValue = i;
				}

				gameRegionDrowDown.options.Add(new Dropdown.OptionData(nodeRegion1.description));
			}

			gameRegionDrowDown.value = currentValue;

			NodeRegion nodeRegion = NetworkClient.Instance.AvailableNodeRegions[currentValue];
			NetworkClient.Instance.NodeRegion = nodeRegion.name;

			gameRegionDrowDown.onValueChanged.AddListener(GameRegionChanged);
		}

		public void GetPlayersInRoom()
		{
			NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
			{
				if (successful)
				{
					Debug.Log("Got players " + reply);

					playersDict_ = new Dictionary<string, string>();
					foreach (SWPlayer player in reply.players)
					{
						playersDict_[player.id] = player.GetCustomDataString();
					}

					GetRoomCustomData();
				}
				else
				{
					Debug.Log("Failed to get players " + error);
				}
			});
		}

		public void GetRoomCustomData()
		{
			NetworkClient.Lobby.GetRoomCustomData((successful, reply, error) =>
			{
				if (successful)
				{
					Debug.Log("Got room custom data " + reply);

					roomData_ = reply.GetCustomData<RoomCustomData>();
					UpdateTeamSelectPanel();
					UpdateWaitingPanel();
				}
				else
				{
					Debug.Log("Failed to get room data " + error);
				}
			});
		}

		public void RefreshPlayerList()
		{
			GetPlayersInRoom();
		}

		void LeaveRoom()
		{
			NetworkClient.Lobby.LeaveRoom((successful, error) => {
				if (successful)
				{
					Debug.Log("Left room");
				}
				else
				{
					Debug.Log("Failed to leave room " + error);
				}
			});
		}

		void StartRoom()
		{
			NetworkClient.Lobby.StartRoom((successful, error) => {
				if (successful)
				{
					Debug.Log("Started room.");
				}
				else
				{
					Debug.Log("Failed to start room " + error);
				}
			});
		}

		void ConnectToRoom()
		{
			// connect to the game server of the room.
			NetworkClient.Instance.ConnectToRoom((connected) =>
			{
				if (connected)
				{
					SceneManager.LoadScene("MultiplayerGameScene");
				}
				else
				{
					Debug.Log("Failed to connect to the game server.");
				}
			});
		}

		//****************** Lobby events *********************//
		void OnLobbyConnected()
		{
			Debug.Log("Lobby_OnLobbyConncetedEvent");
			NetworkClient.Lobby.Register(playerName_, (successful, reply, error) =>
			{
				if (successful)
				{
					Debug.Log("Lobby registered " + reply);

					if (reply.started)
					{
						ConnectToRoom();
					}
					else
					{
						RegionPanel.SetActive(true);
					}
				}
				else
				{
					Debug.Log("Lobby failed to register " + error);
					EnterNicknamePanel.SetActive(true);
				}
			});
		}

		void OnNewPlayerJoinRoomEvent(SWJoinRoomEventData eventData)
		{
			Debug.Log("Player joined room");
			Debug.Log(eventData);

			// Store the new playerId and player name pair
			playersDict_[eventData.newPlayerId] = eventData.GetString();

			// Update the room custom data
			NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
			{
				if (successful)
				{
					Debug.Log("ChangeRoomCustomData successful");
				}
				else
				{
					Debug.Log("ChangeRoomCustomData failed: " + error);
				}
			});
		}

		void OnPlayerLeaveRoomEvent(SWLeaveRoomEventData eventData)
		{
			Debug.Log("Player left room " + eventData);

			if (NetworkClient.Lobby.IsOwner)
			{
				roomData_.team1.players.RemoveAll(eventData.leavePlayerIds.Contains);
				roomData_.team2.players.RemoveAll(eventData.leavePlayerIds.Contains);

				NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
				{
					if (successful)
					{
						Debug.Log("ChangeRoomCustomData successful");
						UpdateTeamSelectPanel();
						UpdateWaitingPanel();
					}
					else
					{
						Debug.Log("ChangeRoomCustomData failed: " + error);
					}
				});
			}
		}

		void OnRoomReadyEvent(SWRoomReadyEventData eventData)
		{
			ConnectToRoom();
		}

		void OnRoomCustomDataChangeEvent(SWRoomCustomDataChangeEventData eventData)
		{
			Debug.Log("Room custom data changed: " + eventData);

			SWRoom room = NetworkClient.Lobby.RoomData;
			roomData_ = room.GetCustomData<RoomCustomData>();

			// Room custom data changed, refresh the player list.
			RefreshPlayerList();
		}

		//****************** UI event handlers *********************//

		/// <summary>
		/// Cancel button in the popover was clicked.
		/// </summary>
		public void OnCancelClicked()
		{
			Debug.Log("OnCancelClicked");

			LeaveRoom();
		}

		/// <summary>
		/// Start button in the WaitForOpponentPopover was clicked.
		/// </summary>
		public void OnStartRoomClicked()
		{
			Debug.Log("OnStartRoomClicked");
			// players are ready to player now.
			StartRoom();
		}

		/// <summary>
		/// Ok button in the EnterNicknamePopover was clicked.
		/// </summary>
		public void OnConnectClicked()
		{
			client.Init("127.0.0.1", 8007);
			playerName_ = NicknameInputField.text;
			playerName_ = myTI.ToTitleCase(playerName_);
			//NetSendName sn = new NetSendName();
			//sn.name = playerName_;
			//Debug.Log("send name " + sn.name + " " + sn);
			//Client.Instance.SendToServer(sn);
			//Checkin();
			EnterNicknamePanel.SetActive(false);
		}

		public void OnHostClicked()
		{
			server.Init(8007);
			client.Init("127.0.0.1", 8007);
			playerName_ = NicknameInputField.text;
			playerName_ = myTI.ToTitleCase(playerName_);
			//NetSendName sn = new NetSendName();
			//sn.name = playerName_;
			//Debug.Log("send name " + sn.name + " " + sn);
			//Client.Instance.SendToServer(sn);
			//Checkin();
			EnterNicknamePanel.SetActive(false);
		}

		public void UpdateWaitingPanel()
		{
			TeamOneFirstWS.text = "";
			TeamOneFirstWSI.sprite = Atlas.GetSprite("Black");
			TeamOneSecondWS.text = "";
			TeamOneSecondWSI.sprite = Atlas.GetSprite("Black");
			TeamTwoFirstWS.text = "";
			TeamTwoFirstWSI.sprite = Atlas.GetSprite("Black");
			TeamTwoSecondWS.text = "";
			TeamTwoSecondWSI.sprite = Atlas.GetSprite("Black");

			foreach (string pID in roomData_.team1.players)
			{
				Debug.Log("team data" + roomData_.team1);
				string playerName = playersDict_[pID];

				if (TeamOneFirstWS.text == "")
				{
					TeamOneFirstWS.text = playerName;
					TeamOneFirstWSI.sprite = Atlas.GetSprite(GetPlayerSprite(playerName));
				}
				else
				{
					TeamOneSecondWS.text = playerName;
					TeamOneSecondWSI.sprite = Atlas.GetSprite(GetPlayerSprite(playerName));
				}
			}
			foreach (string pID in roomData_.team2.players)
			{
				string playerName = playersDict_[pID];

				if (TeamTwoFirstWS.text == "")
				{
					TeamTwoFirstWS.text = playerName;
					TeamTwoFirstWSI.sprite = Atlas.GetSprite(GetPlayerSprite(playerName));
				}
				else
				{
					TeamTwoSecondWS.text = playerName;
					TeamTwoSecondWSI.sprite = Atlas.GetSprite(GetPlayerSprite(playerName));
				}
			}
		}

		public void UpdateTeamSelectPanel()
		{
			Debug.Log("TS");
			TeamOneFirstTS.text = "Open";
			TeamOneSecondTS.text = "Open";
			TeamTwoFirstTS.text = "Open";
			TeamTwoSecondTS.text = "Open";

			foreach (string pID in roomData_.team1.players)
			{
				string playerName = playersDict_[pID];

				if (TeamOneFirstTS.text == "Open")
				{
					TeamOneFirstTS.text = playerName;
				}
				else
				{
					TeamOneSecondTS.text = playerName;
				}
			}

			if (roomData_.team1.players.Count == 2)
			{
				SelectTeamOne.interactable = false;
			}
			else
			{
				SelectTeamOne.interactable = true;
			}

			foreach (string pID in roomData_.team2.players)
			{
				string playerName = playersDict_[pID];

				if (TeamTwoFirstTS.text == "Open")
				{
					TeamTwoFirstTS.text = playerName;
				}
				else
				{
					TeamTwoSecondTS.text = playerName;
				}
			}

			if (roomData_.team2.players.Count == 2)
			{
				SelectTeamTwo.interactable = false;
			}
			else
			{
				SelectTeamTwo.interactable = true;
			}
		}

		public string GetPlayerSprite(string playerName)
		{
			playerName = playerName.ToLower();

			if (playerName.Contains("mom") || playerName.Contains("sandy"))
			{
				return "Mom";
			}
			else if (playerName.Contains("dad") || playerName.Contains("russ"))
			{
				return "Dad";
			}
			else if (playerName.Contains("trev"))
			{
				return "Trev";
			}
			else if (playerName.Contains("kate"))
			{
				return "Kate";
			}
			else
			{
				return "player";
			}
		}

		public void OnTeamOneSelected()
		{
			if (NetworkClient.Lobby.IsOwner)
			{
				roomData_.team1.players.Add(NetworkClient.Lobby.PlayerId);

				NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
				{
					if (successful)
					{
						Debug.Log("ChangeRoomCustomData successful");
						UpdateTeamSelectPanel();
						UpdateWaitingPanel();
					}
					else
					{
						Debug.Log("ChangeRoomCustomData failed: " + error);
					}
				});
			}

			NetworkClient.Lobby.MessageRoom("1", (bool successful, SWLobbyError error) =>
			{
				if (successful)
				{
					Debug.Log("Sent room message");
				}
				else
				{
					Debug.Log("Failed to send room message " + error);
				}
			});

			waitingStatusPanel.SetActive(true);
			TeamSelectPanel.SetActive(false);
		}

		public void OnTeamTwoSelected()
		{
			if (NetworkClient.Lobby.IsOwner)
			{
				roomData_.team2.players.Add(NetworkClient.Lobby.PlayerId);

				NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
				{
					if (successful)
					{
						Debug.Log("ChangeRoomCustomData successful");
						UpdateTeamSelectPanel();
						UpdateWaitingPanel();
					}
					else
					{
						Debug.Log("ChangeRoomCustomData failed: " + error);
					}
				});
			}

			NetworkClient.Lobby.MessageRoom("2", (bool successful, SWLobbyError error) =>
			{
				if (successful)
				{
					Debug.Log("Sent room message");
				}
				else
				{
					Debug.Log("Failed to send room message " + error);
				}
			});

			waitingStatusPanel.SetActive(true);
			TeamSelectPanel.SetActive(false);
		}

		public void OnRoomMessageEvent(SWMessageRoomEventData eventData)
		{
			if (NetworkClient.Lobby.IsOwner)
			{
				if (eventData.data == "1")
				{
					roomData_.team1.players.Add(eventData.playerId);
				}
				else if (eventData.data =="2")
				{
					roomData_.team2.players.Add(eventData.playerId);
				}
				else if (eventData.data == "left")
				{
					roomData_.team1.players.Remove(eventData.playerId);
					roomData_.team2.players.Remove(eventData.playerId);
				}

				NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
				{
					if (successful)
					{
						Debug.Log("ChangeRoomCustomData successful");
						UpdateTeamSelectPanel();
						UpdateWaitingPanel();
					}
					else
					{
						Debug.Log("ChangeRoomCustomData failed: " + error);
					}
				});
			}
		}

		public void OnCancelWaiting()
		{
			if (NetworkClient.Lobby.IsOwner)
			{
				roomData_.team1.players.Remove(NetworkClient.Lobby.PlayerId);
				roomData_.team2.players.Remove(NetworkClient.Lobby.PlayerId);

				NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
				{
					if (successful)
					{
						Debug.Log("ChangeRoomCustomData successful");
						UpdateTeamSelectPanel();
						UpdateWaitingPanel();
					}
					else
					{
						Debug.Log("ChangeRoomCustomData failed: " + error);
					}
				});
			}

			NetworkClient.Lobby.MessageRoom("left", (bool successful, SWLobbyError error) =>
			{
				if (successful)
				{
					Debug.Log("Sent room message");
				}
				else
				{
					Debug.Log("Failed to send room message " + error);
				}
			});
			
			TeamSelectPanel.SetActive(true);
			waitingStatusPanel.SetActive(false);
		}

		public void OnCancelTeamSelect()
		{
			NetworkClient.Lobby.LeaveRoom((successful, error) =>
			{
				if (successful)
				{
					Debug.Log("Left room");
					RegionPanel.SetActive(true);
					TeamSelectPanel.SetActive(false);
				}
				else
				{
					Debug.Log("Failed to leave room " + error);
				}
			});
		}

		static bool WantsToQuit()
		{
			NetworkClient.Lobby.LeaveRoom((successful, error) =>
			{
				if (successful)
				{
					Debug.Log("Left room");
				}
				else
				{
					Debug.Log("Failed to leave room " + error);
				}
			});

			Debug.Log("Quitting.");
			return true;
		}

		[RuntimeInitializeOnLoadMethod]
		static void RunOnStart()
		{
			Application.wantsToQuit += WantsToQuit;
		}
	}
}
