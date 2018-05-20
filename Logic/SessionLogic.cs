﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }


		////////////////

		public ResetModeSessionData Data { get; private set; }

		internal IDictionary<string, float> Rewards = new Dictionary<string, float>();


		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();
		}

		////////////////

		public void Load( ResetModeMod mymod ) {
			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				this.Data = data;
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}


		////////////////

		public bool Start( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.Start" );
			}

			if( mymod.Session.Data.IsRunning ) {
				return false;
			}

			var myworld = mymod.GetModWorld<ResetModeWorld>();

			myworld.Logic.EngageForCurrentSession( mymod );

			return true;
		}
		
		public void End( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				string world_id = WorldHelpers.GetUniqueId();   //Main.ActiveWorldFileData.UniqueId.ToString();
				LogHelpers.Log( "WorldLogic.End " + world_id );
			}

			this.Data.ClearSessionData();

			this.Save( mymod );

			myworld.Logic.ResetForCurrentSession();
		}


		////////////////

		public void AddRewards( Player player, float rewards ) {
			if( Main.netMode == 1 ) { return; }

			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) { return; }

			if( this.Data.PlayerPP.ContainsKey( pid ) ) {
				this.Data.PlayerPP[pid] += rewards;
			} else {
				this.Data.PlayerPP[pid] = rewards;
			}
		}
	}
}
