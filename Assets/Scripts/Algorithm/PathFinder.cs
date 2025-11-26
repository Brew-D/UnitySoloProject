using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Node
{
    public Vector2Int Pos; // 타일의 각 칸이 1만큼 차이가 나므로, 소수점을 계산하지 않는 것으로 계산의 속도를 높임.
    public float G; // 시작점으로부터 현재 위치까지의 실제 비용(거리)
    public float H; // 휴리스틱으로 사용할 가치. 여기서는 이동 불가능 여부와 상관없이 가로 + 세로 기준 종착까지의 타일 개수.
    public float F => G + H; // 가중치
    public Node Parent; // 경로 복원을 위한 부모노드
    public bool IsMoveable = true; // 해당 노드의 타일이 이동 가능한 타일인지 여부.
    public Node(Vector2Int pos, bool isMoveable)
    {
        Pos = pos;
        IsMoveable = isMoveable;
    }

}

public class PathFinder : MonoBehaviour
{
    //경로 탐색을 위한 범위의 좌측 최하단, 우측 최상단, 시작 좌표와 대상의 좌표.
    public Vector2Int _bottomLeft, _topRight;
    public Vector2Int _startPos, _targetPos;
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject Player;
    int _index = 0;

    //대상을 향해 이동할 경로.
    public List<Node> _path = new List<Node>();

    //타일맵 하나하나를 노드로 지정하기 위한 배열.
    Node[,] _nodeArray;

    //시작점 노드, 종착점 노드, 현재 위치 노드.
    Node _startNode, _targetNode, _currentNode;

    //방문할 노드의 리스트 OpenList와 방문한 노드의 리스트 ClosedList
    List<Node> OpenList, ClosedList;

    //시작 시에 시작점과 종착점 갱신, 해당 경로 탐색
    private void Start()
    {
        if (Enemy == null)
            Enemy = gameObject;
        if (Player == null)
        {
            Player = GameObject.Find("Astar_Target");
        }
        Init();
        PathFinding();
    }

    /// <summary>
    /// 시작점과 종착점을 정해진 대상으로 지정하는 메서드입니다.
    /// </summary>
    public void Init()
    {
        _startPos = new Vector2Int((int)Enemy.transform.position.x, (int)Enemy.transform.position.y + 1);
        _targetPos = new Vector2Int((int)Player.transform.position.x, (int)Player.transform.position.y);
    }

