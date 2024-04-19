using UnityEngine;
using Mirror;
/*
Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

[AddComponentMenu("")]
public class NetworkRoomManagerExt : NetworkRoomManager
{
    [Header("Spawner Setup")]
    [Tooltip("Reward Prefab for the Spawner")]
    public GameObject rewardPrefab;

    public static new NetworkRoomManagerExt singleton => NetworkManager.singleton as NetworkRoomManagerExt;

    /// <summary>
    /// This is called on the server when a networked scene finishes loading.
    /// </summary>
    /// <param name="sceneName">Name of the new scene.</param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        // spawn the initial batch of Rewards
        //if (sceneName == GameplayScene)
            //Spawner.InitialSpawn();
    }

    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        Debug.Log("OnRoomServerSceneLoadedForPlayer");
        //PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
        //playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
        return true;
    }

    public override void OnRoomStopClient()
    {
        base.OnRoomStopClient();
    }

    public override void OnRoomStopServer()
    {
        base.OnRoomStopServer();
    }

    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
        is set as DontDestroyOnLoad = true.
    */

    public bool showStartButton;

    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Utils.IsHeadless())
        {
            Debug.Log("calling the base method calls ServerChangeScene as soon as all players are in Ready state.");
            base.OnRoomServerPlayersReady();
        }
        else
        {
            showStartButton = true;
            Debug.Log("展示开始按钮");
            if(roomSlots[0] !=null)
            {
                roomSlots[0].TargetRpcShowStartGameButton();
            }
        }
    }

    public override void OnRoomServerPlayersNotReady()
    {

        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Utils.IsHeadless())
        {
            Debug.Log("calling the base method calls ServerChangeScene as soon as all players are in Ready state.");
            base.OnRoomServerPlayersReady();
        }
        else
        {
            Debug.Log("禁用开始按钮");
            if (roomSlots[0] != null)
            {
                roomSlots[0].TargetRpcHideStartGameButton();
            }
        }
    }
}