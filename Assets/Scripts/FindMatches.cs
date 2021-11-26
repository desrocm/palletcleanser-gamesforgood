using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		yield return new WaitForSeconds (.4f);
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

							if(leftPaint.tag == currentPaint.tag && rightPaint.tag == currentPaint.tag)
							{
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
								GetNearbyPieces(upPaint, currentPaint, downPaint);
							}
						}
					}
				}
			}
		}
	}

}
