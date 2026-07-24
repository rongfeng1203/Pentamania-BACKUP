using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private EventReference firepitEvent;
    [SerializeField] private EventReference clockTickEvent;


    [Header("Machines")]
    [SerializeField] private EventReference crystalBallAmbience;
    [SerializeField] private EventReference crystalBallUsage;
    public EventInstance crystallBallUsageInstance;

    [SerializeField] private EventReference mortarGrind;

    [SerializeField] private EventReference cauldronStart;
    [SerializeField] private EventReference cauldronSuccess;
    [SerializeField] private EventReference cauldronFailure;
    [SerializeField] private EventReference cauldronTakeIngredient;
    [SerializeField] private EventReference cauldronShaking;
    
    [SerializeField] private EventReference magicCircleAccept;

    [SerializeField] private EventReference bunsenBurnerStart;
    [SerializeField] private EventReference bunsenBurnerStop;
    [SerializeField] private EventReference bunsenBurnerBurn;

    [SerializeField] private EventReference waterPourStart;
    [SerializeField] private EventReference waterPourStop;

    [SerializeField] private EventReference cthuluTentacleSpawn;

    [Header("Items")]
    [SerializeField] private EventReference pickupEvent;
    [SerializeField] private EventReference dropEvent;
    [SerializeField] private EventReference pageFlip;

    [Header("Voicelines")]
    [SerializeField] private EventReference voiceLineManager;
    public EventInstance voiceLineManagerInstance;

    [Header("Music")] 
    [SerializeField] private EventReference musicManager;
    public EventInstance musicManagerInstance;

    public EventInstance pickupInstance;
    public EventInstance dropInstance;


    public static AudioManager instance;

    void Awake()
    {
        // Who up singling their ton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    
    
    public void PlaySound(string sound, GameObject gameobject)
    {
        EventInstance eventInstance;
        switch (sound)
        {
            case "firepit":
                eventInstance = RuntimeManager.CreateInstance(firepitEvent);
                break;
            
            case "cauldron_start":
                eventInstance = RuntimeManager.CreateInstance(cauldronStart);
                break;
            
            case "cauldron_success":
                eventInstance = RuntimeManager.CreateInstance(cauldronSuccess);
                break;
            
            case "cauldron_failure":
                eventInstance = RuntimeManager.CreateInstance(cauldronFailure);
                break;
            
            case "magic_circle_accept":
                eventInstance = RuntimeManager.CreateInstance(magicCircleAccept);
                break;
            
            case "cauldron_pickup":
                eventInstance = RuntimeManager.CreateInstance(cauldronTakeIngredient);
                break;
            
            case "bunsen_start":
                eventInstance = RuntimeManager.CreateInstance(bunsenBurnerStart);
                break;
            
            case "bunsen_stop":
                eventInstance = RuntimeManager.CreateInstance(bunsenBurnerStop);
                break;
            
            case "bunsen_burn":
                eventInstance = RuntimeManager.CreateInstance(bunsenBurnerBurn);
                break;
            
            case "book_page_flip":
                eventInstance = RuntimeManager.CreateInstance(pageFlip);
                break;
            
            case "crystal_ball_ambience":
                eventInstance = RuntimeManager.CreateInstance(crystalBallAmbience);
                break;
            
            case "clock_tick":
                eventInstance = RuntimeManager.CreateInstance(clockTickEvent);
                break;
            
            case "mortar_grind":
                eventInstance = RuntimeManager.CreateInstance(mortarGrind);
                break;

            
            case "water_pour_start":
                eventInstance = RuntimeManager.CreateInstance(waterPourStart);
                break;
            
            case "water_pour_stop":
                eventInstance = RuntimeManager.CreateInstance(waterPourStop);
                break;
            
            case "cthulu_tentacle_spawn":
                eventInstance = RuntimeManager.CreateInstance(cthuluTentacleSpawn);
                break;
            
            case "crystal_ball_usage":
                eventInstance = RuntimeManager.CreateInstance(crystalBallUsage);
                break;
            
            default:
                return;
        }
        
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameobject);
            
        eventInstance.start();
        eventInstance.release();
    }

    public void PlaySound(string sound, string param_name, int param, GameObject gameobject)
    {
        EventInstance eventInstance;
        switch (sound)
        {
            case "crystal_ball_usage":
                eventInstance = RuntimeManager.CreateInstance(crystalBallUsage);
                break;
            
            case "pickup":
                eventInstance = RuntimeManager.CreateInstance(pickupEvent);
                break;
            
            case "drop":
                eventInstance = RuntimeManager.CreateInstance(dropEvent);
                break;

            default:
                return;
        }
        
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameobject);
        eventInstance.setParameterByName(param_name, param);
        
        eventInstance.start();
        eventInstance.release();
    }

    public void PlayMusic(int scene)
    {
        EventInstance musicInstance = RuntimeManager.CreateInstance(musicManager);
        musicInstance.setParameterByName("Scene", scene);
        musicInstance.start();
        musicInstance.release();
    }


    public void PickupIngredient(PassableIngredientObject ingredientObject)
    {
        pickupInstance = RuntimeManager.CreateInstance(pickupEvent);
        RuntimeManager.AttachInstanceToGameObject(pickupInstance, ingredientObject.gameObject);
        print("playing ts pmo " + ingredientObject.GetAudioType());
        pickupInstance.setParameterByName("Item", ingredientObject.GetAudioType());
        pickupInstance.start();
        pickupInstance.release();
    }
    
    public void PickupIngredient(int audioType, GameObject gameObject)
    {
        pickupInstance = RuntimeManager.CreateInstance(pickupEvent);
        RuntimeManager.AttachInstanceToGameObject(pickupInstance, gameObject);
        print("playing ts pmo " + audioType);
        pickupInstance.setParameterByName("Item", audioType);
        pickupInstance.start();
        pickupInstance.release();
    }

    public void DropIngredient(PassableIngredientObject ingredientObject)
    {
        dropInstance = RuntimeManager.CreateInstance(dropEvent);
        RuntimeManager.AttachInstanceToGameObject(dropInstance, ingredientObject.gameObject);
        pickupInstance.setParameterByName("Item", ingredientObject.GetAudioType());
        dropInstance.start();
        dropInstance.release();
        
    }
    
    public void DropIngredient(int audioType, GameObject gameObject)
    {
        dropInstance = RuntimeManager.CreateInstance(dropEvent);
        RuntimeManager.AttachInstanceToGameObject(dropInstance, gameObject);
        pickupInstance.setParameterByName("Item", audioType);
        dropInstance.start();
        dropInstance.release();
        
    }
    
    public void PlayVoiceLine(int voicelineEvent, GameObject gameobject, int potionId = -1)
    {
        voiceLineManagerInstance = RuntimeManager.CreateInstance(voiceLineManager);
        RuntimeManager.AttachInstanceToGameObject(voiceLineManagerInstance, gameobject);
        voiceLineManagerInstance.setParameterByName("Voiceline Event", voicelineEvent);
        
        if (potionId != -1)
        {
            voiceLineManagerInstance.setParameterByName("Potion Brewed", potionId);
        }

        voiceLineManagerInstance.start();
        voiceLineManagerInstance.release();
    }
}
