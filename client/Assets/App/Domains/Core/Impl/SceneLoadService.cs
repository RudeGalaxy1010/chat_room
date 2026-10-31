using App.Domains.Core.Api;
using UnityEngine.SceneManagement;

namespace App.Domains.Core.Impl
{
    public class SceneLoadService : ISceneLoadService
    {
        public void LoadScene(SceneName scene) =>
            SceneManager.LoadScene((int)scene);
    }
}