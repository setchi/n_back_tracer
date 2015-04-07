using UnityEngine;
using UniRx;
using System;

public class ObservableLifeTime : ObservableMonoBehaviour
{
	public override void Start()
	{
		base.Start();
		
		//1秒ごとにメッセージを発信するObservable
		Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
			.Subscribe(x => Debug.Log(x))
				.AddTo(this.gameObject); //指定GameObjectの寿命に紐付ける
		
		//3秒後にGameObjectを削除する
		Invoke("DestroyGameObject", 3);
	}
	
	/// <summary>
	/// ログを吐いて削除する
	/// </summary>
	private void DestroyGameObject()
	{
		Debug.Log("Destroy");
		Destroy(this.gameObject);
	}
}