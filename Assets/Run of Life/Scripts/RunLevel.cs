using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.UI;
using PathCreation;
using TMPro;

public class RunLevel : LevelManager
{
    [SerializeField] private Text moneyText = null;
    [SerializeField] private Color clockRimColor;
    [SerializeField] private Color clockBackColor;
    [SerializeField] private Color goodPoolColor;
    [SerializeField] private Color badPoolColor;
    [SerializeField] private Color[] wallColors = null;
    [SerializeField] private Material transparentMaterial = null;
    [SerializeField] private GameObject clockDestroyEffectPrefab = null;
    [SerializeField] private GameObject checkpointConfettiPrefab = null;
    [SerializeField] private Transform finishLine = null;
    [SerializeField] private Transform finishCameraPosition = null;
    [SerializeField] private Slider progress = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [Header("Layer Masks")]
    [SerializeField] private LayerMask wallLayerMask = 0;
    [SerializeField] private LayerMask poolLayerMask = 0;
    private Checkpoint[] checkpoints;
    private WallRow[] wallRows = null;
    private Player player = null;
    private float finishPosition = 0;
    private PathCreator pathCreator = null;
    public int playerMoney = 0;



    public LayerMask WallLayerMask { get => wallLayerMask; }
    public Checkpoint[] Checkpoints { get => checkpoints; }
    public LayerMask PoolLayerMask { get => poolLayerMask; }
    public Player Player { get => player; }
    public Color ClockBackColor { get => clockBackColor; }
    public Color ClockRimColor { get => clockRimColor; }
    public WallRow[] WallRows { get => wallRows; }
    public PathCreator PathCreator { get => pathCreator; }
    public Color[] WallColors { get => wallColors; }
    public Material TransparentMaterial { get => transparentMaterial; }
    public GameObject ClockDestroyEffectPrefab { get => clockDestroyEffectPrefab; }
    public Color GoodPoolColor { get => goodPoolColor; }
    public Color BadPoolColor { get => badPoolColor; }
    public Transform FinishLine { get => finishLine; }
    public float FinishPosition { get => finishPosition; }
    public Transform FinishCameraPosition { get => finishCameraPosition; }

    private new void Awake()
    {
        base.Awake();
        checkpoints = FindObjectsOfType<Checkpoint>();
        player = FindObjectOfType<Player>();
        wallRows = FindObjectsOfType<WallRow>();
        moneyText.text = GameManager.MONEY.ToString();
        pathCreator = FindObjectOfType<PathCreator>();
        finishPosition = pathCreator.path.GetClosestDistanceAlongPath(finishLine.position);
        levelText.text = GameManager.Instance.CurrentLevel.ToString();
    }

    private void Update()
    {
        progress.value = Mathf.Clamp((player.Distance - 5) / (finishPosition - 5), 0, 1);
    }

    public override void FinishLevel(bool success)
    {
        GameManager.Instance.State = GameManager.GameState.FINISHED;
        if (!success)
            LeanTween.delayedCall(1.2f, () => { GameManager.Instance.FinishLevel(success); });
        else
        {
            playerMoney *= Mathf.CeilToInt(Mathf.Clamp((player.Distance - finishPosition - 2) / 3f + 1, 1, 3));
            GameManager.MONEY += playerMoney;
            GameManager.Instance.FinishLevel(success);
        }
    }

    public override void StartLevel()
    {
        player.ChangeState(Player.PlayerState.RUNNING);
    }

}
