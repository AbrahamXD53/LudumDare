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
    public List<PlayerInput> players;
    public int currentControllerIndex = 0;

    public static PartyController ControlController;

    public Canvas canvas;

    public bool playersReady = false;

    public bool[] coldDowns = new bool[] { true, true, true, true };

    public bool readingControls = false;

    public EventSystem eventSystem;

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
    public void ShowControlCapture(GameType mode, int scene)
    {
        gameMode = mode;
        canvas.enabled = true;
        sceneToPlay = scene;
        eventSystem.firstSelectedGameObject = null;
        StartCoroutine(DetectControllers());
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
        canvas.enabled = false;
    }

    private void Update()
    {
        if (readingControls)
        {
            if (!playersReady)
            {
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (IsControllIn(true, GamePad.Index.Any))
                        {

                            RemovePlayer(true, GamePad.Index.Any);
                        }
                        else
                        {
                            CloseSelector();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!IsControllIn(true, GamePad.Index.Any))
                        {
                            AddPlayer(true, GamePad.Index.Any);
                        }
                        else
                        {
                            if (canvas.enabled)
                            {
                                for (int i = 0; i < players.Count; i++)
                                {
                                    if (players[i].useKeyboard)
                                    {
                                        players[i].ready = true;
                                    }
                                }
                            }
                        }


                    }
                    CheckController(GamePad.Index.One);
                    CheckControllerQuit(GamePad.Index.One);
                    CheckController(GamePad.Index.Two);
                    CheckControllerQuit(GamePad.Index.Two);
                    CheckController(GamePad.Index.Three);
                    CheckControllerQuit(GamePad.Index.Three);
                    CheckController(GamePad.Index.Four);
                    CheckControllerQuit(GamePad.Index.Four);

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
            else
            {
                
                /*Players are ready*/
            }
        }
        if (canvas.enabled)
        {
            if (players != null)
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].ready)
                        continue;
                    if (players[i].useKeyboard)
                    {
                        if (Input.GetButtonDown("Vertical"))
                        {
                            SoundManager.Instance.PlayEffect("Button1");


                            if (Input.GetAxisRaw("Vertical") > 0)
                                players[i].character++;
                            else
                                players[i].character--;
                            if (players[i].character > Characters.Pina)
                                players[i].character = Characters.SrPenguin;
                            if (players[i].character < Characters.SrPenguin)
                                players[i].character = Characters.Pina;
                            characterSelectors[i].sprite = charactersImage[(int)players[i].character];
                        }
                    }
                    else
                    {
                        var sa = GamePad.GetAxis(GamePad.Axis.LeftStick, players[i].controlIndex).y;
                        if (sa != 0)
                        {
                            if (coldDowns[i])
                            {
                                SoundManager.Instance.PlayEffect("Button1");
                                if (sa > 0)
                                    players[i].character++;
                                else
                                    players[i].character--;
                                StartCoroutine(StarColdDown(i));
                            }
                            if (players[i].character > Characters.Pina)
                                players[i].character = Characters.SrPenguin;
                            if (players[i].character < Characters.SrPenguin)
                                players[i].character = Characters.Pina;
                            characterSelectors[i].sprite = charactersImage[(int)players[i].character];
                        }
                    }
                }
        }
    }

    /*Make resets to work again*/
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex > 0)
        {
            SoundManager.Instance.PlayAudio(0);
            canvas.enabled = false;
        }
        else
        {
            //canvas.enabled = true;
            currentControllerIndex = 0;
            for (int i = 0; i < characterSelectors.Length; i++)
            {
                characterSelectors[i].color = Color.white;
                characterSelectors[i].gameObject.SetActive(false);
            }
            players = null;
            characterSelectorTouch.gameObject.SetActive(false);
            characterSelectorTouch.color = Color.white;
            //StartCoroutine(DetectControllers());
        }
    }

    /*Check players lock*/
    private void CheckController(GamePad.Index index)
    {
        if (GamePad.GetButtonDown(GamePad.Button.A, index))
        {
            SoundManager.Instance.PlayEffect("Button3");
            if (!IsControllIn(false, index))
            {
                AddPlayer(false, index);
            }
            else
            {
                if (canvas.enabled)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].controlIndex == index)
                        {
                            players[i].ready = true;
                            characterSelectors[i].color = new Color(.2f, .2f, .2f);
                        }
                    }
                }
            }
        }
    }

    private void CheckControllerQuit(GamePad.Index index)
    {
        if (GamePad.GetButtonDown(GamePad.Button.B, index))
        {
            SoundManager.Instance.PlayEffect("Button3");
            if (IsControllIn(false, index))
            {
                RemovePlayer(false, index);
            }
            else
            {
                CloseSelector();
            }
        }
    }

    public void CloseSelector()
    {
        readingControls = false;
        canvas.enabled = false;
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
                    characterSelectors[players[i].realIndex].color = Color.white;
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
                    characterSelectors[players[i].realIndex].color = Color.white;
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
        if (canvas.enabled)
        {
            characterSelectors[cont.realIndex].gameObject.SetActive(false);
        }
        for (int i = cont.realIndex + 1; i < players.Count; i++)
        {
            //var prevPlayer = players[i].character;

            characterSelectors[players[i].realIndex].gameObject.SetActive(false);
            characterSelectors[players[i].realIndex].color = Color.white;

            players[i].realIndex--;

            if (players[i].ready)
                characterSelectors[players[i].realIndex].color = new Color(.2f, .2f, .2f);
            characterSelectors[players[i].realIndex].sprite = charactersImage[(int)players[i].character];
            characterSelectors[players[i].realIndex].gameObject.SetActive(true);
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

        if (canvas.enabled)
            characterSelectors[players.Count].gameObject.SetActive(true);
        players.Add(cont);
    }

    IEnumerator DetectControllers()
    {
        playersReady = false;
        yield return new WaitForSeconds(1);
        readingControls = true;
    }

    public void JoinAndroidPlayer()
    {
        if (!IsControllIn(true, 0))
        {
            characterSelectorTouch.gameObject.SetActive(true);
            if (players == null)
                players = new List<PlayerInput>();
            var cont = new PlayerInput();
            cont.controlIndex = 0;
            cont.realIndex = players.Count;
            cont.useKeyboard = true;
            players.Add(cont);
        }
    }
    public void ChangePicture(int ratio)
    {
        for (int i = 0; i < players.Count; i++)
            if (players[i].useKeyboard)
            {
                if (players[i].ready)
                    return;
            }
        for (int i = 0; i < players.Count; i++)
        {
            SoundManager.Instance.PlayEffect("Button1");
            players[i].character += ratio;
            if (players[i].character > Characters.Pina)
                players[i].character = Characters.SrPenguin;
            if (players[i].character < Characters.SrPenguin)
                players[i].character = Characters.Pina;
            characterSelectorTouch.sprite = charactersImage[(int)players[i].character];
        }
    }
    public void LockPlayer()
    {
        if (IsControllIn(true, 0))
        {
            if (canvas.enabled)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].useKeyboard)
                    {
                        players[i].ready = true;
                        characterSelectorTouch.color = new Color(.2f, .2f, .2f);
                    }
                }
            }
        }
    }
}
