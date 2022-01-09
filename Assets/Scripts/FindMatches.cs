using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour {

	private Board board;
	public List<GameObject> currentMatches = new List<GameObject>();

	// Use this for initialization
	void Start () {
		board = FindObjectOfType<Board>();
	}
	
public void FindAllMatches()
	{
		StartCoroutine(FindAllMatchesCo());
	}

private void AddToListAndMatch(GameObject paint){
	if (!currentMatches.Contains(paint))
	{
		currentMatches.Add(paint);
	}
	paint.GetComponent<Paint>().isMatched = true;
}

private void GetNearbyPieces(GameObject paint1, GameObject paint2, GameObject paint3)
{
	AddToListAndMatch(paint1);
	AddToListAndMatch(paint2);
	AddToListAndMatch(paint3);
}
private IEnumerator FindAllMatchesCo()
	{
		yield return new WaitForSeconds (.2f);
		//Debug.Log("FindAll Matches Co wait back to normal 2");
		for (int i = 0; i < board.width; i++)
		{
			for(int j = 0; j < board.height; j++)
			{
				GameObject currentPaint = board.allPaints[i, j];
				if(currentPaint != null)
				{
					//for horizontal
					if (i > 0 && i < board.width - 1)
					{
						//directly to left
						GameObject leftPaint = board.allPaints[i - 1, j];
						//directly to right
						GameObject rightPaint = board.allPaints[i + 1, j];
						if (leftPaint != null && rightPaint != null) 
						{
							//Debug.Log("Check tags in Find all Matches Co");
							if(leftPaint.tag == currentPaint.tag && rightPaint.tag == currentPaint.tag)
							{
								if(currentPaint.GetComponent<Paint>().isRowBomb 
									|| leftPaint.GetComponent<Paint>().isRowBomb
									|| rightPaint.GetComponent<Paint>().isRowBomb)
								{
									currentMatches.Union(GetRowPieces(j));
								}

								if (currentPaint.GetComponent<Paint>().isColumnBomb)
								{
									currentMatches.Union(GetColumnPieces(i));
								}
								if (leftPaint.GetComponent<Paint>().isColumnBomb)
								{
									currentMatches.Union(GetColumnPieces(i - 1));
								}
								if (rightPaint.GetComponent<Paint>().isColumnBomb)
								{
									currentMatches.Union(GetColumnPieces(i + 1));
								}


								GetNearbyPieces(leftPaint, currentPaint, rightPaint);
				
							}
						}
					}
					//for vertical
					if (j > 0 && j < board.height - 1)
					{
						//directly below
						GameObject downPaint = board.allPaints[i, j - 1];
						//directly above
						GameObject upPaint = board.allPaints[i, j + 1];
						if (downPaint != null && upPaint != null)
						{

							if (downPaint.tag == currentPaint.tag && upPaint.tag == currentPaint.tag)
							{
								if (currentPaint.GetComponent<Paint>().isColumnBomb
									|| upPaint.GetComponent<Paint>().isColumnBomb
									|| downPaint.GetComponent<Paint>().isColumnBomb)
								{
									currentMatches.Union(GetColumnPieces(i));
								}
								if (currentPaint.GetComponent<Paint>().isRowBomb)
								{
									currentMatches.Union(GetRowPieces(j));
								}
								if (upPaint.GetComponent<Paint>().isRowBomb)
								{
									currentMatches.Union(GetRowPieces(j+1));
								}
								if (downPaint.GetComponent<Paint>().isRowBomb)
								{
									currentMatches.Union(GetRowPieces(j-1));
								}
								GetNearbyPieces(upPaint, currentPaint, downPaint);
							}
						}
					}
				}
			}
		}
	}

	public void MatchPiecesOfcolor(string color)
	{

	}

	List<GameObject> GetColumnPieces (int column)
	{
		List<GameObject> paints = new List<GameObject>();
		for(int i = 0; i < board.height; i++)
		{
			if(board.allPaints[column, i] != null)
			{
				paints.Add(board.allPaints[column, i]);
				board.allPaints[column, i].GetComponent<Paint>().isMatched = true;
			}
		}

		return paints;
	}
	List<GameObject> GetRowPieces(int row)
	{
		List<GameObject> paints = new List<GameObject>();
		for (int i = 0; i < board.width; i++)
		{
			if (board.allPaints[i, row] != null)
			{
				paints.Add(board.allPaints[i, row]);
				board.allPaints[i, row].GetComponent<Paint>().isMatched = true;
			}
		}

		return paints;
	}

	public void CheckBombs()
	{
		//Did the player move something?
		if(board.currentPaint != null)
		{
			//is the piece they move a match?
			if (board.currentPaint.isMatched)
			{
				//make it unmatched so it doesn't get distroyed
				board.currentPaint.isMatched = false;
				//randomly decide what kind of bomb to make
				/*int typeOfBomb = Random.Range(1, 100);
				if(typeOfBomb < 50)
				{
					//make a row bomb
					board.currentPaint.MakeRowBomb();
				} else if (typeOfBomb >= 50)
				{
					//make a column bomb
					board.currentPaint.MakeColumnBomb();
				}
				*/
				if((board.currentPaint.swipeAngle > -45 && board.currentPaint.swipeAngle <= 45)
					|| (board.currentPaint.swipeAngle < -135 || board.currentPaint.swipeAngle >= 135))
				{
					board.currentPaint.MakeRowBomb();
				} else
				{
					board.currentPaint.MakeColumnBomb();
				}

			}
			//is the other piece matched?
			else if (board.currentPaint.otherPaint != null)
			{
				Paint otherPaint = board.currentPaint.otherPaint.GetComponent<Paint>();
				if (otherPaint.isMatched)
				{
					//Make it unmatched
					otherPaint.isMatched = false;
					//decide what kind of bomb to make
					/*int typeOfBomb = Random.Range(1, 100);
					if (typeOfBomb < 50)
					{
						//make a row bomb
						otherPaint.MakeRowBomb();
					}
					else if (typeOfBomb >= 50)
					{
						//make a column bomb
						otherPaint.MakeColumnBomb();
					}
					*/
					if ((board.currentPaint.swipeAngle > -45 && board.currentPaint.swipeAngle <= 45)
						|| (board.currentPaint.swipeAngle < -135 || board.currentPaint.swipeAngle >= 135))
					{
						otherPaint.MakeRowBomb();
					}
					else
					{
						otherPaint.MakeColumnBomb();
					}
				}
			}
		}
	}

}
