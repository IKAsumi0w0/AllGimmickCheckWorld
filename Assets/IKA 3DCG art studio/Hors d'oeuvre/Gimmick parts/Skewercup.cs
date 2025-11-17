
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Skewercup : UdonSharpBehaviour
{
    private Egg _egg;
    private FriedShrimp _friedShrimp;
    private Steak _steak;
    private PotatoRing _potatoRing;
    private Tomato _tomato;
    private Takoyaki _takoyaki;
    private FruitSandwich _fruitSandwich;
    private SalmonPotato _salmonPotato;
    private HamburgerSteak _hamburgerSteak; 

    void OnTriggerStay(Collider other)
    {
        _egg = other.GetComponent<Egg>();
        if (_egg != null)
        {
            _egg.ReSpawn();
        }

        _friedShrimp = other.GetComponent<FriedShrimp>();
        if (_friedShrimp != null)
        {
            _friedShrimp.ReSpawn();
        }

        _steak = other.GetComponent<Steak>();
        if (_steak != null)
        {
            _steak.ReSpawn();
        }

        _potatoRing = other.GetComponent<PotatoRing>();
        if (_potatoRing != null)
        {
            _potatoRing.ReSpawn();
        }

        _tomato = other.GetComponent<Tomato>();
        if (_tomato != null)
        {
            _tomato.ReSpawn();
        }

        _takoyaki = other.GetComponent<Takoyaki>();
        if (_takoyaki != null)
        {
            _takoyaki.ReSpawn();
        }

        _fruitSandwich = other.GetComponent<FruitSandwich>();
        if (_fruitSandwich != null)
        {
            _fruitSandwich.ReSpawn();
        }

        _salmonPotato = other.GetComponent<SalmonPotato>();
        if (_salmonPotato != null)
        {
            _salmonPotato.ReSpawn();
        }

        _hamburgerSteak = other.GetComponent<HamburgerSteak>();
        if (_hamburgerSteak != null)
        {
            _hamburgerSteak.ReSpawn();
        }

    }
}
