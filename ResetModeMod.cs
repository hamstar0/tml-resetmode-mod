using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.DotNetHelpers;
using HamstarHelpers.Services.DataDumper;
using HamstarHelpers.Services.Promises;
using ResetMode.Data;
using ResetMode.Logic;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModeMod : Mod {
		public static ResetModeMod Instance { get; private set; }

		internal readonly static object MyValidatorKey;
		internal readonly static PromiseValidator WorldExitValidator;

		////////////////

		static ResetModeMod() {
			ResetModeMod.MyValidatorKey = new object();
			ResetModeMod.WorldExitValidator = new PromiseValidator( ResetModeMod.MyValidatorKey );
		}



		////////////////

		internal JsonConfig<ResetModeConfigData> ConfigJson;
		public ResetModeConfigData Config => ConfigJson.Data;
		
		public SessionLogic Session { get; internal set; }

		internal int CurrentNetMode = -1;


		////////////////

		public ResetModeMod() {
			this.ConfigJson = new JsonConfig<ResetModeConfigData>( ResetModeConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new ResetModeConfigData() );

			this.Session = new SessionLogic();
		}


		public override void Load() {
			ResetModeMod.Instance = this;

			this.LoadConfigs();
			
			Promises.AddWorldLoadEachPromise( delegate {
				this.CurrentNetMode = Main.netMode;
			} );

			Promises.AddValidatedPromise( ResetModeMod.WorldExitValidator, () => {
				this.CurrentNetMode = -1;
				return true;
			} );
			
			this.Session.OnModLoad();

			DataDumper.SetDumpSource( "ResetMode", () => {
				return ResetModeMod.Instance.Session.Data.ToString();
			} );
		}

		
		public override void Unload() {
			ResetModeMod.Instance = null;
		}
		
		////////////////

		private void LoadConfigs() {
			if( !this.ConfigJson.LoadFile() ) {
				LogHelpers.Alert( "Reset Mode missing/invalid config created anew." );
				this.ConfigJson.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Reset Mode updated to " + ResetModeConfigData.ConfigVersion.ToString() );
				this.ConfigJson.SaveFile();
			}
		}


		////////////////

		public override object Call( params object[] args ) {
			if( args == null || args.Length == 0 ) { throw new HamstarException( "Undefined call type." ); }

			string callType = args[0] as string;
			if( callType == null ) { throw new HamstarException( "Invalid call type." ); }

			var methodInfo = typeof( ResetModeAPI ).GetMethod( callType );
			if( methodInfo == null ) { throw new HamstarException( "Invalid call type " + callType ); }

			var newArgs = new object[args.Length - 1];
			Array.Copy( args, 1, newArgs, 0, args.Length - 1 );

			try {
				return ReflectionHelpers.SafeCall( methodInfo, null, newArgs );
			} catch( Exception e ) {
				throw new HamstarException( "Bad API call.", e );
			}
		}
	}
}
