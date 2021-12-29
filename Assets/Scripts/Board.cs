using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	wait,
	move
}

public enum TileKind
{
	Breakable,
	Blank,
	Normal
}

[System.Serializable]
public class TileType
{
	public int x;
	public int y;
	public TileKind tileKind;
}
public class Board : MonoBehaviour {

	
	public GameState currentState = GameState.move;
	public int width;
	public int height;
	public int offSet;
	public GameObject tilePrefab;
	public GameObject[] paints;
	public GameObject destroyEffect;
	public TileType[] boardLayout;
	private bool[,] blankSpaces;
	public GameObject[,] allPaints;
	private FindMatches findMatches;

	// Use this for initialization
	void Start () {
		findMatches = FindObjectOfType<FindMatches>();
		//tell how big the grid should be
		blankSpaces = new bool[width , height];
		allPaints = new GameObject[width, height];
		SetUp();
	}
	
	public void GenerateBlankSpaces()
	{
		for (int i = 0; i < boardLayout.Length; i++)
		{
			if(boardLayout[i].tileKind == TileKind.Blank)
			{
				blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
			}
		}
	}
	private void SetUp(){
		GenerateBlankSpaces();
		for (int i = 0; i < width; i++){
			for (int j = 0; j < height; j++)
			{
				if (!blankSpaces[i, j])
				{
					Vector2 tempPosition = new Vector2(i, j + offSet);
					GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
					backgroundTile.transform.parent = this.transform;
					backgroundTile.name = "( " + i + ", " + j + " )";
					int paintToUse = Random.Range(0, paints.Length);

					int maxIterations = 0;

					while (MatchesAt(i, j, paints[paintToUse]) && maxIterations < 100)
					{
						paintToUse = Random.Range(0, paints.Length);
						maxIterations++;
					}
					maxIterations = 0;

					GameObject paint = Instantiate(paints[paintToUse], tempPosition, Quaternion.identity);
					paint.GetComponent<Paint>().row = j;
					paint.GetComponent<Paint>().column = i;
					paint.transform.parent = this.transform;
					paint.name = "( " + i + ", " + j + " )";
					allPaints[i, j] = paint;

				}
			}
		}
	}
	private bool MatchesAt(int column, int row,GameObject piece)
	{
		if(column > 1 && row > 1)
		{
			if (allPaints[column - 1, row] != null && allPaints[column - 2, row] != null)
			{
				if (allPaints[column - 1, row].tag == piece.tag && allPaints[column - 2, row].tag == piece.tag)
				{
					return true;
				}
			}

			if (allPaints[column, row - 1] != null && allPaints[column, row - 2] != null)
			{
				if (allPaints[column, row - 1].tag == piece.tag && allPaints[column, row - 2].tag == piece.tag)
				{
					return true;
				}
			}
		}else if(column <= 1 || row <= 1)
		{
			if (row > 1)
			{
				if (allPaints[column, row - 1] != null && allPaints[column, row - 2] != null)
				{
					if (allPaints[column, row - 1].tag == piece.tag && allPaints[column, row - 2].tag == piece.tag)
					{
						return true;
					}
				}
			}
			if (column > 1)
			{
				if (allPaints[column - 1, row] != null && allPaints[column - 2, row] != null)
				{
					if (allPaints[column - 1, row].tag == piece.tag && allPaints[column - 2, row].tag == piece.tag)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void DestroyMatchesAt(int column, int row)
	{
		if (allPaints[column, row].GetComponent<Paint>().isMatched)
		{
			GameObject particle = Instantiate(destroyEffect, allPaints[column, row].transform.position, Quaternion.identity);
			Destroy(particle,.5f);
			Destroy(allPaints[column, row]);
			allPaints[column, row] = null;
		}
	}

	public void DestroyMatches()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				if (allPaints[i,j] != null)
				{
					DestroyMatchesAt(i, j);
				}
			}
		}
		findMatches.currentMatches.Clear();
		StartCoroutine(DecreaseRowCo2());
	}
	private IEnumerator DecreaseRowCo2()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				//if current spot isn't blank and is empty
				if(!blankSpaces[i,j] && allPaints[i,j] == null)
				{
					//Debug.Log("not blank space but null");
					//loop from space above to the top of the column
					for(int k = j + 1; k < height; k ++)
					{
						//Debug.Log("paint found");
						//if paint is found 
						if (allPaints[i, k] != null)
						{
							//Debug.Log("move dot to empty space");
							//move dot to this empty space
							allPaints[i, k].GetComponent<Paint>().row = j;
							//set that spot to be null
							allPaints[i, k] = null;
							break;
						}

					}
				}
			}
		}
		yield return new WaitForSeconds(.4f);
		StartCoroutine(FillBoardCo());
	}
	/*
	private IEnumerator DecreaseRowCo()
	{
		int nullCount = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if(allPaints[i,j] == null)
				{
					//Debug.Log("Allpaints list is null");
					nullCount++;
				} else if(nullCount > 0){
					//Debug.Log("Allpaints has more than 1 null");
					allPaints[i, j].GetComponent<Paint>().row -= nullCount;
					allPaints[i, j] = null;
				}
			}
			nullCount = 0;
		}
		yield return new WaitForSeconds(.4f);
		//Debug.Log("Decrease Row Co wait .4f increased from 4");
		StartCoroutine(FillBoardCo());

	}
	*/
	//creating new paints
	private void RefillBoard()
	{
		Debug.Log("RefillBoard"); 
		for (int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				if (!blankSpaces[i , j])
				{
					if (allPaints[i, j] == null)
					{
						Debug.Log("Need to fill space");
						Vector2 tempPosition = new Vector2(i, j + offSet);
						int paintToUse = Random.Range(0, paints.Length);
						GameObject piece = Instantiate(paints[paintToUse], tempPosition, Quaternion.identity);
						allPaints[i, j] = piece;
						piece.GetComponent<Paint>().row = j;
						piece.GetComponent<Paint>().column = i;
						piece.transform.parent = this.transform;
						piece.name = "( " + i + ", " + j + " )";
					}
					else
					{
						//Debug.Log("Update name of all paints");
						allPaints[i, j].GetComponent<Paint>().row = j;
						allPaints[i, j].GetComponent<Paint>().column = i;
						allPaints[i, j].transform.parent = this.transform;
						allPaints[i, j].name = "( " + i + ", " + j + " )";
					}
					
				}

			}
		}
	}

	//if there are any matches on the board it will return true
	private bool MatchesOnBoard()
	{
		for (int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				if(allPaints[i, j] != null)
				{
					//Debug.Log(allPaints[i, j].GetComponent<Paint>().isMatched + " isMatches in MatchesOnBoard");
					if (allPaints[i, j].GetComponent<Paint>().isMatched)
					{
						Debug.Log("MatchesAt found after Refill");
						return true;
					}
				}
			}
		}
		return false;
	}

	//we wait a bit then check if there are matches
	private IEnumerator FillBoardCo()
	{
		RefillBoard();
		yield return new WaitForSeconds(.5f);
		Debug.Log("Fill board Co wait .5f btn");
		findMatches.FindAllMatches();
		yield return new WaitForSeconds(.5f);
		Debug.Log("Find all matches wait .5f btn");
		int maxChecks = 0;
		//Debug.Log("MatchesAt on board is" + MatchesOnBoard());
		while (MatchesOnBoard() && maxChecks < 100)
		{
			//Debug.Log("maxChecks" + maxChecks); 
			yield return new WaitForSeconds(.3f);
			//Debug.Log("Checking matches wait .3f from 2");
			DestroyMatches();
			maxChecks++;
			
		}
		yield return new WaitForSeconds(.5f);
		//Debug.Log("FillBoard Co wait to move .5f");
		currentState = GameState.move;
	}

}
