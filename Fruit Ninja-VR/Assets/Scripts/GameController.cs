using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using TMPro;
public class GameController : MonoBehaviour
{
    //Singleton pattern
    public static GameController instance;
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

    //Reference for other singletons
    Pooler pooler;
    UIManager uIManager;

    //sliced new mesh material
    public Material sliceMaterial;
    public TextureRegion textureRegion;

    //3,2,1, GO!
    public List<string> CountdownElements;
    public TextMeshPro CountdownText;
    SoundManager soundManager;
    private void Start()
    {
        pooler = Pooler.instance;
        uIManager = UIManager.instance;
        CountdownText.gameObject.SetActive(false);
        soundManager = SoundManager.instance;
    }
    //for write in scene 3-2-1-Go! 
    IEnumerator Countdown(GameObject _Fruit)
    {
  
        string parentName =  _Fruit.transform.parent.name;
        if (parentName != "MainMenu")
        {
            CountdownText.gameObject.SetActive(true);
            for (int i = 0; i < CountdownElements.Count; i++)
            {
                CountdownText.text = CountdownElements[i];
                yield return new WaitForSeconds(1f);
            }
            CountdownText.gameObject.SetActive(false);
        }
        uIManager.SelectButton(parentName);
    }

    //Check you slice a fruit or not , and slice it!
    public void Slice(GameObject _Fruit, Weapon weapon)
    {
        SlicedHull slicedObject = Sliceed(_Fruit, weapon.slicePanel, sliceMaterial);

        if (slicedObject != null)
        {
            if (_Fruit.CompareTag("fruit"))
            {
                _Fruit.SetActive(false);
                Fruit fruit = _Fruit.GetComponent<Fruit>();
                pooler.ReycleFruit(fruit);
                if (fruit.particleTyp == particleType.Explosion)
                {
                    soundManager.BombSound();
                    weapon.Vibrate(0.5f);
                }
                else if (fruit.particleTyp == particleType.Ice)
                {
                    soundManager.FrozenSound();
                    uIManager.IncreaseScore(4);
                    weapon.Vibrate(0.2f);
                }
                else
                {
                    soundManager.SplashFruit();
                    uIManager.IncreaseScore(1);
                    weapon.Vibrate(0.1f);
                }
            }
            else if (_Fruit.CompareTag("button"))
            {
                weapon.Vibrate(0.1f);
                uIManager.BeforeSelectButton();
                soundManager.SplashFruit();
                StartCoroutine(Countdown(_Fruit));
            }
            pooler.GetParticle(_Fruit.GetComponent<Fruit>().particleTyp, _Fruit.transform.position, _Fruit.transform.rotation);
            GameObject upperPart = slicedObject.CreateUpperHull(_Fruit);
            GameObject lowPart = slicedObject.CreateLowerHull(_Fruit);
            AddComponents(upperPart);
            AddComponents(lowPart);
        }
        else
        {
            Debug.Log("null");
        }
    }

    //the function that performs the actual cutting
    public SlicedHull Sliceed(GameObject objectToSlice, GameObject plane, Material material)
    {
        return objectToSlice.Slice(plane.transform.position, plane.transform.up, material);

    }
    //add components to new sliced objects
    public void AddComponents(GameObject objPart)
    {

        objPart.AddComponent<BoxCollider>();
        objPart.GetComponent<MeshRenderer>().materials[1] = sliceMaterial;

        objPart.AddComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        objPart.GetComponent<Rigidbody>().AddExplosionForce(350, objPart.transform.position, 30);
        Destroy(objPart, 8f);
    }


}
