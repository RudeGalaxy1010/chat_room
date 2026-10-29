using UnityEngine;

namespace R3.Unity.Examples
{

	public class NoAwakeTest : MonoBehaviour
	{
		private void OnDestroy()
		{
			Debug.Log("Destroy");
		}
	}

}
