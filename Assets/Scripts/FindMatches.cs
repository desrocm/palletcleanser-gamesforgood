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
	private IEnumerator FindAllMatchesCo()
	{
		yield return new WaitForSeconds (.2f);
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
								if (!currentMatches.Contains(leftPaint))
								{
									currentMatches.Add(leftPaint);
								}

								//Debug.Log("left paint" + leftPaint);
								leftPaint.GetComponent<Paint>().isMatched = true;
								if (!currentMatches.Contains(rightPaint))
								{
									currentMatches.Add(rightPaint);
								}
								rightPaint.GetComponent<Paint>().isMatched = true;
								if (!currentMatches.Contains(currentPaint))
								{
									currentMatches.Add(currentPaint);
								}
								currentPaint.GetComponent<Paint>().isMatched = true;
								//Debug.Log("horizontal isMatched");
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
								if (!currentMatches.Contains(downPaint))
								{
									currentMatches.Add(downPaint);
								}
								downPaint.GetComponent<Paint>().isMatched = true;
								if (!currentMatches.Contains(upPaint))
								{
									currentMatches.Add(upPaint);
								}
								upPaint.GetComponent<Paint>().isMatched = true;
								if (!currentMatches.Contains(currentPaint))
								{
									currentMatches.Add(currentPaint);
								}
								currentPaint.GetComponent<Paint>().isMatched = true;
								//Debug.Log("vertical isMatched");
							}
						}
					}
				}
			}
		}
	}

}
