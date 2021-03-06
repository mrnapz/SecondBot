﻿using BetterSecondBot.bottypes;
using BetterSecondBot.Static;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using Newtonsoft.Json;
using System.Reflection;
using BetterSecondBotShared.Static;
using RestSharp;

namespace BetterSecondBot.HttpService
{
    public class HTTP_StreamAdmin : WebApiControllerWithTokens
    {
        public HTTP_StreamAdmin(SecondBot mainbot, TokenStorage setuptokens) : base(mainbot, setuptokens) { }

        [About("an improved version of near me with extra details<br/>NearMeDetails is a object formated as follows<br/><ul><li>id</li><li>name</li><li>x</li><li>y</li><li>z</li><li>range</li></ul>")]
        [ReturnHints("True|False")]
        [ReturnHints("Bad reply:  ...")]
        [ReturnHints("Endpoint is empty")]
        [ReturnHints("Endpointcode is empty")]
        [ReturnHints("HTTP status code: ...")]
        [ReturnHints("Error: ...")]
        [ReturnHints("Notecard title is to short")]
        [ArgHints("endpoint","Text","The end point")]
        [ArgHints("endpointcode", "Text", "The end point code")]
        [Route(HttpVerbs.Post, "/FetchNextNotecard/{token}")]
        public object FetchNextNotecard([FormField] string endpoint, [FormField] string endpointcode, string token)
        {
            if (tokens.Allow(token, "streamadmin", "FetchNextNotecard", handleGetClientIP()) == false)
            {
                return Failure("Token not accepted");
            }
            if(helpers.notempty(endpoint) == false)
            {
                return Failure("Endpoint is empty");
            }
            if (helpers.notempty(endpointcode) == false)
            {
                return Failure("Endpointcode is empty");
            }

            string attempt_endpoint = endpoint + "endpoint.php";
            token = helpers.GetSHA1(helpers.UnixTimeNow().ToString() + "notecardnext" + endpointcode);
            var client = new RestClient(attempt_endpoint);
            var request = new RestRequest("notecard/next", Method.POST);
            request.AddParameter("token", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            IRestResponse endpoint_checks = client.Post(request);
            if (endpoint_checks.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return Failure("HTTP status code: " + endpoint_checks.StatusCode.ToString());
            }
            try
            {
                notecardendpoint server_reply = JsonConvert.DeserializeObject<notecardendpoint>(endpoint_checks.Content);
                if (server_reply.status == false)
                {
                    return Failure("Bad reply: " + server_reply.message);
                }
                if (server_reply.NotecardTitle.Length < 3)
                {
                    return Failure("Notecard title is to short");
                }
                return BasicReply(bot.SendNotecard(server_reply.NotecardTitle, server_reply.NotecardContent, (UUID)server_reply.AvatarUUID).ToString());
            }
            catch (Exception e)
            {
                return Failure("Error: " + e.Message + "");
            }
        }

        public class notecardendpoint : endpoint
        {
            public string AvatarUUID { get; set; }
            public string NotecardTitle { get; set; }
            public string NotecardContent { get; set; }
        }

        public class endpoint
        {
            public bool status { get; set; }
            public string message { get; set; }
        }
    }
}
