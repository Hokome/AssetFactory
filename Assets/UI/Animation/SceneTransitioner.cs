using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Originally from AssetFactory
namespace AssetFactory
{
	public class SceneTransitioner : MonoSingleton<SceneTransitioner>
	{
		[SerializeField] private UnityEvent<float> onProgress;
		protected RectTransform CanvasRect => (RectTransform)transform;

		protected override void Awake()
		{
			DontDestroyOnLoad(gameObject);
			base.Awake();
		}

		//public void LoadScene(int index) => LoadScene(index, null);
		//public void LoadScene(int index, Action callback) 
		//	=> LoadScene(SceneManager.GetSceneByBuildIndex(index).name, callback);
		public void LoadScene(string name) => LoadScene(name, null);
		public virtual void LoadScene(string name, Action callback)
		{
			LTDescr startAnim = StartAnimation();
			if (startAnim == null)
			{
				SceneManager.LoadScene(name);
				EndTransition(callback);
				return;
			}
			onProgress.Invoke(0);
			startAnim.setOnComplete(() =>
			{
				AsyncOperation operation = SceneManager.LoadSceneAsync(name);
				operation.completed += _ => EndTransition(callback);
				StartCoroutine(LoadingCheck(operation));
			});
		}

		private void EndTransition(Action callback)
		{
			callback?.Invoke();
			EndAnimation();
		}

		private IEnumerator LoadingCheck(AsyncOperation loading)
		{
			while (!loading.isDone)
			{
				onProgress.Invoke(loading.progress);
				yield return null;
			}
			onProgress.Invoke(1f);
		}

		protected virtual LTDescr StartAnimation() => null;
		protected virtual void EndAnimation() { }
	}
}