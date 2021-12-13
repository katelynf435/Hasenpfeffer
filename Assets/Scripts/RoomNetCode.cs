using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.Events;
using System;

namespace Hasenpfeffer
{
    [Serializable]
    public class TeamSelectedEvent : UnityEvent<int, string>
    {

    }
    public class RoomNetCode : MonoBehaviour
    {
        public TeamSelectedEvent OnTeamSelectedEvent = new TeamSelectedEvent();
        const string TEAM_SELECTED = "TeamSelected";

        RoomRemoteEventAgent roomRemoteEventAgent;

        private void Awake()
        {
            roomRemoteEventAgent = FindObjectOfType<RoomRemoteEventAgent>();
        }

        public void NotifyHostPlayerTeamPicked(int team, string pID)
        {
            SWNetworkMessage message = new SWNetworkMessage();
            message.Push(team);
            message.PushUTF8LongString(pID);
            roomRemoteEventAgent.Invoke(TEAM_SELECTED, message);
        }

        public void OnTeamPickedRemoteEvent(SWNetworkMessage message)
        {
            int team = message.PopInt32();
            string pID = message.PopUTF8LongString();
            OnTeamSelectedEvent.Invoke(team, pID);
        }
    }
}
