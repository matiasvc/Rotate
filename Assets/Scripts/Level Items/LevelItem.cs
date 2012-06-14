using UnityEngine;
using System.Collections;

public abstract class LevelItem : MonoBehaviour {
	
	public bool itemEnabled = true;
	
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