    //플레이어의 움직임에 따라 대상의 위치 정보를 계속 갱신합니다.
    private void FixedUpdate()
    {
        Init();
        PathFinding();
        if(_path.Count > 0)
            EnemyChase();
    }
    /// <summary>
    /// 받아온 정보들을 기준으로 목적지까지 이동하기 위한 경로를 탐색하는 메서드입니다.
    /// </summary>
    public void PathFinding()
    {
        //길찾기를 실행할 장소의 좌측 최하단부터 우측 최상단까지 범위를 설정
        int SizeX = (int)_topRight.x - (int)_bottomLeft.x + 1;
        int SizeY = (int)_topRight.y - (int)_bottomLeft.y + 1;

        //설정된 범위까지 배열 생성
        _nodeArray = new Node[SizeX, SizeY];

        //밟을 수 있는 땅에 속해 있는지 체크 ( Jumpable / Walkable 타일맵에 속해있는지. )
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                //이동 가능한 타일인지 체크하기 위한 지역 매개변수
                bool isMoveable = false;

                //좌측 최하단을 기준으로 찾도록 좌측 최하단의 좌표를 각 부분에 더한 Vector2 값을 사용.
                //이동 가능 타일의 콜라이더가 발견된 타일에는 isMoveable을 true로 설정, 타일의 중앙 부분을 기점으로 일정 범위만큼 콜라이더 체크
                foreach (var collision in Physics2D.OverlapCircleAll(new Vector2Int(i + _bottomLeft.x, j + _bottomLeft.y), 0.4f))
                {
                    //콜라이더의 레이어가 7번(움직일 수 있는 영역)일 경우 이동 가능 여부를 참으로.
                    if (collision.gameObject.layer == 7)
                        isMoveable = true;
                }

                //노드 배열의 i와 j값을 x좌표와 y좌표로 취급하여 해당 노드의 타일 정보를 갱신
                _nodeArray[i, j] = new Node(new Vector2Int(i + _bottomLeft.x, j + _bottomLeft.y), isMoveable);
            }
        }
        //시작 지점, 종착 지점, OpenList와 ClosedList 초기화, 노드 기록 초기화
        _startNode = _nodeArray[_startPos.x - _bottomLeft.x, _startPos.y - _bottomLeft.y];
        _targetNode = _nodeArray[_targetPos.x - _bottomLeft.x, _targetPos.y - _bottomLeft.y];

        //최초에 방문해야 하는 노드는 시작점.
        OpenList = new List<Node> { _startNode };
        ClosedList = new List<Node>();
        _path = new List<Node>();

        //OpenList에 들어있는 것 중 현재 노드로 할 것 지정하는 함수
        while (OpenList.Count > 0)
        {
            //일단 방문할 노드의 가장 앞에 있는 곳으로 이동 방향을 잡기
            _currentNode = OpenList[0];

            //방문 리스트에 포함된 노드 중에, F와 H가 모두 OpenList의 가장 앞에 있던 노드보다 작은 경우에는 그 노드를 이동 방향으로 설정
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= _currentNode.F && OpenList[i].H < _currentNode.H) _currentNode = OpenList[i];
            }

            //설정된 노드를 방문할 목록에서 제거하고, 방문한 목록에 추가.
            OpenList.Remove(_currentNode);
            ClosedList.Add(_currentNode);

            //마지막 지점 도착 시의 함수
            if (_currentNode == _targetNode)
            {
                //경로를 찾기 위한, 종착점에서 출발할 노드 생성
                Node PathNode = _targetNode;

                //출발한 노드가 출발점으로 도달할 때까지
                while (PathNode != _startNode)
                {
                    //경로에 현재 지점을 기록하고
                    _path.Add(PathNode);
                    //그 부모 노드로 이동
                    PathNode = PathNode.Parent;
                }

                //이 코드까지 왔다는건 시작점에 도착했다는 것이므로 경로에 시작점 추가
                _path.Add(_startNode);

                //역방향으로 기록했으니 다시 뒤집기
                _path.Reverse();
                break;
            }

            //대각선 이동과 직접 이동 추가
            //대각선 ( 45도 각도로 시작하여 시계방향 90도 ), 정방향 ( 0도 각도로 시작하여 시계방향 90도 )
            OpenListAdd(_currentNode.Pos.x + 1, _currentNode.Pos.y + 1);
            OpenListAdd(_currentNode.Pos.x - 1, _currentNode.Pos.y + 1);
            OpenListAdd(_currentNode.Pos.x - 1, _currentNode.Pos.y - 1);
            OpenListAdd(_currentNode.Pos.x + 1, _currentNode.Pos.y - 1);
            OpenListAdd(_currentNode.Pos.x, _currentNode.Pos.y + 1);
            OpenListAdd(_currentNode.Pos.x + 1, _currentNode.Pos.y);
            OpenListAdd(_currentNode.Pos.x, _currentNode.Pos.y - 1);
            OpenListAdd(_currentNode.Pos.x - 1, _currentNode.Pos.y);
        }





    }

    void EnemyChase()
    {
        if (_path.Count > 0)
        {
            
            Rigidbody2D rigid = Enemy.GetComponent<Rigidbody2D>();
            Vector2 EnemyPos = new Vector2(Enemy.transform.position.x, Enemy.transform.position.y);
            Vector2 direction = _path[_index].Pos - EnemyPos;
            if (direction.magnitude > 0.1f)
            {
                Vector2 velocity = direction.normalized * 2f;
                rigid.linearVelocity = velocity;
            }
            else
            {
                _index++;
            }
        }
    }
    /// <summary>
    /// OpenList에 특정 좌표의 노드를 추가하기 위한 메서드입니다.
    /// </summary>
    /// <param name="CheckX">추가할 노드의 X좌표</param>
    /// <param name="CheckY">추가할 노드의 Y좌표</param>
    void OpenListAdd(int CheckX, int CheckY)
    {
        //상하좌우 범위를 벗어나지 않고, 밟을 수 있는 지역이면서, 닫힌 리스트에 없을 때 시행할 함수
        if (CheckX >= _bottomLeft.x && CheckX < _topRight.x + 1 && CheckY >= _bottomLeft.y && CheckY < _topRight.y + 1 &&
            _nodeArray[CheckX - _bottomLeft.x, CheckY - _bottomLeft.y].IsMoveable &&
            !ClosedList.Contains(_nodeArray[CheckX - _bottomLeft.x, CheckY - _bottomLeft.y]))
        {

            // 이웃 노드에 포함 후 직선 : 10, 대각선 : 14의 비용 추가.
            Node Neighbor = _nodeArray[CheckX - _bottomLeft.x, CheckY - _bottomLeft.y];
            int cost = (int)_currentNode.G + (_currentNode.Pos.x - CheckX == 0 || _currentNode.Pos.y - CheckY == 0 ? 10 : 14);

            //비용이 이웃노드의 G보다 작거나 OpenList에 이웃 노드가 없으면 G,H,부모노드 설정 후 열린 리스트에 추가
            if (!OpenList.Contains(Neighbor) || cost < Neighbor.G )
            {
                //이웃 노드의 G가 더 크거나 방문할 노드에 없었다면 해당 지점의 G 값에 현재까지 소모한 비용을 기입.
                Neighbor.G = cost;
                //이웃 노드의 H는 그 노드의 지점에서 도착 지점까지 이동 가능 여부를 무시하고 X축, Y축 칸 수. 한 번 이동 시에 10의 이동비용이 드므로 10을 곱함.
                Neighbor.H = (Mathf.Abs(Neighbor.Pos.x - _targetNode.Pos.x) + Mathf.Abs(Neighbor.Pos.y - _targetNode.Pos.y)) * 10;
                //이웃 노드의 부모는 현재 지점에서 출발했으므로 현재 지점 기입.
                Neighbor.Parent = _currentNode;

                //작성 완료한 이웃 노드를 방문할 노드에 추가.
                OpenList.Add(Neighbor);
            }
        }


    }
    void OnDrawGizmos()
    {
        if (_path.Count != 0)
        {
            for (int i = 0; i < _path.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(_path[i].Pos.x, _path[i].Pos.y), new Vector2(_path[i + 1].Pos.x, _path[i + 1].Pos.y));
        }

    }


}
