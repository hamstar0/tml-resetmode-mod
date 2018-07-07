﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }



		////////////////

		public ResetModeSessionData SessionData { get; private set; }

		public bool IsExiting = false;



		////////////////

		internal SessionLogic() {
			this.SessionData = new ResetModeSessionData();
			
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
				this.SessionData = data;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.Load - Success? "+success );
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.SessionData );
		}
		
		////////////////

		internal void SetData( ResetModeMod mymod, ResetModeSessionData data ) {
			this.SessionData = data;
		}


		////////////////

		private static void _Update() {
			var mymod = ResetModeMod.Instance;
			if( mymod == null ) { return; }

			if( LoadHelpers.IsWorldBeingPlayed() ) {
				mymod.Session.UpdateSession();
			}
		}

		internal void UpdateSession() {
			var mymod = ResetModeMod.Instance;

			if( this.SessionData.IsRunning ) {
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
				LogHelpers.Log( "ResetMode.SessionLogic.LogRewardsPPSpending - Invalid player UID for " + player.name );
				return;
			}

			if( this.SessionData.PlayerPPSpendings.ContainsKey( pid ) ) {
				this.SessionData.PlayerPPSpendings[pid] += pp;
			} else {
				this.SessionData.PlayerPPSpendings[pid] = pp;
			}
		}

		
		public void RunModCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "ResetMode.SessionLogic.RunModCalls - Invalid mod name for API call " );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "ResetMode.SessionLogic.RunModCalls - Missing or invalid mod \"" + mod_name + "\" for API call" );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "ResetMode.SessionLogic.RunModCalls - Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "ResetMode.SessionLogic.RunModCalls - World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
