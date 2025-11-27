using UnityEngine;
using UnityEngine.UI; // As we use Image, which need UnityEngine.UI to use
using System.IO; // for Input and Output
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

//저장할 값들
public class SaveData
{
    public bool _savedAllSoldiers;//from the first play, record player saved all 2 soldiers in at least one game or not.
    public bool _killedAllEnemy;//from the first play, record player killed all 5 enemies in at least one game or not.
    public int _lastSoldierSaveTime;//record time player saved all 2 soldiers in last game which successfully save 2 soldiers.
    public int _bestSaveTime;//record the fastest time player saved all 2 soldiers.
    public int _lastEnemyKillTime;//record time player killed all 5 enemies in last game which successfully kill 5 enemies.
    public int _bestKillTime;//record the fastest time player killed all 5 enemies.
}
public class GameManager : MonoBehaviour
{
    #region 매개변수
    [Header("Objects")]
    [SerializeField] PlayerStatePattern _player; // 플레이어
    [SerializeField] GameObject _enemyPrefab; // 적
    [SerializeField] GameObject _soldierPrefab; // 구출할 병사

    [Header("Audio")]
    [SerializeField] AudioSource _audio; // 오디오(배경음)
    [SerializeField] AudioClip _practiceBGM;
    [SerializeField] AudioClip _actualCombatBGM;

    [Header("GameField")]
    [SerializeField] EnemySpawner _spawner;
    [SerializeField] Transform[] _soldierWaypoints;
    [SerializeField] Transform[] _EnemyWaypoints;

    [Header("Record")]
    [SerializeField] TextMeshProUGUI LSST;
    [SerializeField] TextMeshProUGUI BSST;
    [SerializeField] TextMeshProUGUI LEAKT;
    [SerializeField] TextMeshProUGUI BEAKT;

