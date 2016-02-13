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
	public int [] m_DefendRandomMapper = new int[4] ;

	public DummyBattlePlay m_BattlePlay = null ;
	public DummyServerRequester m_Server = null ;
	public LanguageUIState m_State = LanguageUIState.LanguageUIState_Invalid  ;

	public QuestionAndAnswers m_AttackQuestion = null ;
	public QuestionAndAnswers m_DefendQuestion = null ;
	public Animation m_StartActionAnimaton = null ;

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
			StartActionAnimation( false ) ;
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
		int actuallyIndexFromAnswer = m_DefendRandomMapper[ _Index ] ;
		Debug.Log("_Index=" + _Index + " actuallyIndexFromAnswer=" +actuallyIndexFromAnswer);
		if (m_State != LanguageUIState.LanguageUIState_WaitPlayer )
		{
			return ;
		}
		if( m_BattlePlay )
		{
			float ratio = m_DefendQuestion.CalculateRatioOfIndex( actuallyIndexFromAnswer ) ;
			m_BattlePlay.Defend( ratio  * 2 ) ;
		}
		StartActionAnimation( false ) ;
		m_State = LanguageUIState.LanguageUIState_WaitBattlePlay ;
	}


	// Use this for initialization
	void Start () {
		// m_BattlePlay = this.GetComponent<DummyBattlePlay>() ;
		m_Server = this.GetComponent<DummyServerRequester>() ;

		for( int i = 0 ; i < m_DefendRandomMapper.Length ; ++i )
		{
			m_DefendRandomMapper[ i ] = i ;
		}
	
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
		if( null != m_Server )
		{
			this.m_Server.RequestQuestions() ;
			m_State = LanguageUIState.LanguageUIState_WaitInitialize ;
		}

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

		RandomizeMapper() ;

		int actuallyIndexFromAnswer = 0 ;
		for( int i = 0 ;  i < m_DenfendButtonText.Length && i < m_DefendRandomMapper.Length ; ++i )
		{

			actuallyIndexFromAnswer = m_DefendRandomMapper[ i ] ;
			// Debug.Log("button i=" + i + " actuallyIndexFromAnswer=" + actuallyIndexFromAnswer );

			if( i < this.m_DefendQuestion.m_Answers.Count )
			{
				m_DenfendButtonText[ i ].text = m_DefendQuestion.m_Answers[ actuallyIndexFromAnswer ].AnswerString ;
			}
			else
			{
				m_DenfendButtonText[i].text = "" ;
			}
		}

		StartActionAnimation( true ) ;
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

	private void RandomizeMapper()
	{
		int randomMax = 10 ;
		int rangeMax = this.m_DefendRandomMapper.Length ;
		int index1 ;
		int index2 ;
		int tmp ;
		for( int i = 0 ; i < randomMax ; ++i )
		{
			index1 = Random.Range( 0 , rangeMax ) ;
			index2 = Random.Range( 0 , rangeMax ) ;

			tmp = m_DefendRandomMapper[ index1 ] ;
			m_DefendRandomMapper[ index1 ] = m_DefendRandomMapper[ index2 ] ;
			m_DefendRandomMapper[ index2 ] = tmp ;
		}
	}

	private void StartActionAnimation( bool _Show ) 
	{
		if( null == m_StartActionAnimaton )
		{
			return ;
		}

		if( true == _Show )
		{
			m_StartActionAnimaton.Play( "Language_StartAction_Show" ) ;
		}
		else
		{
			m_StartActionAnimaton.Play( "Language_StartAction_Hide" ) ;
		}
	}
}
