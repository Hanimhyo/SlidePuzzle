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


    void Start()
    {
        //시작할때 난이도 조절부터 시작
        m_state = GameState.selectdifficult;
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
                break;
            case GameState.playingGame:
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




    void MakePuzzle()
    {
        //배열 크기 지정
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        //퍼즐이 놓일 기준위치
        Vector3 origin = new Vector3(-originObjFactory.transform.position.x + (originObjFactory.transform.localScale.x * -0.5f) +
                                      originObjFactory.transform.localScale.x * 0.5f / selectLineNumber,
                                     originObjFactory.transform.position.y + (originObjFactory.transform.localScale.y * 0.5f) -
                                     originObjFactory.transform.localScale.y * 0.5f / selectLineNumber,
                                     0);

        //  - 3 line 퍼즐이니 3x3 = 9 개 생성할 필요
        for (int i = 0; i < selectLineNumber * selectLineNumber; i++)
        {
            //퍼즐 생성
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //부모 지정하기
            puzzlePiece.transform.parent = transform;

            //생성 순서에 맞게 이름을 지정
            puzzlePiece.transform.name = i.ToString();

            //생성한 퍼즐을 2차원 배열에 넣기
            //행
            int line = i / selectLineNumber;
            //렬
            int row = i % selectLineNumber;

            //퍼즐
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = origin + (new Vector3(originObjFactory.transform.localScale.x * row / selectLineNumber,
                                                                    originObjFactory.transform.localScale.y * -line / selectLineNumber,
                                                                    0));

            arrayPuzzlePiece[line, row].transform.position = arrayPuzzlePosition[line, row];

            //  - 크기가 가로, 세로가 1/3로 줄어들 필요
            puzzlePiece.transform.localScale =
                new Vector3(puzzlePiece.transform.localScale.x / (float)selectLineNumber, puzzlePiece.transform.localScale.y / (float)selectLineNumber, 1);

            //퍼즐 이미지 조정
            Material myMat = puzzlePiece.GetComponent<MeshRenderer>().material;
            //Tiling 조절
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset조절
            myMat.mainTextureOffset = new Vector2(row * 1f / selectLineNumber, 1 - ((line + 1) * 1f / selectLineNumber));
        }
    }
}