using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public int width;
	public int height;
	public GameObject tilePrefab;
	public GameObject[] paints;
	private BackgroundTile[,] allTiles;
	public GameObject[,] allPaints;

	// Use this for initialization
	void Start () {
		//tell how big the grid should be
		allTiles = new BackgroundTile[width, height];
		allPaints = new GameObject[width, height];
		SetUp();
	}
	
	private void SetUp(){
		for (int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				Vector2 tempPosition = new Vector2(i, j);
				GameObject backgroundTile = Instantiate(tilePrefab,tempPosition,Quaternion.identity) as GameObject;
				backgroundTile.transform.parent = this.transform;
				backgroundTile.name = "( " + i + ", " + j + " )";
				int paintToUse = Random.Range(0, paints.Length);
				GameObject paint = Instantiate(paints[paintToUse], tempPosition, Quaternion.identity);
				paint.transform.parent = this.transform;
				paint.name = "( " + i + ", " + j + " )";
				allPaints[i, j] = paint;

			}
		}
	}
	private void DestroyMatchesAt(int column, int row)
	{
		if (allPaints[column, row].GetComponent<Paint>().isMatched)
		{
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

	private void RefillBoard()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j=0; j<height; j++)
			{
				if (allPaints[i, j] == null)
				{
					Vector2 tempPosition = new Vector2(i, j);
					int paintToUse = Random.Range(0, paints.Length);
					GameObject piece = Instantiate(paints[paintToUse], tempPosition, Quaternion.identity);
					allPaints[i, j] = piece;
				}
			}
		}
	}

	private IEnumerator FillBoardCo()
	{
		RefillBoard();
		yield return new WaitForSeconds(.5f);
	}

}
