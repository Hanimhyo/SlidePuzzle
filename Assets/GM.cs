using System;
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
    int puzzleAllNumber = 0;

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

    //�������� Ȯ��
    int[] linePuzzle;
    int inversion = 0;

    //����������
    public GameObject lastPuzzlePeace;


    //Ŭ���� ���� ���
    public Vector2Int selectMatrix;

    //���������� ���
    public Vector2Int lastMatrix;

    //������ ����
    Vector2Int skimaMatrix;

    //��ȭ��
    Vector2Int moveVector;


    //���ѻ��¸ӽ�
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

    //�̵��ӽ�
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
        //�����Ҷ� ���̵� �������� ����
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

    //���� ����
    public void StartGame()
    {
        //make���� �ܰ�� ����
        m_state = GameState.makePuzzle;
    }




    void MakePuzzle()
    {
        puzzleAllNumber = selectLineNumber * selectLineNumber;
        //�迭 ũ�� ����
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        linePuzzle = new int[puzzleAllNumber];


        //������ ũ�� ����
        puzzleWidth = originObjFactory.transform.GetChild(0).transform.localScale.x / selectLineNumber;
        puzzleHeight = originObjFactory.transform.GetChild(0).transform.localScale.y / selectLineNumber;

        //  - 3 row �����̴� 3x3 = 9 �� ������ �ʿ�
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //���� ����
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //�θ� �����ϱ�
            puzzlePiece.transform.parent = puzzleStartTransform;

            //���� ������ �°� �̸��� ����
            puzzlePiece.transform.name = i.ToString();
            puzzlePiece.transform.GetChild(0).name = i.ToString();
            //���̾ ����
            puzzlePiece.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("PuzzlePiece");

            //1���迭�� �ֱ�
            linePuzzle[i] = i;
            //������ ������ 2���� �迭�� �ֱ�
            //��
            int row = i / selectLineNumber;
            //��
            int column = i % selectLineNumber;

            //����
            arrayPuzzlePiece[row, column] = puzzlePiece;
            arrayPuzzlePosition[row, column] = (new Vector3(puzzleWidth * column , 
                                                          puzzleHeight * -row,
                                                          0));

            arrayPuzzlePiece[row, column].transform.localPosition = arrayPuzzlePosition[row, column];

            //  - ũ�Ⱑ ����, ���ΰ� 1/n�� �پ�� �ʿ�
            puzzlePiece.transform.localScale = Vector3.one / selectLineNumber;

            //���� �̹��� ����
            Material myMat = puzzlePiece.GetComponentInChildren<MeshRenderer>().material;
            //Tiling ����
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset����
            myMat.mainTextureOffset = new Vector2(column * 1f / selectLineNumber, 1 - ((row + 1) * 1f / selectLineNumber));

            TextMesh tm = puzzlePiece.GetComponentInChildren<TextMesh>();
            //tm.text = (i + 1).ToString();

            //������ ������ ������ �ʵ��� �Ѵ�
            if (i == puzzleAllNumber - 1)
            {
                //puzzlePiece.GetComponentInChildren<MeshRenderer>().enabled = false;
                puzzlePiece.GetComponentInChildren<TextMesh>().text = "";
                lastPuzzlePeace = puzzlePiece;
            }
        }

        //���������� ��ġ �Է�
        lastMatrix = new Vector2Int(selectLineNumber - 1, selectLineNumber - 1);

        //�����Ϸ��Ŀ� ���÷� ����
        m_state = GameState.suffle;
    }


    void SufflePuzzle()
    {
        return;

        //���� Ƚ�� = �Ѱ����� 2�� * 2
        int suffleNum = puzzleAllNumber * 2;

        //�������� 0
        inversion = 0;

        //�����ش�!
        for (int i = 0; i < suffleNum; i++)
        {
            //���� ��÷
            int randomFirstNum = UnityEngine.Random.Range(0, puzzleAllNumber - 1);
            int randomSecondNum = UnityEngine.Random.Range(0, puzzleAllNumber - 1);
            //�ߺ��̸� SecondNum�� ���� ��ȣ�� (���� ���ڰ� �ִ�ġ�� 0�� �ǵ��� % puzzleAllNumber) ���ش�
            if (randomFirstNum == randomSecondNum)
            {
                randomSecondNum = (randomSecondNum + 1) % (puzzleAllNumber - 1);
            }

            //1���迭 ����
            int temp = linePuzzle[randomFirstNum];
            linePuzzle[randomFirstNum] = linePuzzle[randomSecondNum];
            linePuzzle[randomSecondNum] = temp;

            //fistNum�� secNum�� �ش�Ǵ� ������ ��ü���ش�
            //fistNum�� �ش��ϴ� ����� ã�� ��
            int firstRow = randomFirstNum / selectLineNumber;
            int firstColumn = randomFirstNum % selectLineNumber;
            //������ �ӽ� ������ �־��ְ�
            GameObject tempObj = arrayPuzzlePiece[firstRow, firstColumn];

            //secNum�� �ش��ϴ� ����� ã�Ƽ�
            int secRow = randomSecondNum / selectLineNumber;
            int secColumn = randomSecondNum % selectLineNumber;
            //firstNum�� �ִ� ������ secNum ����� �������ش�
            arrayPuzzlePiece[firstRow, firstColumn] = arrayPuzzlePiece[secRow, secColumn];
            //secNum ���� fisrtNum����� �����Ѵ�
            arrayPuzzlePiece[secRow, secColumn] = tempObj;
        }

        //����� �۵��Ǵ� ������ Ȯ���ϱ�!
        PuzzleCheck();
        


        //���� ��ġ �����ϱ�
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //��
            int row = i / selectLineNumber;
            //��
            int column = i % selectLineNumber;

            //��Ŀ� ���缭 ��ġ ����
            arrayPuzzlePiece[row, column].transform.localPosition = arrayPuzzlePosition[row, column];
        }

        //�÷��� ��������~
        m_state = GameState.playingGame;
    }

    //������ Ǯ �� �ִ°� Ȯ���ϱ�
    void PuzzleCheck()
    {
        lastMatrix.x = selectLineNumber - 1;
        lastMatrix.y = selectLineNumber - 1;

        //�������� ���ϱ�
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

        //������ Ǯ �� �ִ��� �˾ƺ��� ���
        //row�� ���� Ȧ���̰� ���������� ¦���̸� Ǯ �� �ִ� 
        //row���� ¦���̰� 
        // - ��ĭ������ �ؿ��� ��� ¦�����̸� ����������  Ȧ��
        // - ��ĭ������ �ؿ��� ��� Ȧ���̰� ���������� ¦���̸�
        // Ǯ���ִ� -> �ݴ��̸� Ǯ�� ����
        // �׷��ϴٸ� ��� �ؾ� Ǯ �� �ִ� ������ �ǳ�?
        //�� ������ ����� �� ���� ������ ��ü�ϸ� ¦��Ȧ���� ���� �� �� �ִ�


        //Ǯ�� ���� ������
        //1. row�� Ȧ�� �̰�, ���������� Ȧ����
        if(selectLineNumber % 2 == 1 && inversion % 2 == 1)
        {
            LastPieceChange();
        }
        else if(selectLineNumber % 2 == 0)
        {
            //2.�ؿ��� Ȧ�����̰� ���������� Ȧ����
            if((selectLineNumber - lastMatrix.x) % 2 == 1 && inversion % 2 == 1)
            {
                LastPieceChange();
            }
        }
    }

    void LastPieceChange()
    {
        //���̴� ������ ����
        int lastNum = puzzleAllNumber - 2;
        //�ٷ� �� ���� ����
        int semilastNum = lastNum - 1;


        int lastRow = lastNum / selectLineNumber;
        int lastColumn = lastNum % selectLineNumber;

        int semiLastRow = semilastNum / selectLineNumber;
        int semiLastColumn = semilastNum % selectLineNumber;

        //���� �������� ����
        GameObject temp = arrayPuzzlePiece[lastRow, lastColumn];
        //��ġ����
        arrayPuzzlePiece[lastRow, lastColumn] = arrayPuzzlePiece[semiLastRow, semiLastColumn];
        arrayPuzzlePiece[semiLastRow, semiLastColumn] = temp;
    }

    void WaitSelect()
    {
        //���콺�� Ŭ���ϸ�
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            //Ŭ���Ѱ� ���� �����̶��
            if(Physics.Raycast(ray,out hitinfo, 100, 1 << LayerMask.NameToLayer("PuzzlePiece")))
            {
                
                //�ش� ��ġ �ľ��ϱ�
                //ã�� ������ �θ��� localPos�� ã�Ƽ�
                Vector3 hitPos = hitinfo.transform.parent.localPosition;
                //�ش� ��� ���ϱ�
                selectMatrix.x = (int)Mathf.Round(-hitPos.y / puzzleHeight);
                selectMatrix.y = (int)Mathf.Round(hitPos.x / puzzleWidth);

                //������ �ȿ����δ�!
                move_state = MoveState.noMove;

                //�ش���ο� ����������(��ĭ)�� �ִ��� Ȯ��
                for (int i = 0; i < selectLineNumber; i++)
                {
                    //��˻�
                    if(arrayPuzzlePiece[selectMatrix.x, i].name == (puzzleAllNumber - 1).ToString())
                    {
                        if(i > selectMatrix.y)
                        {
                            //������ �̵�����
                            move_state = MoveState.right;
                            break;
                        }
                        else if(i < selectMatrix.y)
                        {
                            //�����̵�����
                            move_state = MoveState.left;
                            break;
                        }
                    }
                    //���˻�
                    if (arrayPuzzlePiece[i, selectMatrix.y].name == (puzzleAllNumber - 1).ToString())
                    {
                        if (i > selectMatrix.x)
                        {
                            //������ �̵�
                            move_state = MoveState.down;
                            break;
                        }
                        else if (i < selectMatrix.x)
                        {
                            //���� �̵�
                            move_state = MoveState.up;
                            break;
                        }
                    }
                }

                //�̵��ϴ°Ÿ�
                if(move_state != MoveState.noMove)
                {
                    //������� = ������ - ����
                    skimaMatrix = lastMatrix - selectMatrix;


                    //�̵����� �ٲ۴�
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
            //�̰� ���� ���
            Vector2Int goalMatrix = skimaMatrix + selectMatrix;
            //��ĭ �̵�
            skimaMatrix = skimaMatrix + moveVector;
            //�ǵ� ���� ���
            Vector2Int startMatrix = skimaMatrix + selectMatrix;

            //�̵�
            StartCoroutine(Co_Moving(startMatrix, goalMatrix, arrayPuzzlePiece[startMatrix.x, startMatrix.y]));

            //���� ����
            arrayPuzzlePiece[goalMatrix.x, goalMatrix.y].transform.localPosition = arrayPuzzlePosition[startMatrix.x, startMatrix.y];

            //����
            GameObject temp = arrayPuzzlePiece[goalMatrix.x, goalMatrix.y];

            arrayPuzzlePiece[goalMatrix.x, goalMatrix.y] = arrayPuzzlePiece[startMatrix.x, startMatrix.y];

            arrayPuzzlePiece[startMatrix.x, startMatrix.y] = temp;

        }

        //���ƾƾ�
        lastMatrix = selectMatrix;

        m_state = GameState.puzzleMoveWait;
    }

    //�̵� �ڷ�ƾ
    IEnumerator Co_Moving(Vector2Int startMatrix, Vector2Int goalMatrix, GameObject puzzlePiece)
    {
        float alpha = 0;

        while(alpha < 1)
        {
            alpha += Time.deltaTime * 5f;
            //�̵�!
            puzzlePiece.transform.localPosition = Vector3.Lerp(arrayPuzzlePosition[startMatrix.x, startMatrix.y], arrayPuzzlePosition[goalMatrix.x, goalMatrix.y], alpha);

            yield return null;
        }

        puzzlePiece.transform.localPosition = arrayPuzzlePosition[goalMatrix.x, goalMatrix.y];

        if(ClearCheck())
        {
            m_state = GameState.clear;
            lastPuzzlePeace.GetComponentInChildren<MeshRenderer>().enabled = true;

            //���ھ��ֱ�
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
        //���� ���̰� �ϱ�
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //��
            int row = i / selectLineNumber;
            //��
            int column = i % selectLineNumber;

            //��Ŀ� ���缭 ��ġ ����
            arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = (Int32.Parse(arrayPuzzlePiece[row, column].name) + 1).ToString();

            if(arrayPuzzlePiece[row, column].name == (puzzleAllNumber - 1).ToString())
            {
                arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = "";
            }
        }
    }
    void ClearText()
    {
        //���ھ��ֱ�
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //��
            int row = i / selectLineNumber;
            //��
            int column = i % selectLineNumber;

            //��Ŀ� ���缭 ��ġ ����
            arrayPuzzlePiece[row, column].GetComponentInChildren<TextMesh>().text = "";
        }
    }
}