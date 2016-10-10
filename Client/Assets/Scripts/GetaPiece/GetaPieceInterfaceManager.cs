
// #define DEBUG_JUDGE_VICTORY
// #define DEBUG_WAIT_ANIMATION
using UnityEngine;
using System.Collections.Generic;


public enum GetaPieceInterfaceState
{
	UnActive = 0 ,
	InterfaceInitialize ,
	BattleInitialize ,
	WaitBattleInitialize ,

	EnterRound ,
	WaitPlayerInput ,
	EnterAnimation ,
	WaitAnimation ,
	JudgeVictory ,

	EndGame ,
}

public class GetaPieceInterfaceManager : MonoBehaviour 
{
	public GameObject m_UnitDataGameObject = null ;
	public GetaPieceUnitData m_Player = null ;
	public GetaPieceUnitData m_Enemy = null ;
	public Queue<GetaPieceBattleEvent> m_BattleEvents = null ;

	public DummyBattlePlay m_Battle = null ;

	public UnityEngine.UI.Image [] m_Component0ButtonsImages = null ;
	public UnityEngine.UI.Image [] m_Component1ButtonsImages = null ;
	public UnityEngine.UI.Image [] m_Component2ButtonsImages = null ;
	public UnityEngine.UI.Image [] m_ComponentSwordImages = null ;
	
	public UnityEngine.UI.Image m_EnergyLabelBackground = null ;
	public UnityEngine.UI.Text m_EnergyLabel = null ;
	public GameObject m_EnergyGridParent = null ;
	public List<UnityEngine.UI.Image> m_EnergyGrids = new List<UnityEngine.UI.Image>() ;
	public GameObject m_StartButton = null ;

	public Animation m_Victory = null ;
	public Animation m_Lose = null ;
	public Animation m_InAttackBlockBackground = null ;

	public RectTransform m_HitPointPlayer = null ;
	public RectTransform m_HitPointEnemy = null ;

	private ActionKey [] m_SelectedActions = new ActionKey[3] ;


	public void TrySetAction0( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;

		PressComponentButton( 0 , key ) ;

	}

	public void TrySetAction1( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;

		PressComponentButton( 1 , key ) ;

	}

	public void TrySetAction2( string _ActionStr )
	{
		ActionKey key = m_ActionKeyEnumHelper.GetKey( _ActionStr ) ;
		PressComponentButton( 2 , key ) ;

	}
	
	public void SetAction( int _ComponentIndex , ActionKey _Action )
	{
		// Debug.Log("SetAction() _ComponentIndex=" + _ComponentIndex + " _Action=" + _Action );
		m_SelectedActions[ _ComponentIndex ] = _Action ;

		int currentCostEnergy = CalculateCostEnergyFromInput() ;
		SetEnergyGrid( m_Player.Energy , currentCostEnergy , 0 ) ;
	}

	
	public void TryStart()
	{
		Debug.Log("TryStart" );

		// copy select actions to player
		for( int i = 0 ; i < m_SelectedActions.Length ; ++i )
		{
			m_Player.m_Action[ i ] = m_SelectedActions[ i ] ;
		}

		CalculateActionsOfEnemy() ;

		m_InAttackBlockBackground.Blend("Language_StartAction_Show");

		m_State = GetaPieceInterfaceState.EnterAnimation ;
	}

	
	// Update is called once per frame
	void Update () 
	{
	

		switch( m_State ) 
		{
		case GetaPieceInterfaceState.UnActive :
			m_State = GetaPieceInterfaceState.InterfaceInitialize ;
			break ;
		case GetaPieceInterfaceState.InterfaceInitialize :
			InterfaceInitialize() ;
			break ;
		case GetaPieceInterfaceState.BattleInitialize :
			BattleInitialize() ;
			break ;
		case GetaPieceInterfaceState.WaitBattleInitialize :
			WaitBattleInitialize() ;
			break ;
		case GetaPieceInterfaceState.EnterRound :
			EnterRound() ;
			break ;
		// case GetaPieceInterfaceState.WaitPlayerInput : break ;

		case GetaPieceInterfaceState.EnterAnimation :
			EnterAnimation() ;
			break ;
		case GetaPieceInterfaceState.WaitAnimation :
			WaitAnimation() ;
			break ;
		case GetaPieceInterfaceState.JudgeVictory :
			JudgeVictory() ;
			break ;
		case GetaPieceInterfaceState.EndGame : break ;


		}

	}


