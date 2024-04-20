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
        Debug.Log("OnRoomServerSceneChanged");
        if(sceneName == RoomScene)
        {
            StopServer();
            StartServer();
        }
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
    public override void OnRoomServerPlayersReady()
    {

        Debug.Log("所有玩家准备，房主展示开始按钮");
        if (roomSlots[0] != null)
        {
            //告诉房主可以开始游戏
            roomSlots[0].TargetRpcShowStartGameButton();
        }
        /*
        //检测当前是否处于无界面（headless）或专用服务器模式下
        if (Utils.IsHeadless())
        {
            Debug.Log("calling the base method calls ServerChangeScene as soon as all players are in Ready state.");
            base.OnRoomServerPlayersReady();
        }
        else
        {
            
        }*/
    }

    public override void OnRoomServerPlayersNotReady()
    {
        Debug.Log("非所有玩家准备，禁用房主的开始按钮");
        if (roomSlots.Count > 0)
        {
            roomSlots[0]?.TargetRpcHideStartGameButton();
        }
        /*
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Utils.IsHeadless())
        {
            Debug.Log("calling the base method calls ServerChangeScene as soon as all players are in Ready state.");
            base.OnRoomServerPlayersReady();
        }
        else
        {
           
        }*/
    }
}