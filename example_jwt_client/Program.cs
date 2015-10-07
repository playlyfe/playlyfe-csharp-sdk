using System;
using Playlyfe;
using System.Collections.Generic;

namespace example_jwt_client
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var jwtClient = new Playlyfe.Playlyfe (
				client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
				client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
				type: "jwt",
				store: null,
				load: null,
				version: "v2"									
			);
			// Note you will need to run the ASP Server in the example_server project which serves the token
			jwtClient.getJWTToken ("http://localhost:3000/JWT_TOKEN");
			Console.WriteLine ("Got Token");
			var player_id = new Dictionary<string, string> (){ { "player_id", "student1" } };
			dynamic player = jwtClient.get(route: "/runtime/player", query: player_id);
			Console.WriteLine (player);
		}
	}
}
