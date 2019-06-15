using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class hieroglyphicsScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public Animator[] pointerAnimations;
    public Animator sealAnimation;
    public KMSelectable[] animationButtons;
    public KMSelectable mainButton;
    public int[] pointerMoves = new int[2] {0,0};
    private bool moving;
    public GameObject buttonNonAnim;
    public GameObject buttonAnim;

    private string[] potentialHieroglyphs = new string[18] {"A","B","C","D","E","K","L","N","U","V","W","Y","R","T","Q","O","M","G"};
    private string[] hieroglyphicNames = new string[18] {"Male","Bull","Urn","Eye of Horus","Ankh","Goose","Lion","Water","Head of Cow","Mosaic","Lasso","Two Reeds","Scales","Bone","Triangle","Horn","Owl","Tent"};
    public TextMesh[] hieroglyphsText;
    private int[] hieroglyphsSumInt = new int[3];
    public TextMesh[] hieroglyphsSum;
    private string[] chosenHieroglyphs = new string[5] {"","","","",""};
    public string[] chosenHieroglyphNames = new string[5] {"","","","",""};
    private List<int> chosenIndices = new List<int>();
    public TextMesh displayedHieroglyphs;

    public TextMesh[] lockText;
    public int[] lockValue = new int[4];
    public int[] correctLockPosition = new int[2];
    private string[] lockPositionLog = new string[4] {"left", "centre", "right", "centre"};

    public int[] statementOccurrences = new int[15];
    private bool invalid;
    public bool positionsSet;

    public string priorityHieroglyph = "";
    public int priorityHieroglyphValue = 0;
    public int instancesOfPriority = 1;
    public float pressTime = 0f;
    private float correctPressTime = 0f;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in animationButtons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
        }
        mainButton.OnInteract += delegate () { PressMainButton(); return false; };
    }


    void Start()
    {
        buttonAnim.SetActive(false);
        invalid = false;
        PickFiveHieroglyphs();
        WriteHieroglyphs();
    }

    void PickFiveHieroglyphs()
    {
        displayedHieroglyphs.text = "";
        for(int i = 0; i <= 4; i++)
        {
            int index = UnityEngine.Random.Range(0,17);
            while(chosenIndices.Contains(index))
            {
                index = UnityEngine.Random.Range(0,17);
            }
            chosenIndices.Add(index);
            chosenHieroglyphs[i] = potentialHieroglyphs[index];
            chosenHieroglyphNames[i] = hieroglyphicNames[index];
        }
        chosenIndices.Clear();

        for(int i = 0; i <= 4; i++)
        {
            int index2 = UnityEngine.Random.Range(0,5);
            while(chosenIndices.Contains(index2))
            {
                index2 = UnityEngine.Random.Range(0,5);
            }
            chosenIndices.Add(index2);
            displayedHieroglyphs.text += chosenHieroglyphs[index2];
        }
        chosenIndices.Clear();

        for(int i = 0; i <= 13; i++)
        {
            if(chosenHieroglyphNames.Contains(hieroglyphicNames[i]))
            {
                priorityHieroglyph = hieroglyphicNames[i];
                priorityHieroglyphValue = Array.FindIndex(chosenHieroglyphNames, x => x.Contains(hieroglyphicNames[i])) + 1;
                break;
            }
        }
    }

    void WriteHieroglyphs()
    {
        instancesOfPriority = 1;
        for(int i = 0; i <= 14; i++)
        {
            statementOccurrences[i] = 0;
        }
        for(int i = 0; i <= 2; i++)
        {
            hieroglyphsText[i].text = "";
            hieroglyphsSumInt[i] = 0;
            if(i == 0)
            {
                hieroglyphsText[0].text += chosenHieroglyphs[0];
                hieroglyphsSumInt[0] += 1;
                statementOccurrences[0]++;
                if(priorityHieroglyphValue - 1 == 0)
                {
                    instancesOfPriority++;
                }
            }
            else
            {
                int index1 = UnityEngine.Random.Range(0,5);
                hieroglyphsText[i].text = chosenHieroglyphs[index1];
                hieroglyphsSumInt[i] += (index1 + 1);
                statementOccurrences[index1 + (i * 5)]++;
                if(priorityHieroglyphValue - 1 == index1)
                {
                    instancesOfPriority++;
                }
            }

            for(int j = 0; j <= 1; j++)
            {
                int index2 = UnityEngine.Random.Range(1,5);
                hieroglyphsText[i].text += chosenHieroglyphs[index2];
                hieroglyphsSumInt[i] += (index2 + 1);
                statementOccurrences[index2 + (i * 5)]++;
                if(priorityHieroglyphValue - 1 == index2)
                {
                    instancesOfPriority++;
                }
            }

            if(i == 1 || i == 2)
            {
                int index3 = UnityEngine.Random.Range(0,5);
                hieroglyphsText[i].text += chosenHieroglyphs[index3];
                hieroglyphsSumInt[i] += (index3 + 1);
                statementOccurrences[index3 + (i * 5)]++;
                if(priorityHieroglyphValue - 1 == index3)
                {
                    instancesOfPriority++;
                }
            }
            if(i == 2)
            {
                int index4 = UnityEngine.Random.Range(0,5);
                hieroglyphsText[2].text += chosenHieroglyphs[index4];
                hieroglyphsSumInt[2] += (index4 + 1);
                statementOccurrences[index4 + (i * 5)]++;
                if(priorityHieroglyphValue - 1 == index4)
                {
                    instancesOfPriority++;
                }
            }
        }
        CheckValidity();
    }

    void CheckValidity()
    {
        int symbol1 = (100 * statementOccurrences[1]) + (10 * statementOccurrences[6]) + statementOccurrences[11];
        int symbol2 = (100 * statementOccurrences[2]) + (10 * statementOccurrences[7]) + statementOccurrences[12];
        int symbol3 = (100 * statementOccurrences[3]) + (10 * statementOccurrences[8]) + statementOccurrences[13];
        int symbol4 = (100 * statementOccurrences[4]) + (10 * statementOccurrences[9]) + statementOccurrences[14];
        if(symbol1 == 0)
        {
            symbol1 = -1;
        }
        if(symbol2 == 0)
        {
            symbol2 = -1;
        }
        if(symbol3 == 0)
        {
            symbol3 = -1;
        }
        if(symbol4 == 0)
        {
            symbol4 = -1;
        }

        if ( ((symbol1 % symbol2) == 0) ||
             ((symbol1 % symbol3) == 0) ||
             ((symbol1 % symbol4) == 0) ||
             ((symbol2 % symbol3) == 0) ||
             ((symbol2 % symbol4) == 0) ||
             ((symbol3 % symbol4) == 0) ||
             ((symbol2 % symbol1) == 0) ||
             ((symbol3 % symbol1) == 0) ||
             ((symbol4 % symbol1) == 0) ||
             ((symbol3 % symbol2) == 0) ||
             ((symbol4 % symbol2) == 0) ||
             ((symbol4 % symbol3) == 0) ||
             (Math.Min(Math.Min(symbol1, symbol2), Math.Min(symbol3, symbol4)) < 0 )  )
             {  invalid = true; }

        if(!invalid)
        {
            for(int i = 0; i <= 2; i++)
            {
                hieroglyphsSum[i].text = hieroglyphsSumInt[i].ToString();
            }
            LogInformation();
            PickLockHieroglyphs();
            CalculateLockPositions();
            CalculatePressTime();
        }
        else
        {
            Start();
        }
    }

    void LogInformation()
    {
        Debug.LogFormat("[Hieroglyphics #{0}] Your 5 chosen hieroglyphics are {1} (1 point), {2} (2 points), {3} (3 points), {4} (4 points) & {5} (5 points).", moduleId, chosenHieroglyphNames[0], chosenHieroglyphNames[1], chosenHieroglyphNames[2], chosenHieroglyphNames[3], chosenHieroglyphNames[4]);
        Debug.LogFormat("[Hieroglyphics #{0}] The 3-character hieroglyphic sum is {1}. The 4-character hieroglyphic sum is {2}. The 5-character hieroglyphic sum is {3}.", moduleId, hieroglyphsSumInt[0], hieroglyphsSumInt[1], hieroglyphsSumInt[2]);
    }

    void PickLockHieroglyphs()
    {
        for(int i = 0; i <= 1; i++)
        {
            int index = UnityEngine.Random.Range(1,5);
            while(chosenIndices.Contains(index))
            {
                index = UnityEngine.Random.Range(1,5);
            }
            chosenIndices.Add(index);
            lockText[i*2].text = chosenHieroglyphs[index];
            lockValue[i*2] = index+1;
            if(priorityHieroglyphValue - 1 == index)
            {
                instancesOfPriority++;
            }
        }
        chosenIndices.Clear();
        for(int i = 0; i <= 1; i++)
        {
            int index = UnityEngine.Random.Range(1,5);
            while(chosenIndices.Contains(index))
            {
                index = UnityEngine.Random.Range(1,5);
            }
            chosenIndices.Add(index);
            lockText[i*2+1].text = chosenHieroglyphs[index];
            lockValue[i*2+1] = index+1;
            if(priorityHieroglyphValue - 1 == index)
            {
                instancesOfPriority++;
            }
        }
        chosenIndices.Clear();
    }

    void CalculateLockPositions()
    {
        if((lockValue[0] == 2 && lockValue[2] == 3) || (lockValue[0] == 3 && lockValue[2] == 2) || (lockValue[0] == 3 && lockValue[2] == 5) || (lockValue[0] == 5 && lockValue[2] == 3))
        {
            correctLockPosition[0] = 0;
        }
        else if((lockValue[0] == 2 && lockValue[2] == 4) || (lockValue[0] == 4 && lockValue[2] == 2) || (lockValue[0] == 3 && lockValue[2] == 4) || (lockValue[0] == 4 && lockValue[2] == 3))
        {
            correctLockPosition[0] = 1;
        }
        else
        {
            correctLockPosition[0] = 2;
        }

        if((lockValue[1] == 2 && lockValue[3] == 5) || (lockValue[1] == 5 && lockValue[3] == 2) || (lockValue[1] == 3 && lockValue[3] == 4) || (lockValue[1] == 4 && lockValue[3] == 3))
        {
            correctLockPosition[1] = 0;
        }
        else if((lockValue[1] == 2 && lockValue[3] == 3) || (lockValue[1] == 3 && lockValue[3] == 2) || (lockValue[1] == 5 && lockValue[3] == 4) || (lockValue[1] == 4 && lockValue[3] == 5))
        {
            correctLockPosition[1] = 1;
        }
        else
        {
            correctLockPosition[1] = 2;
        }
        Debug.LogFormat("[Hieroglyphics #{0}] The Anubis lock values are {1} & {2}. Set the Anubis lock to the {3} position.", moduleId, lockValue[0], lockValue[2], lockPositionLog[correctLockPosition[0]]);
        Debug.LogFormat("[Hieroglyphics #{0}] The Horus lock values are {1} & {2}. Set the Horus lock to the {3} position.", moduleId, lockValue[1], lockValue[3], lockPositionLog[correctLockPosition[1]]);
        CheckPositions();
    }

    void CalculatePressTime()
    {
        correctPressTime = (((priorityHieroglyphValue * instancesOfPriority) - 1) % 9) + 1;
        Debug.LogFormat("[Hieroglyphics #{0}] The priority hieroglyphic is {1}. There are {2} instances of it in total. Set the locks and then press the seal when the last digit of the bomb timer is {3}.", moduleId, priorityHieroglyph, instancesOfPriority, correctPressTime);
    }

    public void PressMainButton()
    {
        if(moduleSolved || moving)
        {
            return;
        }
        mainButton.AddInteractionPunch();
        if(!positionsSet)
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[Hieroglyphics #{0}] Strike! The Anubis lock was set to {1} and the Horus lock was set to {2}. That is incorrect.", moduleId, lockPositionLog[pointerMoves[0]], lockPositionLog[pointerMoves[1]]);
        }
        else
        {
            pressTime = (Bomb.GetTime() % 60) % 10;
            pressTime = Mathf.FloorToInt(pressTime);
            if(pressTime == correctPressTime)
            {
                mainButton.enabled = false;
                moduleSolved = true;
                Debug.LogFormat("[Hieroglyphics #{0}] You pressed the seal when the last second of the bomb timer was {1}. That is correct. Module disarmed.", moduleId, correctPressTime);
                StartCoroutine(SolveAnimation());
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Hieroglyphics #{0}] Strike! You pressed the seal when the last second of the bomb timer was {1}. That is incorrect.", moduleId, pressTime);
            }
        }
    }

    IEnumerator SolveAnimation()
    {
        buttonNonAnim.SetActive(false);
        buttonAnim.SetActive(true);
        Audio.PlaySoundAtTransform("gong", transform);
        sealAnimation.SetTrigger("solved");
        yield return new WaitForSeconds(10f);
        GetComponent<KMBombModule>().HandlePass();
    }

    void ButtonPress(KMSelectable button)
    {
        if(moduleSolved || moving)
        {
            return;
        }
        button.AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("gear", transform);
        int pressedButtonIndex;
        moving = true;
        if(button == animationButtons[0])
        {
            pressedButtonIndex = 0;
        }
        else
        {
            pressedButtonIndex = 1;
        }
        pointerMoves[pressedButtonIndex]++;
        if(pointerMoves[pressedButtonIndex] == 1 || pointerMoves[pressedButtonIndex] == 3)
        {
            pointerAnimations[pressedButtonIndex].SetTrigger("toCentre");
        }
        else if(pointerMoves[pressedButtonIndex] == 2)
        {
            pointerAnimations[pressedButtonIndex].SetTrigger("toRight");
        }
        else if(pointerMoves[pressedButtonIndex] == 4)
        {
            pointerAnimations[pressedButtonIndex].SetTrigger("toLeft");
            pointerMoves[pressedButtonIndex] = 0;
        }
        CheckPositions();
        StartCoroutine(StopMoving());
    }

    IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(2.5f);
        moving = false;
    }

    void CheckPositions()
    {
        if(((pointerMoves[0] == correctLockPosition[0]) || (pointerMoves[0] == 3 && correctLockPosition[0] == 1)) && ((pointerMoves[1] == correctLockPosition[1]) || (pointerMoves[1] == 3 && correctLockPosition[1] == 1)))
        {
            positionsSet = true;
        }
        else
        {
            positionsSet = false;
        }
    }

    string TwitchHelpMessage = "Submit a full solution by using !{0} submit right and left on 8 or !{0} submit r,l,8 where locks are interacted in reading order. You may interact with each item individually using !{0} set anubis centre, !{0} set horus centre, and !{0} submit on 8.";

    IEnumerator DetermineButtonPress(string cmd, int btn)
    {
        if (!lockPositionLog.Contains(cmd))
            yield break;
        var move = Array.IndexOf(lockPositionLog, cmd);
        var point = pointerMoves[btn];
        if (point == 3 && move == 1)
            move = 3;
        if (move > point)
            move = move - point;
        else if (move < point)
            move = move + 4 - point;
        else
        {
            yield return null;
            yield break;
        }
        yield return null;
        for (int i = 0; i < move; i++)
        {
            yield return animationButtons[btn].OnInteract();
            yield return new WaitUntil(() => !moving);
        }
    }

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Replace("submit ", "").Replace("set ", "").Replace("center","centre");
        var list = new List<KMSelectable>();
        if (command.StartsWith("anubis") || command.StartsWith("horus"))
        {
            var dir = command.StartsWith("anubis") ? 0 : 1;
            command = command.Replace("anubis ", "").Replace("horus ", "");
            var compare = DetermineButtonPress(command, dir);
            while (compare.MoveNext())
                yield return compare.Current;
            yield break;
        }
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^(left|right|centre|l|r|c)(?: |)(?:and|&|)(?: |)(left|right|centre|l|r|c) on ([0-9])$");
        var split = new List<string>();
        if (regex.Match(command).Success)
        {
            foreach (System.Text.RegularExpressions.Group match in regex.Match(command).Groups)
                split.Add(match.Value);
            split.RemoveAt(0);
        }
        else
            split = command.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        split = split.Select(x => x.Substring(0, 1)).ToList();
        if (split.Count == 3)
        {
            for (int i = 0; i < 2; i++)
            {
                var index = lockPositionLog.First(x => x[0] == split[i][0]);
                if (index == null) yield break;
                var compare = DetermineButtonPress(index, i);
                while (compare.MoveNext())
                    yield return compare.Current;
            }
            command = "on " + split.Last();
        }
        int num;
        if (command.StartsWith("on ") && command.Length == 4 && int.TryParse(command[3].ToString(), out num))
        {
            while (!Mathf.FloorToInt(Bomb.GetTime() % 10).Equals(num));
                yield return "trycancel";
            yield return mainButton.OnInteract();
            yield return "solve";
        }
    }
}
