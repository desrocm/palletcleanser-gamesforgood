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

	private FindMatches findMatches;
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
		findMatches = FindObjectOfType<FindMatches>();
		//targetX = (int)transform.position.x;
		//targetY = (int)transform.position.y;
		//row = targetY;
		//column = targetX;
		//previousRow = row;
		//previousColumn = column;
	}
	
	// Update is called once per frame
	void Update () {
		//FindMatches();
		if (isMatched)
		{
			SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
			mySprite.color = new Color(1f, 1f, 1f, .2f);
			
		}

		//After we change the column position in MovePeices we Update the position actually here
		targetX = column;
		targetY = row;
		//move horizontally
		if (Mathf.Abs(targetX - transform.position.x) > .1)
		{
			//Move towards the target
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
			if (board.allPaints[column, row] != this.gameObject)
			{
				board.allPaints[column, row] = this.gameObject;
			}
			findMatches.FindAllMatches();


		} else
		{
			//Directly set the position
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = tempPosition;
		}
		//move vertically
		if (Mathf.Abs(targetY - transform.position.y) > .1)
		{
			//Move towards the target
			tempPosition = new Vector2(transform.position.x, targetY);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
			if (board.allPaints[column, row] != this.gameObject)
			{
				board.allPaints[column, row] = this.gameObject;
			}
			findMatches.FindAllMatches();


		}
		else
		{
			//Directly set the position
			tempPosition = new Vector2(transform.position.x, targetY);
			transform.position = tempPosition;
		}

	}

	private void OnMouseDown(){
		if (board.currentState == GameState.move)
		{
			firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		//Debug.Log(firstTouchPosition);
	}
	private void OnMouseUp()
	{
		if (board.currentState == GameState.move)
		{
			finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			CalculateAngle();
		}
	}

	void CalculateAngle()
	{
		if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
		{
			board.currentState = GameState.wait; 
			swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
			//Debug.Log(swipeAngle);
			MovePieces();


		}
		else
		{
			board.currentState = GameState.move;

		}
	}

	void MovePiecesActual(Vector2 direction)
	{
		otherPaint = board.allPaints[column + (int)direction.x, row + (int)direction.y];
		previousRow = row;
		previousColumn = column;
		if (otherPaint != null)
		{
			otherPaint.GetComponent<Paint>().column += -1 * (int)direction.x;
			otherPaint.GetComponent<Paint>().row += -1 * (int)direction.y;
			column += (int)direction.x;
			row += (int)direction.y;
			StartCoroutine(CheckMoveCo());
		} else
		{
			board.currentState = GameState.move;
		}
	}
	//calculate direction for the move
	void MovePieces()
	{

		if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
		{
			//Right Swipe
			MovePiecesActual(Vector2.right);
		}
		else if (swipeAngle > 45 && swipeAngle <= 134 && row < board.height - 1)
		{
			//Up Swipe
			MovePiecesActual(Vector2.up);
		}
		else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
		{
			//Left Swipe
			MovePiecesActual(Vector2.left);
		}
		else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
		{
			//Down Swipe
			MovePiecesActual(Vector2.down);
		}
		else
		{
			board.currentState = GameState.move;
		}
	}
		
				
	//Makes sure there is a match there before finishing the move
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
				yield return new WaitForSeconds(.5f);
				board.currentState = GameState.move;
				Debug.Log("Swipe failed");
			}
			else
			{
				Debug.Log("Swipe Made");
				board.DestroyMatches();


			}
			otherPaint = null;
		}
		
	}

}
