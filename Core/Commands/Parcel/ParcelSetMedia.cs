﻿using BetterSecondBotShared.Static;
using OpenMetaverse;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSB.Commands.CMD_Parcel
{
    class ParcelSetMedia : CoreCommand
    {
        public override string[] ArgTypes { get { return new[] { "Flag [Repeatable]" }; } }
        public override string[] ArgHints { get { return new[] { "See flags table" }; } }
        public override int MinArgs { get { return 1; } }
        public override string Helpfile { get { return "Supports updating one or multiple media options for the current parcel<br/>Example: ParcelSetMedia|||MediaURL=http://google.com~#~MediaDesc=Google.com website<br/>Flags: " + helpers.create_dirty_table(parcel_static.get_flag_names()) + "<br/>"; } }

        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                if (bot.GetClient.Network.CurrentSim.Parcels.ContainsKey(localid) == true)
                {
                    Parcel p = bot.GetClient.Network.CurrentSim.Parcels[localid];
                    if (parcel_static.has_parcel_perm(p, bot) == true)
                    {
                        bool all_flags_ok = true;
                        string why_failed = "";
                        int loop = 1;
                        foreach (string A in args)
                        {
                            bool format_failed = false;
                            string[] bits = A.Split('=');
                            if (bits.Length == 2)
                            {
                                switch (bits[0])
                                {
                                    case "MediaType":
                                        {
                                            if (bits[1] == "IMG-PNG")
                                            {
                                                p.Media.MediaType = "image/png";
                                            }
                                            else if (bits[1] == "IMG-JPG")
                                            {
                                                p.Media.MediaType = "image/jpeg";
                                            }
                                            else if (bits[1] == "VID-MP4")
                                            {
                                                p.Media.MediaType = "video/mp4";
                                            }
                                            else if (bits[1] == "VID-AVI")
                                            {
                                                p.Media.MediaType = "video/x-msvideo";
                                            }
                                            else if (bits[1].StartsWith("Custom-") == true)
                                            {
                                                string mime = bits[1];
                                                mime = mime.Replace("Custom-", "");
                                                p.Media.MediaType = mime;
                                            }
                                            else
                                            {
                                                format_failed = true;
                                            }
                                            break;
                                        }
                                    case "MediaWidth":
                                    case "MediaHeight":
                                        {
                                            if (int.TryParse(bits[1], out int size) == true)
                                            {
                                                if ((size >= 256) || (size <= 1024))
                                                {
                                                    if (bits[0] == "MediaHeight")
                                                    {
                                                        p.Media.MediaHeight = size;
                                                    }
                                                    else if (bits[0] == "MediaWidth")
                                                    {
                                                        p.Media.MediaWidth = size;
                                                    }
                                                    else
                                                    {
                                                        format_failed = true; // not sure how this happened
                                                    }
                                                }
                                                else
                                                {
                                                    format_failed = true;
                                                }
                                            }
                                            else
                                            {
                                                format_failed = true;
                                            }
                                            break;
                                        }
                                    case "MediaID":
                                        {
                                            if (UUID.TryParse(bits[1], out UUID texture) == true)
                                            {
                                                p.Media.MediaID = texture;
                                            }
                                            else
                                            {
                                                format_failed = true;
                                            }
                                            break;
                                        }
                                    case "MediaURL":
                                        {
                                            p.Media.MediaURL = bits[1];
                                            break;
                                        }
                                    case "MediaDesc":
                                        {
                                            p.Media.MediaDesc = bits[1];
                                            break;
                                        }
                                    case "MediaAutoScale":
                                        {
                                            if (bool.TryParse(bits[1], out bool output) == true)
                                            {
                                                p.Media.MediaAutoScale = output;
                                            }
                                            else
                                            {
                                                format_failed = true;
                                            }
                                            break;
                                        }
                                    case "MediaLoop":
                                        {
                                            if (bool.TryParse(bits[1], out bool output) == true)
                                            {
                                                p.Media.MediaLoop = output;
                                            }
                                            else
                                            {
                                                format_failed = true;
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            why_failed = "Arg:" + loop.ToString() + " is not a supported media option!";
                                            all_flags_ok = false;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                format_failed = true;
                            }
                            if (format_failed == true)
                            {
                                why_failed = "Arg:" + loop.ToString() + " Not formated correctly";
                                all_flags_ok = false;
                            }
                            if (all_flags_ok == false)
                            {
                                break;
                            }
                            loop++;
                        }
                        if (all_flags_ok == true)
                        {
                            p.Update(bot.GetClient.Network.CurrentSim, false);
                            return true;
                        }
                        else
                        {
                            return Failed(why_failed);
                        }
                    }
                    else
                    {
                        return Failed("Do not have expected perms for this parcel!");
                    }
                }
                else
                {
                    return Failed("Unable to find parcel in memory, please wait and try again");
                }
            }
            return false;
        }
    }
}
