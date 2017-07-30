using GamepadInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;

public class PartyController : MonoBehaviour
{
    /*Arreglo con los jugadores*/
    public List<PlayerInput> players;

    /*Utilizada para controlar el acceso de los jugadores*/
    public int currentControllerIndex = 0;

    /*Referencia estatica para acceder*/
    public static PartyController ControlController;

    /*No necesaria*/
    public bool playersReady = false;

    public bool[] coldDowns = new bool[] { true, true, true, true };

    /*Establece si se pueden leer las entradas*/
    public bool readingControls = false;

    public GamePad.Index[] controllers = new GamePad.Index[] { GamePad.Index.One, GamePad.Index.Two , GamePad.Index.Three , GamePad.Index.Four };

    IEnumerator StarColdDown(int control)
    {
        coldDowns[control] = false;
        yield return new WaitForSeconds(.3f);
        coldDowns[control] = true;
    }

    /*Check if controller has joinned*/
    public bool IsControllIn(bool keyb, GamePad.Index k)
    {
        if (players == null || players.Count == 0)
            return false;
        for (int i = 0; i < players.Count; i++)
        {
            if (keyb)
            {
                if (players[i].useKeyboard)
                    return true;
            }
            else
            {
                if (players[i].controlIndex == k)
                    return true;
            }
        }
        return false;
    }

    private void Start()
    {
        if (ControlController)
        {
            Destroy(gameObject);
            return;
        }
        ControlController = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (readingControls)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (IsControllIn(true, GamePad.Index.Any))
                    {

                        RemovePlayer(true, GamePad.Index.Any);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!IsControllIn(true, GamePad.Index.Any))
                    {
                        AddPlayer(true, GamePad.Index.Any);
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (players[i].useKeyboard)
                            {
                                players[i].ready = true;
                            }
                        }
                        Debug.Log("Keyboard added");
                    }
                }

                foreach (var item in controllers)
                {
                    CheckController(item);
                    CheckControllerQuit(item);
                }

                if (players != null)
                {
                    var res = players.Count > 0;

                    for (int i = 0; i < players.Count; i++)
                    {
                        res &= players[i].ready;
                    }

                    playersReady = res;
                }
            }

        }
        
    }


    /*Check players lock*/
    private void CheckController(GamePad.Index index)
    {
        if (GamePad.GetButtonDown(GamePad.Button.A, index))
        {
            if (!IsControllIn(false, index))
            {
                AddPlayer(false, index);
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].controlIndex == index)
                    {
                        players[i].ready = true;
                    }
                }
                Debug.Log("Control added");
            }
        }
    }

    private void CheckControllerQuit(GamePad.Index index)
    {
        if (GamePad.GetButtonDown(GamePad.Button.Back, index))
        {
            if (IsControllIn(false, index))
            {
                RemovePlayer(false, index);
            }
        }
    }

    private void RemovePlayer(bool usek, GamePad.Index index)
    {
        if (usek)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ready && players[i].useKeyboard)
                {
                    players[i].ready = false;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ready && !players[i].useKeyboard && players[i].controlIndex == index)
                {
                    players[i].ready = false;
                    return;
                }
            }
        }
        if (!usek)
            currentControllerIndex--;
        PlayerInput cont;
        if (usek)
        {
            cont = (from p in players where p.useKeyboard == usek select p).First();

        }
        else
        {
            cont = (from p in players where p.useKeyboard == false && p.controlIndex == index select p).First();
        }
        for (int i = cont.realIndex + 1; i < players.Count; i++)
        {
            players[i].realIndex--;

        }
        players.Remove(cont);
    }
    private void AddPlayer(bool usek, GamePad.Index index)
    {
        if (players == null)
            players = new List<PlayerInput>();
        if (!usek)
            currentControllerIndex++;

        var cont = new PlayerInput();
        cont.controlIndex = index;
        cont.realIndex = players.Count;
        cont.useKeyboard = usek;
        cont.ready = false;

        players.Add(cont);
    }

    IEnumerator DetectControllers()
    {
        playersReady = false;
        yield return new WaitForSeconds(1);
        readingControls = true;
    }


    void ComoLeerEntradas()
    {
        //Para entrar es space o (a,x)
        //para desconectar un control es escape o back
        //los jugadores estan en el array players
        //Saber de cual jugador vamos a pedir las entradas   jugador=1 -> 0

        //Saber si se trata de un jugador con teclado
        if (players[0/*jugador*/].useKeyboard) {
            //En este caso simplemente utilizar el sistema de unity
            var res = Input.GetButton("Jump");
            //O
            var resKey = Input.GetKeyDown(KeyCode.A);
        }
        else//El jugador esta utilizando controll
        {
            var res = GamePad.GetButtonDown(GamePad.Button.A, players[0/*jugador*/].controlIndex);
        }
    }
}
