using System;
using System.Collections.Generic;
using NUnit.Framework;
using Playlyfe;

namespace Test
{
	[TestFixture]
	public class PlaylyfeTest
	{
		Dictionary<string, string> player_id = new Dictionary<string, string> (){ { "player_id", "student1" } };

		[Test]
		public void Error()
		{
			var pl = new Playlyfe.Playlyfe(
				client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
				client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
				type: "client",
				store: null,
				load: null
			);
			try {
				pl.get(
					route: "/unkown",
					query: player_id
				);
			}
			catch(Playlyfe.PlaylyfeException ex) {
				Assert.AreEqual(ex.Name, "route_not_found");
				Assert.AreEqual(ex.Message, "This route does not exist");
			}
		}

		[Test]
		public void v1API()
		{
			var pl = new Playlyfe.Playlyfe(
				client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
				client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
				type: "client",
				store: null,
				load: null,
				version: "v1"
			);

			dynamic all_players = pl.api(
				method: "GET",
				route: "/players",
				query: player_id
			);
			//Assert.AreEqual(all_players["total"], 3);
			Assert.IsNotNull(all_players["data"]);

			dynamic players = pl.get(
				route: "/players",
				query: player_id
			);
			//Assert.AreEqual(players["total"], 3);

			dynamic player = pl.get(route: "/player", query: player_id,raw: true);
			Assert.IsInstanceOf<String>(player);

			pl.get (route: "/definitions/processes", query: player_id);
			pl.get (route:  "/definitions/teams", query:  player_id);
			pl.get (route:  "/processes", query:  player_id);
			pl.get (route:  "/teams",   query:  player_id);

			dynamic new_process = pl.post (route: "/definitions/processes/module1", query: player_id, body: null);
			Assert.AreEqual (new_process ["definition"], "module1");
			Assert.AreEqual (new_process ["state"], "ACTIVE");

			dynamic patched_process = pl.patch (
				route: "/processes/" + new_process ["id"],
				query: player_id,
				body: new { name = "patched_process", access = "PUBLIC"}
			);
			Assert.AreEqual(patched_process["name"], "patched_process");
			Assert.AreEqual(patched_process["access"], "PUBLIC");

			dynamic deleted_process = pl.delete(
				route: "/processes/"+new_process["id"],
				query: player_id
			);
			Assert.IsNotNullOrEmpty (deleted_process ["message"]);
		}

		[Test]
		public void v2API()
		{
			var pl = new Playlyfe.Playlyfe(
				client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
				client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
				type: "client",
				store: null,
				load: null
			);

			dynamic all_players = pl.api(
				method: "GET",
				route: "/runtime/players",
				query: player_id
			);
			Assert.IsNotNull(all_players["data"]);

			dynamic players = pl.get(
				route: "/runtime/players",
				query: player_id
			);

			dynamic player = pl.get(route: "/runtime/player", query: player_id,raw: true);
			Assert.IsInstanceOf<String>(player);

			pl.get (route: "/runtime/definitions/processes", query: player_id);
			pl.get (route:  "/runtime/definitions/teams", query:  player_id);
			pl.get (route:  "/runtime/processes", query:  player_id);
			//pl.get (route:  "/teams",   query:  player_id);

			dynamic new_process = pl.post (route: "/runtime/processes", query: player_id, body: new { definition = "module1" });
			Assert.AreEqual (new_process ["definition"]["id"], "module1");
			Assert.AreEqual (new_process ["state"], "ACTIVE");

			dynamic patched_process = pl.patch (
				route: "/runtime/processes/" + new_process ["id"],
				query: player_id,
				body: new { name = "patched_process", access = "PUBLIC"}
			);
			Assert.AreEqual(patched_process["name"], "patched_process");
			Assert.AreEqual(patched_process["access"], "PUBLIC");

			dynamic deleted_process = pl.delete(
				route: "/runtime/processes/"+new_process["id"],
				query: player_id
			);
			Assert.IsNotNullOrEmpty (deleted_process ["message"]);
		}

		[Test]
		public void APIProduction()
		{
			var pl = new Playlyfe.Playlyfe (
			      client_id: "N2Y4NjNlYTItODQzZi00YTQ0LTkzZWEtYTBiNTA2ODg3MDU4",
			      client_secret: "NDc3NTA0NmItMjBkZi00MjI2LWFhMjUtOTI0N2I1YTkxYjc2M2U3ZGI0MDAtNGQ1Mi0xMWU0LWJmZmUtMzkyZTdiOTYxYmMx",
				  type: "client",
				  store: null,
				  load: null,
				  version: "v1"
			);
			dynamic players = pl.get(route: "/game/players", query: player_id);
			Assert.IsNotNull(players);
		}

		[Test]
		public void StoreAndLoad()
		{
			Dictionary<string, string> tok = null;
			var pl = new Playlyfe.Playlyfe (
				client_id: "N2Y4NjNlYTItODQzZi00YTQ0LTkzZWEtYTBiNTA2ODg3MDU4",
				client_secret: "NDc3NTA0NmItMjBkZi00MjI2LWFhMjUtOTI0N2I1YTkxYjc2M2U3ZGI0MDAtNGQ1Mi0xMWU0LWJmZmUtMzkyZTdiOTYxYmMx",
				type: "client",
				store: token =>
				{
					Console.WriteLine("hello", token);
					tok = new Dictionary<string, string>();
					tok["access_token"] = token["access_token"];
					tok["expires_at"] = token["expires_at"];
					return 0;
				},
				load: () =>
				{
					return tok;
				},
				version: "v1"
			);
			dynamic players = pl.get(route: "/game/players", query: player_id);
			pl.get(route: "/game/players", query: player_id);
			pl.get(route: "/game/players", query: player_id);
			var pl2 = new Playlyfe.Playlyfe (
				client_id: "N2Y4NjNlYTItODQzZi00YTQ0LTkzZWEtYTBiNTA2ODg3MDU4",
				client_secret: "NDc3NTA0NmItMjBkZi00MjI2LWFhMjUtOTI0N2I1YTkxYjc2M2U3ZGI0MDAtNGQ1Mi0xMWU0LWJmZmUtMzkyZTdiOTYxYmMx",
				type: "client",
				store: token =>
				{
					Console.WriteLine("hello", token);
					tok = new Dictionary<string, string>();
					tok["access_token"] = token["access_token"];
					tok["expires_at"] = token["expires_at"];
					return 0;
				},
				load: () =>
				{
					return tok;
				},
				version: "v1"
			);
			pl2.get(route: "/game/players", query: player_id);
			pl2.get(route: "/game/players", query: player_id);
		}
	}
}

