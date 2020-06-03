using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Pooler : MonoBehaviour
{
    public static Pooler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //  DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    UIManager uIManager;
    SoundManager soundManager;
    [Header("SpawnPoints and Fruits")]
    public List<GameObject> FruitPrefabs;
    public List<GameObject> UsedFruit = new List<GameObject>();
    public List<GameObject> UnusedFruit = new List<GameObject>();

    public Transform SpawnPoints;
    List<Vector3> SpawnPointList = new List<Vector3>();

    [Header("FrozenFruit and Explosion")]
    public List<GameObject> FrozenFruit;
    public GameObject explosionBombPrefab;
    public Dictionary<string, List<GameObject>> SpecialFruits = new Dictionary<string, List<GameObject>>();
    public List<GameObject> UsedSpecialFrutisList;


    [Header("Splash Particle Effects")]

    public List<GameObject> SplashParticles;
    public Dictionary<string, Queue<ParticleSystem>> SplashParticleDict = new Dictionary<string, Queue<ParticleSystem>>();

    //for now there isn't a slash effect 
    [Header("SlashParticleEffect")]
    public GameObject SlashParticleEffect;
    Queue<ParticleSystem> SlashParticles = new Queue<ParticleSystem>();

    [Header("FrostEffect")]

    public FrostEffect VRCameraFrostEffect;
    Coroutine FrostEffect;

    private void Start()
    {

        FirstPooling();
        uIManager = UIManager.instance;
        soundManager = SoundManager.instance;
    }



    //-----------------------------------------------------//

     void FirstPooling()
    {
        //for spawnpoints
        foreach (Transform spawnpoint in SpawnPoints)
        {
            SpawnPointList.Add(spawnpoint.localPosition);
        }

        //for regular fruits
        foreach (GameObject fruit in FruitPrefabs)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject fruitGameObject = Instantiate(fruit, Vector3.zero, Quaternion.identity, transform);
                fruitGameObject.SetActive(false);
                UnusedFruit.Add(fruitGameObject);
            }
        }



        //for  frozen fruits
        List<GameObject> FrozenFruitList = new List<GameObject>();
        foreach (GameObject frozenFruit in FrozenFruit)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject frozenFruitGameObj = Instantiate(frozenFruit, Vector3.zero, Quaternion.identity, transform);
                frozenFruitGameObj.SetActive(false);
                FrozenFruitList.Add(frozenFruitGameObj);
            }
        }
        SpecialFruits.Add("Ice", FrozenFruitList);

        //for bomb
        List<GameObject> BombList = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject bombGameObj = Instantiate(explosionBombPrefab, Vector3.zero, Quaternion.identity, transform);
            bombGameObj.SetActive(false);
            BombList.Add(bombGameObj);
        }
        SpecialFruits.Add("Explosion", BombList);

        //for  splash particles
        foreach (GameObject particle in SplashParticles)
        {
            Queue<ParticleSystem> ParticleQueue = new Queue<ParticleSystem>();
            for (int i = 0; i < 10; i++)
            {
                GameObject particleGameObject = Instantiate(particle, Vector3.zero, Quaternion.identity, transform);
                ParticleQueue.Enqueue(particleGameObject.GetComponent<ParticleSystem>());
            }
            SplashParticleDict.Add(particle.name, ParticleQueue);
        }
    }
    //spawn regular fruit
    void SpawnSpecialFruits()
    {

        int SpecialFruitsIndex = Random.Range(0, SpecialFruits.Keys.Count);

        int SpecialFruitCount = Random.Range(0, 3);
        for (int i = 0; i < SpecialFruitCount; i++)
        {
            if (SpecialFruits.ElementAt(SpecialFruitsIndex).Key == "Ice" && Random.Range(0, 10) != 0)
            {
                return;
            }

            List<GameObject> SpecialFruitList = SpecialFruits.ElementAt(Random.Range(0, SpecialFruits.Keys.Count)).Value;
            GameObject randomSpecialFruit = SpecialFruitList[Random.Range(0, SpecialFruitList.Count)];

            Vector3 randomPos = SpawnPointList[Random.Range(0, SpawnPointList.Count)];

            randomSpecialFruit.transform.localPosition = randomPos;
            AddSpecialForce(randomSpecialFruit);
            SpecialFruitList.Remove(randomSpecialFruit);
            UsedSpecialFrutisList.Add(randomSpecialFruit);
        }
    }

    //spawn regular fruit
    public void SpawnFruit()
    {
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            if (UnusedFruit.Count > 0 || UsedFruit.Count < 15)
            {
                Vector3 randomPos = SpawnPointList[Random.Range(0, SpawnPointList.Count)];
                GameObject randomFruit = UnusedFruit[Random.Range(0, UnusedFruit.Count)];

                randomFruit.transform.localPosition = randomPos;
                AddSpecialForce(randomFruit);
                UsedFruit.Add(randomFruit);
                UnusedFruit.Remove(randomFruit);
            }
        }
    }
    //add random force in a fruit
    void AddSpecialForce(GameObject obj)
    {
        obj.SetActive(true);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        rig.AddForce(obj.transform.up * Random.Range(270, 330) * (1f / Time.timeScale));
        rig.AddForce(obj.transform.forward * Random.Range(-20, -40) * (1f / Time.timeScale));
        rig.AddForce(obj.transform.right * Random.Range(-30, 30) * (1f / Time.timeScale));
    }

    //-----------------------------------------------------//

    //called after cut a fruit
    public void GetParticle(particleType _particleType, Vector3 pos, Quaternion rot)
    {
        string particlType = _particleType.ToString();
        ParticleSystem particle = SplashParticleDict[particlType].Dequeue();
        //ParticleSystem SlashParticle = SlashParticles.Dequeue();

        particle.transform.localPosition = pos;
        particle.transform.rotation = rot;
        // SlashParticle.transform.localPosition = pos;
        //  SlashParticle.transform.rotation = rot;

        particle.Play();
        //  SlashParticle.Play();

        SplashParticleDict[particlType].Enqueue(particle);
        //   SlashParticles.Enqueue(SlashParticle);

        if (_particleType == particleType.Ice)
        {

           FrostEffect = StartCoroutine(IceCameraEffect());
        }
        else if (_particleType == particleType.Explosion)
        {
            ResetPool();
            StartGameInPool(2f);
        }
    }

    //start spawning in "starTime" seconds
    public void StartGameInPool(float startTime)
    {
        InvokeRepeating("SpawnFruit", startTime, 1.5f);
        InvokeRepeating("SpawnSpecialFruits", startTime, 1.5f);

    }
    // reset all objects and cancel spawn functions
    public void ResetPool()
    {
        CancelInvoke("SpawnFruit");
        CancelInvoke("SpawnSpecialFruits");
        foreach (GameObject fruitGameObj in UsedFruit.ToArray())
        {
            Fruit fruit = fruitGameObj.GetComponent<Fruit>();
            ReycleFruit(fruit);
        }
        if (UsedSpecialFrutisList.Count > 0)
        {
            foreach (GameObject item in UsedSpecialFrutisList.ToArray())
            {
                Fruit fruit = item.GetComponent<Fruit>();
                ReycleFruit(fruit);
            }
        }
    }



    //for frozen effect
    IEnumerator IceCameraEffect()
    {
        VRCameraFrostEffect.enabled = true;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(4f);
        VRCameraFrostEffect.enabled = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

    }

    //-----------------------------------------------------//

    //called when cut a fruit (include bomb and ice)
    public void ReycleFruit(Fruit fruit)
    {
        //Fruit fruit = _fruitGameObj.GetComponent<Fruit>();
        fruit.ResetVelocity();
        if (fruit.particleTyp == particleType.Explosion || fruit.particleTyp == particleType.Ice)
        {
            ReycleSpecialFruitPool(fruit);
        }
        else
        {
            UsedFruit.Remove(fruit.gameObject);
            UnusedFruit.Add(fruit.gameObject);
        }
    }

    //called when cut a special Fruit like Bomb or FrozenFruit 
    public void ReycleSpecialFruitPool(Fruit _fruit)
    {
        if (UsedSpecialFrutisList.Count > 0)
        {
            SpecialFruits[_fruit.particleTyp.ToString()].Add(_fruit.gameObject);
            UsedSpecialFrutisList.Remove(_fruit.gameObject);
        }
    }



}
