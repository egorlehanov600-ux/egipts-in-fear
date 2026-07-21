using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Слоты для игроков")]
    public Transform[] playerSlots;

    private bool isCreatingRoom = false;
    private bool[] playerSpawned;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerSpawned = new bool[playerSlots.Length];
        
        // Проверяем точное состояние подключения
        if (PhotonNetwork.IsConnected && PhotonNetwork.Server == ServerConnection.MasterServer)
        {
            Debug.Log("✅ Уже на Master Server. Ищем комнату...");
            PhotonNetwork.JoinRandomRoom();
        }
        else if (PhotonNetwork.IsConnected)
        {
            // Подключен, но ещё не на Master Server (NameServer или GameServer)
            Debug.Log("⏳ Подключен, но ещё не на Master Server. Ждём...");
            // Ничего не делаем, ждём OnConnectedToMaster()
        }
        else
        {
            Debug.Log("🔌 Подключение к Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Подключено к Master Server. Ищем комнату...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("❌ Комната не найдена, создаем новую...");
        CreateRoom();
    }

    void CreateRoom()
    {
        isCreatingRoom = true;
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 5,
            IsVisible = true,
            IsOpen = true
        };

        string roomName = "Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log("✅ Создана комната: " + roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(" Комната создана!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("👤 Игрок в комнате! Всего: " + PhotonNetwork.CurrentRoom.PlayerCount + "/5");
        UpdateRoomUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("👥 Новый игрок: " + newPlayer.NickName);
        UpdateRoomUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomUI();
    }

    void UpdateRoomUI()
    {
        int slotIndex = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (slotIndex < playerSlots.Length)
            {
                if (!playerSpawned[slotIndex])
                {
                    SpawnPlayerInSlot(slotIndex, player);
                    playerSpawned[slotIndex] = true;
                }
                slotIndex++;
            }
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 5)
        {
            Debug.Log("🏁 Все игроки собрались! Запуск игры...");
            Invoke(nameof(StartGame), 2f);
        }
    }

    void SpawnPlayerInSlot(int slotIndex, Player player)
    {
        if (playerSlots.Length > slotIndex)
        {
            Vector3 spawnPos = playerSlots[slotIndex].position;
            GameObject newPlayer = PhotonNetwork.Instantiate("PlayerCapsule", spawnPos, Quaternion.identity);
            
            Debug.Log($"📍 Игрок {player.NickName} заспавнился в слоте {slotIndex + 1}");
        }
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}