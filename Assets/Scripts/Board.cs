using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	wait,
	move
}

public class Board : MonoBehaviour {

	
	public GameState currentState = GameState.move;
	public int width;
	public int height;
	public int offSet;
	public GameObject tilePrefab;
	public GameObject[] paints;
	public GameObject destroyEffect;
	private BackgroundTile[,] allTiles;
	public GameObject[,] allPaints;
	private FindMatches findMatches;

	// Use this for initialization
	void Start () {
		findMatches = FindObjectOfType<FindMatches>();
		//tell how big the grid should be
		allTiles = new BackgroundTile[width, height];
		allPaints = new GameObject[width, height];
		SetUp();
	}
	
	private void SetUp(){
		for (int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				Vector2 tempPosition = new Vector2(i, j + offSet);
				GameObject backgroundTile = Instantiate(tilePrefab,tempPosition,Quaternion.identity) as GameObject;
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
	private bool MatchesAt(int column, int row,GameObject piece)
	{
		if(column > 1 && row > 1)
		{
			if (allPaints[column - 1, row].tag == piece.tag && allPaints[column - 2, row].tag == piece.tag)
			{
				return true;
			}
			if (allPaints[column, row-1].tag == piece.tag && allPaints[column, row-2].tag == piece.tag)
			{
				return true;
			}
		}else if(column <= 1 || row <= 1)
		{
			if (row > 1)
			{
				if (allPaints[column, row - 1].tag == piece.tag && allPaints[column, row - 2].tag == piece.tag)
				{
					return true;
				}
			}
			if (column > 1)
			{
				if (allPaints[column - 1, row].tag == piece.tag && allPaints[column - 2, row].tag == piece.tag)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void DestroyMatchesAt(int column, int row)
	{
		if (allPaints[column, row].GetComponent<Paint>().isMatched)
		{
			findMatches.currentMatches.Remove(allPaints[column, row]);
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
		StartCoroutine(DecreaseRowCo());
	}

	private IEnumerator DecreaseRowCo()
	{
		int nullCount = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j<height; j++)
			{
				if(allPaints[i,j] == null)
				{
					nullCount++;
				} else if(nullCount>0){
					allPaints[i, j].GetComponent<Paint>().row -= nullCount;
					allPaints[i, j] = null;
				}
			}
			nullCount = 0;
		}
		yield return new WaitForSeconds(.4f);
		StartCoroutine(FillBoardCo());

	}
	//creating new paints
	private void RefillBoard()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j=0; j < height; j++)
			{
				if (allPaints[i, j] == null)
				{
					Vector2 tempPosition = new Vector2(i, j + offSet);
					int paintToUse = Random.Range(0, paints.Length);
					GameObject piece = Instantiate(paints[paintToUse], tempPosition, Quaternion.identity);
					allPaints[i, j] = piece;
					piece.GetComponent<Paint>().row = j;
					piece.GetComponent<Paint>().column = i;
					piece.transform.parent = this.transform;
					piece.name = "( " + i + ", " + j + " )";
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
		Debug.Log("RefillBoard");
		yield return new WaitForSeconds(.3f);
		int maxChecks = 0;
		Debug.Log("MatchesAt on board is" + MatchesOnBoard());
		while (MatchesOnBoard() && maxChecks < 100)
		{
			Debug.Log("maxChecks" + maxChecks); 
			yield return new WaitForSeconds(.2f);
			DestroyMatches();
			maxChecks++;
			
		}
		yield return new WaitForSeconds(.5f);
		currentState = GameState.move;
	}

}
