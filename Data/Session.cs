﻿using HamstarHelpers.Helpers.DebugHelpers;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		public ISet<string> PlayersValidated = new HashSet<string>();
		public ConcurrentDictionary<string, float> PlayerPPSpendings = new ConcurrentDictionary<string, float>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public string CurrentSessionedWorldId = "";

		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////
		
		internal void ResetAll() {
			this.PlayersValidated.Clear();
			this.AllPlayedWorlds.Clear();
			this.PlayerPPSpendings.Clear();
			this.CurrentSessionedWorldId = "";
			this.AwaitingNextWorld = false;
			this.IsRunning = false;

			if( ResetModeMod.Instance.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.Data.ResetModeSessionData.ResetAll" );
			}
		}


		internal ResetModeSessionData Clone() {
			return (ResetModeSessionData)this.MemberwiseClone();
		}

		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this, Formatting.Indented );
		}
	}
}
