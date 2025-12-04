
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
        _eggObj01.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj02.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj03.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj04.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj05.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj06.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj07.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj08.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj09.gameObject.GetComponent<Egg>().ReSpawn();
        _eggObj10.gameObject.GetComponent<Egg>().ReSpawn();
    }

    public void ResetFriedShrimp()
    {
        _friedShrimpObj01.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj02.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj03.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj04.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj05.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj06.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj07.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj08.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj09.gameObject.GetComponent<FriedShrimp>().ReSpawn();
        _friedShrimpObj10.gameObject.GetComponent<FriedShrimp>().ReSpawn();
    }

    public void ResetFruitSandwich()
    {
        _fruitSandwichObj01.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj02.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj03.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj04.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj05.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj06.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj07.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj08.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj09.gameObject.GetComponent<FruitSandwich>().ReSpawn();
        _fruitSandwichObj10.gameObject.GetComponent<FruitSandwich>().ReSpawn();
    }

    public void ResetHamburgerSteak()
    {
        _hamburgerSteakObj01.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj02.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj03.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj04.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj05.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj06.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj07.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj08.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj09.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
        _hamburgerSteakObj10.gameObject.GetComponent<HamburgerSteak>().ReSpawn();
    }

    public void ResetTomato()
    {
        _tomatoObj01.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj02.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj03.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj04.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj05.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj06.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj07.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj08.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj09.gameObject.GetComponent<Tomato>().ReSpawn();
        _tomatoObj10.gameObject.GetComponent<Tomato>().ReSpawn();
    }

    public void ResetTakoyaki()
    {
        _takoyakiObj01.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj02.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj03.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj04.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj05.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj06.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj07.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj08.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj09.gameObject.GetComponent<Takoyaki>().ReSpawn();
        _takoyakiObj10.gameObject.GetComponent<Takoyaki>().ReSpawn();
    }

    public void ResetSteak()
    {
        _steakObj01.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj02.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj03.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj04.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj05.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj06.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj07.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj08.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj09.gameObject.GetComponent<Steak>().ReSpawn();
        _steakObj10.gameObject.GetComponent<Steak>().ReSpawn();
    }

    public void ResetSalmonPotato()
    {
        _salmonPotatoObj01.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj02.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj03.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj04.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj05.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj06.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj07.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj08.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj09.gameObject.GetComponent<SalmonPotato>().ReSpawn();
        _salmonPotatoObj10.gameObject.GetComponent<SalmonPotato>().ReSpawn();
    }

    public void ResetPotatoRing()
    {
        _potatoRingObj01.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj02.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj03.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj04.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj05.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj06.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj07.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj08.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj09.gameObject.GetComponent<PotatoRing>().ReSpawn();
        _potatoRingObj10.gameObject.GetComponent<PotatoRing>().ReSpawn();
    }
}
