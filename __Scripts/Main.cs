using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;   // Enables the loading & reloading of scenes

public class Main : MonoBehaviour
{
    static private Main S;                        // A private singleton for Main
 
     [Header("Inscribed")]
     public GameObject[]  prefabEnemies;               // Array of Enemy prefabs
     public float         enemySpawnPerSecond = 0.5f;  // # Enemies spawned/second
     public float         enemyInsetDefault = 1.5f;    // Inset from the sides
     public float         gameRestartDelay = 2; 
 
     private BoundsCheck  bndCheck;
 
     void Awake() {
         S = this;
         // Set bndCheck to reference the BoundsCheck component on this 
         // GameObject
         bndCheck = GetComponent<BoundsCheck>();
 
         // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
         Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );                
     }
 
     public void SpawnEnemy() {
         // Pick a random Enemy prefab to instantiate
         int ndx = Random.Range(0, prefabEnemies.Length);                  
         GameObject go = Instantiate<GameObject>( prefabEnemies[ ndx ] );    
 
         // Position the Enemy above the screen with a random x position
         float enemyInset = enemyInsetDefault;                               
         if (go.GetComponent<BoundsCheck>() != null) {                        
             enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius );
         }
 
         // Set the initial position for the spawned Enemy                    
         Vector3 pos = Vector3.zero;                      
         float xMin = -bndCheck.camWidth + enemyInset;
         float xMax =  bndCheck.camWidth - enemyInset;
         pos.x = Random.Range( xMin, xMax );
         pos.y = bndCheck.camHeight + enemyInset;
         go.transform.position = pos;
 
         // Invoke SpawnEnemy() again
         Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );                
     }

    void DelayedRestart() {                                                   
         // Invoke the Restart() method in gameRestartDelay seconds
         Invoke( nameof(Restart), gameRestartDelay );                    
     }
 
    void Restart() {
         // Reload __Scene_0 to restart the game
         // "__Scene_0" below starts with 2 underscores and ends with a zero.
         SceneManager.LoadScene( "__Scene_0" );                              
     }

    static public void HERO_DIED() {
         S.DelayedRestart();                                                  
     }
 }
