using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.Data.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageForCurrentSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueId();   //Main.ActiveWorldFileData.UniqueId.ToString();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EngageForCurrentSession " + world_id );
			}

			if( !mymod.Session.Data.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			mymod.Session.Data.AddActiveWorld( world_id );

			mymod.Session.Save( mymod );

			this.WorldStatus = ResetModeStatus.Active;

			this.RunWorldCalls( mymod );

			PlayerLogic.ValidateAll( mymod );
		}


		public void ExpireForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.ExpireForCurrentSession" );
			}

			mymod.Session.Data.AwaitingNextWorld = true;

			mymod.Session.Save( mymod );

			this.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}

		public void ResetForCurrentSession() {
			this.WorldPlayers.Clear();
			this.WorldStatus = ResetModeStatus.Normal;
		}


		////////////////

		public void RunWorldCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "Invalid mod name" );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "Missing or invalid mod \"" + mod_name + '"' );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
