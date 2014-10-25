using System;
using RestSharp;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using SimpleJSON;


	public class PlaylyfeException : Exception
	{
		public string Name;

		public PlaylyfeException(JSONNode errors) : base(errors ["error_description"])
		{
			this.Name = (errors ["error"]);
		}
	}

	public class Playlyfe
	{
		private String client_id;
		private String client_secret;
		private String type;
		private String redirect_uri;
		private String code;
		private Func<Dictionary<string, string>, int> store;
		private Func<Dictionary<string, string>> load;
		private	RestClient apiClient;

		public Playlyfe(String client_id, String client_secret, String type, Func<Dictionary<string, string>, int> store, Func<Dictionary<string, string>> load, string redirect_uri="")
		{
			apiClient = new RestClient("https://api.playlyfe.com/v1");
			this.client_id = client_id;
			this.client_secret = client_secret;
			this.type = type;
			if (store == null) {
				store = token => { Console.WriteLine("Storing Token");return 0;};
			}
			this.store = store;
			this.load = load;
			if (type == "client") {
				get_access_token ();
			}
			else
			{
				this.redirect_uri = redirect_uri;
			}
		}

		public void get_access_token()
		{
			var client = new RestClient ("https://playlyfe.com/auth/token");
			var request = new RestRequest ("", Method.POST);
			request.AddParameter ("client_id", client_id);
			request.AddParameter ("client_secret", client_secret);
			if (type == "client")
			{
				request.AddParameter ("grant_type", "client_credentials");
			}
			else
			{
				request.AddParameter ("grant_type", "authorization_code");
				request.AddParameter ("redirect_uri", redirect_uri);
				request.AddParameter ("code", code);
			}
			var response = client.Execute<Dictionary<string, string>>(request);
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
			double expires_at = timeSpan.TotalSeconds + Int32.Parse(response.Data["expires_in"]);
			response.Data.Add ("expires_at", expires_at.ToString ());
			response.Data.Remove ("expires_in");
			store.Invoke (response.Data);
			if (load == null) {
				load =  delegate { return response.Data;};
			}
		}

		public JSONNode api(String method, String route, Dictionary<string, string> query, object body=null, bool raw=false)
		{
			var token = load.Invoke ();
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
			double now = timeSpan.TotalSeconds;
			if(now >= Double.Parse(token["expires_at"]) ) {
				get_access_token ();
				token = load.Invoke ();
			}
			Method reqMethod = Method.GET;
			bool hasBody = false;
			switch(method.ToUpper ()) {
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
			request.AddHeader ("Content-Type", "application/json");
			request.AddParameter ("access_token", token["access_token"], ParameterType.QueryString);
			request.RequestFormat = DataFormat.Json;
			if (query != null)
			{
				foreach (var pair in query)
				{
					request.AddParameter (pair.Key, pair.Value, ParameterType.QueryString);
				}
			}
			if (hasBody) {
				if (body != null) {
					request.AddBody (body);
				}
			}
			var response = apiClient.Execute(request);
			if (response.Content.Contains ("error") && response.Content.Contains ("error_description"))
			{
				throw new PlaylyfeException (JSON.Parse (response.Content));
			}
		    if (raw == true) {
		        return response.Content;
		    }
		    else {
		      	return JSON.Parse(response.Content);
		    }
		}

		public JSONNode get(String route, Dictionary<string, string> query, bool raw=false)
		{
			return api("GET", route, query, new {}, raw);
		}

		public JSONNode post(string route, Dictionary<string, string> query, object body)
		{
			return api ("POST", route, query, body);
		}

		public JSONNode put(string route, Dictionary<string, string> query, object body)
		{
			return api ("PUT", route, query, body);
		}

		public JSONNode patch(string route, Dictionary<string, string> query, object body)
		{
			return api ("PATCH", route, query, body);
		}

		public JSONNode delete(string route, Dictionary<string, string> query)
		{
			return api ("DELETE", route, query, new {});
		}

		public string get_login_url() {
			var url = "https://playlyfe.com/auth?response_type=code&client_id" + client_id + "&redirect_uri" + redirect_uri;
			return HttpUtility.UrlEncode(url);
		}

		public void exchange_code(string code) {
			this.code = code;
			get_access_token ();
		}
	}

