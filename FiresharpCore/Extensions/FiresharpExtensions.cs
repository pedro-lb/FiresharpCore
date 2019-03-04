using FiresharpCore.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace FiresharpCore.Extensions
{
    public static class FiresharpExtensions
    {
        /// <summary>
        /// Obtém o ID retornado em uma resposta de uma requisição.
        /// </summary>
        public static string GetID(this FirebaseResponse firebaseResponse)
        {
            var result = JsonConvert.DeserializeAnonymousType(firebaseResponse.Body, new
            {
                Name = string.Empty
            });

            return result?.Name;
        }

        /// <summary>
        /// Deserializa uma lista de resultados de um banco Firebase para um dicionário de chaves do objeto e valor do objeto.
        /// </summary>
        public static Dictionary<string, T> ToDictionary<T>(this FirebaseResponse firebaseResponse)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(firebaseResponse.Body) as IDictionary<string, JToken>;
            return data.ToDictionary(kv => kv.Key, kv => JsonConvert.DeserializeObject<T>(kv.Value.ToString()));
        }

        /// <summary>
        /// Deserializa uma resposta Firebase para um objeto.
        /// </summary>
        public static T FirstOrDefault<T>(this FirebaseResponse firebaseResponse)
        {
            return firebaseResponse.ToDictionary<T>()
                .Select(kv => kv.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Deserializa uma resposta Firebase para uma lista de objetos.
        /// </summary>
        public static List<T> ToList<T>(this FirebaseResponse firebaseResponse)
        {
            return firebaseResponse.ToDictionary<T>()
                .Select(kv => kv.Value)
                .ToList();
        }
    }
}