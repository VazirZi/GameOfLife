using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Cell;
    [SerializeField] private GameObject ParentForCells;
    private SpriteRenderer[ , ] CellsArray;
    private bool[ , ] BoolCellsArray;
    private Color[] ColorsArray;
    private Vector3 StartPointForCells, EndPointForCells;
    private Vector2 SizeOfScreen;
    private bool playButtonDown, pauseButtonDown;
    private int numberOfRows, numberOfColumns, countRedCells;
    private float indentationBetweenCells, cellWidth, cellHeight;
    private System.Random rand;

    private void Awake()
    {
        rand = new System.Random();

        EndPointForCells = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        SizeOfScreen = new Vector2(EndPointForCells.x * 2, EndPointForCells.y * 2);

        indentationBetweenCells = 0.01f;
        float CellSizeAndIndent = 0.11f;

        float screenWidth = SizeOfScreen.x;
        float screenHeight = SizeOfScreen.y - 1f;

        numberOfRows = (int)(screenHeight / CellSizeAndIndent);
        numberOfColumns = (int)(screenWidth / CellSizeAndIndent);

        float remainOfHeightScreen = screenHeight - (CellSizeAndIndent * numberOfRows);
        float remainOfWidthScreen = screenWidth - (CellSizeAndIndent * numberOfColumns);

        cellWidth = (remainOfWidthScreen / numberOfColumns) + CellSizeAndIndent - indentationBetweenCells;
        cellHeight = (remainOfHeightScreen / numberOfRows) + CellSizeAndIndent - indentationBetweenCells;

        Cell.GetComponent<Transform>().localScale = new Vector3(cellWidth, cellHeight, 0);
        StartPointForCells = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0)).origin;

        CellsArray = new SpriteRenderer[numberOfRows, numberOfColumns];
        BoolCellsArray = new bool[numberOfRows, numberOfColumns];
        ColorsArray = new Color[2] {Color.red, Color.white};

        pauseButtonDown = true;
        
        CreateGameField();
    }
    
    private void CreateGameField()
    {
        float stepX = 0;
        float stepY = 0;
        float targetStepX = cellWidth + indentationBetweenCells;
        float targetStepY = cellHeight + indentationBetweenCells;

        for (int i = 0; i < CellsArray.GetLength(0); i++, stepY += targetStepY)
        {
            for (int j = 0; j < CellsArray.GetLength(1); j++, stepX += targetStepX)
            {
                Cell.gameObject.name = $"square";
                CellsArray[i, j] = Instantiate(Cell, new Vector3((StartPointForCells.x + (cellWidth / 2f) + (indentationBetweenCells / 2f)) + stepX, (StartPointForCells.y + (cellHeight / 2f) + (indentationBetweenCells / 2f) + 1f) + stepY, 0), new Quaternion(0, 0, 0, 0));
                CellsArray[i, j].GetComponent<SpriteRenderer>().color = ColorsArray[rand.Next(0, 2)];
                CellsArray[i, j].transform.parent = ParentForCells.transform;
            }
            stepX = 0;
        }
    }

    private void Update()
    {
        if (playButtonDown)
        { 
            StartingPlay();
        }
    }

    private async void StartingPlay()
    {  
        for (int i = 1; i < CellsArray.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < CellsArray.GetLength(1) - 1; j++)
            {
                if (CellWillBorn(i, j) && CellsArray[i, j].GetComponent<SpriteRenderer>().color == Color.white)
                {
                    BoolCellsArray[i, j] = true;
                }
                else if (CellWillDead(i, j) && CellsArray[i, j].GetComponent<SpriteRenderer>().color == Color.red)
                {
                    BoolCellsArray[i, j] = false;
                }
                else if (CellWillSurvives(i, j) && CellsArray[i, j].GetComponent<SpriteRenderer>().color == Color.red)
                {
                    BoolCellsArray[i, j] = true;
                }
            }
        }

        RewriteCellsArrays();

        await Task.Delay(TimeSpan.FromSeconds(0.01));
    }

    private bool CellWillBorn(int i, int j)
    {
        bool willBorn = false;

        countRedCells = RedCellsCounter(i, j);

        if (countRedCells == 3)
        {
            willBorn = true;
        }
        else 
        {
            willBorn = false;
        }
        countRedCells = 0;

        return willBorn;
    }

    private bool CellWillSurvives(int i, int j)
    {
        bool willSurvives = false;
        
        countRedCells = RedCellsCounter(i, j);

        if (countRedCells - 1 == 2 || countRedCells - 1 == 3)
        {
            willSurvives = true;
        }
        else 
        {
            willSurvives = false;
        }
        countRedCells = 0;

        return willSurvives;
    }

    private bool CellWillDead(int i, int j) 
    {
        bool willDead = false;
        
        countRedCells = RedCellsCounter(i, j);

        if (countRedCells - 1 >= 4 || countRedCells - 1 == 1 || countRedCells - 1 == 0)
        {
            willDead = true;
        }
        else 
        {
            willDead = false;
        }
        countRedCells = 0;

        return willDead;
    }

    private int RedCellsCounter(int i, int j)
    {
        int count = 0;

        for (int str = i - 1; str <= i + 1; str++)
        {
            for (int column = j - 1; column <= j + 1; column++)
            {
                if (CellsArray[str, column].GetComponent<SpriteRenderer>().color == Color.red)
                {
                    count++;
                }
            }
        }

        return count;
    }

    protected void RewriteCellsArrays()
    {
        for (int i = 0; i < CellsArray.GetLength(0); i++)
        {
            for (int j = 0; j < CellsArray.GetLength(1); j++)
            {
                if (BoolCellsArray[i, j] == true)
                {
                    CellsArray[i, j].GetComponent<SpriteRenderer>().color = Color.red;
                }
                else 
                {
                    CellsArray[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }

    public void SetPlay()
    {
        if (pauseButtonDown)
        {
            pauseButtonDown = false;
            playButtonDown = true;
        }
    }

    public void SetPause()
    {
        if (playButtonDown)
        {
            playButtonDown = false;
            pauseButtonDown = true;
        }
    }

    public void ClearCellsField()
    {
        for (int i = 0; i < CellsArray.GetLength(0); i++)
        {
            for (int j = 0; j < CellsArray.GetLength(1); j++)
            {
                CellsArray[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                BoolCellsArray[i, j] = false;
            }
        }
    }
    
    public void SetRandomColorOfCells()
    {
        for (int i = 0; i < CellsArray.GetLength(0); i++)
        {
            for (int j = 0; j < CellsArray.GetLength(1); j++)
            {
                CellsArray[i, j].GetComponent<SpriteRenderer>().color = ColorsArray[rand.Next(0, 2)];
            }
        }
    }
}
