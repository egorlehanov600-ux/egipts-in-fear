using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Слоты для игроков")]
    public Transform[] playerSlots; // 5 слотов (пустых объектов)

    private bool isCreatingRoom = false;
    private bool[] playerSpawned; // Защита от двойного спавна

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // Инициализируем массив, чтобы знать, кого мы уже заспавнили
        playerSpawned = new bool[playerSlots.Length];
        
        Debug.Log("🔍 Поиск комнаты...");
        PhotonNetwork.JoinRandomRoom();
    }

    // Если не удалось найти комнату - создаем свою
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
        // Если игрок вышел, можно сбросить его слот (пока просто обновляем)
        UpdateRoomUI();
    }

    void UpdateRoomUI()
    {
        int slotIndex = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (slotIndex < playerSlots.Length)
            {
                // Спавним только если в этом слоте еще никого нет
                if (!playerSpawned[slotIndex])
                {
                    SpawnPlayerInSlot(slotIndex, player);
                    playerSpawned[slotIndex] = true;
                }
                slotIndex++;
            }
        }

        // Если набралось 5 игроков - начинаем игру
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
            
            // ✅ Спавним капсулу по сети (имя префаба должно быть строго "PlayerCapsule")
            GameObject newPlayer = PhotonNetwork.Instantiate("PlayerCapsule", spawnPos, Quaternion.identity);
            
            // ✅ Красим её в зависимости от номера слота
            PlayerColor colorScript = newPlayer.GetComponent<PlayerColor>();
            if (colorScript != null)
            {
                colorScript.SetColorBySlot(slotIndex);
            }
            
            Debug.Log($"📍 Игрок {player.NickName} заспавнился в слоте {slotIndex + 1}");
        }
    }

    void StartGame()
    {
        // Переход на сцену игры (имя должно точно совпадать с тем, что в Build Settings)
        PhotonNetwork.LoadLevel("GameScene");
    }
}