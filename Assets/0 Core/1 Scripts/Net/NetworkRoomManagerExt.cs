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

        Debug.Log("�������׼��������չʾ��ʼ��ť");
        if (roomSlots[0] != null)
        {
            //���߷������Կ�ʼ��Ϸ
            roomSlots[0].TargetRpcShowStartGameButton();
        }
        /*
        //��⵱ǰ�Ƿ����޽��棨headless����ר�÷�����ģʽ��
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
        Debug.Log("���������׼�������÷����Ŀ�ʼ��ť");
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