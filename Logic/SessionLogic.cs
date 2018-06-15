using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using ResetMode.NetProtocols;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }



		////////////////

		public ResetModeSessionData Data { get; private set; }

		internal IDictionary<string, float> Rewards = new Dictionary<string, float>();



		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();

			Main.OnTick += SessionLogic._Update;
		}

		~SessionLogic() {
			Main.OnTick -= SessionLogic._Update;
		}


		////////////////

		public void Load( ResetModeMod mymod ) {
			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				this.Data = data;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.Load - Success? "+success );
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}
		
		////////////////

		internal void SetData( ResetModeMod mymod, ResetModeSessionData data, ResetModeStatus world_status ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			this.Data = data;
			myworld.Data.WorldStatus = world_status;
		}


		////////////////

		private static void _Update() {
			var mymod = ResetModeMod.Instance;
			if( mymod == null ) { return; }

			if( TmlLoadHelpers.IsWorldLoaded() ) {
				mymod.Session.UpdateSession();
			}
		}

		internal void UpdateSession() {
			var mymod = ResetModeMod.Instance;

			if( this.Data.IsRunning ) {
				if( Main.netMode == 0 ) {
					this.UpdateSessionWorldSingle( mymod );
				} else if( Main.netMode == 1 ) {
					this.UpdateSessionWorldClient( mymod );
				} else {
					this.UpdateSessionWorldServer( mymod );
				}
			}
		}


		////////////////

		public void LogRewardsPPSpending( Player player, float pp ) {
			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode - SessionLogic.LogRewardsPPSpending - Invalid player UID for " + player.name );
				return;
			}

			if( this.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
				this.Data.PlayerPPSpendings[pid] += pp;
			} else {
				this.Data.PlayerPPSpendings[pid] = pp;
			}

			if( Main.netMode == 1 ) {
				PacketProtocol.QuickSendToServer<RewardsSpendingProtocol>();
			}
		}

		
		public void RunModCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "ResetMode - SessionLogic.RunModCalls - Invalid mod name for API call " );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "ResetMode - SessionLogic.RunModCalls - Missing or invalid mod \"" + mod_name + "\" for API call" );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "ResetMode - SessionLogic.RunModCalls - Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "ResetMode - SessionLogic.RunModCalls - World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
