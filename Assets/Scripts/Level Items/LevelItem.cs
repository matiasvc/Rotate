using UnityEngine;
using System.Collections;

public abstract class LevelItem : MonoBehaviour {
	
	[SerializeField]
	protected bool itemEnabled;
	
	public bool ItemEnabled{
		get { return itemEnabled; }
		set { ItemSwitch( value ); }
	}
	
	public virtual void ItemEnable ()
	{
		itemEnabled = true;
	}
	
	public virtual void ItemDisable ()
	{
		itemEnabled = false;
	}
	
	public virtual void ItemSwitch ( bool setTo )
	{
		itemEnabled = setTo;
	}
	
	public virtual void ItemToggle ()
	{
		itemEnabled = !itemEnabled;
	}
}
