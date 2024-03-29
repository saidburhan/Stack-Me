﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    public static TheStack instance;

    public Color32[] gameColors = new Color32[4];
    public Color32[] gameColors2 = new Color32[4];
    public Color32[] gameColors3 = new Color32[4];
    public Color32[] gameColors4 = new Color32[4];
    public Material stackMat;
    private Color32 spawnColor;

    private const float BOUNDS_SIZE = 5F;
    private const float STACK_MOVING_SPEED = 2.5F; // bu aşağı doğru hareket.
    private const float ERROR_MARGIN = 0.15F;
    private const float STACK_BOUNDS_GAIN = 0.25F;
    private const int COMBO_START_GAIN = 3;

    private GameObject[] theStack;

    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);
    private Vector2 oldStackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int scoreCount;
    private int stackIndex = 0;
    private int combo = 0;
    private int colorCount = 11;
    private int taleCount;
    private int chooseColorSet;
    private int setReverse;
    private int reklamSayaci = 0;


    private float tileTransition = 0.0f;
    private float tileSpeed = 1.5f; // bu sağa sola hareket hızı olacak..
    private float secondaryPosition;
    //private float createDistance = 1.5f;

    private bool isMovingOnX = true;
    private bool gameOver = true;
    private bool isRewardAds = true;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;
    private Vector3 rewardTilePosition;

    

	private void Awake()
	{
        if (instance == null) instance = this;
	}


	private void Start()
    {
        taleCount = transform.childCount;
        theStack = new GameObject[taleCount];
        //setReverse = taleCount - 1;
        chooseColorSet = Random.Range(0, 4);
		for (int i = 0; i < taleCount; i++)
		{
           
            theStack[i] = transform.GetChild(i).gameObject;

            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", CreateColor()) ;
            theStack[i].GetComponent<Renderer>().SetPropertyBlock(block);
            //ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
            //setReverse--;
        }	
    }

    private void Update()
    {

        
		if (!gameOver)
		{
           
            if (Input.GetMouseButtonDown(0))
            {
                if (PlaceTile())
                {
                    SpawnTile();
                    scoreCount++;
                    UIControl.instance.setScore(scoreCount);
                }
                else
                {
                    EndGame();
                }
            }
            MoveTile(); // Salınım hareketleri...
        }
		
       

        // Aşağı yönlü hareket...
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);      
    }



	private void CreateRubble(Vector3 pos , Vector3 scale)
	{
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.transform.tag = "rubble";
        go.GetComponent<Rigidbody>().mass = .5f;
        go.GetComponent<Rigidbody>().AddForce(Vector3.down * 100);  
        go.GetComponent<MeshRenderer>().material = stackMat;
        var block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", spawnColor);
        go.GetComponent<Renderer>().SetPropertyBlock(block);
        SoundManager.instance.RubbleSounds();
       
    }

    private void MoveTile()
	{
        if (gameOver)
            return;

        tileTransition += Time.deltaTime * tileSpeed;
		if (isMovingOnX)
            theStack[stackIndex].transform.localPosition =
                new Vector3(Mathf.Cos(tileTransition) * BOUNDS_SIZE * 1.5f, scoreCount, secondaryPosition);
                
        else
            theStack[stackIndex].transform.localPosition =
                new Vector3(secondaryPosition, scoreCount, Mathf.Cos(tileTransition) * BOUNDS_SIZE * 1.5f);

    }

    private void SpawnTile()
	{
        tileTransition = 0;
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0) stackIndex = transform.childCount - 1;
        desiredPosition = (Vector3.down) * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(20, scoreCount, 20);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
        var block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", CreateColor());  
        theStack[stackIndex].GetComponent<Renderer>().SetPropertyBlock(block);
        
    }



    private Color32 CreateColor()
	{
       
        colorCount++;

        float f = Mathf.Abs(Mathf.Sin(colorCount * 0.05f));

        if (chooseColorSet == 0) spawnColor = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        else if (chooseColorSet == 1) spawnColor = Lerp4(gameColors2[0], gameColors2[1], gameColors2[2], gameColors2[3], f);
        else if (chooseColorSet == 2) spawnColor = Lerp4(gameColors3[0], gameColors3[1], gameColors3[2], gameColors3[3], f);
        else if (chooseColorSet == 3) spawnColor = Lerp4(gameColors4[0], gameColors4[1], gameColors4[2], gameColors4[3], f);

        return spawnColor;
	}

 //   private void ColorMesh(Mesh mesh)
	//{
 //       Vector3[] vertices = mesh.vertices;
 //       Color32[] colors = new Color32[vertices.Length];

 //       colorCount++;

 //       float f =Mathf.Abs( Mathf.Sin(colorCount*0.06f));
 //       stackColor = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
 //       Debug.Log(stackColor);
 //       Debug.Log(colorCount);

 //       if (chooseColorSet == 0)
	//	{
 //           for (int i = 0; i < vertices.Length; i++)
 //           {
 //               colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
 //           }
 //       }else if(chooseColorSet == 1)
	//	{
 //           for (int i = 0; i < vertices.Length; i++)
 //           {
 //               colors[i] = Lerp4(gameColors2[0], gameColors2[1], gameColors2[2], gameColors2[3], f);
 //           }
	//	}
	//	else if(chooseColorSet == 2)
	//	{
 //           for (int i = 0; i < vertices.Length; i++)
 //           {
 //               colors[i] = Lerp4(gameColors3[0], gameColors3[1], gameColors3[2], gameColors3[3], f);
 //           }
 //       }
 //       else if (chooseColorSet == 3)
 //       {
 //           for (int i = 0; i < vertices.Length; i++)
 //           {
 //               colors[i] = Lerp4(gameColors4[0], gameColors4[1], gameColors4[2], gameColors4[3], f);
 //           }
 //       }


 //       mesh.colors32 = colors;
 //       oldColors = colors;
       
	//}


    private bool PlaceTile()
	{
        Transform t = theStack[stackIndex].transform;

		if (isMovingOnX)
		{
            

            float deltaX = lastTilePosition.x -  t.position.x;
            
            if(Mathf.Abs(deltaX) > ERROR_MARGIN)
			{
                // CUT THE TILE
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);               
                if (stackBounds.x <= 0) return false;
                
                oldStackBounds.x = stackBounds.x;
                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
                
                CreateRubble(
                     new Vector3( (t.position.x - lastTilePosition.x > 0) 
                     ? t.position.x +(t.localScale.x/2)
                     : t.position.x - (t.localScale.x / 2)
                     , t.position.y
                     , t.position.z)
                     , new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
                    );
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
                rewardTilePosition = t.transform.localPosition;
            }
			else
			{       
                SoundManager.instance.ComboSounds(combo);
                if (PlayerPrefs.GetInt("vibration") == 1) Vibrator.Vibrate(50);
                if (combo > COMBO_START_GAIN)
                {
                    UIControl.instance.BiggerAnim();
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    oldStackBounds.x = stackBounds.x;
                    
                    if (stackBounds.x > BOUNDS_SIZE) stackBounds.x = BOUNDS_SIZE;
                    float middle = lastTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);                 
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
                    rewardTilePosition = t.transform.localPosition;
                }
                combo++;
                UIControl.instance.Combo(combo);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
		}
		else
		{
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                // CUT THE TILE
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0) return false;
               
                oldStackBounds.y = stackBounds.y;
                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(
                     new Vector3(t.position.x
                     , t.position.y
                     , (t.position.z - lastTilePosition.z> 0)
                     ? t.position.z + (t.localScale.z / 2)
                     : t.position.z - (t.localScale.z / 2))
                     , new Vector3(t.localScale.x, 1,  Mathf.Abs(deltaZ))
                    );            
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                rewardTilePosition = t.transform.localPosition;

            }
            else
            {
                SoundManager.instance.ComboSounds(combo);
                //if (PlayerPrefs.GetInt("vibration") == 1) Handheld.Vibrate();
                if (PlayerPrefs.GetInt("vibration") == 1) Vibrator.Vibrate(50);
                if (combo > COMBO_START_GAIN)
                {
                    UIControl.instance.BiggerAnim();
                    stackBounds.y += STACK_BOUNDS_GAIN;     
                    oldStackBounds.y = stackBounds.y;
                    if (stackBounds.y > BOUNDS_SIZE) stackBounds.y = BOUNDS_SIZE;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }
                combo++;
                UIControl.instance.Combo(combo);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
                rewardTilePosition = t.transform.localPosition;
            }
        }

        

        secondaryPosition = (isMovingOnX) 
            ? t.localPosition.x
            : t.localPosition.z;

        isMovingOnX = !isMovingOnX;        
        
        return true; // doğru yerleştiyse bu dönsün.. 
	}

    private void EndGame()
	{
		if (!gameOver)
		{
            if(scoreCount > PlayerPrefs.GetInt("highscore"))
			{
                PlayerPrefs.SetInt("highscore", scoreCount);
                PlayServices.instance.AddScoreToLeaderBoard(scoreCount);
                UIControl.instance.SetHighScore(scoreCount);
			}

            Debug.Log("End Game");
            gameOver = true;
            theStack[stackIndex].AddComponent<Rigidbody>();
            UIControl.instance.OpenMenu();
			if (reklamSayaci >= 3)
			{
				AdsManager.instance.ShowInsterstitial();
				reklamSayaci = 0;
			}
			else if (reklamSayaci <3)
			{
				reklamSayaci++;
			}
			UIControl.instance.RestartButtonActive();
            
        }
      
	}

    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
	{
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);

	}

    public void SetGameOver()
	{
        gameOver = false;
	}

    public void StartGame()
	{
        // burada tüm stackler eski konumuna getirilecek..
        // bg değiştirilecek..
        // stackların renk paleti değiştirilecek...
        for (int i = 0; i < theStack.Length; i++)
        {
            if (theStack[i].GetComponent<Rigidbody>()) Destroy(theStack[i].GetComponent<Rigidbody>());
        }
        colorCount = Random.Range(0, 100); // renklerin her defasında farklı bir yerden başlamasını istiyoruz
        stackIndex = 0;
        scoreCount = 0;
        UIControl.instance.setScore(0);
        combo = 0;
        secondaryPosition = 0;
        DestroyRubbles("rubble");
        desiredPosition = Vector3.zero;
        lastTilePosition = Vector3.zero;
        stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);
        isMovingOnX = true;
        taleCount = transform.childCount;
        setReverse = taleCount - 1;
        gameOver = false;
        isRewardAds = true;
        chooseColorSet = Random.Range(0, 4);
        for (int i = 0; i < taleCount; i++)
        {
            theStack[i].transform.localPosition = new Vector3(0, -i, 0);
            theStack[i].transform.localRotation = new Quaternion(0,0,0,0);
            theStack[i].transform.localScale = new Vector3(BOUNDS_SIZE,1,BOUNDS_SIZE);
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", CreateColor());
            theStack[setReverse].GetComponent<Renderer>().SetPropertyBlock(block);
            setReverse--;
        }
        GradientBg.instance.Start();
        AdsManager.instance.InterstitialReklam(); // bu sadece reklamı hazırlıyor göstermiyor..
        AdsManager.instance.RewardedReklam();
    }

    private void DestroyRubbles(string tagName)
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

    public void RewardedStacks()
	{
		if (gameOver && isRewardAds)
		{

            StartCoroutine(RewardedStackCoroutine());
        }
	}

    IEnumerator RewardedStackCoroutine()
	{
       
        isRewardAds = false;
        UIControl.instance.CloseMenu();
        for(int i = 0; i < theStack.Length; i++)
		{
            if (theStack[i].GetComponent<Rigidbody>()) Destroy(theStack[i].GetComponent<Rigidbody>());
        }
       
        for (int i = 0; i < 5; i++)
        {
            SoundManager.instance.RewardedStacksSound();
            tileTransition = 0;
            //lastTilePosition = theStack[stackIndex].transform.localPosition;
            stackIndex--;
            if (stackIndex < 0) stackIndex = transform.childCount - 1;
            Debug.Log(theStack[stackIndex].transform.localScale);
            desiredPosition = (Vector3.down) * scoreCount;
            theStack[stackIndex].transform.localPosition = new Vector3(rewardTilePosition.x, scoreCount, rewardTilePosition.z);
            if (oldStackBounds.x < 5f) oldStackBounds.x += STACK_BOUNDS_GAIN;
            if (oldStackBounds.y < 5f) oldStackBounds.y += STACK_BOUNDS_GAIN;
            theStack[stackIndex].transform.localScale = new Vector3(oldStackBounds.x, 1, oldStackBounds.y);
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", CreateColor());
            theStack[stackIndex].GetComponent<Renderer>().SetPropertyBlock(block);
            scoreCount++;       
            yield return new WaitForSeconds(0.4f);
        }
       
        stackBounds.x = oldStackBounds.x;
        stackBounds.y = oldStackBounds.y;


		SpawnTile();
		gameOver = false;


	}

    public bool getRewardState()
	{
        return isRewardAds;
	}

}


// RIGIDBODY SİLİNECEKK BİR KONULACAK KÜPLERİN YERİ DÜZENLENECEK VE HERŞEY TAMAM OLACAK İNŞALLAH BAKALIM....
// süreli şekilde çıkması sağlanacask..
// her elde bir defaya mahsus ödüllü reklam ile çıkması sağlanacak..
// geçiş reklamıyla çakıştırmamaya dikkat et.. 

// tam yerine koyma efekti düzenlenecek
// müzikler eklenecek...
// post processing bakalım biraz.. ışık v.s.