	private void InterfaceInitialize()
	{
		if( null == m_EnergyGridParent )
		{
			Debug.LogError("null == m_EnergyGridParent");
			return ;
		}
		
		if( null == m_UnitDataGameObject )
		{
			Debug.LogError("null == m_UnitDataGameObject");
			return ;
		}

		InterfaceInitialize_Data() ;

		InterfaceInitialize_EnergyGrid() ;

		m_State = GetaPieceInterfaceState.BattleInitialize ;
	}
	
	private void InterfaceInitialize_Data()
	{
		GetaPieceUnitDataComponent unitDataComponent = m_UnitDataGameObject.GetComponent<GetaPieceUnitDataComponent>() ;
		if( null != unitDataComponent )
		{
			m_Player = unitDataComponent.m_Player ;
			m_Player.Reset() ;
			m_Enemy = unitDataComponent.m_Enemy ;
			m_Enemy.Reset() ;

			m_BattleEvents = unitDataComponent.m_BattleEvent ;
		}

		for( int i = 0 ; i < m_SelectedActions.Length ; ++i )
		{
			m_SelectedActions[ i ] = ActionKey.Concentrate ;
		}

	}
	
	private void InterfaceInitialize_EnergyGrid()
	{
		UnityEngine.UI.Image image = null ;
		Transform trans = null ;
		int size = 10 ;
		for( int i = 0 ; i < size ; ++i )
		{
			trans = m_EnergyGridParent.transform.FindChild("E" + i.ToString() ) ;
			if( null == trans )
			{
				Debug.LogWarning("null == trans i=" + i );
				continue ;
			}
			
			image = trans.gameObject.GetComponent<UnityEngine.UI.Image>() ;
			if( null != image )
			{
				m_EnergyGrids.Add( image ) ;
			}
			
		}
		// Debug.Log("m_EnergyGrids.Count=" + m_EnergyGrids.Count );
	}

	private void BattleInitialize()
	{
		m_State = GetaPieceInterfaceState.WaitBattleInitialize ;
	}
	
	private void WaitBattleInitialize()
	{
		m_State = GetaPieceInterfaceState.EnterRound ;
	}
	
	private void EnterRound()
	{
		CalculateEnergyRefill() ;

		PressComponentButton( 0 , ActionKey.Concentrate ) ;
		PressComponentButton( 1 , ActionKey.Concentrate ) ;
		PressComponentButton( 2 , ActionKey.Concentrate ) ;

		CheckPowerAttackPicture() ;

		m_InAttackBlockBackground.Blend("Language_StartAction_Hide");


		m_State = GetaPieceInterfaceState.WaitPlayerInput ;
	}

	private void CostEnergyForUnit( GetaPieceUnitData _Unit )
	{
		// calculate cost 
		int currentEnerty = m_Player.Energy ;
		int cost = m_Player.CalculateCostEnergy() ;
		int resultEnergy = currentEnerty - cost ;
		m_Player.Energy = resultEnergy ;
	}

	private void EnterAnimation()
	{

		// calculate cost 
		CostEnergyForUnit( m_Player ) ;
		SetEnergyGrid( m_Player.Energy , 0 , 0 ) ;

		m_Battle.StartBattle() ;

		m_State = GetaPieceInterfaceState.WaitAnimation ;
	}

#if DEBUG_WAIT_ANIMATION
	public bool DEBUG_IsBattleInAnimation = true ;
#endif 
// DEBUG_WAIT_ANIMATION

	private void WaitAnimation()
	{
		CheckBattleEvent() ;


#if DEBUG_WAIT_ANIMATION
		if( true == DEBUG_IsBattleInAnimation )
		{
			return ;
		}
#else
// DEBUG_WAIT_ANIMATION
		if( true == m_Battle.IsInAnimation() )
		{
			return ;
		}
#endif 
// DEBUG_WAIT_ANIMATION


		m_State = GetaPieceInterfaceState.JudgeVictory ;
	}

#if DEBUG_JUDGE_VICTORY
	public bool DEBUG_IsVicotryJudged = false ;
	public bool DEBUG_IsPlayerWin = false ;
#endif 
	// DEBUG_JUDGE_VICTORY
	private void JudgeVictory()
	{
		CalculateDamageFromData() ;
		UpdateHitPointFromDataOnce() ;

		CalculateDefendSucceed() ;

		bool isVicotryJudged = false ;
		bool isPlayerWin = false ;

		if( m_Enemy.HitPoint <= 0 )
		{
			isVicotryJudged = true ;
			isPlayerWin = true ;
		}
		else if( m_Player.HitPoint <= 0 )
		{
			isVicotryJudged = true ;
			isPlayerWin = false ;
		}

#if DEBUG_JUDGE_VICTORY
		isVicotryJudged = DEBUG_IsVicotryJudged ;
		isPlayerWin = DEBUG_IsPlayerWin ;
#endif 
// DEBUG_JUDGE_VICTORY
		if( isVicotryJudged )
		{
			if( isPlayerWin )
			{
				m_Victory.Blend("Language_Victory_Show");
			}
			else
			{
				m_Lose.Blend("Language_Victory_Show");
			}
			
			m_InAttackBlockBackground.Blend("Language_StartAction_Hide");

			m_State = GetaPieceInterfaceState.EndGame ;
		}
		else
		{
			m_State = GetaPieceInterfaceState.EnterRound ;
		}

		// after check hitpoint this turn
		CalculatePowerAttackEffect() ;
	}

