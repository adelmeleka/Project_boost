using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // levels in game
    int LEVELS = 2;

    //refrence rigid body component for the same object(rocket)
    Rigidbody rigidBody;
    
    //refrence audio source component 
    AudioSource audioSource;

    [SerializeField] float levelLoadDelay = 1f;

    //serialize field to able to be changed from unity with a defualt value 100
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 5f;
    //multiple sounds to a clip
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    // trigger paricle on rocket
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;



    //for handling delays between level switching
    enum State { Alive, Dying, Transcending}
    State state = State.Alive; 


    // Start is called before the first frame update
    void Start()
    {
        //get the current rigid body component
        rigidBody = GetComponent<Rigidbody>();
        //extract audio source from it
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    //handling input must be done every frame update
    void Update()
    {
        //apply all the time for all frames
        //enabled controlling ship during alive state only
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }        

    }
    // handling any type of collisions
    void OnCollisionEnter(Collision collision)
    {
        print("Collided !");

        // in order not to execute when died or transcending
        if (state != State.Alive) return;

        //switch btween many collided objects types (tags)
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("Ok");
                break;

            case "Finish":
                print("Hit Finish");
                StartSuccessSequence();
                break;

            default:
                print("Dead");
                StartDeathSequence();
                break;
        } 
        
    }
    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // parameterise time
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("ReloadCurrentLevel", levelLoadDelay); // parameterise time
    }

    private void ReloadCurrentLevel()
    {
        //// start level sound
        //if (!m_MyAudioSource.isPlaying) //so it doen't layer
        //    m_MyAudioSource.PlayOneShot(LevelLoadSound);
            

        //when dead, navigate to firt level (reset to level 1 ALWAYS)
        //SceneManager.LoadScene(0);
        //when dead, navigate to current level (reset to same level)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        //SceneManager.LoadScene(1); // todo allow for more than 2 levels
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1)%LEVELS);
    }

    private void RespondToThrustInput()
    {
        // thrust while rotating -> 2 if stateemnts
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        //print("Thrusting");

        //add relative(to object's local coordinates) force
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        //rigidBody.AddRelativeForce(Vector3.up * mainThrust);

        if (!audioSource.isPlaying) //so it doen't layer
            audioSource.PlayOneShot(mainEngine);

        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;    //take manual control of rotation

        //rotation is calculated wrt to each frame
        float roationThisFrame = rcsThrust * Time.deltaTime;

        //cannot rotate left and right at the same time -> if and else if
        //if (Input.GetKey(KeyCode.A))
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //print("Rotating Right");
            //transform component no need to make for him a variable
            transform.Rotate(Vector3.forward * roationThisFrame);

        }
        //else if (Input.GetKey(KeyCode.D))
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //print("Rotating Left");
            transform.Rotate(-Vector3.forward * roationThisFrame);
        }


        rigidBody.freezeRotation = false;    //resume physics control of rotation
    }


}
