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

		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "Police killed a man." , 6 ) ) ;
		list.Add( new AnswerChoice( "police kill person" , 3 ) ) ;
		list.Add( new AnswerChoice( "police kill people" , 2 ) ) ;
		list.Add( new AnswerChoice( "police kill a people" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "警察殺死人" , list ) ) ;

		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "全球股市" , 6 ) ) ;
		list.Add( new AnswerChoice( "全球證券市場" , 3 ) ) ;
		list.Add( new AnswerChoice( "全球股份市場" , 2 ) ) ;
		list.Add( new AnswerChoice( "全球儲積市場" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "global stock market" , list ) ) ;


		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "古典與現代" , 6 ) ) ;
		list.Add( new AnswerChoice( "經典與現代" , 3 ) ) ;
		list.Add( new AnswerChoice( "經典與同時" , 2 ) ) ;
		list.Add( new AnswerChoice( "一流與輩份" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "classic and contemporary" , list ) ) ;


		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "有幾個人還活著" , 6 ) ) ;
		list.Add( new AnswerChoice( "一些人還活著" , 3 ) ) ;
		list.Add( new AnswerChoice( "人少數仍然活著" , 2 ) ) ;
		list.Add( new AnswerChoice( "少數人誰仍然活著" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "A few people are still alive" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "他們解決了母親節的家庭問題" , 6 ) ) ;
		list.Add( new AnswerChoice( "他們解決了他們的母親節的家庭問題" , 3 ) ) ;
		list.Add( new AnswerChoice( "在母親節他們解決了他們的家庭問題" , 2 ) ) ;
		list.Add( new AnswerChoice( "在母親節他們解決了他們的一些家庭問題" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "They solve their family problems in Mother's Day" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "很快以五比二落後" , 6 ) ) ;
		list.Add( new AnswerChoice( "很快落後五比二損失" , 3 ) ) ;
		list.Add( new AnswerChoice( "很快以五減二落後" , 2 ) ) ;
		list.Add( new AnswerChoice( "很快以五之二落後" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "fall behind quickly in 5-2 loss" , list ) ) ;
		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "留個好印象" , 6 ) ) ;
		list.Add( new AnswerChoice( "製造一個好印象" , 3 ) ) ;
		list.Add( new AnswerChoice( "製造一個好凹槽" , 2 ) ) ;
		list.Add( new AnswerChoice( "製造一個好印章" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "make a good impression" , list ) ) ;


		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "重力波" , 6 ) ) ;
		list.Add( new AnswerChoice( "引力浪" , 3 ) ) ;
		list.Add( new AnswerChoice( "地心引力波浪" , 2 ) ) ;
		list.Add( new AnswerChoice( "地心引力招呼" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "gravitational waves" , list ) ) ;
		

		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "氣候變化" , 6 ) ) ;
		list.Add( new AnswerChoice( "趨勢變化" , 3 ) ) ;
		list.Add( new AnswerChoice( "氣候變更" , 2 ) ) ;
		list.Add( new AnswerChoice( "趨勢轉變" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "climate change" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "瑞士科學家開發出無人救援機" , 6 ) ) ;
		list.Add( new AnswerChoice( "瑞士科學家開發出無人駕駛飛機救援" , 3 ) ) ;
		list.Add( new AnswerChoice( "瑞士研究員開發出無人駕駛飛機救援" , 2 ) ) ;
		list.Add( new AnswerChoice( "瑞士科學家開發救援機器" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "Swiss researchers develop rescue drone" , list ) ) ;

			
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "我應該吃宇航員" , 6 ) ) ;
		list.Add( new AnswerChoice( "宇航員應該吃什麼" , 3 ) ) ;
		list.Add( new AnswerChoice( "太空人應該吃什麼" , 2 ) ) ;
		list.Add( new AnswerChoice( "什麼應該太空人吃" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "What should astronauts eat" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "去氧核醣核酸可能增加憂鬱症的風險" , 6 ) ) ;
		list.Add( new AnswerChoice( "去氧核醣核酸可能會影響憂鬱症的風險" , 3 ) ) ;
		list.Add( new AnswerChoice( "DNA可能會影響現代抑鬱症風險" , 2 ) ) ;
		list.Add( new AnswerChoice( "DNA可能會影響現代抑鬱症" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "DNA may influence modern depression risk" , list ) ) ;
		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "健身越多，越聰明" , 6 ) ) ;
		list.Add( new AnswerChoice( "健身越久，越聰明" , 3 ) ) ;
		list.Add( new AnswerChoice( "健身越多，腦越大" , 2 ) ) ;
		list.Add( new AnswerChoice( "鍛煉的時間越長，大腦中的更大" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "The longer the workout, the bigger the brain" , list ) ) ;
		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "Comprehensive water from households without water tonight." , 6 ) ) ;
		list.Add( new AnswerChoice( "Households will be suplied water tonight." , 3 ) ) ;
		list.Add( new AnswerChoice( "Villiages will have water tonight." , 2 ) ) ;
		list.Add( new AnswerChoice( "Households will have water tonight." , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "停水戶今晚起全面供水" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "condom" , 6 ) ) ;
		list.Add( new AnswerChoice( "condo" , 3 ) ) ;
		list.Add( new AnswerChoice( "comdum" , 2 ) ) ;
		list.Add( new AnswerChoice( "camdum" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "保險套" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "suspect" , 6 ) ) ;
		list.Add( new AnswerChoice( "victim" , 3 ) ) ;
		list.Add( new AnswerChoice( "laywer" , 2 ) ) ;
		list.Add( new AnswerChoice( "police" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "凶嫌" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "Fallen flowers small indeed fortunate." , 6 ) ) ;
		list.Add( new AnswerChoice( "Fallen flowers binbin with small luck." , 3 ) ) ;
		list.Add( new AnswerChoice( "Fallen flowers spark with small indeed." , 2 ) ) ;
		list.Add( new AnswerChoice( "Fallen Ing sparks with small indeed." , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "落英繽紛小確幸" , list ) ) ;

		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "fire alarm" , 6 ) ) ;
		list.Add( new AnswerChoice( "fire police" , 3 ) ) ;
		list.Add( new AnswerChoice( "flame raider" , 2 ) ) ;
		list.Add( new AnswerChoice( "fire" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "火警" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "wake up at two o'clock" , 6 ) ) ;
		list.Add( new AnswerChoice( "Two in the morning to get up" , 3 ) ) ;
		list.Add( new AnswerChoice( "get up at two" , 2 ) ) ;
		list.Add( new AnswerChoice( "morning two get up" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "凌晨兩點就起床" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "air pollution" , 6 ) ) ;
		list.Add( new AnswerChoice( "climate poison" , 3 ) ) ;
		list.Add( new AnswerChoice( "climate dirty" , 2 ) ) ;
		list.Add( new AnswerChoice( "air dirty" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "空氣污染" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "gentleman" , 6 ) ) ;
		list.Add( new AnswerChoice( "hentai" , 3 ) ) ;
		list.Add( new AnswerChoice( "crimnal" , 2 ) ) ;
		list.Add( new AnswerChoice( "police" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "紳士" , list ) ) ;

		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "big part" , 6 ) ) ;
		list.Add( new AnswerChoice( "big body" , 3 ) ) ;
		list.Add( new AnswerChoice( "dead body" , 2 ) ) ;
		list.Add( new AnswerChoice( "ghost rider" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "大體" , list ) ) ;
		
		list = new List<AnswerChoice>() ;
		list.Add( new AnswerChoice( "carbon monoxide" , 6 ) ) ;
		list.Add( new AnswerChoice( "one o2 with ca" , 3 ) ) ;
		list.Add( new AnswerChoice( "one oxgen and carben" , 2 ) ) ;
		list.Add( new AnswerChoice( "one oxgen with carbon" , 1 ) ) ;
		m_Questions.Add( new QuestionAndAnswers( ++id , "一氧化碳" , list ) ) ;







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