	private void SetEnergyGrid( int _EnergyNow , int _PreCostValue , int _PreBuffValue )
	{
		int maxSize = m_EnergyGrids.Count ;
		int costedValue = _EnergyNow - _PreCostValue ;
		// Debug.Log("costedValue="+costedValue);

		for( int i = 0 ; i < maxSize ; ++i )
		{

			ShowEnergyTooLow( costedValue < 0 ) ;

			if( i >= _EnergyNow ) 
			{
				m_EnergyGrids[ i ].color = Color.grey ;
			}
			else if( i < _EnergyNow && i >= costedValue )
			{
				m_EnergyGrids[ i ].color = Color.red ;
			}
			else 
			{
				m_EnergyGrids[ i ].color = Color.green ;
			}
		}


	}

	private void ShowEnergyTooLow( bool _Show )
	{
		if( null == m_EnergyLabel )
		{
			return ;
		}

		m_EnergyLabel.color = (_Show) ? COLOR_PURPLE : COLOR_HIDE ;
		m_EnergyLabel.text = (_Show) ? "Energy Too Low!!!" : "Energy" ;
		m_EnergyLabelBackground.enabled = _Show ;

		m_StartButton.SetActive( !_Show ) ;
	}

	private void PressComponentButton( int _ComponentIndex , ActionKey _Action )
	{
		int keyIndex = (int) _Action ;
		UnityEngine.UI.Image [] _Images = null ;
		switch( _ComponentIndex )
		{
		case 0 :
			_Images = m_Component0ButtonsImages ;
			break ;
		case 1 :
			_Images = m_Component1ButtonsImages ;
			break ;
		case 2 :
			_Images = m_Component2ButtonsImages ;
			break ;
		}

		SetColorForSelectComponentButtons( _Images , keyIndex ) ;
		
		SetAction( _ComponentIndex , _Action ) ;
	}

	private void SetColorForSelectComponentButtons( UnityEngine.UI.Image [] _ImageArray , int _SelectIndex )
	{
		if( null == _ImageArray )
		{
			return ;
		}

		for( int i = 0 ; i < _ImageArray.Length ; ++i )
		{
			_ImageArray[ i ].color = ( i != _SelectIndex ) ? ( Color.white ) : (Color.green) ;
		}
	}

	private int CalculateCostEnergyFromInput()
	{
		int ret = 0 ;
		for( int i = 0 ; i < this.m_SelectedActions.Length ; ++i )
		{
			if( this.m_SelectedActions[ i ] == ActionKey.Attack )
			{
				ret += GetaPieceConst.COST_ENERGY_ATTACK ;
			}
			else if( this.m_SelectedActions[ i ] == ActionKey.Defend )
			{
				ret += GetaPieceConst.COST_ENERGY_DEFEND ;
			}
			else if( this.m_SelectedActions[ i ] == ActionKey.Concentrate )
			{
				ret += GetaPieceConst.COST_ENERGY_CONCENTRATE ;
			}
		}
		return ret ;
	}

	private void UpdateHitPointFromDataOnce()
	{
		// player
		if( null != m_HitPointPlayer && null != m_Player )
		{
			m_HitPointPlayer.sizeDelta = 
				new Vector2( m_Player.HitPoint * 100.0f / m_Player.MaxHitPoint , 0 ) ;
		}

		if( null != m_HitPointEnemy && null != m_Enemy )
		{
			m_HitPointEnemy.sizeDelta = 
				new Vector2( m_Enemy.HitPoint * 100.0f / m_Enemy.MaxHitPoint , 0 ) ;
		}

	}

