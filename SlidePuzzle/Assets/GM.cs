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
        //�迭 ũ�� ����
        arrayPuzzlePiece = new GameObject[selectLineNumber, selectLineNumber];
        arrayPuzzlePosition = new Vector3[selectLineNumber, selectLineNumber];

        puzzleAllNumber = selectLineNumber * selectLineNumber;

        //������ ũ�� ����
        puzzleWidth = originObjFactory.transform.GetChild(0).transform.localScale.x / selectLineNumber;
        puzzleHeight = originObjFactory.transform.GetChild(0).transform.localScale.y / selectLineNumber;

        //  - 3 line �����̴� 3x3 = 9 �� ������ �ʿ�
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

            //������ ������ 2���� �迭�� �ֱ�
            //��
            int line = i / selectLineNumber;
            //��
            int row = i % selectLineNumber;

            //����
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = (new Vector3(puzzleWidth * row , 
                                                          puzzleHeight * -line,
                                                          0));

            arrayPuzzlePiece[line, row].transform.localPosition = arrayPuzzlePosition[line, row];

            //  - ũ�Ⱑ ����, ���ΰ� 1/n�� �پ�� �ʿ�
            puzzlePiece.transform.localScale = Vector3.one / selectLineNumber;

            //���� �̹��� ����
            Material myMat = puzzlePiece.GetComponentInChildren<MeshRenderer>().material;
            //Tiling ����
            myMat.mainTextureScale = new Vector2(1f / selectLineNumber, 1f / selectLineNumber);
            //offset����
            myMat.mainTextureOffset = new Vector2(row * 1f / selectLineNumber, 1 - ((line + 1) * 1f / selectLineNumber));
            
            //������ ������ ������ �ʵ��� �Ѵ�
            if(i == puzzleAllNumber - 1)
            {
                puzzlePiece.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }

        //�����Ϸ��Ŀ� ���÷� ����
        m_state = GameState.suffle;
    }


    void SufflePuzzle()
    {
        //���� Ƚ�� = �Ѱ����� 2�� * 2
        int suffleNum = puzzleAllNumber * 2;

        //�����ش�!
        for (int i = 0; i < suffleNum; i++)
        {
            //���� ��÷
            int randomFirstNum = Random.Range(0, puzzleAllNumber);
            int randomSecondNum = Random.Range(0, puzzleAllNumber);
            //�ߺ��̸� SecondNum�� ���� ��ȣ�� (���� ���ڰ� �ִ�ġ�� 0�� �ǵ��� % puzzleAllNumber) ���ش�
            if (randomFirstNum == randomSecondNum)
            {
                randomSecondNum = (randomSecondNum + 1) % puzzleAllNumber;
            }

            //fistNum�� secNum�� �ش�Ǵ� ������ ��ü���ش�
            //fistNum�� �ش��ϴ� ����� ã�� ��
            int firstLine = randomFirstNum / selectLineNumber;
            int firstRow = randomFirstNum % selectLineNumber;
            //������ �ӽ� ������ �־��ְ�
            GameObject tempObj = arrayPuzzlePiece[firstLine, firstRow];

            //secNum�� �ش��ϴ� ����� ã�Ƽ�
            int secLine = randomSecondNum / selectLineNumber;
            int secRow = randomSecondNum % selectLineNumber;
            //firstNum�� �ִ� ������ secNum ����� �������ش�
            arrayPuzzlePiece[firstLine, firstRow] = arrayPuzzlePiece[secLine, secRow];
            //secNum ���� fisrtNum����� �����Ѵ�
            arrayPuzzlePiece[secLine, secRow] = tempObj;
        }

        //���� ��ġ �����ϱ�
        for (int i = 0; i < puzzleAllNumber; i++)
        {
            //��
            int line = i / selectLineNumber;
            //��
            int row = i % selectLineNumber;

            //��Ŀ� ���缭 ��ġ ����
            arrayPuzzlePiece[line, row].transform.localPosition = arrayPuzzlePosition[line, row];
        }

        m_state = GameState.playingGame;
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
                int line = (int)Mathf.Round(-hitPos.y / puzzleHeight);
                int low = (int)Mathf.Round(hitPos.x / puzzleWidth);


                bool left = false;
                bool right = false;
                bool up = false;
                bool down = false;
                //�ش���ο� ����������(��ĭ)�� �ִ��� Ȯ��
                for(int i = 0; i < selectLineNumber; i++)
                {
                    //��˻�
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
                    //���˻�
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