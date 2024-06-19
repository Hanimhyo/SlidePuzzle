using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    // ������ �ּҶ���, �ִ���� ��
    public int minLineNum = 3;
    public int maxLineNum = 8;
    // ������ ���� ����
    public int selectLineNumber = 3;

    //���� ������Ʈ
    public GameObject originObjFactory;
    //������� ���� ������ ��ġ!
    public Transform puzzleStartTransform;
    //���� �ϳ��� ����
    float puzzleWidth = 0;
    float puzzleHeight = 0;

    //2���� �迭 ����
    GameObject[,] arrayPuzzlePiece;
    Vector3[,] arrayPuzzlePosition;

    //���ѻ��¸ӽ�
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
        //�����Ҷ� ���̵� �������� ����
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

    //���� ���̵� ��
    public void PlusDifficultUp()
    {
        //���� ����!
        selectLineNumber++;

        //�ִ�ġ�� ������ �ִ�ġ�� 
        if(maxLineNum < selectLineNumber)
        {
            selectLineNumber = maxLineNum;
        }
    }
    
    //���� ���̵� �ٿ�
    public void MinusDifficultDown()
    {
        //���� ����!
        selectLineNumber--;

        //�ּ�ġ ������ ���� �ּ�ġ��
        if (minLineNum > selectLineNumber)
        {
            selectLineNumber = minLineNum;
        }
    }




    void MakePuzzle()
    {
        //�迭 ũ�� ����
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        //������ ���� ������ġ
        Vector3 origin = new Vector3(-originObjFactory.transform.position.x + (originObjFactory.transform.localScale.x * -0.5f) +
                                      originObjFactory.transform.localScale.x * 0.5f / selectLineNumber,
                                     originObjFactory.transform.position.y + (originObjFactory.transform.localScale.y * 0.5f) -
                                     originObjFactory.transform.localScale.y * 0.5f / selectLineNumber,
                                     0);

        //  - 3 line �����̴� 3x3 = 9 �� ������ �ʿ�
        for (int i = 0; i < selectLineNumber * selectLineNumber; i++)
        {
            //���� ����
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //�θ� �����ϱ�
            puzzlePiece.transform.parent = transform;

            //���� ������ �°� �̸��� ����
            puzzlePiece.transform.name = i.ToString();

            //������ ������ 2���� �迭�� �ֱ�
            //��
            int line = i / selectLineNumber;
            //��
            int row = i % selectLineNumber;

            //����
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = origin + (new Vector3(originObjFactory.transform.localScale.x * row / selectLineNumber,
                                                                    originObjFactory.transform.localScale.y * -line / selectLineNumber,
                                                                    0));

            arrayPuzzlePiece[line, row].transform.position = arrayPuzzlePosition[line, row];

            //  - ũ�Ⱑ ����, ���ΰ� 1/3�� �پ�� �ʿ�
            puzzlePiece.transform.localScale =
                new Vector3(puzzlePiece.transform.localScale.x / (float)selectLineNumber, puzzlePiece.transform.localScale.y / (float)selectLineNumber, 1);

            //���� �̹��� ����
            Material myMat = puzzlePiece.GetComponent<MeshRenderer>().material;
            //Tiling ����
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset����
            myMat.mainTextureOffset = new Vector2(row * 1f / selectLineNumber, 1 - ((line + 1) * 1f / selectLineNumber));
        }
    }
}