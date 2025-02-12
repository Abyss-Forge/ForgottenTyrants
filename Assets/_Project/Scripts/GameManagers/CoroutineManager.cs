using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

namespace Systems.GameManagers
{
    public class CoroutineManager : Singleton<CoroutineManager>
    {

        public Task StartTask(IEnumerator coroutine)
        {
            var tcs = new TaskCompletionSource<object>();
            StartCoroutine(RunCoroutine(coroutine, tcs));
            return tcs.Task;
        }

        private IEnumerator RunCoroutine(IEnumerator coroutine, TaskCompletionSource<object> tcs)
        {
            yield return StartCoroutine(coroutine);
            tcs.SetResult(null);
        }
    }
}