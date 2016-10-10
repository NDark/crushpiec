using UnityEngine;
using System.Collections;

public enum GameState
{
    Initization,
    NextBattle,
    Idle,
    WaitForInput,
    WaitForAnimation,
    ActionForCharacterAttack,
    ActionForCharacterDefend,
    ActionForMonster,
    ValidateVictory,
    Win,
    Lose,

    RequestNext,

    InValid,
}
public class BattleEventManager : DummyBattlePlay {
    static GameState m_State = GameState.InValid;
    Character m_CharacterRef = null;
    Character m_MonsterRef = null;
    public int m_SeedMax = 8;

    static int s_WaitTurnForMonsterMax = 2;
    int m_WaitTurnForMonsterAttack = s_WaitTurnForMonsterMax;

    // override interface
    public override bool IsInitialized()
    {
        return true;
    }

    public override bool IsInAnimation()
    {
        return true;
    }

    public override void StartBattle()
    {

    }

    public override void Attack(float _Ratio)
    {
        base.Attack(_Ratio);
        DoAttck(ref m_CharacterRef, ref m_MonsterRef, _Ratio);
        m_State = GameState.ActionForCharacterAttack;
    }

    public override void Defend(float _Ratio)
    {
        base.Defend(_Ratio);
        DoDefend(ref m_CharacterRef, _Ratio);
        m_State = GameState.ActionForCharacterDefend;
    }

    void DoAttck(ref Character _Hitter, ref Character _Receiver, float _Ratio)
    {
        int atk = (int)(_Hitter.atk.randomOneValue * _Ratio);
        int def = _Receiver.def.value;

        GlobalSingleton.DEBUG("DoAttck atk = " + atk + ",def = " + def);

        // min damage at least 1
        int damage = Mathf.Max(1, atk - def);

        _Receiver.hp.Offset(-damage);
        _Receiver.def.ToMin();
    }

    void DoDefend(ref Character _Receiver, float _Ratio)
    {
        int def = (int)(_Receiver.def.min * _Ratio);
        _Receiver.def.Offset(def);
    }

    void DoPlayAnimation(string _Object, string _Animation)
    {
        GameObject obj = GameObject.Find(_Object);
        if (obj != null) {
            Animation anim = obj.GetComponent<Animation>();
            if (anim != null) {
                anim.Play(_Animation);
            }    
        }
    }

    void DoUpdateCountDown()
    {
        GameObject coountDown = GameObject.Find("CountDownText");
        if (coountDown != null) {
            string cdStr = "CD " + (m_WaitTurnForMonsterAttack + 1);
            coountDown.GetComponent<UnityEngine.UI.Text>().text = cdStr;
        }
    }

    void DoCreateOneCharacter(ref Character _CharacterRef, bool _Random = true)
    {
        // if (_Random) {
        //    _CharacterRef.m_MeshName = "Unit_" + Random.Range(1, m_SeedMax);
        // }

        // re-create
        _CharacterRef.ReCreate();
        _CharacterRef.DoUpdateHP();
    }

    // Use this for initialization
    void Start () {
        m_CharacterRef = GameObject.Find("Unit_Character").GetComponent<Character>();
        m_MonsterRef = GameObject.Find("Unit_Monster").GetComponent<Character>();

        m_State = GameState.Initization;
    }

    // Update is called once per frame
    void Update () {
        switch (m_State)
        {
            case GameState.Initization:
                DoCreateOneCharacter(ref m_CharacterRef, false);
                DoCreateOneCharacter(ref m_MonsterRef, false);
                // DoUpdateCountDown();
                // m_CharacterRef.DoChangeModel(2, MODELTYPE.E_DEFENSE);

                m_State = GameState.Idle;
                break;

            case GameState.NextBattle:
                DoCreateOneCharacter(ref m_MonsterRef);
                DoUpdateCountDown();
                m_State = GameState.Idle;
                break;

            case GameState.Idle:
                m_State = GameState.WaitForInput;
                break;

            case GameState.WaitForInput:
                if (Input.GetKeyUp(KeyCode.A))
                {
                    m_CharacterRef.DoChangeModel(2, MODELTYPE.E_ATTACK);
                    m_MonsterRef.DoChangeModel(0, MODELTYPE.E_CONCENTRATE);
                }
                break;

            case GameState.ActionForCharacterAttack:
                GlobalSingleton.DEBUG("ActionForCharacterAttack");
                m_CharacterRef.DoAction(AnimationState.Attack);
                m_MonsterRef.DoAction(AnimationState.Hitted, TransformAnimation.Define.TIME_ATTACK);
                Invoke("OnActionForCharacterFinish", 1.0f);

                m_State = GameState.WaitForAnimation;
                break;

            case GameState.ActionForCharacterDefend:
                GlobalSingleton.DEBUG("ActionForCharacterDefend");
                m_CharacterRef.DoAction(AnimationState.Defend);
                Invoke("OnActionForCharacterFinish", 1.0f);

                m_State = GameState.WaitForAnimation;
                break;
                
            case GameState.ActionForMonster:
                GlobalSingleton.DEBUG("ActionForMonster");

                if (m_WaitTurnForMonsterAttack <= 0)
                {
                    m_MonsterRef.DoAction(AnimationState.Attack);
                    m_CharacterRef.DoAction(AnimationState.Hitted, TransformAnimation.Define.TIME_ATTACK);

                    DoAttck(ref m_MonsterRef, ref m_CharacterRef, Random.Range(0.15f, 0.75f));

                    m_WaitTurnForMonsterAttack = s_WaitTurnForMonsterMax;
                }

                --m_WaitTurnForMonsterAttack;

                Invoke("OnActionForMonsterFinish", 1.0f);

                m_State = GameState.WaitForAnimation;
                break;

            case GameState.ValidateVictory:
                GlobalSingleton.DEBUG(
                    "ValidateVictory : Character HP = "
                   + m_CharacterRef.hp.value
                   + ", Monster HP = "
                   + m_MonsterRef.hp.value);

                if (m_CharacterRef.hp.value <= 0)
                {
                    m_State = GameState.Lose;
                }
                else if (m_MonsterRef.hp.value <= 0)
                {
                    m_State = GameState.Win;
                }
                else
                {
                    m_State = GameState.RequestNext;
                }                
                break;

            case GameState.Win:
                GlobalSingleton.DEBUG("Win");
                DoPlayAnimation("Victory", "Language_Victory_Show");
                Invoke("OnWinAnimationFinish", 1.0f);
                m_State = GameState.WaitForAnimation;
                break;

            case GameState.Lose:
                GlobalSingleton.DEBUG("Lose");
                DoPlayAnimation("Lose", "Language_Lose_Show");
                Invoke("OnLoseAnimationFinish", 1.0f);
                m_State = GameState.WaitForAnimation;
                break;

            case GameState.RequestNext:
                GlobalSingleton.DEBUG("RequestNext");
                m_State = GameState.Idle;
                break;
        } // End for switch
	}

    void OnActionForCharacterFinish() {
        m_MonsterRef.DoUpdateHP();
        m_State = GameState.ActionForMonster;
    }

    void OnActionForMonsterFinish() {
        DoUpdateCountDown();
        m_CharacterRef.DoUpdateHP();
        m_State = GameState.ValidateVictory;
    }

    void OnWinAnimationFinish() {
        m_State = GameState.NextBattle;
    }

    void OnLoseAnimationFinish() {
        m_State = GameState.Initization;
    }
}
