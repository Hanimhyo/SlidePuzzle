using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    //1.라인갯수 선언
    //2.퍼즐을 만들기 위해 필요한 원본 데이터

    // 퍼즐의 라인 갯수
    public int lineNumber = 3;
    //원본 오브젝트
    public GameObject originObjFactory;


    void Start()
    {
        //3.퍼즐 만들기 함수 선언 및 만들기
        MakePuzzle();
    }


    //2차원 배열 선언
    GameObject[,] arrayPuzzlePiece;
    Vector3[,] arrayPuzzlePosition;

    void MakePuzzle()
    {
        //4.라인에 맞춰 퍼즐 갯수 생성 + 크기 + 모양

        //배열 크기 지정
        arrayPuzzlePiece = new GameObject[lineNumber, lineNumber];
        arrayPuzzlePosition = new Vector3[lineNumber, lineNumber];

        //퍼즐이 놓일 기준위치
        Vector3 origin = new Vector3(-originObjFactory.transform.position.x + (originObjFactory.transform.localScale.x * -0.5f) +                   
                                      originObjFactory.transform.localScale.x * 0.5f / lineNumber,
                                     originObjFactory.transform.position.y + (originObjFactory.transform.localScale.y * 0.5f) -
                                     originObjFactory.transform.localScale.y * 0.5f / lineNumber, 
                                     0);

        //  - 3 line 퍼즐이니 3x3 = 9 개 생성할 필요
        for (int i = 0; i < lineNumber * lineNumber; i++) 
        {
            //퍼즐 생성
            GameObject puzzlePiece = Instantiate(originObjFactory);

            //부모 지정하기
            puzzlePiece.transform.parent = transform;

            //생성 순서에 맞게 이름을 지정
            puzzlePiece.transform.name = i.ToString();

            //생성한 퍼즐을 2차원 배열에 넣기
            //행
            int line = i / lineNumber;
            //렬
            int row = i % lineNumber;

            //퍼즐
            arrayPuzzlePiece[line, row] = puzzlePiece;
            arrayPuzzlePosition[line, row] = origin + (new Vector3(originObjFactory.transform.localScale.x * row / lineNumber,
                                                                    originObjFactory.transform.localScale.y * -line / lineNumber,
                                                                    0));

            arrayPuzzlePiece[line, row].transform.position = arrayPuzzlePosition[line, row];

            //  - 크기가 가로, 세로가 1/3로 줄어들 필요
            puzzlePiece.transform.localScale = 
                new Vector3(puzzlePiece.transform.localScale.x / (float)lineNumber, puzzlePiece.transform.localScale.y / (float)lineNumber, 1);

            //퍼즐 이미지 조정
            Material myMat = puzzlePiece.GetComponent<MeshRenderer>().material;
            //Tiling 조절
            myMat.mainTextureScale = new Vector2(1f / lineNumber, 1f / lineNumber);
            //offset조절
            myMat.mainTextureOffset = new Vector2(row * 1f / lineNumber, 1 - ((line + 1) * 1f / lineNumber));
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
