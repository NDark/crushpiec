using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LanguageUIState
{
	LanguageUIState_Invalid = 0 ,
	LanguageUIState_StartInitialize ,
	LanguageUIState_WaitInitialize ,
	LanguageUIState_StartRequestQuestion ,
	LanguageUIState_WaitRequestQuestion ,
	LanguageUIState_UpdateQuestionToGUI ,
	LanguageUIState_WaitPlayer ,
	LanguageUIState_WaitBattlePlay ,

	LanguageUIState_StartRequestAddAnswer ,
	LanguageUIState_WaitRequestAddAnswer ,

}

public class LanguageManager : MonoBehaviour 
{
	public Text m_AttackQuestionLabel = null ;
	public InputField m_AttackInputField = null;
	
	public Text m_DefendQuestionLabel = null ;
	public Text [] m_DenfendButtonText ;


	public DummyBattlePlay m_BattlePlay = null ;
	public DummyServerRequester m_Server = null ;
	public LanguageUIState m_State = LanguageUIState.LanguageUIState_Invalid  ;

	public QuestionAndAnswers m_AttackQuestion = null ;
	public QuestionAndAnswers m_DefendQuestion = null ;

	public void TryDoAttack()
	{
		Debug.Log("TryDoAttack");
		if (m_State != LanguageUIState.LanguageUIState_WaitPlayer )
		{
			return ;
		}

		if( null != m_AttackInputField )
		{
			string stringFromInput = m_AttackInputField.text ;
			Debug.Log("stringFromInput=" + stringFromInput );
			if( m_BattlePlay )
			{
				float ratio = m_AttackQuestion.TryCalculateRatioOfAnAnswer( stringFromInput ) ;
				m_BattlePlay.Attack( ratio * 2 ) ;
			}
			m_State = LanguageUIState.LanguageUIState_StartRequestAddAnswer ;
		}
	}
	public void TryDoDefend0()
	{
		Debug.Log("TryDoDefend0");
		TryDoDefend( 0 );
	}
	public void TryDoDefend1()
	{
		Debug.Log("TryDoDefend1");
		TryDoDefend( 1 );
	}
	public void TryDoDefend2()
	{
		Debug.Log("TryDoDefend2");
		TryDoDefend( 2 );
	}
	public void TryDoDefend3()
	{
		Debug.Log("TryDoDefend3");
		TryDoDefend( 3 );
	}
	private void TryDoDefend( int _Index )
	{
		if (m_State != LanguageUIState.LanguageUIState_WaitPlayer )
		{
			return ;
		}
		if( m_BattlePlay )
		{
			float ratio = m_AttackQuestion.CalculateRatioOfIndex( _Index ) ;
			m_BattlePlay.Defend( ratio  * 2 ) ;
		}
		m_State = LanguageUIState.LanguageUIState_WaitBattlePlay ;
	}


	// Use this for initialization
	void Start () {
		m_BattlePlay = this.GetComponent<DummyBattlePlay>() ;
		m_Server = this.GetComponent<DummyServerRequester>() ;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_State )
		{
		case LanguageUIState.LanguageUIState_Invalid :
			m_State = LanguageUIState.LanguageUIState_StartInitialize ;
			break ;
		case LanguageUIState.LanguageUIState_StartInitialize :
			Process_StartInitialize() ;
			break ;
		case LanguageUIState.LanguageUIState_WaitInitialize :
			Process_WaitInitialize() ;
			break ;

		case LanguageUIState.LanguageUIState_StartRequestQuestion :
			Process_StartRequestQuestion() ;
			break ;
		case LanguageUIState.LanguageUIState_WaitRequestQuestion :
			Process_WaitRequestQuestion() ;
			break ;

		case LanguageUIState.LanguageUIState_UpdateQuestionToGUI :
			Process_UpdateQuestionToGUI() ;
			break ;
		case LanguageUIState.LanguageUIState_WaitPlayer :
			break ;
		case LanguageUIState.LanguageUIState_WaitBattlePlay :
			Process_WaitBattlePlay() ;
			break ;

		case LanguageUIState.LanguageUIState_StartRequestAddAnswer :
			Process_StartRequestAddAnswer() ;
			break ;
		case LanguageUIState.LanguageUIState_WaitRequestAddAnswer :
			Process_WaitRequestAddAnswer() ;
			break ;


		}
	
	}

	private void Process_StartInitialize()
	{
		this.m_Server.RequestQuestions() ;
		m_State = LanguageUIState.LanguageUIState_WaitInitialize ;
	}

	private void Process_WaitInitialize()
	{
		m_State = LanguageUIState.LanguageUIState_StartRequestQuestion ;
	}

	private void Process_StartRequestQuestion()
	{
		if( m_Server )
		{
			m_AttackQuestion = m_Server.GetAQuestion() ;
			m_DefendQuestion = m_Server.GetAQuestion() ;
		}
		m_State = LanguageUIState.LanguageUIState_WaitRequestQuestion ;
	}
	
	private void Process_WaitRequestQuestion()
	{
		m_State = LanguageUIState.LanguageUIState_UpdateQuestionToGUI ;
	}

	private void Process_UpdateQuestionToGUI()
	{
		m_AttackQuestionLabel.text = this.m_AttackQuestion.QuestionString ;
		this.m_AttackInputField.text = "" ;
		m_DefendQuestionLabel.text = this.m_DefendQuestion.QuestionString ;

		for( int i = 0 ;  i < m_DenfendButtonText.Length ; ++i )
		{
			if( i < this.m_DefendQuestion.m_Answers.Count )
			{
				m_DenfendButtonText[i].text = m_DefendQuestion.m_Answers[ i ].AnswerString ;
			}
			else
			{
				m_DenfendButtonText[i].text = "" ;
			}
		}

		m_State = LanguageUIState.LanguageUIState_WaitPlayer ;
	}

	private void Process_WaitBattlePlay()
	{

		if( m_BattlePlay && false == m_BattlePlay.IsInAnimation() )
		{
			m_State = LanguageUIState.LanguageUIState_StartRequestQuestion ;
		}

	}

	private void Process_StartRequestAddAnswer()
	{
		if( string.Empty == this.m_AttackInputField.text )
		{
			return ;
		}

		if( m_Server )
		{
			m_Server.RequestAddAnswer( m_AttackQuestion.ID , this.m_AttackInputField.text ) ;

		}
		m_State = LanguageUIState.LanguageUIState_WaitRequestAddAnswer ;
	}

	private void Process_WaitRequestAddAnswer()
	{
		m_State = LanguageUIState.LanguageUIState_WaitBattlePlay ;
	}
}
