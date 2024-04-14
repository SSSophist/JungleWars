using UnityEngine;
using Mirror;

//用于存放玩家的信息
/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// <para>The RoomPrefab object of the NetworkRoomManager must have this component on it. This component holds basic room player data required for the room to function. Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.</para>
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Room Player")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-player")]
public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("Diagnostics")]
    /// <summary>
    /// Diagnostic flag indicating whether this player is ready for the game to begin.
    /// <para>Invoke CmdChangeReadyState method on the client to set this flag.</para>
    /// <para>When all players are ready to begin, the game will start. This should not be set directly, CmdChangeReadyState should be called on the client to set it on the server.</para>
    /// </summary>
    [ReadOnly, Tooltip("Diagnostic flag indicating whether this player is ready for the game to begin")]
    [SyncVar(hook = nameof(ReadyStateChanged))]
    public bool readyToBegin;

    /// <summary>
    /// Diagnostic index of the player, e.g. Player1, Player2, etc.
    /// </summary>
    [ReadOnly, Tooltip("Diagnostic index of the player, e.g. Player1, Player2, etc.")]
    [SyncVar(hook = nameof(IndexChanged))]
    public int index;

    #region Unity Callbacks

    /// <summary>
    /// Do not use Start - Override OnStartHost / OnStartClient instead!
    /// </summary>
    public virtual void Start()
    {
        if (NetworkManager.singleton is NetworkRoomManager room)
        {
            // NetworkRoomPlayer object must be set to DontDestroyOnLoad along with NetworkRoomManager
            // in server and all clients, otherwise it will be respawned in the game scene which would
            // have undesirable effects.
            if (room.dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            room.roomSlots.Add(this);

            if (NetworkServer.active)
                room.RecalculateRoomPlayerIndices();

            if (NetworkClient.active)
                room.CallOnClientEnterRoom();
        }
        else Debug.LogError("RoomPlayer could not find a NetworkRoomManager. The RoomPlayer requires a NetworkRoomManager object to function. Make sure that there is one in the scene.");
    }

    public virtual void OnDisable()
    {
        
        if (NetworkClient.active && NetworkManager.singleton is NetworkRoomManager room)
        {
            // only need to call this on client as server removes it before object is destroyed
            room.roomSlots.Remove(this);

            room.CallOnClientExitRoom();
        }
    }

    #endregion

    #region Commands

    [Command]
    public void CmdChangeReadyState(bool readyState)
    {
        readyToBegin = readyState;
        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        room?.ReadyStatusChanged();
    }
    [Command]
    public void CmdOnStartGame()
    {
        RpcOnStartGame();
    }
    [Server]
    public void RpcOnStartGame()
    {
        RoomManager.st.OnStartGame();
    }
    #endregion

    #region SyncVar Hooks

    /// <summary>
    /// This is a hook that is invoked on clients when the index changes.
    /// </summary>
    /// <param name="oldIndex">The old index value</param>
    /// <param name="newIndex">The new index value</param>
    public virtual void IndexChanged(int oldIndex, int newIndex) { }

    /// <summary>
    /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
    /// <para>This function is called when the a client player calls CmdChangeReadyState.</para>
    /// </summary>
    /// <param name="newReadyState">New Ready State</param>
    public virtual void ReadyStateChanged(bool oldReadyState, bool newReadyState) { }

    #endregion

    #region Room Client Virtuals

    /// <summary>
    /// This is a hook that is invoked on clients for all room player objects when entering the room.
    /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
    /// </summary>
    public virtual void OnClientEnterRoom() { }

    /// <summary>
    /// This is a hook that is invoked on clients for all room player objects when exiting the room.
    /// </summary>
    public virtual void OnClientExitRoom() { }

    #endregion

    #region Optional UI

    public override void OnStartClient()
    {
        base.OnStartClient();
        //RoomManager.st.curRoomPlayerInfo
        //RoomManager.st.UpdateRoom();
    }
    /// <summary>
    /// Render a UI for the room. Override to provide your own UI
    /// </summary>
    public virtual void Update()
    {

        /*
        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (room)
        {
            if (!Utils.IsSceneActive(room.RoomScene))
                return;

            DrawPlayerReadyState();
            DrawPlayerReadyButton();
        }*/
    }

    void DrawPlayerReadyState()
    {
        GUILayout.BeginArea(new Rect(20f + (index * 100), 200f, 90f, 130f));

        GUILayout.Label($"Player [{index + 1}]");

        if (readyToBegin)
            GUILayout.Label("Ready");
        else
            GUILayout.Label("Not Ready");

        if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
        {
            // This button only shows on the Host for all players other than the Host
            // Host and Players can't remove themselves (stop the client instead)
            // Host can kick a Player this way.
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }

        GUILayout.EndArea();
    }

    void DrawPlayerReadyButton()
    {
        if (NetworkClient.active && isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));

            if (readyToBegin)
            {
                if (GUILayout.Button("Cancel"))
                    CmdChangeReadyState(false);
            }
            else
            {
                if (GUILayout.Button("Ready"))
                    CmdChangeReadyState(true);
            }

            GUILayout.EndArea();
        }
    }

    #endregion
}