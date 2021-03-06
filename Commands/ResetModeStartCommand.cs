﻿using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.User;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStartCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 && !Main.dedServ ) {
					return CommandType.World;
				}
				return CommandType.Console | CommandType.World;
			}
		}
		public override string Command => "rm-start";
		public override string Usage => "/"+this.Command;
		public override string Description => "Begins reset mode.";
		

		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			if( Main.netMode == 1 ) {
				LogHelpers.Warn( "Not supposed to run on client." );
				return;
			}

			if( Main.netMode == 2 && caller.CommandType != CommandType.Console ) {
				bool hasPriv = UserHelpers.HasBasicServerPrivilege( caller.Player );

				if( !hasPriv ) {
					caller.Reply( "Access denied.", Color.Red );
					return;
				}
			}

			var mymod = (ResetModeMod)this.mod;

			try {
				if( mymod.Session.StartSession() ) {
					caller.Reply( "Reset mode begun! This will continue until for each new world /rm-stop is called.", Color.YellowGreen );
				} else {
					caller.Reply( "Reset mode is already in session.", Color.Red );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "!ResetMode.Commands.ResetModeStart - Failed to start: " + e.ToString() );
				caller.Reply( "Reset mode could not be started.", Color.Red );
			}
		}
	}
}
