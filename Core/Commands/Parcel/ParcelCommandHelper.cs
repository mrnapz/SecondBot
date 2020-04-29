﻿using BSB.Commands;
using OpenMetaverse;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSB.Commands
{
    public abstract class ParcelCommand_RequirePerms_1arg_Group : ParcelCommand_RequirePerms
    {
        protected UUID groupuuid = UUID.Zero;
        public override string[] ArgTypes { get { return new[] { "UUID" }; } }
        public override string[] ArgHints { get { return new[] { "Group UUID" }; } }
        public override int MinArgs { get { return 1; } }
        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                if (UUID.TryParse(args[0], out groupuuid) == true)
                {
                    return true;
                }
                else
                {
                    return Failed("Unable to process group uuid");
                }
            }
            else
            {
                return false;
            }
        }
    }
    public abstract class ParcelCommand_RequirePerms : ParcelCommand_CheckParcel
    {
        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                if (targetparcel != null)
                {
                    if (parcel_static.has_parcel_perm(targetparcel, bot) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return Failed("Incorrect perms to control parcel");
                    }
                }
                else
                {
                    return Failed("No parcel targeted.");
                }
            }
            else
            {
                return false;
            }
        }
    }
    public abstract class ParcelCommand_CheckParcel_1arg_smart : ParcelCommand_CheckParcel
    {
        public override string[] ArgTypes { get { return new[] { "Smart" }; } }
        public override string[] ArgHints { get { return new[] { "Smart reply [Channel|Avatar|http url]" }; } }
        public override int MinArgs { get { return 1; } }
    }

    public abstract class ParcelCommand_CheckParcel : CoreCommand
    {
        protected OpenMetaverse.Parcel targetparcel;
        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                if (bot.GetClient.Network.CurrentSim.Parcels.ContainsKey(localid) == true)
                {
                    targetparcel = bot.GetClient.Network.CurrentSim.Parcels[localid];
                    return true;
                }
                else
                {
                    return Failed("Unable to find parcel in memory, please wait and try again");
                }
            }
            else
            {
                return false;
            }
        }
    }
}