using UnityEngine;
using System.Collections;

public class DestructibleWall : MonoBehaviour {
	public WallBrick brickType;
//	public Vector2 wallSize = new Vector2(5.0f, 2.0f);
	
	private WallBrick[,] bricks;
	private int numBricksX;
	private int numBricksY;
	
	public int weakBrickX = -1;
	public int weakBrickY = -1;
	
	// Use this for initialization
	void Start () {
		if(brickType == null)
		{
			Debug.LogError("No brick type for DestructibleWall!");
			enabled = false;
			return;
		}
		
		renderer.enabled = false;
		
		numBricksX = Mathf.RoundToInt(transform.localScale.x / brickType.transform.localScale.x);
		numBricksY = Mathf.RoundToInt(transform.localScale.y / brickType.transform.localScale.y);
		bricks = new WallBrick[numBricksX, numBricksY];
		float objectHeight = brickType.transform.localScale.y;
		float objectWidth = brickType.transform.localScale.x;
		
		float offsetX = transform.localScale.x / 2.0f;
		float offsetY = transform.localScale.y / 2.0f;
		
		bool evenY = true;
		for(int y = 0; y < numBricksY; ++y)
		{
			float heightSoFar = objectHeight * y;
			for(int x = 0; x < numBricksX; ++x)
			{
				float widthSoFar = objectWidth * x;
				if(evenY)
				{
					widthSoFar += objectWidth / 2.0f;
				}
				Vector3 brickPosition = transform.position + transform.TransformDirection(new Vector3(widthSoFar - offsetX + (objectWidth / 2.0f), heightSoFar - offsetY + (objectHeight / 2.0f), 0.0f));
				bricks[x, y] = (WallBrick)Instantiate (brickType, brickPosition, transform.rotation);
				bricks[x, y].SetWall (this);
			}
			evenY = !evenY;
		}
		
		if(weakBrickX >= 0 && weakBrickY >= 0 && weakBrickX < numBricksX && weakBrickY < numBricksY)
		{
			bricks[weakBrickX, weakBrickY].SetIsWeak (true);
		}
	}
	
	public void NotifyBrickPunched(WallBrick brick)
	{
		bool evenY = true;
		for(int y = 0; y < numBricksY; ++y)
		{
			for(int x = 0; x < numBricksX; ++x)
			{
				if(bricks[x, y] == brick)
				{
					// weaken the surrounding bricks
					if(y > 0)
					{
						if(evenY)
						{
							if(x < numBricksX - 1)
							{
								bricks[x + 1, y - 1].SetIsWeak (true);
							}
						}
						else
						{
							if(x > 0)
							{
								bricks[x - 1, y - 1].SetIsWeak (true);
							}
						}
						bricks[x, y - 1].SetIsWeak (true);
					}
					
					if(x > 0)
					{
						bricks[x - 1, y].SetIsWeak (true);
					}
					if(x < numBricksX - 1)
					{
						bricks[x + 1, y].SetIsWeak (true);
					}
					
					if(y < numBricksY - 1)
					{
						if(evenY)
						{
							if(x < numBricksX - 1)
							{
								bricks[x + 1, y + 1].SetIsWeak (true);
							}
						}
						else
						{
							if(x > 0)
							{
								bricks[x - 1, y + 1].SetIsWeak (true);
							}
						}
						bricks[x, y + 1].SetIsWeak (true);
					}
				}
			}
			
			evenY = !evenY;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
