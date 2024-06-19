using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    //1.���ΰ��� ����
    //2.������ ����� ���� �ʿ��� ���� ������

    // ������ ���� ����
    public int lineNumber = 3;
    //���� ������Ʈ
    public GameObject originObjFactory;


    void Start()
    {
        //3.���� ����� �Լ� ���� �� �����
        MakePuzzle();
    }


    //2���� �迭 ����
    GameObject[,] arrayPuzzlePiece;
    Vector3[,] arrayPuzzlePosition;

    void MakePuzzle()
    {
        //4.���ο� ���� ���� ���� ���� + ũ�� + ���

        //�迭 ũ�� ����
        arrayPuzzlePiece = new GameObject[lineNumber, lineNumber];
        arrayPuzzlePosition = new Vector3[lineNumber, lineNumber];

        //������ ���� ������ġ
        Vector3 origin = new Vector3(-originObjFactory.transform.position.x + (originObjFactory.transform.localScale.x * -0.5f) +                   
                                      originObjFactory.transform.localScale.x * 0.5f / lineNumber,
                                     originObjFactory.transform.position.y + (originObjFactory.transform.localScale.y * 0.5f) -
                                     originObjFactory.transform.localScale.y * 0.5f / lineNumber, 
                                     0);

        //  - 3 line �����̴� 3x3 = 9 �� ������ �ʿ�
        for (int i = 0; i < lineNumber * lineNumber; i++) 
        {
            //���� ����
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //�θ� �����ϱ�
            puzzlePiece.transform.parent = transform;

            //���� ������ �°� �̸��� ����
            puzzlePiece.transform.name = i.ToString();

            //������ ������ 2���� �迭�� �ֱ�
            //��
            int line = i / lineNumber;
            //��
            int row = i % lineNumber;

            //����
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = origin + (new Vector3(originObjFactory.transform.localScale.x * row / lineNumber,
                                                                    originObjFactory.transform.localScale.y * -line / lineNumber,
                                                                    0));

            arrayPuzzlePiece[line, row].transform.position = arrayPuzzlePosition[line, row];

            //  - ũ�Ⱑ ����, ���ΰ� 1/3�� �پ�� �ʿ�
            puzzlePiece.transform.localScale = 
                new Vector3(puzzlePiece.transform.localScale.x / (float)lineNumber, puzzlePiece.transform.localScale.y / (float)lineNumber, 1);

            //���� �̹��� ����
            Material myMat = puzzlePiece.GetComponent<MeshRenderer>().material;
            //Tiling ����
            myMat.mainTextureScale = new Vector2(1f / lineNumber, 1f / lineNumber);
            //offset����
            myMat.mainTextureOffset = new Vector2(row * 1f / lineNumber, 1 - ((line + 1) * 1f / lineNumber));
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
