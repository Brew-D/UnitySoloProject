using UnityEngine;
using UnityEngine.UI; // As we use Image, which need UnityEngine.UI to use
using System.IO; // for Input and Output
using UnityEditor.SceneManagement;
using TMPro;

//저장할 값들
public class SaveData
{
    public bool _savedAllSoldiers;//from the first play, record player saved all 2 soldiers in at least one game or not.
    public bool _killedAllEnemy;//from the first play, record player killed all 5 enemies in at least one game or not.
    public float _lastSoldierSaveTime;//record time player saved all 2 soldiers in last game which successfully save 2 soldiers.
    public float _bestSaveTime;//record the fastest time player saved all 2 soldiers.
    public float _lastEnemyKillTime;//record time player killed all 5 enemies in last game which successfully kill 5 enemies.
    public float _bestKillTime;//record the fastest time player killed all 5 enemies.
}
public class GameManager : MonoBehaviour
{
    #region SerializeField
    //object
    [SerializeField] PlayerStatePattern _player; // 플레이어
    [SerializeField] GameObject _enemyPrefab; // 적
    [SerializeField] GameObject _soldierPrefab; // 구출할 병사

    [SerializeField] AudioSource _audio; // 오디오(배경음)
    [SerializeField] AudioClip _practiceBGM;
    [SerializeField] AudioClip _actualCombatBGM;
    [SerializeField] EnemySpawner _spawner;
    [SerializeField] Transform[] _soldierWaypoints;
    [SerializeField] Transform[] _EnemyWaypoints;
    
    //UI
    [SerializeField] Image _healthBar;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] TextMeshProUGUI _soldierText;
    [SerializeField] TextMeshProUGUI _enemyText;
    #endregion
    //전역적으로 접근 가능한 인스턴스
    public static GameManager Instance;

    //저장용
    private bool _saveAllSoldier;
    private bool _killedAllEnemy;
    private int _bestRecordA = 359999; // for the first play
    private int _bestRecordB = 359999; // for the first play

    //게임 플레이 관련
    private bool _isStarted = false;
    private float _playerMaxHealth = 3;
    private float _timeFlow = 0;

    //기록 관련
    public float _savedSoldier = 0;
    public float _killedEnemy = 0;
    private int _recordTimeA = 359999; // for the first play
    private int _recordTimeB = 359999; // for the first play

    private string filePath;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        Debug.Log("파일경로 : " + filePath);
    }

    private void Update()
    {
        HealthUpdate();
        //check the time flow
        if(_isStarted == true)
        _timeFlow += Time.deltaTime;
        _timeText.text = ((int)Instance._timeFlow/60).ToString() + " : " + ((int)Instance._timeFlow %60).ToString();
        _soldierText.text = ((int)Instance._savedSoldier).ToString();
        _enemyText.text = ((int)Instance._killedEnemy).ToString();
    }

    public void HealthUpdate()
    {
        //get health from PlayerStatePattern cs file and synchronize health with healthbar's fill Amount
        _healthBar.fillAmount = _player._health / _playerMaxHealth;
    }

    /// <summary>
    /// save all report when player died, or player killed all enemies and saved all soldiers.
    /// </summary>
    public void SaveFile()
    {
        //플레이어가 게임오버 상태가 되거나 조건을 전부 충족하면
        //기록한 시간들과 완료한 목록을 저장하여 파일로 저장
        SaveData data = new SaveData();

        if (_player._health == 0 || (_killedEnemy == 5 && _savedSoldier == 2))
        {
            //병사 기록 관련
            data._savedAllSoldiers = _savedSoldier == 2 ? true : false;
            if (_savedSoldier == 2)
                data._lastSoldierSaveTime = _recordTimeA;
            if (_recordTimeA < data._bestSaveTime)
                data._bestSaveTime = _recordTimeA;

            //적 기록 관련
            data._killedAllEnemy = _killedEnemy == 5 ? true : false;
            if (_killedEnemy == 5)
                data._lastEnemyKillTime = _recordTimeB;
            if (_recordTimeB < data._bestKillTime)
                data._bestSaveTime = _recordTimeB;

            //기록할 사항들을 전부 기입하여 세이브파일 생성
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }
    }

    /// <summary>
    /// if there's save file that saved before, load the record from the save file.
    /// </summary>
    public void LoadFile()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // 파일에서 문자열을 읽어옴
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            _saveAllSoldier = data._savedAllSoldiers;
            _killedAllEnemy = data._killedAllEnemy;

        }
    }

    public void SetField()
    {
        //첫 번째 병사 생성
        int FirstSoldierLoc = Random.Range(0, 3);
        Instantiate(_soldierPrefab, _soldierWaypoints[FirstSoldierLoc]);

        //두 번째 병사 생성
        int SecondSoldierLoc = Random.Range(0, 3);
        if (SecondSoldierLoc == FirstSoldierLoc)
        {
            while (SecondSoldierLoc == FirstSoldierLoc)
            {
                SecondSoldierLoc = Random.Range(0, 3);
            }
        }
        Instantiate(_soldierPrefab, _soldierWaypoints[SecondSoldierLoc]);

        //적 생성 위치 지정
        Transform[] enemySpawnLoc = new Transform[5];
        for (int i = 0; i < 5; i++)
        {
            int EnemySpawnLoc = Random.Range(0, 5);
            enemySpawnLoc[i] = _EnemyWaypoints[EnemySpawnLoc];
        }
    }
    //병사를 놓을 공간 3칸과 적을 놓을 공간 5칸

    //병사를 놓을 공간을 랜덤으로 생성

    //적이 만들어질 때마다 대상을 플레이어로 설정
    //초기에 만들어지는 수는 5마리

    public void GameStart()
    {
        _isStarted = true;
        _player.gameObject.transform.position = new Vector3(2.05f, 0.51f, 0); 
    }
}
