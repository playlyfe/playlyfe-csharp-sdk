using System;
using RestSharp;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using JWT;

namespace Playlyfe
{

    public class PlaylyfeException : Exception
    {
        public string Name { get; set; }

        public PlaylyfeException(String error, String error_description)
            : base(error_description)
        {
            this.Name = error;
        }
    }

    public class Playlyfe
    {
        private String client_id;
        private String client_secret;
        private String type;
        private String redirect_uri;
        private String code;
		private String tokenEndPoint;
		private String apiEndPoint;
        private Func<Dictionary<string, string>, int> store;
        private Func<Dictionary<string, string>> load;
        private RestClient apiClient;

		public static String createJWT(String client_id, String client_secret, String player_id, String[] scopes, int expires) {
			var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds) + expires;
			var payload = new Dictionary<string, object>()
			{
				{ "player_id", player_id },
				{ "scopes", scopes },
				{ "exp", now }
			};
			string token = JWT.JsonWebToken.Encode(payload, client_secret, JWT.JwtHashAlgorithm.HS256);
			token = client_id + ':' + token;
			return token;
		}

		public void getJWTToken(String url) {
			var client = new RestClient(url);
			var request = new RestRequest("", Method.GET);
			var response = client.Execute (request);
			if (load == null) {
				load = delegate {
					return new Dictionary<string, string> (){ { "jwt_token", response.Content } };
				};
			}
		}

        public Playlyfe(String client_id, String client_secret, String type,
            Func<Dictionary<string, string>, int> store, Func<Dictionary<string, string>> load,
			string redirect_uri = "", string version = "v2",
			String tokenEndPoint = "https://playlyfe.com/auth/token",
			String apiEndPoint = "https://api.playlyfe.com"
		)
        {
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            apiClient = new RestClient(apiEndPoint + "/" + version);
            this.client_id = client_id;
            this.client_secret = client_secret;
			this.apiEndPoint = apiEndPoint;
			this.tokenEndPoint = tokenEndPoint;
            this.type = type;
            if (store == null)
            {
                store = token => { Console.WriteLine("Storing Token"); return 0; };
            }
            this.store = store;
			if (load != null) {
				this.load = load;
				var token = load.Invoke ();
				if (token == null) {
					get_access_token ();
				}
			}
			else {
				if (type != "jwt")
				 get_access_token ();
			}
            this.redirect_uri = redirect_uri;
        }

        private void get_access_token()
        {
			var client = new RestClient(this.tokenEndPoint);
            var request = new RestRequest("", Method.POST);
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);
            if (type == "client")
            {
                request.AddParameter("grant_type", "client_credentials");
            }
            else
            {
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("redirect_uri", redirect_uri);
                request.AddParameter("code", code);
            }
            var response = client.Execute<Dictionary<string, string>>(request);
			// Console.WriteLine (response.Content);
            if (response.Content.Contains("error") && response.Content.Contains("error_description"))
            {
                throw new PlaylyfeException(response.Data["error"], response.Data["error_description"]);
            }
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            double expires_at = timeSpan.TotalSeconds + Int32.Parse(response.Data["expires_in"]);
            response.Data.Add("expires_at", expires_at.ToString());
            response.Data.Remove("expires_in");
            store.Invoke(response.Data);
            if (load == null)
            {
                load = delegate { return response.Data; };
            }
        }

		public dynamic api(String method, String route, Dictionary<string, string> query, object body = null, bool raw = false, bool list = false)
        {   
            Method reqMethod = Method.GET;
            bool hasBody = false;
            switch (method.ToUpper())
            {
                case "GET":
                    reqMethod = Method.GET;
                    break;
                case "POST":
                    hasBody = true;
                    reqMethod = Method.POST;
                    break;
                case "PUT":
                    hasBody = true;
                    reqMethod = Method.PUT;
                    break;
                case "PATCH":
                    hasBody = true;
                    reqMethod = Method.PATCH;
                    break;
                case "DELETE":
                    reqMethod = Method.DELETE;
                    break;
                default:
                    reqMethod = Method.GET;
                    break;
            }
            var request = new RestRequest(route, reqMethod);
            request.AddHeader("Content-Type", "application/json");
			if (type != "jwt") {
				var token = load.Invoke ();
				TimeSpan timeSpan = DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0);
				double now = timeSpan.TotalSeconds;
				if (now >= Double.Parse (token ["expires_at"])) {
					get_access_token ();
					token = load.Invoke ();
				}
				request.AddParameter ("access_token", token ["access_token"], ParameterType.QueryString);
			} else {
				var token = load.Invoke ();
				request.AddParameter("jwt", token["jwt_token"], ParameterType.QueryString);
			}
            
            request.RequestFormat = DataFormat.Json;
            if (query != null)
            {
                foreach (var pair in query)
                {
                    request.AddParameter(pair.Key, pair.Value, ParameterType.QueryString);
                }
            }
            if (hasBody)
            {
                if (body != null)
                {
                    request.AddBody(body);
                }
            }
            var response = apiClient.Execute(request);
			// Console.WriteLine (response.ErrorException);
            if (response.Content.Contains("error") && response.Content.Contains("error_description"))
            {
                dynamic error = SimpleJson.DeserializeObject<object>(response.Content);
                throw new PlaylyfeException(error["error"], error["error_description"]);
            }
			if (raw == true) {
				return response.Content;
			} else if (list == true) {
				return SimpleJson.DeserializeObject<List<dynamic>> (response.Content);
			} else {
				return SimpleJson.DeserializeObject<dynamic> (response.Content);
			}
        }

        public dynamic get(String route, Dictionary<string, string> query, bool raw = false, bool list = false)
        {
            return api("GET", route, query, new { }, raw, list);
        }

		public dynamic post(string route, Dictionary<string, string> query, object body, bool list = false)
        {
            return api("POST", route, query, body, list);
        }

		public dynamic put(string route, Dictionary<string, string> query, object body, bool list = false)
        {
            return api("PUT", route, query, body, list);
        }

		public dynamic patch(string route, Dictionary<string, string> query, object body, bool list = false)
        {
            return api("PATCH", route, query, body, list);
        }

		public dynamic delete(string route, Dictionary<string, string> query, bool list = false)
        {
            return api("DELETE", route, query, new { }, list);
        }

        public string get_login_url()
        {
            var url = "https://playlyfe.com/auth?response_type=code&client_id=" + client_id + "&redirect_uri=" + redirect_uri;
            //Console.WriteLine (HttpUtility.UrlEncode (url));
            return url;
        }

        public void exchange_code(string code)
        {
            this.code = code;
            get_access_token();
        }
    }
}
