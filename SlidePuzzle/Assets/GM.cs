using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    // 퍼즐의 최소라인, 최대라인 수
    public int minLineNum = 3;
    public int maxLineNum = 8;
    // 퍼즐의 라인 갯수
    public int selectLineNumber = 3;
    int puzzleAllNumber = 0;

    //원본 오브젝트
    public GameObject originObjFactory;
    //퍼즐들이 나올 기준점 위치!
    public Transform puzzleStartTransform;
    //퍼즐 하나의 길이
    float puzzleWidth = 0;
    float puzzleHeight = 0;

    //2차원 배열 선언
    GameObject[,] arrayPuzzlePiece;
    Vector3[,] arrayPuzzlePosition;

    //유한상태머신
    public enum GameState
    {
        selectdifficult,
        makePuzzle,
        suffle,
        playingGame,
        puzzleMove,
        clear
    }

    public GameState m_state;

    enum MoveState
    {
        up,
        down,
        left,
        right
    }
    MoveState move_state;


    void Start()
    {
        //시작할때 난이도 조절부터 시작
        m_state = GameState.selectdifficult;

        StartGame();
    }


    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case GameState.selectdifficult:
                break;
            case GameState.makePuzzle:
                MakePuzzle();
                break;
            case GameState.suffle:
                SufflePuzzle();
                break;
            case GameState.playingGame:
                WaitSelect();
                break;
            case GameState.puzzleMove:
                break;
            case GameState.clear:
                break;
        }
    }

    //퍼즐 난이도 업
    public void PlusDifficultUp()
    {
        //라인 증가!
        selectLineNumber++;

        //최대치를 넘으면 최대치로 
        if(maxLineNum < selectLineNumber)
        {
            selectLineNumber = maxLineNum;
        }
    }
    
    //퍼즐 난이도 다운
    public void MinusDifficultDown()
    {
        //라인 감소!
        selectLineNumber--;

        //최소치 밑으로 가면 최소치로
        if (minLineNum > selectLineNumber)
        {
            selectLineNumber = minLineNum;
        }
    }

    //게임 시작
    public void StartGame()
    {
        //make퍼즐 단계로 이행
        m_state = GameState.makePuzzle;
    }




    void MakePuzzle()
    {
        //배열 크기 지정
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        puzzleAllNumber = selectLineNumber * selectLineNumber;

        //퍼즐의 크기 측정
        puzzleWidth = originObjFactory.transform.GetChild(0).transform.localScale.x / selectLineNumber;
        puzzleHeight = originObjFactory.transform.GetChild(0).transform.localScale.y / selectLineNumber;

        //  - 3 line 퍼즐이니 3x3 = 9 개 생성할 필요
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //퍼즐 생성
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //부모 지정하기
            puzzlePiece.transform.parent = puzzleStartTransform;

            //생성 순서에 맞게 이름을 지정
            puzzlePiece.transform.name = i.ToString();
            puzzlePiece.transform.GetChild(0).name = i.ToString();
            //레이어도 지정
            puzzlePiece.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("PuzzlePiece");

            //생성한 퍼즐을 2차원 배열에 넣기
            //행
            int line = i / selectLineNumber;
            //렬
            int row = i % selectLineNumber;

            //퍼즐
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = (new Vector3(puzzleWidth * row , 
                                                          puzzleHeight * -line,
                                                          0));

            arrayPuzzlePiece[line, row].transform.localPosition = arrayPuzzlePosition[line, row];

            //  - 크기가 가로, 세로가 1/n로 줄어들 필요
            puzzlePiece.transform.localScale = Vector3.one / selectLineNumber;

            //퍼즐 이미지 조정
            Material myMat = puzzlePiece.GetComponentInChildren<MeshRenderer>().material;
            //Tiling 조절
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset조절
            myMat.mainTextureOffset = new Vector2(row * 1f / selectLineNumber, 1 - ((line + 1) * 1f / selectLineNumber));
            
            //마지막 퍼즐은 보이지 않도록 한다
            if(i == puzzleAllNumber - 1)
            {
                puzzlePiece.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }

        //생성완료후에 셔플로 이행
        m_state = GameState.suffle;
    }


    void SufflePuzzle()
    {
        //셔플 횟수 = 총갯수의 2배 * 2
        int suffleNum = puzzleAllNumber * 2;

        //섞어준다!
        for (int i = 0; i < suffleNum; i++)
        {
            //숫자 추첨
            int randomFirstNum = Random.Range(0, puzzleAllNumber);
            int randomSecondNum = Random.Range(0, puzzleAllNumber);
            //중복이면 SecondNum은 다음 번호로 (뽑은 숫자가 최대치면 0이 되도록 % puzzleAllNumber) 해준다
            if (randomFirstNum == randomSecondNum)
            {
                randomSecondNum = (randomSecondNum + 1) % puzzleAllNumber;
            }

            //fistNum과 secNum의 해당되는 퍼즐을 교체해준다
            //fistNum에 해당하는 행렬을 찾은 후
            int firstLine = randomFirstNum / selectLineNumber;
            int firstRow = randomFirstNum % selectLineNumber;
            //퍼즐을 임시 변수에 넣어주고
            GameObject tempObj = arrayPuzzlePiece[firstLine, firstRow];

            //secNum에 해당하는 행렬을 찾아서
            int secLine = randomSecondNum / selectLineNumber;
            int secRow = randomSecondNum % selectLineNumber;
            //firstNum에 있는 퍼즐을 secNum 퍼즐로 변경해준다
            arrayPuzzlePiece[firstLine, firstRow] = arrayPuzzlePiece[secLine, secRow];
            //secNum 퍼즐엔 fisrtNum퍼즐로 변경한다
            arrayPuzzlePiece[secLine, secRow] = tempObj;
        }

        //퍼즐 위치 정렬하기
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //행
            int line = i / selectLineNumber;
            //렬
            int row = i % selectLineNumber;

            //행렬에 맞춰서 위치 변경
            arrayPuzzlePiece[line, row].transform.localPosition = arrayPuzzlePosition[line, row];
        }

        m_state = GameState.playingGame;
    }

    void WaitSelect()
    {
        //마우스를 클릭하면
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            //클릭한게 퍼즐 조각이라면
            if(Physics.Raycast(ray,out hitinfo, 100, 1 << LayerMask.NameToLayer("PuzzlePiece")))
            {
                //해당 위치 파악하기
                //찾은 퍼즐의 부모의 localPos를 찾아서
                Vector3 hitPos = hitinfo.transform.parent.localPosition;
                //해당 행렬 구하기
                int line = (int)Mathf.Round(-hitPos.y / puzzleHeight);
                int low = (int)Mathf.Round(hitPos.x / puzzleWidth);


                bool left = false;
                bool right = false;
                bool up = false;
                bool down = false;
                //해당라인에 마지막퍼즐(빈칸)이 있는지 확인
                for(int i = 0; i < selectLineNumber; i++)
                {
                    //행검사
                    if(arrayPuzzlePiece[line, i].name == (puzzleAllNumber - 1).ToString())
                    {
                        if(i > low)
                        {
                            right = true;
                            break;
                        }
                        else if(i < low)
                        {
                            left = true;
                            break;
                        }
                    }
                    //열검사
                    if (arrayPuzzlePiece[i, low].name == (puzzleAllNumber - 1).ToString())
                    {
                        if (i > line)
                        {
                            down = true;
                            break;
                        }
                        else if (i < line)
                        {
                            up = true;
                            break;
                        }
                    }
                }

                print("Line :" + line + " /  Low : " + low);
                print(left.ToString() + right.ToString() + up.ToString() + down.ToString()) ;

            }
        }
    }
}