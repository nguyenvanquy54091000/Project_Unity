using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine.SceneManagement;

public class CardDistribution : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] AceCards;
    public GameObject[] KingCards;
    public GameObject[] QueenCards;
    public GameObject[] JokerCards;

    [Header("Select Player Button")]
    public Button SelectPlayer1;
    public Button SelectPlayer2;
    public Button SelectPlayer3;
    public Button SelectPlayer4;

    [Header("Hand Player Objects")]
    public Transform HandPlayer1;
    public Transform HandPlayer2;
    public Transform HandPlayer3;
    public Transform HandPlayer4;

    [Header("Special Objects")]
    public Transform Assign;
    public Transform Center;

    [Header("Game Controls")]
    public Button StartButton;

    [Header("Buttons")]
    public Button Throw1;
    public Button Throw2;
    public Button Throw3;
    public Button Throw4;
    public Button Lair1;
    public Button Lair2;
    public Button Lair3;
    public Button Lair4;

    public Text Gun1;
    public Text Gun2;
    public Text Gun3;
    public Text Gun4;

    private List<GameObject> player1Hand = new List<GameObject>();
    private List<GameObject> player2Hand = new List<GameObject>();
    private List<GameObject> player3Hand = new List<GameObject>();
    private List<GameObject> player4Hand = new List<GameObject>();

    List<int> listPlayer1 = new List<int> { 0, 0, 0, 0, 0, 1 };
    List<int> listPlayer2 = new List<int> { 0, 0, 0, 0, 0, 1 };
    List<int> listPlayer3 = new List<int> { 0, 0, 0, 0, 0, 1 };
    List<int> listPlayer4 = new List<int> { 0, 0, 0, 0, 0, 1 };

    private Dictionary<Transform, (Button, Button, Text)> playerButtons;
    private Dictionary<int, List<GameObject>> playerHands = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, List<int>> playerLists = new Dictionary<int, List<int>>();

    private List<Transform> handPlayers = new List<Transform>();
    private int playerCount = 0;
    private bool[] playerSelected = new bool[4];
    private List<int> activePlayers = new List<int>();
    private int currentPlayer = 0; // Người chơi hiện tại (0: Player1, 1: Player2,...)
    private List<GameObject> selectedCards = new List<GameObject>();
    private Dictionary<Text, List<int>> gunToPlayerList;
    private Dictionary<int, List<GameObject>> playerSelectedCards = new Dictionary<int, List<GameObject>>();

    private bool[] playerEliminated = new bool[4];
    private List<int> eliminationOrder = new List<int>(); // Track the order of elimination

    void Start()
    {
        handPlayers.Add(HandPlayer1);
        handPlayers.Add(HandPlayer2);
        handPlayers.Add(HandPlayer3);
        handPlayers.Add(HandPlayer4);

        // Gắn sự kiện cho các nút SelectPlayer
        SelectPlayer1.onClick.AddListener(() => OnSelectPlayer(0));
        SelectPlayer2.onClick.AddListener(() => OnSelectPlayer(1));
        SelectPlayer3.onClick.AddListener(() => OnSelectPlayer(2));
        SelectPlayer4.onClick.AddListener(() => OnSelectPlayer(3));

        // Gắn sự kiện cho StartButton
        StartButton.onClick.AddListener(StartGame);
        // Khởi tạo danh sách cho từng người chơi
        playerLists.Add(0, new List<int> { 0, 0, 0, 0, 0, 1 });
        playerLists.Add(1, new List<int> { 0, 0, 0, 0, 0, 1 });
        playerLists.Add(2, new List<int> { 0, 0, 0, 0, 0, 1 });
        playerLists.Add(3, new List<int> { 0, 0, 0, 0, 0, 1 });

        // Khởi tạo ánh xạ giữa Gun và danh sách trong phương thức Start  
        gunToPlayerList = new Dictionary<Text, List<int>>
        {
            { Gun1, playerLists[0] },
            { Gun2, playerLists[1] },
            { Gun3, playerLists[2] },
            { Gun4, playerLists[3] }
        };
        playerButtons = new Dictionary<Transform, (Button, Button, Text)>
            {
                { HandPlayer1, (Throw1, Lair1, Gun1) },
                { HandPlayer2, (Throw2, Lair2, Gun2) },
                { HandPlayer3, (Throw3, Lair3, Gun3) },
                { HandPlayer4, (Throw4, Lair4, Gun4) }
            };
        // Vô hiệu hóa StartButton ban đầu
        StartButton.interactable = false;
        // Gắn sự kiện cho các nút ThrowButton
        Throw1.onClick.AddListener(() => OnThrowCard(0));
        Throw2.onClick.AddListener(() => OnThrowCard(1));
        Throw3.onClick.AddListener(() => OnThrowCard(2));
        Throw4.onClick.AddListener(() => OnThrowCard(3));
        // Gắn sự kiện cho các nút LairButton
        Lair1.onClick.AddListener(() => { Debug.Log("Lair1 Clicked"); OnLair(0); });
        Lair2.onClick.AddListener(() => { Debug.Log("Lair2 Clicked"); OnLair(1); });
        Lair3.onClick.AddListener(() => { Debug.Log("Lair3 Clicked"); OnLair(2); });
        Lair4.onClick.AddListener(() => { Debug.Log("Lair4 Clicked"); OnLair(3); });
    }
    void OnSelectPlayer(int playerIndex)// Đủ 4 người tham gia thì có thể bắt đầu
    {
        if (playerSelected[playerIndex])
        {
            Debug.Log($"Người chơi {playerIndex + 1} đã tham gia, không thể chọn lại.");
            return; // Người chơi đã chọn, bỏ qua
        }

        playerSelected[playerIndex] = true;
        playerCount++;
        Debug.Log($"Người chơi {playerIndex + 1} tham gia. Tổng số người chơi: {playerCount}");

        // Vô hiệu hóa nút SelectPlayer đã chọn
        switch (playerIndex)
        {
            case 0: SelectPlayer1.interactable = false; break;
            case 1: SelectPlayer2.interactable = false; break;
            case 2: SelectPlayer3.interactable = false; break;
            case 3: SelectPlayer4.interactable = false; break;
        }

        // Bật StartButton nếu có ít nhất 2 người chơi tham gia
        if (playerCount >= 4)
        {
            StartButton.interactable = true;
        }
    }

    void StartGame()// Bắt đầu game
    {
        Debug.Log("Game bắt đầu!");
        // activePlayers = playerCount;
        for (int i = 0; i < playerSelected.Length; i++)
        {
            if (playerSelected[i])
            {
                activePlayers.Add(i);
                StartCoroutine(DistributeCards(handPlayers[i]));
                HideBackInHand(handPlayers[i]);
            }
        }
        // Chia bài cho các người chơi đã chọn

        AssignRandomCard();
        // Tắt các nút SelectPlayer
        SelectPlayer1.gameObject.SetActive(false);
        SelectPlayer2.gameObject.SetActive(false);
        SelectPlayer3.gameObject.SetActive(false);
        SelectPlayer4.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
    }

    IEnumerator DistributeCards(Transform handPlayer)//Chia bài cho người chơi
    {
        List<GameObject> allCards = new List<GameObject>();
        allCards.AddRange(AceCards);
        allCards.AddRange(KingCards);
        allCards.AddRange(QueenCards);
        allCards.AddRange(JokerCards);

        for (int i = 0; i < 5; i++)
        {
            if (allCards.Count == 0)
            {
                Debug.LogError("Không còn đủ bài để chia!");
                yield break;
            }
            int randomIndex = Random.Range(0, allCards.Count);
            GameObject card = Instantiate(allCards[randomIndex], handPlayer);
            card.transform.localPosition = Vector3.zero; // Căn chỉnh vị trí
            allCards.RemoveAt(randomIndex); // Loại bỏ bài đã sử dụng
            yield return new WaitForSeconds(0.2f);
            // Ẩn BACK của lá bài
            Transform backTransform = card.transform.Find("BACK");
            if (backTransform != null)
            {
                backTransform.gameObject.SetActive(true);
            }
            // Gắn sự kiện OnCardClicked
            Button cardButton = card.GetComponent<Button>();
            if (cardButton == null)
            {
                cardButton = card.AddComponent<Button>(); // Thêm Button nếu lá bài chưa có
            }
            cardButton.onClick.AddListener(() => OnCardClicked(card));
        }
    }

    void HideBackInHand(Transform handPlayer)
    {
        foreach (Transform card in handPlayer)
        {
            Transform backTransform = card.Find("BACK");
            if (backTransform != null)
            {
                backTransform.gameObject.SetActive(true);
            }
        }
    }


    void AssignRandomCard()
    {
        List<GameObject> possibleCards = new List<GameObject>();
        if (AceCards.Length > 0) possibleCards.Add(AceCards[0]);
        if (KingCards.Length > 0) possibleCards.Add(KingCards[0]);
        if (QueenCards.Length > 0) possibleCards.Add(QueenCards[0]);

        if (possibleCards.Count == 0)
        {
            Debug.LogError("No cards available for ASSIGN!");
            return;
        }

        int randomIndex = Random.Range(0, possibleCards.Count);
        GameObject assignedCard = Instantiate(possibleCards[randomIndex], Assign);
        Transform backTransform = assignedCard.transform.Find("BACK");
        if (backTransform != null)
        {
            backTransform.gameObject.SetActive(false); // Ẩn BACK
        }
        assignedCard.transform.localPosition = Vector3.zero;
    }

    public void ShowSelectPlayer1()
    {
        SelectPlayer1.gameObject.SetActive(false);
        Throw1.gameObject.SetActive(true);
        Lair1.gameObject.SetActive(true);
        Gun1.gameObject.SetActive(true);
    }

    public void ShowSelectPlayer2()
    {
        SelectPlayer2.gameObject.SetActive(false);
        Throw2.gameObject.SetActive(true);
        Lair2.gameObject.SetActive(true);
        Gun2.gameObject.SetActive(true);
    }

    public void ShowSelectPlayer3()
    {
        SelectPlayer3.gameObject.SetActive(false);
        Throw3.gameObject.SetActive(true);
        Lair3.gameObject.SetActive(true);
        Gun3.gameObject.SetActive(true);
    }

    public void ShowSelectPlaye4()
    {
        SelectPlayer4.gameObject.SetActive(false);
        Throw4.gameObject.SetActive(true);
        Lair4.gameObject.SetActive(true);
        Gun4.gameObject.SetActive(true);
    }

    public void OnCardClicked(GameObject card)
    {
        Color defaultColor = Color.white; // Màu mặc định
        Color selectedColor = new Color(1f, 0.658f, 0f); // #FFA800

        Image cardImage = card.GetComponent<Image>();
        if (cardImage.color == selectedColor)
        {
            cardImage.color = defaultColor;// Trở về màu mặc định
            selectedCards.Remove(card); // Xóa khỏi danh sách được chọn
        }
        else
        {
            cardImage.color = selectedColor; // Đổi thành màu được chọn
            if (!selectedCards.Contains(card))
            {
                selectedCards.Add(card); // Thêm vào danh sách được chọn
            }
        }
    }

    int GetTotalCardsInHand(int playerIndex)
    {
        Transform playerHand = handPlayers[playerIndex];
        return playerHand.childCount;
    }
    int GetNextPlayer()
    {
        int nextPlayer;
        if (activePlayers.Count == 0)
        {
            return -1;
        }
        if (activePlayers.Count == 1)
        {
            return activePlayers[0];
        }

        int currentPlayerIndex = activePlayers.IndexOf(currentPlayer);
        nextPlayer = (currentPlayerIndex + 1) % activePlayers.Count;
        return activePlayers[nextPlayer];
    }


    void OnThrowCard(int playerIndex)
    {
        if (!activePlayers.Contains(playerIndex))
        {
            Debug.Log($"Người chơi {playerIndex + 1} đã bị loại và không thể thực hiện hành động.");
            currentPlayer = GetNextPlayer();
            Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
            return;
        }
        if (GetTotalCardsInHand(playerIndex) == 0)
        {
            Debug.Log($"Người chơi {playerIndex + 1} hết bài, bỏ lượt");
            currentPlayer = GetNextPlayer();
            Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
            return;
        }
        if (currentPlayer != playerIndex)
        {
            Debug.Log($"Không phải lượt của người chơi {playerIndex + 1}");
            return;
        }
        // Xóa các lá bài hiện tại trong Center
        foreach (Transform child in Center)
        {
            Destroy(child.gameObject);
        }
        // Lấy danh sách các lá bài được chọn
        Transform playerHand = handPlayers[playerIndex];
        var selectedCards = playerHand.GetComponentsInChildren<Image>()
            .Where(img => img.color == new Color(1f, 0.658f, 0f))
            .Select(img => img.gameObject)
            .ToList();
        if (selectedCards.Count == 0)
        {
            Debug.Log($"Người chơi {playerIndex + 1} chưa chọn lá bài.");
            return;
        }
        // Di chuyển tất cả các lá bài được chọn đến TableCenter
        foreach (var card in selectedCards)
        {
            card.transform.SetParent(Center); // Di chuyển đến bàn chơi
            card.transform.localPosition = Vector3.zero; // Đặt vị trí trung tâm
            card.GetComponent<Image>().color = Color.white; // Reset màu của lá bài
            // Ẩn BACK của lá bài
            Transform backTransform = card.transform.Find("BACK");
            if (backTransform != null)
            {
                backTransform.gameObject.SetActive(true);
            }
        }
        Debug.Log($"Người chơi {playerIndex + 1} đã đánh {selectedCards.Count} lá bài.");
        // Chuyển lượt
        currentPlayer = GetNextPlayer();
        Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
        //if (!activePlayers.Contains(playerIndex))
        //{
        //    Debug.Log($"Người chơi {playerIndex + 1} đã bị loại và không thể thực hiện hành động.");
        //    currentPlayer = GetNextPlayer();
        //    Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
        //    return;
        //}
    }

    bool IsCenterMatchingAssign()
    {
        if (Center.childCount == 0) return false;
        foreach (Transform card in Center)
        {
            if (card.name != Assign.GetChild(0).name && card.name != "JOKER(Clone)")
            {
                return false;
            }
        }
        return true;
    }

    Text GetGunFromPlayerIndex(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return Gun1;
            case 1: return Gun2;
            case 2: return Gun3;
            case 3: return Gun4;
            default:
                Debug.LogError($"Invalid player index: {playerIndex}");
                return null;
        }
    }


    void RandomizeList(Text gun, int playerIndex)
    {
        if (!gunToPlayerList.ContainsKey(gun))
        {
            Debug.LogError("Gun không được ánh xạ đến danh sách nào!");
            return;
        }
        List<int> playerList = gunToPlayerList[gun];
        if (playerList == null || playerList.Count == 0)
        {
            Debug.Log($"Người chơi {playerIndex + 1} không còn giá trị để random.");
            return;
        }
        int totalAmmo = 6;
        int randomIndex = Random.Range(0, playerList.Count);
        int randomValue = playerList[randomIndex];

        if (randomValue == 0)
        {
            HandleGunFire(playerList, gun, playerIndex, totalAmmo);
        }
        else if (randomValue == 1)
        {
            HandlePlayerEliminated(playerList, playerIndex);
        }
    }

    void HandleGunFire(List<int> playerList, Text gun, int playerIndex, int totalAmmo)
    {
        playerList.RemoveAt(0); // Loại giá trị đầu tiên khỏi danh sách
        gun.text = $"{totalAmmo - playerList.Count}/6";
        Debug.Log($"Người chơi {playerIndex + 1} may mắn sống sót. Giá trị hiện tại: {gun.text}");
        // Bắt đầu coroutine để trì hoãn 3 giây trước khi reset và chuyển lượt
        StartCoroutine(DelayedResetAndNextTurn());
    }
    IEnumerator DelayedResetAndNextTurn()
    {
        yield return new WaitForSeconds(3); // Trì hoãn 3 giây
        ResetCenter();
        // Skip eliminated or empty-handed players
        currentPlayer = GetNextPlayer();
        Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
    }

    IEnumerator DelayedResetRound()
    {
        yield return new WaitForSeconds(3);
        ResetCenter();
        ResetAssign();
        ClearPlayerHand();

        // Re-distribute cards
        for (int i = 0; i < playerSelected.Length; i++)
        {
            if (playerSelected[i] && activePlayers.Contains(i))
            {
                StartCoroutine(DistributeCards(handPlayers[i]));
                HideBackInHand(handPlayers[i]);
            }
        }
        AssignRandomCard();
        if (activePlayers.Count > 0)
        {
            currentPlayer = activePlayers[0];
        }
        // Reset eliminated players
        for (int i = 0; i < playerEliminated.Length; i++)
        {
            playerEliminated[i] = false;
        }
        //Enable All Buttons
        Throw1.gameObject.SetActive(true);
        Throw2.gameObject.SetActive(true);
        Throw3.gameObject.SetActive(true);
        Throw4.gameObject.SetActive(true);
        Lair1.gameObject.SetActive(true);
        Lair2.gameObject.SetActive(true);
        Lair3.gameObject.SetActive(true);
        Lair4.gameObject.SetActive(true);

        if (activePlayers.Count <= 1)
        {
            Debug.Log("Game Over! Going to WinScene");
            SceneManager.LoadScene("Winner");
        }
    }
    void HandlePlayerEliminated(List<int> playerList, int playerIndex)
    {
        Debug.Log($"Người chơi {playerIndex + 1} bị loại!");
        playerEliminated[playerIndex] = true; // Mark the player as eliminated
        eliminationOrder.Add(playerIndex + 1); // Add to the elimination order (player number)

        activePlayers.Remove(playerIndex); // Remove from active players list
                                           //Disable buttons
        Transform handToDisable = handPlayers[playerIndex];
        handToDisable.gameObject.SetActive(false);
        switch (playerIndex)
        {
            case 0: Throw1.gameObject.SetActive(false); Lair1.gameObject.SetActive(false); Gun1.gameObject.SetActive(false); break;
            case 1: Throw2.gameObject.SetActive(false); Lair2.gameObject.SetActive(false); Gun2.gameObject.SetActive(false); break;
            case 2: Throw3.gameObject.SetActive(false); Lair3.gameObject.SetActive(false); Gun3.gameObject.SetActive(false); break;
            case 3: Throw4.gameObject.SetActive(false); Lair4.gameObject.SetActive(false); Gun4.gameObject.SetActive(false); break;

        }
        StartCoroutine(DelayedResetRound());
    }

    void ResetCenter()
    {
        foreach (Transform child in Center)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Center đã được reset.");
    }

    void ResetAssign()
    {
        foreach (Transform child in Assign)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Center đã được reset.");
    }

    void ResetRound()
    {
        Debug.Log("Vòng chơi đã được reset.");
        currentPlayer = 0;
        foreach (var hand in handPlayers)
        {
            foreach (Transform card in hand)
            {
                card.GetComponent<Image>().color = Color.white;
            }
        }
        currentPlayer = 0;
    }

    void ClearPlayerHand()
    {
        foreach (Transform handToClear in handPlayers)
        {
            foreach (Transform card in handToClear)
            {
                Destroy(card.gameObject);
            }
        }
    }

    void OnLair(int playerIndex)
    {
        if (!activePlayers.Contains(playerIndex))
        {
            Debug.Log($"Người chơi {playerIndex + 1} đã bị loại và không thể thực hiện hành động.");
            currentPlayer = GetNextPlayer();
            Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
            return;
        }
        if (GetTotalCardsInHand(playerIndex) == 0)
        {
            Debug.Log($"Người chơi {playerIndex + 1} hết bài, bỏ lượt");
            currentPlayer = GetNextPlayer();
            Debug.Log($"Đến lượt người chơi {currentPlayer + 1}.");
            return;
        }
        Debug.Log($"OnLair called for player {playerIndex}");

        if (Center == null || Center.childCount == 0)
        {
            Debug.Log("Không cso lá bài nào để kiểm tra");
        }
        else
        {
            if (currentPlayer != playerIndex)
            {
                Debug.Log($"Không phải lượt của người chơi {playerIndex + 1}");
                return;
            }
            // Ẩn BACK của bài trong Assign
            if (Center.childCount > 0)
            {
                foreach (var card in selectedCards)
                {
                    if (card == null)
                    {
                        Debug.LogWarning("A selected card is null. Skipping this card!");
                        continue; // Skip card if it has been destroyed
                    }
                    Transform backTransform = card.transform.Find("BACK");
                    if (backTransform != null)
                    {
                        backTransform.gameObject.SetActive(false);
                    }
                }
            }

            // Kiểm tra bài trong Center
            bool isMatching = IsCenterMatchingAssign();
            if (isMatching)
            {
                Debug.Log($"Bài trong Center trùng với Assign. Xử lý cho người chơi {playerIndex + 1}.");
                Text currentGun = GetGunFromPlayerIndex(playerIndex);
                RandomizeList(currentGun, playerIndex);
            }
            else
            {
                int previousPlayer = GetPreviousPlayer(playerIndex);
                Debug.Log($"Bài trong Center không trùng với Assign. Xử lý cho người chơi trước đó {previousPlayer + 1}.");
                Text previousGun = GetGunFromPlayerIndex(previousPlayer);
                RandomizeList(previousGun, previousPlayer);
            }
        }
    }
    int GetPreviousPlayer(int playerIndex)
    {
        if (activePlayers.Count <= 1)
        {
            return -1; // No previous player
        }
        int currentPlayerIndex = activePlayers.IndexOf(playerIndex);
        int previousPlayerIndex = (currentPlayerIndex - 1 + activePlayers.Count) % activePlayers.Count;
        return activePlayers[previousPlayerIndex];

    }
}