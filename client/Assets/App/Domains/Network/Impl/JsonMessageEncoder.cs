using App.Domains.Network.Api;
using UnityEngine;

namespace App.Domains.Network.Impl
{
    public class JsonMessageEncoder : IMessageEncoder
    {
        public string Encode<T>(T message) =>
            JsonUtility.ToJson(message);

        public T Decode<T>(string message) =>
            JsonUtility.FromJson<T>(message);
    }
}