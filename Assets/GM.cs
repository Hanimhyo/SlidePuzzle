using System;
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

    //무작위도 확인
    int[] linePuzzle;
    int inversion = 0;

    //마지막퍼즐
    public GameObject lastPuzzlePeace;


    //클릭한 퍼즐 행렬
    public Vector2Int selectMatrix;

    //마지막퍼즐 행렬
    public Vector2Int lastMatrix;

    //차이의 간격
    Vector2Int skimaMatrix;

    //변화량
    Vector2Int moveVector;


    //유한상태머신
    public enum GameState
    {
        selectdifficult,
        makePuzzle,
        suffle,
        playingGame,
        puzzleMove,
        puzzleMoveWait,
        clear
    }

    public GameState m_state;

    //이동머신
    enum MoveState
    {
        noMove,
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
                PuzzleMove();
                break;
            case GameState.puzzleMoveWait:
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
        puzzleAllNumber = selectLineNumber * selectLineNumber;
        //배열 크기 지정
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        linePuzzle = new int[puzzleAllNumber];


        //퍼즐의 크기 측정
        puzzleWidth = originObjFactory.transform.GetChild(0).transform.localScale.x / selectLineNumber;
        puzzleHeight = originObjFactory.transform.GetChild(0).transform.localScale.y / selectLineNumber;

        //  - 3 row 퍼즐이니 3x3 = 9 개 생성할 필요
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

            //1차배열에 넣기
            linePuzzle[i] = i;
            //생성한 퍼즐을 2차원 배열에 넣기
            //행
            int row = i / selectLineNumber;
            //렬
            int column = i % selectLineNumber;

            //퍼즐
            arrayPuzzlePiece[row, column] = puzzlePiece;
            arrayPuzzlePosition[row, column] = (new Vector3(puzzleWidth * column , 
                                                          puzzleHeight * -row,
                                                          0));

            arrayPuzzlePiece[row, column].transform.localPosition = arrayPuzzlePosition[row, column];

            //  - 크기가 가로, 세로가 1/n로 줄어들 필요
            puzzlePiece.transform.localScale = Vector3.one / selectLineNumber;

            //퍼즐 이미지 조정
            Material myMat = puzzlePiece.GetComponentInChildren<MeshRenderer>().material;
            //Tiling 조절
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset조절
            myMat.mainTextureOffset = new Vector2(column * 1f / selectLineNumber, 1 - ((row + 1) * 1f / selectLineNumber));

            TextMesh tm = puzzlePiece.GetComponentInChildren<TextMesh>();
            //tm.text = (i + 1).ToString();

            //마지막 퍼즐은 보이지 않도록 한다
            if (i == puzzleAllNumber - 1)
            {
                //puzzlePiece.GetComponentInChildren<MeshRenderer>().enabled = false;
                puzzlePiece.GetComponentInChildren<TextMesh>().text = "";
                lastPuzzlePeace = puzzlePiece;
            }
        }

        //마지막퍼즐 위치 입력
        lastMatrix = new Vector2Int(selectLineNumber - 1, selectLineNumber - 1);

        //생성완료후에 셔플로 이행
        m_state = GameState.suffle;
    }


    void SufflePuzzle()
    {
        return;

        //셔플 횟수 = 총갯수의 2배 * 2
        int suffleNum = puzzleAllNumber * 2;

        //무질서도 0
        inversion = 0;

        //섞어준다!
        for (int i = 0; i < suffleNum; i++)
        {
            //숫자 추첨
            int randomFirstNum = UnityEngine.Random.Range(0, puzzleAllNumber - 1);
            int randomSecondNum = UnityEngine.Random.Range(0, puzzleAllNumber - 1);
            //중복이면 SecondNum은 다음 번호로 (뽑은 숫자가 최대치면 0이 되도록 % puzzleAllNumber) 해준다
            if (randomFirstNum == randomSecondNum)
            {
                randomSecondNum = (randomSecondNum + 1) % (puzzleAllNumber - 1);
            }

            //1차배열 변경
            int temp = linePuzzle[randomFirstNum];
            linePuzzle[randomFirstNum] = linePuzzle[randomSecondNum];
            linePuzzle[randomSecondNum] = temp;

            //fistNum과 secNum의 해당되는 퍼즐을 교체해준다
            //fistNum에 해당하는 행렬을 찾은 후
            int firstRow = randomFirstNum / selectLineNumber;
            int firstColumn = randomFirstNum % selectLineNumber;
            //퍼즐을 임시 변수에 넣어주고
            GameObject tempObj = arrayPuzzlePiece[firstRow, firstColumn];

            //secNum에 해당하는 행렬을 찾아서
            int secRow = randomSecondNum / selectLineNumber;
            int secColumn = randomSecondNum % selectLineNumber;
            //firstNum에 있는 퍼즐을 secNum 퍼즐로 변경해준다
            arrayPuzzlePiece[firstRow, firstColumn] = arrayPuzzlePiece[secRow, secColumn];
            //secNum 퍼즐엔 fisrtNum퍼즐로 변경한다
            arrayPuzzlePiece[secRow, secColumn] = tempObj;
        }

        //제대로 작동되는 것인지 확인하기!
        PuzzleCheck();
        


        //퍼즐 위치 정렬하기
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //행
            int row = i / selectLineNumber;
            //렬
            int column = i % selectLineNumber;

            //행렬에 맞춰서 위치 변경
            arrayPuzzlePiece[row, column].transform.localPosition = arrayPuzzlePosition[row, column];
        }

        //플레이 게임으로~
        m_state = GameState.playingGame;
    }

    //퍼즐을 풀 수 있는가 확인하기
    void PuzzleCheck()
    {
        lastMatrix.x = selectLineNumber - 1;
        lastMatrix.y = selectLineNumber - 1;

        //무질서도 구하기
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            for (int j = i; j < puzzleAllNumber; j++)
            {
                if(linePuzzle[i] > linePuzzle[j])
                {
                    inversion++;
                }
            }
        }

        //퍼즐을 풀 수 있는지 알아보는 방법
        //row의 수가 홀수이고 무질서도가 짝수이면 풀 수 있다 
        //row수가 짝수이고 
        // - 빈칸퍼즐이 밑에서 새어서 짝수행이며 무질서도가  홀수
        // - 빈칸퍼즐이 밑에서 새어서 홀수이고 무질서도가 짝수이면
        // 풀수있다 -> 반대이면 풀수 없다
        // 그러하다면 어떻게 해야 풀 수 있는 퍼즐이 되나?
        //맨 마지막 퍼즐과 그 앞의 퍼즐을 교체하면 짝수홀수를 변경 할 수 있다


        //풀수 없는 조건인
        //1. row이 홀수 이고, 무질서도가 홀수면
        if(selectLineNumber % 2 == 1 && inversion % 2 == 1)
        {
            LastPieceChange();
        }
        else if(selectLineNumber % 2 == 0)
        {
            //2.밑에서 홀수층이고 무질서도가 홀수면
            if((selectLineNumber - lastMatrix.x) % 2 == 1 && inversion % 2 == 1)
            {
                LastPieceChange();
            }
        }
    }

    void LastPieceChange()
    {
        //보이는 마지막 퍼즐
        int lastNum = puzzleAllNumber - 2;
        //바로 그 전에 퍼즐
        int semilastNum = lastNum - 1;


        int lastRow = lastNum / selectLineNumber;
        int lastColumn = lastNum % selectLineNumber;

        int semiLastRow = semilastNum / selectLineNumber;
        int semiLastColumn = semilastNum % selectLineNumber;

        //가장 마지막을 집고
        GameObject temp = arrayPuzzlePiece[lastRow, lastColumn];
        //위치교대
        arrayPuzzlePiece[lastRow, lastColumn] = arrayPuzzlePiece[semiLastRow, semiLastColumn];
        arrayPuzzlePiece[semiLastRow, semiLastColumn] = temp;
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
                selectMatrix.x = (int)Mathf.Round(-hitPos.y / puzzleHeight);
                selectMatrix.y = (int)Mathf.Round(hitPos.x / puzzleWidth);

                //기준은 안움직인다!
                move_state = MoveState.noMove;

                //해당라인에 마지막퍼즐(빈칸)이 있는지 확인
                for (int i = 0; i < selectLineNumber; i++)
                {
                    //행검사
                    if(arrayPuzzlePiece[selectMatrix.x, i].name == (puzzleAllNumber - 1).ToString())
                    {
                        if(i > selectMatrix.y)
                        {
                            //오른쪽 이동으로
                            move_state = MoveState.right;
                            break;
                        }
                        else if(i < selectMatrix.y)
                        {
                            //왼쪽이동으로
                            move_state = MoveState.left;
                            break;
                        }
                    }
                    //열검사
                    if (arrayPuzzlePiece[i, selectMatrix.y].name == (puzzleAllNumber - 1).ToString())
                    {
                        if (i > selectMatrix.x)
                        {
                            //밑으로 이동
                            move_state = MoveState.down;
                            break;
                        }
                        else if (i < selectMatrix.x)
                        {
                            //위로 이동
                            move_state = MoveState.up;
                            break;
                        }
                    }
                }

                //이동하는거면
                if(move_state != MoveState.noMove)
                {
                    //차이행렬 = 마지막 - 선택
                    skimaMatrix = lastMatrix - selectMatrix;


                    //이동으로 바꾼다
                    m_state = GameState.puzzleMove;
                }
            }
        }
    }


    void PuzzleMove()
    {
        switch (move_state)
        {
            case MoveState.noMove:
                break;
            case MoveState.up:
                moveVector = Vector2Int.right;
                break;
            case MoveState.down:
                moveVector = Vector2Int.left;
                break;
            case MoveState.left:
                moveVector = Vector2Int.up;
                break;
            case MoveState.right:
                moveVector = Vector2Int.down;
                break;
        }

        int Count = (int)skimaMatrix.magnitude;

        for (int i = 0; i < Count; i++)
        {
            //이건 골의 행렬
            Vector2Int goalMatrix = skimaMatrix + selectMatrix;
            //한칸 이동
            skimaMatrix = skimaMatrix + moveVector;
            //피동 퍼즐 행렬
            Vector2Int startMatrix = skimaMatrix + selectMatrix;

            //이동
            StartCoroutine(Co_Moving(startMatrix, goalMatrix, arrayPuzzlePiece[startMatrix.x, startMatrix.y]));

            //골은 변경
            arrayPuzzlePiece[goalMatrix.x, goalMatrix.y].transform.localPosition = arrayPuzzlePosition[startMatrix.x, startMatrix.y];

            //변경
            GameObject temp = arrayPuzzlePiece[goalMatrix.x, goalMatrix.y];

            arrayPuzzlePiece[goalMatrix.x, goalMatrix.y] = arrayPuzzlePiece[startMatrix.x, startMatrix.y];

            arrayPuzzlePiece[startMatrix.x, startMatrix.y] = temp;

        }

        //으아아아
        lastMatrix = selectMatrix;

        m_state = GameState.puzzleMoveWait;
    }

    //이동 코루틴
    IEnumerator Co_Moving(Vector2Int startMatrix, Vector2Int goalMatrix, GameObject puzzlePiece)
    {
        float alpha = 0;

        while(alpha < 1)
        {
            alpha += Time.deltaTime * 5f;
            //이동!
            puzzlePiece.transform.localPosition = Vector3.Lerp(arrayPuzzlePosition[startMatrix.x, startMatrix.y], arrayPuzzlePosition[goalMatrix.x, goalMatrix.y], alpha);

            yield return null;
        }

        puzzlePiece.transform.localPosition = arrayPuzzlePosition[goalMatrix.x, goalMatrix.y];

        if(ClearCheck())
        {
            m_state = GameState.clear;
            lastPuzzlePeace.GetComponentInChildren<MeshRenderer>().enabled = true;

            //글자없애기
            ClearText();
        }
        else
        {
            m_state = GameState.playingGame;
        }

    }

    void PuzzleMoveWait()
    {

    }

    bool ClearCheck()
    {
        bool clear = true;

        for(int i = 0; i < selectLineNumber; i++)
        {
            for(int j = 0; j < selectLineNumber; j++)
            {
                int puzzleNum = i * selectLineNumber + j;

                if (arrayPuzzlePiece[i,j].name != puzzleNum.ToString())
                {
                    clear = false;
                    break;
                }
            }
        }

        return clear;
    }


    void ViewText()
    {
        //글자 보이게 하기
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //행
            int row = i / selectLineNumber;
            //렬
            int column = i % selectLineNumber;

            //행렬에 맞춰서 위치 변경
            arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = (Int32.Parse(arrayPuzzlePiece[row, column].name) + 1).ToString();

            if(arrayPuzzlePiece[row, column].name == (puzzleAllNumber - 1).ToString())
            {
                arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = "";
            }
        }
    }
    void ClearText()
    {
        //글자없애기
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //행
            int row = i / selectLineNumber;
            //렬
            int column = i % selectLineNumber;

            //행렬에 맞춰서 위치 변경
            arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = "";
        }
    }
}