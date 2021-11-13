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
					if (i>0 && i < board.width - 1)
					{
						GameObject leftPaint = board.allPaints[i - 1, j];
						GameObject rightPaint = board.allPaints[i + 1, j];
						if (leftPaint != null && rightPaint != null)
						{
							if(leftPaint.tag == currentPaint.tag && rightPaint.tag == currentPaint.tag)
							{
								leftPaint.GetComponent<Paint>().isMatched = true;
								rightPaint.GetComponent<Paint>().isMatched = true;
								currentPaint.GetComponent<Paint>().isMatched = true;
							}
						}
					}
					if (j > 0 && j < board.height - 1)
					{
						GameObject downPaint = board.allPaints[i, j - 1];
						GameObject upPaint = board.allPaints[i, j + 1];
						if (downPaint != null && upPaint != null)
						{
							if (downPaint.tag == currentPaint.tag && upPaint.tag == currentPaint.tag)
							{
								downPaint.GetComponent<Paint>().isMatched = true;
								upPaint.GetComponent<Paint>().isMatched = true;
								currentPaint.GetComponent<Paint>().isMatched = true;
							}
						}
					}
				}
			}
		}
	}

}
