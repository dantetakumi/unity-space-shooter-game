using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero      S { get; private set; }  // Singleton property   
 
     [Header("Inscribed")]
     // These fields control the movement of the ship
     public float            speed = 30;
     public float            rollMult = -45;
     public float            pitchMult = 30;
     public GameObject projectilePrefab;
     public float      projectileSpeed = 40;
 
     [Header("Dynamic")] [Range(0,4)] [SerializeField]                                   
     private float _shieldLevel = 1;  // Remember the underscore 
     [Tooltip( "This field holds a reference to the last triggering GameObject" )]
     private GameObject lastTriggerGo = null;
 

     void Awake() {
         if (S == null) {
             S = this; // Set the Singleton only if it’s null               
         } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
     }
 
     void Update () {
         // Pull in information from the Input class
         float hAxis = Input.GetAxis("Horizontal");                         
         float vAxis = Input.GetAxis("Vertical");                         
 
         // Change transform.position based on the axes
         Vector3 pos = transform.position;
         pos.x += hAxis * speed * Time.deltaTime;
         pos.y += vAxis * speed * Time.deltaTime;
         transform.position = pos;
 
         // Rotate the ship to make it feel more dynamic                       
         transform.rotation = Quaternion.Euler(vAxis*pitchMult,hAxis*rollMult,0);

        // Allow the ship to fire
         if ( Input.GetKeyDown( KeyCode.Space ) ) {                       
             TempFire();
         }
     }
     
    void TempFire() {                                                        
         GameObject projGO = Instantiate<GameObject>( projectilePrefab );
         projGO.transform.position = transform.position;
         Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
         rigidB.velocity = Vector3.up * projectileSpeed;
     }

    void OnTriggerEnter(Collider other) {
         Transform rootT = other.gameObject.transform.root;                   
         GameObject go = rootT.gameObject;

         // Make sure it’s not the same triggering go as last time
         if ( go == lastTriggerGo ) return;                                   
         lastTriggerGo = go;                                                 
 
         Enemy enemy = go.GetComponent<Enemy>();                             
         if (enemy != null) {  // If the shield was triggered by an enemy
             shieldLevel--;        // Decrease the level of the shield by 1
             Destroy(go);          // … and Destroy the enemy                 
         } else {
             Debug.LogWarning("Shield trigger hit by non-Enemy: "+go.name);   
         }
     }
     public float shieldLevel {
         get { return ( _shieldLevel ); }                                      
         private set {                                                         
             _shieldLevel = Mathf.Min( value, 4 );                            
             // If the shield is going to be set to less than zero…
             if (value < 0) {                                              
                 Destroy(this.gameObject);  // Destroy the Hero
                Main.HERO_DIED();
            }
         }
     }
 }
