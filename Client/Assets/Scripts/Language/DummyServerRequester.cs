using UnityEngine;
using System;
using System.Collections.Generic;

public class AnswerChoice : IEquatable<AnswerChoice> , IComparable<AnswerChoice>

{
	public string AnswerString { get; set; }
	public int Count { get;set; }
	
	public AnswerChoice( string _AnswerString , int _Count )
	{
		AnswerString = _AnswerString ; 
		Count  = _Count ;
	}

	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		AnswerChoice objAsPart = obj as AnswerChoice;
		if (objAsPart == null) return false;
		else return Equals(objAsPart);
	}

	public bool Equals(AnswerChoice other)
	{
		if (other == null) return false;
		return (this.Count.Equals(other.Count));
	}
	public override int GetHashCode()
	{
		return this.Count ;
	}
	public int CompareTo(AnswerChoice comparePart)
	{
		// A null value means that this object is greater.
		if (comparePart == null)
			return 1;
		
		else
			return comparePart.Count.CompareTo( this.Count );
	}

}

public class QuestionAndAnswers
{
	public int ID {get;set;}
	public string QuestionString { get; set; }
	public List<AnswerChoice> m_Answers = new List<AnswerChoice>() ;

	public QuestionAndAnswers( int _ID , string _Question , List<AnswerChoice> _Answers )
	{
		this.ID = _ID ;
		QuestionString = _Question ; 
		m_Answers  = _Answers ;
	}

	public float TryCalculateRatioOfAnAnswer( string _Answer )
	{
		float ret = 0.0f ;
		int targetCount = 1 ; 
		int sum = 0 ;

		{
			for( int i = 0 ; i < m_Answers.Count ; ++i )
			{
				if( m_Answers[i].AnswerString == _Answer )
				{
					targetCount = m_Answers[ i ].Count ;
				}
				sum += m_Answers[ i ].Count ;
			}
			Debug.Log("targetCount=" + targetCount);
			if( 0 != sum )
			{
				ret = (float)targetCount / (float)sum ;
			}
		}
		return ret ;
	}

	public float CalculateRatioOfIndex( int _Index )
	{
		float ret = 0.0f ;
		int targetCount = 0 ; 
		int sum = 0 ;
		Debug.Log("CalculateRatioOfIndex _Index=" + _Index );
		if( _Index < m_Answers.Count )
		{
			for( int i = 0 ; i < m_Answers.Count ; ++i )
			{
				if( i == _Index )
				{
					targetCount = m_Answers[ i ].Count ;
				}
				sum += m_Answers[ i ].Count ;
			}

			if( 0 != sum )
			{
				ret = (float)targetCount / (float)sum ;
			}
		}
		return ret ;
	}

}

public class DummyServerRequester : MonoBehaviour 
{

	public virtual QuestionAndAnswers GetAQuestion()
	{
		int index = UnityEngine.Random.Range( 0 , this.m_Questions.Count ) ;
		return this.m_Questions[ index ] ;
	}

	public virtual void RequestAddAnswer( int _QuestionID , string _AnswerString )
	{
		int index = GetAQuestionIndexByID( _QuestionID ) ;
		if( -1 == index )
		{
			return ;
		}

		QuestionAndAnswers target = this.m_Questions[ index ] ;
		bool isNewAnswer = true ;
		for( int i = 0 ; i < target.m_Answers.Count ; ++i )
		{
			if( target.m_Answers[ i ].AnswerString == _AnswerString )
			{
				isNewAnswer = false ;
				target.m_Answers[ i ].Count += 1 ;
			}
		}

		if( isNewAnswer )
		{
			target.m_Answers.Add( new AnswerChoice( _AnswerString , 1 ) ) ;
		}
		Debug.Log("RequestAddAnswer()");
		target.m_Answers.Sort() ;
	}

	public virtual void RequestQuestions()
	{
		InitializeQuestion() ;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void InitializeQuestion()
	{
		Debug.Log("InitializeQuestion");

		m_Questions.Clear() ;
		int id = 0 ;
		List<AnswerChoice> list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "宣佈" , 4 ) ) ;
		list.Add( new AnswerChoice( "公佈" , 3 ) ) ;
		list.Add( new AnswerChoice( "通告" , 2 ) ) ;
		list.Add( new AnswerChoice( "發表" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "Announce" , list ) ) ;
		/*
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "Police killed a man." , 60 ) ) ;
		list.Add( new AnswerChoice( "police kill person" , 30 ) ) ;
		list.Add( new AnswerChoice( "police kill people" , 10 ) ) ;
		list.Add( new AnswerChoice( "police kill a people" , 5 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "警察殺死人" , list ) ) ;

		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "全球股市" , 60 ) ) ;
		list.Add( new AnswerChoice( "全球證券市場" , 30 ) ) ;
		list.Add( new AnswerChoice( "全球股份市場" , 10 ) ) ;
		list.Add( new AnswerChoice( "全球儲積市場" , 5 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "global stock market" , list ) ) ;
*/



	}

	protected QuestionAndAnswers GetAQuestionByIndex( int _Index )
	{
		if( _Index >= this.m_Questions.Count )
		{
			return null ;
		}
		return this.m_Questions[ _Index ] ;
	}

	protected int GetAQuestionIndexByID( int _ID )
	{
		int ret = -1 ;
		for( int i = 0 ; i < this.m_Questions.Count ; ++i )
		{
			if( this.m_Questions[ i ].ID == _ID )
			{
				ret = i ;
				break ;
			}
		}
		return ret ;
	}


	protected List<QuestionAndAnswers> m_Questions = new List<QuestionAndAnswers>() ;

}
