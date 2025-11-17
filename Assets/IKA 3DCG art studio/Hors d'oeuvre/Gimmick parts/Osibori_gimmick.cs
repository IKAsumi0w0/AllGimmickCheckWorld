
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Osibori_gimmick : UdonSharpBehaviour
{
    public Transform _eggObj01;
    public Transform _eggObj02;
    public Transform _eggObj03;
    public Transform _eggObj04;
    public Transform _eggObj05;
    public Transform _eggObj06;
    public Transform _eggObj07;
    public Transform _eggObj08;
    public Transform _eggObj09;
    public Transform _eggObj10;
    public Transform _friedShrimpObj01;
    public Transform _friedShrimpObj02;
    public Transform _friedShrimpObj03;
    public Transform _friedShrimpObj04;
    public Transform _friedShrimpObj05;
    public Transform _friedShrimpObj06;
    public Transform _friedShrimpObj07;
    public Transform _friedShrimpObj08;
    public Transform _friedShrimpObj09;
    public Transform _friedShrimpObj10;
    public Transform _fruitSandwichObj01;
    public Transform _fruitSandwichObj02;
    public Transform _fruitSandwichObj03;
    public Transform _fruitSandwichObj04;
    public Transform _fruitSandwichObj05;
    public Transform _fruitSandwichObj06;
    public Transform _fruitSandwichObj07;
    public Transform _fruitSandwichObj08;
    public Transform _fruitSandwichObj09;
    public Transform _fruitSandwichObj10;
    public Transform _hamburgerSteakObj01;
    public Transform _hamburgerSteakObj02;
    public Transform _hamburgerSteakObj03;
    public Transform _hamburgerSteakObj04;
    public Transform _hamburgerSteakObj05;
    public Transform _hamburgerSteakObj06;
    public Transform _hamburgerSteakObj07;
    public Transform _hamburgerSteakObj08;
    public Transform _hamburgerSteakObj09;
    public Transform _hamburgerSteakObj10;
    public Transform _tomatoObj01;
    public Transform _tomatoObj02;
    public Transform _tomatoObj03;
    public Transform _tomatoObj04;
    public Transform _tomatoObj05;
    public Transform _tomatoObj06;
    public Transform _tomatoObj07;
    public Transform _tomatoObj08;
    public Transform _tomatoObj09;
    public Transform _tomatoObj10;
    public Transform _takoyakiObj01;
    public Transform _takoyakiObj02;
    public Transform _takoyakiObj03;
    public Transform _takoyakiObj04;
    public Transform _takoyakiObj05;
    public Transform _takoyakiObj06;
    public Transform _takoyakiObj07;
    public Transform _takoyakiObj08;
    public Transform _takoyakiObj09;
    public Transform _takoyakiObj10;
    public Transform _steakObj01;
    public Transform _steakObj02;
    public Transform _steakObj03;
    public Transform _steakObj04;
    public Transform _steakObj05;
    public Transform _steakObj06;
    public Transform _steakObj07;
    public Transform _steakObj08;
    public Transform _steakObj09;
    public Transform _steakObj10;
    public Transform _salmonPotatoObj01;
    public Transform _salmonPotatoObj02;
    public Transform _salmonPotatoObj03;
    public Transform _salmonPotatoObj04;
    public Transform _salmonPotatoObj05;
    public Transform _salmonPotatoObj06;
    public Transform _salmonPotatoObj07;
    public Transform _salmonPotatoObj08;
    public Transform _salmonPotatoObj09;
    public Transform _salmonPotatoObj10;
    public Transform _potatoRingObj01;
    public Transform _potatoRingObj02;
    public Transform _potatoRingObj03;
    public Transform _potatoRingObj04;
    public Transform _potatoRingObj05;
    public Transform _potatoRingObj06;
    public Transform _potatoRingObj07;
    public Transform _potatoRingObj08;
    public Transform _potatoRingObj09;
    public Transform _potatoRingObj10;

    void Start()
    {
        
    }
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetObj");
    }

    public void ResetObj()
    {
        ResetEgg();
        ResetFriedShrimp();
        ResetFruitSandwich();
        ResetHamburgerSteak();
        ResetPotatoRing();
        ResetSalmonPotato();
        ResetSteak();
        ResetTakoyaki();
        ResetTomato();
    }


    public void ResetEgg()
    {
        _eggObj01.GetComponent<Egg>().ResetPosition();
        _eggObj02.GetComponent<Egg>().ResetPosition();
        _eggObj03.GetComponent<Egg>().ResetPosition();
        _eggObj04.GetComponent<Egg>().ResetPosition();
        _eggObj05.GetComponent<Egg>().ResetPosition();
        _eggObj06.GetComponent<Egg>().ResetPosition();
        _eggObj07.GetComponent<Egg>().ResetPosition();
        _eggObj08.GetComponent<Egg>().ResetPosition();
        _eggObj09.GetComponent<Egg>().ResetPosition();
        _eggObj10.GetComponent<Egg>().ResetPosition();

        if (Vector3.Distance(_eggObj01.position, Vector3.zero) < 0.01f) _eggObj01.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj02.position, Vector3.zero) < 0.01f) _eggObj02.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj03.position, Vector3.zero) < 0.01f) _eggObj03.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj04.position, Vector3.zero) < 0.01f) _eggObj04.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj05.position, Vector3.zero) < 0.01f) _eggObj05.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj06.position, Vector3.zero) < 0.01f) _eggObj06.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj07.position, Vector3.zero) < 0.01f) _eggObj07.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj08.position, Vector3.zero) < 0.01f) _eggObj08.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj09.position, Vector3.zero) < 0.01f) _eggObj09.gameObject.GetComponent<Egg>().ReSpawn();
        if (Vector3.Distance(_eggObj10.position, Vector3.zero) < 0.01f) _eggObj10.gameObject.GetComponent<Egg>().ReSpawn();
    }

    public void ResetFriedShrimp()
    {
        _friedShrimpObj01.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj02.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj03.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj04.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj05.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj06.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj07.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj08.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj09.GetComponent<FriedShrimp>().ResetPosition();
        _friedShrimpObj10.GetComponent<FriedShrimp>().ResetPosition();

        if (Vector3.Distance(_friedShrimpObj01.position, Vector3.zero) < 0.01f) _friedShrimpObj01.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj02.position, Vector3.zero) < 0.01f) _friedShrimpObj02.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj03.position, Vector3.zero) < 0.01f) _friedShrimpObj03.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj04.position, Vector3.zero) < 0.01f) _friedShrimpObj04.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj05.position, Vector3.zero) < 0.01f) _friedShrimpObj05.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj06.position, Vector3.zero) < 0.01f) _friedShrimpObj06.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj07.position, Vector3.zero) < 0.01f) _friedShrimpObj07.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj08.position, Vector3.zero) < 0.01f) _friedShrimpObj08.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj09.position, Vector3.zero) < 0.01f) _friedShrimpObj09.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        if (Vector3.Distance(_friedShrimpObj10.position, Vector3.zero) < 0.01f) _friedShrimpObj10.gameObject.GetComponent<FriedShrimp>().ReSpawn();
    }

    public void ResetFruitSandwich()
    {
        _fruitSandwichObj01.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj02.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj03.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj04.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj05.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj06.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj07.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj08.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj09.GetComponent<FruitSandwich>().ResetPosition();
        _fruitSandwichObj10.GetComponent<FruitSandwich>().ResetPosition();

        if (Vector3.Distance(_fruitSandwichObj01.position, Vector3.zero) < 0.01f) _fruitSandwichObj01.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj02.position, Vector3.zero) < 0.01f) _fruitSandwichObj02.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj03.position, Vector3.zero) < 0.01f) _fruitSandwichObj03.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj04.position, Vector3.zero) < 0.01f) _fruitSandwichObj04.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj05.position, Vector3.zero) < 0.01f) _fruitSandwichObj05.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj06.position, Vector3.zero) < 0.01f) _fruitSandwichObj06.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj07.position, Vector3.zero) < 0.01f) _fruitSandwichObj07.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj08.position, Vector3.zero) < 0.01f) _fruitSandwichObj08.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj09.position, Vector3.zero) < 0.01f) _fruitSandwichObj09.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        if (Vector3.Distance(_fruitSandwichObj10.position, Vector3.zero) < 0.01f) _fruitSandwichObj10.gameObject.GetComponent<FruitSandwich>().ReSpawn();
    }

    public void ResetHamburgerSteak()
    {
        _hamburgerSteakObj01.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj02.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj03.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj04.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj05.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj06.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj07.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj08.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj09.GetComponent<HamburgerSteak>().ResetPosition();
        _hamburgerSteakObj10.GetComponent<HamburgerSteak>().ResetPosition();

        if (Vector3.Distance(_hamburgerSteakObj01.position, Vector3.zero) < 0.01f) _hamburgerSteakObj01.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj02.position, Vector3.zero) < 0.01f) _hamburgerSteakObj02.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj03.position, Vector3.zero) < 0.01f) _hamburgerSteakObj03.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj04.position, Vector3.zero) < 0.01f) _hamburgerSteakObj04.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj05.position, Vector3.zero) < 0.01f) _hamburgerSteakObj05.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj06.position, Vector3.zero) < 0.01f) _hamburgerSteakObj06.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj07.position, Vector3.zero) < 0.01f) _hamburgerSteakObj07.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj08.position, Vector3.zero) < 0.01f) _hamburgerSteakObj08.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj09.position, Vector3.zero) < 0.01f) _hamburgerSteakObj09.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        if (Vector3.Distance(_hamburgerSteakObj10.position, Vector3.zero) < 0.01f) _hamburgerSteakObj10.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
    }

    public void ResetTomato()
    {
        _tomatoObj01.GetComponent<Tomato>().ResetPosition();
        _tomatoObj02.GetComponent<Tomato>().ResetPosition();
        _tomatoObj03.GetComponent<Tomato>().ResetPosition();
        _tomatoObj04.GetComponent<Tomato>().ResetPosition();
        _tomatoObj05.GetComponent<Tomato>().ResetPosition();
        _tomatoObj06.GetComponent<Tomato>().ResetPosition();
        _tomatoObj07.GetComponent<Tomato>().ResetPosition();
        _tomatoObj08.GetComponent<Tomato>().ResetPosition();
        _tomatoObj09.GetComponent<Tomato>().ResetPosition();
        _tomatoObj10.GetComponent<Tomato>().ResetPosition();

        if (Vector3.Distance(_tomatoObj01.position, Vector3.zero) < 0.01f) _tomatoObj01.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj02.position, Vector3.zero) < 0.01f) _tomatoObj02.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj03.position, Vector3.zero) < 0.01f) _tomatoObj03.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj04.position, Vector3.zero) < 0.01f) _tomatoObj04.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj05.position, Vector3.zero) < 0.01f) _tomatoObj05.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj06.position, Vector3.zero) < 0.01f) _tomatoObj06.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj07.position, Vector3.zero) < 0.01f) _tomatoObj07.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj08.position, Vector3.zero) < 0.01f) _tomatoObj08.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj09.position, Vector3.zero) < 0.01f) _tomatoObj09.gameObject.GetComponent<Tomato>().ReSpawn();
        if (Vector3.Distance(_tomatoObj10.position, Vector3.zero) < 0.01f) _tomatoObj10.gameObject.GetComponent<Tomato>().ReSpawn();
    }

    public void ResetTakoyaki()
    {
        _takoyakiObj01.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj02.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj03.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj04.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj05.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj06.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj07.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj08.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj09.GetComponent<Takoyaki>().ResetPosition();
        _takoyakiObj10.GetComponent<Takoyaki>().ResetPosition();

        if (Vector3.Distance(_takoyakiObj01.position, Vector3.zero) < 0.01f) _takoyakiObj01.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj02.position, Vector3.zero) < 0.01f) _takoyakiObj02.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj03.position, Vector3.zero) < 0.01f) _takoyakiObj03.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj04.position, Vector3.zero) < 0.01f) _takoyakiObj04.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj05.position, Vector3.zero) < 0.01f) _takoyakiObj05.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj06.position, Vector3.zero) < 0.01f) _takoyakiObj06.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj07.position, Vector3.zero) < 0.01f) _takoyakiObj07.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj08.position, Vector3.zero) < 0.01f) _takoyakiObj08.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj09.position, Vector3.zero) < 0.01f) _takoyakiObj09.gameObject.GetComponent<Takoyaki>().ReSpawn();
        if (Vector3.Distance(_takoyakiObj10.position, Vector3.zero) < 0.01f) _takoyakiObj10.gameObject.GetComponent<Takoyaki>().ReSpawn();
    }

    public void ResetSteak()
    {
        _steakObj01.GetComponent<Steak>().ResetPosition();
        _steakObj02.GetComponent<Steak>().ResetPosition();
        _steakObj03.GetComponent<Steak>().ResetPosition();
        _steakObj04.GetComponent<Steak>().ResetPosition();
        _steakObj05.GetComponent<Steak>().ResetPosition();
        _steakObj06.GetComponent<Steak>().ResetPosition();
        _steakObj07.GetComponent<Steak>().ResetPosition();
        _steakObj08.GetComponent<Steak>().ResetPosition();
        _steakObj09.GetComponent<Steak>().ResetPosition();
        _steakObj10.GetComponent<Steak>().ResetPosition();

        if (Vector3.Distance(_steakObj01.position, Vector3.zero) < 0.01f) _steakObj01.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj02.position, Vector3.zero) < 0.01f) _steakObj02.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj03.position, Vector3.zero) < 0.01f) _steakObj03.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj04.position, Vector3.zero) < 0.01f) _steakObj04.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj05.position, Vector3.zero) < 0.01f) _steakObj05.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj06.position, Vector3.zero) < 0.01f) _steakObj06.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj07.position, Vector3.zero) < 0.01f) _steakObj07.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj08.position, Vector3.zero) < 0.01f) _steakObj08.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj09.position, Vector3.zero) < 0.01f) _steakObj09.gameObject.GetComponent<Steak>().ReSpawn();
        if (Vector3.Distance(_steakObj10.position, Vector3.zero) < 0.01f) _steakObj10.gameObject.GetComponent<Steak>().ReSpawn();
    }

    public void ResetSalmonPotato()
    {
        _salmonPotatoObj01.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj02.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj03.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj04.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj05.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj06.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj07.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj08.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj09.GetComponent<SalmonPotato>().ResetPosition();
        _salmonPotatoObj10.GetComponent<SalmonPotato>().ResetPosition();

        if (Vector3.Distance(_salmonPotatoObj01.position, Vector3.zero) < 0.01f) _salmonPotatoObj01.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj02.position, Vector3.zero) < 0.01f) _salmonPotatoObj02.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj03.position, Vector3.zero) < 0.01f) _salmonPotatoObj03.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj04.position, Vector3.zero) < 0.01f) _salmonPotatoObj04.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj05.position, Vector3.zero) < 0.01f) _salmonPotatoObj05.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj06.position, Vector3.zero) < 0.01f) _salmonPotatoObj06.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj07.position, Vector3.zero) < 0.01f) _salmonPotatoObj07.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj08.position, Vector3.zero) < 0.01f) _salmonPotatoObj08.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj09.position, Vector3.zero) < 0.01f) _salmonPotatoObj09.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        if (Vector3.Distance(_salmonPotatoObj10.position, Vector3.zero) < 0.01f) _salmonPotatoObj10.gameObject.GetComponent<SalmonPotato>().ReSpawn();
    }

    public void ResetPotatoRing()
    {
        _potatoRingObj01.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj02.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj03.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj04.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj05.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj06.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj07.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj08.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj09.GetComponent<PotatoRing>().ResetPosition();
        _potatoRingObj10.GetComponent<PotatoRing>().ResetPosition();

        if (Vector3.Distance(_potatoRingObj01.position, Vector3.zero) < 0.01f) _potatoRingObj01.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj02.position, Vector3.zero) < 0.01f) _potatoRingObj02.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj03.position, Vector3.zero) < 0.01f) _potatoRingObj03.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj04.position, Vector3.zero) < 0.01f) _potatoRingObj04.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj05.position, Vector3.zero) < 0.01f) _potatoRingObj05.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj06.position, Vector3.zero) < 0.01f) _potatoRingObj06.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj07.position, Vector3.zero) < 0.01f) _potatoRingObj07.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj08.position, Vector3.zero) < 0.01f) _potatoRingObj08.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj09.position, Vector3.zero) < 0.01f) _potatoRingObj09.gameObject.GetComponent<PotatoRing>().ReSpawn();
        if (Vector3.Distance(_potatoRingObj10.position, Vector3.zero) < 0.01f) _potatoRingObj10.gameObject.GetComponent<PotatoRing>().ReSpawn();
    }
}
