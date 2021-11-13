using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour {

	[Header("Board Variables")]
	public int column;
	public int row;
	public int previousColumn;
	public int previousRow;
	public int targetX;
	public int targetY;
	public bool isMatched = false;

	private Board board;
	private GameObject otherPaint;
	private Vector2 firstTouchPosition;
	private Vector2 finalTouchPosition;
	private Vector2 tempPosition;
	public float swipeAngle = 0;
	public float swipeResist = 1f;
	// Use this for initialization
	void Start () {
		board = FindObjectOfType<Board>();
		targetX = (int)transform.position.x;
		targetY = (int)transform.position.y;
		row = targetY;
		column = targetX;
		previousRow = row;
		previousColumn = column;
	}
	
	// Update is called once per frame
	void Update () {
		FindMatches();
		if (isMatched)
		{
			SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
			mySprite.color = new Color(1f, 1f, 1f, .2f);
		}

		//After we change the column position in MovePeices we Update the position actually here
		targetX = column;
		targetY = row;
		if (Mathf.Abs(targetX - transform.position.x) > .1)
		{
			//Move towards the target
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
			if (board.allPaints[column, row] != this.gameObject)
			{
				board.allPaints[column, row] = this.gameObject;

			}
		} else
		{
			//Directly set the position
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = tempPosition;
		}
		if (Mathf.Abs(targetY - transform.position.y) > .1)
		{
			//Move towards the target
			tempPosition = new Vector2(transform.position.x, targetY);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
			if (board.allPaints[column, row] != this.gameObject)
			{
				board.allPaints[column, row] = this.gameObject;
			}
		}
		else
		{
			//Directly set the position
			tempPosition = new Vector2(transform.position.x, targetY);
			transform.position = tempPosition;
		}

	}

	private void OnMouseDown(){
		firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.Log(firstTouchPosition);
	}
	private void OnMouseUp()
	{
		finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		CalculateAngle();
	}

	void CalculateAngle()
	{
		if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
		{
			swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
			//Debug.Log(swipeAngle);
			MovePieces();

		}
	}

	void MovePieces()
	{
		previousRow = row;
		previousColumn = column;
		if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
		{
			//Right Swipe
			otherPaint = board.allPaints[column + 1, row];
			otherPaint.GetComponent<Paint>().column -= 1;
			column += 1;
		} else if (swipeAngle > 45 && swipeAngle <= 134 && row < board.height - 1)
		{
			//Up Swipe
			otherPaint = board.allPaints[column, row + 1];
			otherPaint.GetComponent<Paint>().row -= 1;
			row += 1;
		} else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
		{
			//Left Swipe
			otherPaint = board.allPaints[column - 1, row];
			otherPaint.GetComponent<Paint>().column += 1;
			column -= 1;
		} else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
			{
				//Down Swipe
				otherPaint = board.allPaints[column, row - 1];
				otherPaint.GetComponent<Paint>().row += 1;
				row -= 1;
			}
				
		StartCoroutine(CheckMoveCo());


	}

	public IEnumerator CheckMoveCo()
	{
		yield return new WaitForSeconds(.5f);
		if(otherPaint != null)
		{
			if(!isMatched && !otherPaint.GetComponent<Paint>().isMatched)
			{
				otherPaint.GetComponent<Paint>().row = row;
				otherPaint.GetComponent<Paint>().column = column;
				row = previousRow;
				column = previousColumn;
			}
			else
			{
				board.DestroyMatches();
			}
			otherPaint = null;
		}
		
	}

	void FindMatches()
	{
		if(column > 0 && column < board.width - 1)
		{
			GameObject leftPaint1 = board.allPaints[column - 1, row];
			GameObject rightPaint1 = board.allPaints[column + 1, row];
			if (leftPaint1 != null && rightPaint1 != null)
			{
				if (leftPaint1.tag == this.gameObject.tag && rightPaint1.tag == this.gameObject.tag)
				{
					//mathed horizontally
					leftPaint1.GetComponent<Paint>().isMatched = true;
					rightPaint1.GetComponent<Paint>().isMatched = true;
					isMatched = true;
				}
			}
		}
		if (row > 0 && row < board.height - 1)
		{
			GameObject upPaint1 = board.allPaints[column, row + 1];
			GameObject downPaint1 = board.allPaints[column, row - 1];
			if (upPaint1 != null && downPaint1 != null)
			{
				if (upPaint1.tag == this.gameObject.tag && downPaint1.tag == this.gameObject.tag)
				{
					//mathed horizontally
					upPaint1.GetComponent<Paint>().isMatched = true;
					downPaint1.GetComponent<Paint>().isMatched = true;
					isMatched = true;
				}
			}
		}
	}

}