    [Header("UI")]
    [SerializeField] Image _healthBar;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] TextMeshProUGUI _soldierText;
    [SerializeField] TextMeshProUGUI _enemyText;

    [SerializeField] Image _endCard;
    [SerializeField] Image _achievement;
    [SerializeField] Sprite _half;
    [SerializeField] Sprite _complete;

    
    //전역적으로 접근 가능한 인스턴스
    public static GameManager Instance;

    //저장용
    private bool _saveAllSoldier; // 병사 구출 완료 여부
    private bool _killedAllEnemy; // 적 섬멸 완료 여부
    private int _bestRecordA = 359999; // 병사 구출 완료 시간 최단기록. 최초값은 99시간 59분 59초.
    private int _bestRecordB = 359999; // 적 섬멸 완료 시간 최단기록. 최초값은 99시간 59분 59초.

    //게임 플레이 관련
    public bool _isStarted = false; // false = 정찰 페이즈, true = 실전 게임페이즈

    private float _playerMaxHealth = 3; // 플레이어의 최대 체력은 3. 체력바 UI를 위한 매개변수.
    private float _timeFlow = 0; // 시간 흐름 체크용.

    //기록 관련
    public float _savedSoldier = 0; // 해당 판에서 구출한 병사들의 수
    public float _killedEnemy = 0; // 해당 판에서 격파한 적의 수
    private int _recordTimeA = 359999; // 병사 구출 완료 시간. 최초값은 99시간 59분 59초.
    private int _recordTimeB = 359999; // 적 섬멸 완료 시간. 최초값은 99시간 59분 59초.
    private float _tempA = 0; // 병사구출 시간 기록용.
    private float _tempB = 0; // 적 섬멸 시간 기록용.

    private string filePath; // 세이브파일의 저장 경로.
    #endregion

    #region 라이프사이클 관련
    private void Awake()
    {
        //전역적인 게임매니저가 이미 존재하며, 해당 게임매니저가 자신이 아닐 경우 중복을 피하기 위해 파괴합니다.
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //위의 조건이 만족되지 않았다면, 현재의 오브젝트를 전역적인 게임매니저로 임명합니다.
        Instance = this;

        //이 게임매니저를 파괴되지 않도록 지정해줍니다.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //파일의 저장 경로는 일반적인 데이터의 경로, 저장할 파일은 "PlayerData" json파일입니다.
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        LoadFile();
    }

    private void Update()
    {
        if(_player != null)
            HealthUpdate();
        //시간이 흐르는 것을 체크합니다.
        if(_isStarted == true)
        {
            _timeFlow += Time.deltaTime;
            if (_savedSoldier < 2)
                _tempA += Time.deltaTime;
            else if (_savedSoldier >= 2)
                _recordTimeA = (int)_tempA;
            if (_killedEnemy < 5)
                _tempB += Time.deltaTime;
            else if (_killedEnemy >= 5)
                _recordTimeB = (int)_tempB;
            //제작하면서 기대하고 있는 최대 플레이시간은 99시간 59분 59초로, 그 이상은 어차피 저장되지 않으므로 해당 시점까지만 표기를 진행합니다.
            if(_timeFlow < 359999.3)
            {
                string hour = ((int)Instance._timeFlow / 3600) < 10 ? "0" + ((int)Instance._timeFlow / 3600).ToString() : ((int)Instance._timeFlow / 3600).ToString();
                string minute = ((int)Instance._timeFlow / 60) < 10 ? "0" + ((int)Instance._timeFlow / 60).ToString() : ((int)Instance._timeFlow / 60).ToString();
                string second = ((int)Instance._timeFlow % 60) < 10 ? "0" + ((int)Instance._timeFlow % 60).ToString() : ((int)Instance._timeFlow % 60).ToString();
                _timeText.text = hour + ":" + minute + ":" + second;
            }
            _soldierText.text = (Instance._savedSoldier).ToString();
            _enemyText.text = (Instance._killedEnemy).ToString();


            if (_savedSoldier >= 2 && _killedEnemy >= 5)
            {
                Time.timeScale = 0;
                SaveFile();
                Credit();
            }
        }
    }
    #endregion
    #region 메서드, 함수
    public void HealthUpdate()
    {
        //PlayerStatePattern.cs 파일로부터 체력을 받아와 체력바의 Fill Amount와 연동시킵니다.
        _healthBar.fillAmount = _player._health / _playerMaxHealth;
    }

    /// <summary>
    /// 초를 입력받고 시/분/초로 나누어진 문자열로 반환하는 메서드입니다.
    /// </summary>
    /// <param name="number">시/분/초를 전부 초 기준으로 계산한 수.</param>
    /// <returns></returns>
    public string ChangeIntoTime(int number)
    {
        string Timer = (number / 3600).ToString()  + ":" + (number % 3600 / 60).ToString() + ":" + (number % 3600 % 60).ToString();
        return Timer;
    }
    /// <summary>
    /// 플레이어가 사망했거나, 플레이어가 적을 모두 죽이고 병사를 모두 구출한 상황이라면 해당 기록을 생성하여 세이브 파일에 저장합니다.
    /// </summary>
    public void SaveFile()
    {
        if (_isStarted)
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
                if (_recordTimeA < _bestRecordA)
                    data._bestSaveTime = _recordTimeA;

                //적 기록 관련
                data._killedAllEnemy = _killedEnemy == 5 ? true : false;
                if (_killedEnemy == 5)
                    data._lastEnemyKillTime = _recordTimeB;
                if (_recordTimeB < _bestRecordB)
                    data._bestKillTime = _recordTimeB;

                //기록할 사항들을 전부 기입하여 세이브파일 생성
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, json);
            }
        }
    }

    /// <summary>
    /// 저장된 파일이 있다면, 그 파일을 불러와 적용합니다.
    /// </summary>
    public void LoadFile()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // 파일에서 문자열을 읽어옴
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            _saveAllSoldier = data._savedAllSoldiers;
            _killedAllEnemy = data._killedAllEnemy;

            _recordTimeA = data._lastSoldierSaveTime;
            _bestRecordA = data._bestSaveTime;
            _recordTimeB = data._lastEnemyKillTime;
            _bestRecordB = data._bestKillTime;


            if (_saveAllSoldier != _killedAllEnemy)
                _achievement.sprite = _half;
            else if (_saveAllSoldier && _killedAllEnemy)
                _achievement.sprite = _complete;
            else if(!_saveAllSoldier && !_killedAllEnemy)
                _achievement.gameObject.SetActive(false);

                LSST.text = "Last Soldier Save Time\n" + ChangeIntoTime(_recordTimeA);
            BSST.text = "Best Soldier Save Time\n" + ChangeIntoTime(_bestRecordA);
            LEAKT.text = "Last Enemy All Kill Time\n" + ChangeIntoTime(_recordTimeB);
            BEAKT.text = "Best Enemy All Kill Time\n" + ChangeIntoTime(_bestRecordB);

            _savedSoldier = 0;
            _killedEnemy = 0;
        }
    }
    /// <summary>
    /// 두 마리의 병사와 다섯 마리의 적을, 생성 가능한 좌표에 랜덤으로 생성합니다.
    /// 두 병사의 위치는 겹치면 안 되며, 적의 위치는 겹쳐도 괜찮습니다.
    /// </summary>
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

        //적 생성 위치 지정 후 생성
        Transform[] enemySpawnLoc = new Transform[5];
        for (int i = 0; i < 5; i++)
        {
            int EnemySpawnLoc = Random.Range(0, 5);
            enemySpawnLoc[i] = _EnemyWaypoints[EnemySpawnLoc];
            _spawner.SpawnEnemy(enemySpawnLoc[i].transform.position);
        }
    }
    /// <summary>
    /// 스테이지 내에서 정찰 페이즈 전용 시작 버튼을 눌렀을 때 동작하는 메서드입니다.
    /// </summary>
    public void GameStart()
    {
        _isStarted = true;
        _player.gameObject.transform.position = _player._spawnLocation;
        _audio.Stop();
        _audio.clip = _actualCombatBGM;
        _audio.Play();
    }
    /// <summary>
    /// 스테이지를 시작하는 함수입니다. 실행 시 정찰 페이즈의 게임으로 돌입합니다.
    /// </summary>
    /// <param name="SceneNumber">index of Scene.</param>
    /// <returns></returns>
    private IEnumerator StageStart(int SceneNumber)
    {
        SceneManager.LoadScene(SceneNumber);
        yield return null;
        FieldSetting();
        _savedSoldier = 0;
        _killedEnemy = 0;
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        _audio.Stop();
        _endCard.gameObject.SetActive(true);
        SaveFile();
    }

    public void Credit()
    {
        StartCoroutine(EndGame());
    }
    /// <summary>
    /// 타이틀 화면에서 시작 버튼을 눌렀을 때의 동작입니다.
    /// </summary>
    public void StartButton()
    {
        StartCoroutine(StageStart(1));
        _audio.Play();
    }
    /// <summary>
    /// 필드에 병사와 적을 배치하고, UI와 게임매니저를 연동합니다.
    /// </summary>
    public void FieldSetting()
    {
        if (_player == null)
            _player = FindFirstObjectByType<PlayerStatePattern>();
        if (_spawner == null)
            _spawner = FindFirstObjectByType<EnemySpawner>();
        if (_healthBar == null)
            _healthBar = GameObject.Find("Life").GetComponent<Image>();
        if (_timeText == null)
            _timeText = GameObject.Find("Time").GetComponentInChildren<TextMeshProUGUI>();
        if (_soldierText == null)
            _soldierText = GameObject.Find("SoldierInfo").GetComponentInChildren<TextMeshProUGUI>();
        if (_enemyText == null)
            _enemyText = GameObject.Find("EnemyInfo").GetComponentInChildren<TextMeshProUGUI>();
        Button QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        QuitButton.onClick.AddListener(Quit);
        if (_endCard == null)
            _endCard = GameObject.Find("FadeOut").GetComponent<Image>();
        _endCard.gameObject.SetActive(false);

        Button button = GameObject.Find("GameStart").GetComponent<Button>();
        button.onClick.AddListener(GameStart);

        SetField();
    }
    /// <summary>
    /// 게임을 종료시키기 위한 메서드입니다.
    /// </summary>
    public void Quit()
    {
        SaveFile();
        Application.Quit();
    }
    #endregion
    public void Exit()
    {
        Application.Quit();
    }
    public void SoldierSave()
    {
        _savedSoldier++;
    }

}