	private void CalculateDamageFromData()
	{
		int damageFromEnemy = m_Player.CalculateSufferDamageAsAWhole( m_Enemy ) ;
		m_Player.HitPoint -= damageFromEnemy ;
		int damageFromPlayer = m_Enemy.CalculateSufferDamageAsAWhole( m_Player ) ;
		m_Enemy.HitPoint -= damageFromPlayer ;
	}

	
	private void CalculateDefendSucceed()
	{
		int energyBuff = m_Player.CalculateEnergyBuffAsAWhole( m_Enemy ) ;
		m_Player.Energy += energyBuff ;
		energyBuff = m_Enemy.CalculateEnergyBuffAsAWhole( m_Player ) ;
		m_Enemy.Energy += energyBuff ;
	}

	private void CalculateActionsOfEnemy()
	{
		int currentEnergy = m_Enemy.Energy ;
		// copy select actions to player
		for( int i = 0 ; i < m_Enemy.m_Action.Length ; ++i )
		{
			m_Enemy.m_Action[ i ] = (ActionKey) Random.Range( 0 , (int) (ActionKey.Concentrate + 1) ) ;

			if( m_Enemy.m_Action[ i ]  == ActionKey.Attack && currentEnergy - GetaPieceConst.COST_ENERGY_ATTACK < 0 )
			{
				m_Enemy.m_Action[ i ] = ActionKey.Concentrate ;
			}
			else if( m_Enemy.m_Action[ i ]  == ActionKey.Defend && currentEnergy - GetaPieceConst.COST_ENERGY_DEFEND < 0 )
			{
				m_Enemy.m_Action[ i ] = ActionKey.Concentrate ;
			}

			// m_Enemy.m_Action[ i ] = ActionKey.Concentrate ;

			Debug.Log("m_Enemy.m_Action[ i ]=" + m_Enemy.m_Action[ i ] ) ;
		}
	}

	private void CheckBattleEvent()
	{
		if( null == m_BattleEvents || this.m_BattleEvents.Count <= 0 )
		{
			return ;
		}

		while( this.m_BattleEvents.Count > 0 )
		{
			var front = this.m_BattleEvents.Dequeue() ;
			DoHandleBattleEvent( front ) ;
		}

	}
	
	private void DoHandleBattleEvent( GetaPieceBattleEvent _Event )
	{
		if( null == _Event )
		{
			return ;
		}

		_Event.DEBUG_Print() ;

		var type = _Event.GetBattleType() ;
		switch( type )
		{
		case GetaPieceBattleEventType.Damage :
			{
				// var targetString = _Event.Target ;
				// var damageValue = _Event.AsInt() ;

			}
			break ;
		}
		
	}
	
	private void CheckPowerAttackPicture() 
	{
		Debug.Log("m_Player.m_PowerAttack" + m_Player.m_PowerAttack);
		for( int i = 0 
		; i < this.m_Player.m_Action.Length 
		&& i < m_ComponentSwordImages.Length 
		; ++i )
		{
			m_ComponentSwordImages[ i ].color = (m_Player.m_PowerAttack) ? Color.red : Color.white ;
		}
	}

	private void CalculateEnergyRefill()
	{
		m_Player.Energy += GetaPieceConst.ENERGY_REFILL_EACH_TURN ;
		m_Enemy.Energy += GetaPieceConst.ENERGY_REFILL_EACH_TURN ;
	}

	private void CalculatePowerAttackEffect()
	{
		bool isPowerAttack = false ;
		for( int i = 0 ; i < m_Player.m_Action.Length ; ++i )
		{
			if( m_Player.m_Action[ i ] == ActionKey.Concentrate )
			{
				isPowerAttack = true ;
			}
		}
		m_Player.m_PowerAttack = isPowerAttack ;
		Debug.Log("m_Player.m_PowerAttack="+m_Player.m_PowerAttack);

		isPowerAttack = false ;
		for( int i = 0 ; i < m_Enemy.m_Action.Length ; ++i )
		{
			if( m_Enemy.m_Action[ i ] == ActionKey.Concentrate )
			{
				isPowerAttack = true ;
			}
		}
		m_Enemy.m_PowerAttack = isPowerAttack ;
		Debug.Log("m_Enemy.m_PowerAttack="+m_Enemy.m_PowerAttack);

	}


	private ActionKeyEnumHelper m_ActionKeyEnumHelper = new ActionKeyEnumHelper() ;
	private GetaPieceInterfaceState m_State = GetaPieceInterfaceState.UnActive ;
	private Color COLOR_PURPLE = new Color( 1 , 0 , 1 ) ;
	private Color COLOR_HIDE = new Color( 50.0f/255.0f  ,  50.0f/255.0f ,  50.0f/255.0f , 114.0f / 255.0f  ) ;
}

