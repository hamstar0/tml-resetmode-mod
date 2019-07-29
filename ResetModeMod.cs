using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET;
using HamstarHelpers.Helpers.DotNET.Reflection;
using HamstarHelpers.Helpers.TModLoader.Mods;
using HamstarHelpers.Services.Debug.DataDumper;
using HamstarHelpers.Services.Hooks.LoadHooks;
using ResetMode.Data;
using ResetMode.Logic;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModeMod : Mod {

		internal readonly static object MyValidatorKey;
		internal readonly static CustomLoadHookValidator<object> WorldExitValidator;


		////

		public static ResetModeMod Instance { get; private set; }

		////////////////

		static ResetModeMod() {
			ResetModeMod.MyValidatorKey = new object();
			ResetModeMod.WorldExitValidator = new CustomLoadHookValidator<object>( ResetModeMod.MyValidatorKey );
		}



		////////////////

		public ResetModeConfig Config => this.GetConfig<ResetModeConfig>();
		
		public SessionLogic Session { get; internal set; }

		internal int CurrentNetMode = -1;



		////////////////

		public ResetModeMod() {
			ResetModeMod.Instance = this;


			this.Session = new SessionLogic();
		}


		public override void Load() {
			LoadHooks.AddWorldLoadEachHook( delegate {
				this.CurrentNetMode = Main.netMode;
			} );

			CustomLoadHooks.AddHook( ResetModeMod.WorldExitValidator, (_) => {
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

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof( ResetModeAPI ), args );
		}
	}
}
