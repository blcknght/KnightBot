﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using System.Diagnostics;
using System.Linq;
using KnightBot.Config;
using KnightBot.util;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;

namespace KnightBot.Modules.Economy
{

    [Group("bank")]
    public class BankModule : ModuleBase
    {

        Errors errors = new Errors();

        private BankConfig save = new BankConfig();

        public static readonly string appdir = AppContext.BaseDirectory;

        [Command("open")]
        public async Task bankOpen()
        {
            if (!File.Exists(appdir + "bank/" + Context.User.Id.ToString() + ".json"))
            {
                var newServer = File.Create(Path.Combine(appdir, "bank/" + Context.User.Id.ToString() + ".json"));

                newServer.Close();

                save.userID = Context.User.Id.ToString();
                save.currentMoney = 100;
                save.currentPoints = 0;
                save.Save("bank/" + Context.User.Id.ToString() + ".json");


                var embed = new EmbedBuilder()

                {
                    Color = Colors.moneyCol,
                };


                embed.Title = $"{Context.User.Username} Has Opened A Bank Account!";
                embed.Description = $"\n:money_with_wings: **Welcome To The Bank!** :\n\n:moneybag: **Bank : OneDollaBill**\n";
                await ReplyAsync("", false, embed.Build());

            }
            else await errors.sendError(Context.Channel, "User already has a bank account", Colors.moneyCol);

            //save the users file
            save.userID = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").userID;
            save.currentMoney = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney;
            save.currentPoints = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentPoints;

            save.Save("bank/" + Context.User.Id.ToString() + ".json");
        }

        [Command("balance")]
        public async Task bankBalance()
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.moneyCol,
            };

            embed.Title = $"{Context.User.Username}'s Balance";
            embed.Description = $"\n:money_with_wings: **Balance** :\n\n:moneybag: **{BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney}**\n";
            await ReplyAsync("", false, embed.Build());
        }

        [Command("transfer")]
        public async Task bankTransfer(IGuildUser user, int moneytotransfer)
        {
            int bal1 = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney;

            int bal2 = BankConfig.Load("bank/" + user.Id.ToString() + ".json").currentMoney;

            if (BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney >= moneytotransfer)
            {
                save.currentMoney = bal1 - moneytotransfer;
                save.currentPoints = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentPoints;
                save.userID = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").userID;

                save.Save("bank/" + Context.User.Id.ToString() + ".json");


                if (BankConfig.Load("bank/" + user.Id.ToString() + ".json").currentMoney >= moneytotransfer)
                {
                    save.currentMoney = bal2 + moneytotransfer;
                    save.currentPoints = BankConfig.Load("bank/" + user.Id.ToString() + ".json").currentPoints;
                    save.userID = BankConfig.Load("bank/" + user.Id.ToString() + ".json").userID;

                    save.Save("bank/" + user.Id.ToString() + ".json");

                }

                int finalbal1 = BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney;

                int finalbal2 = BankConfig.Load("bank/" + user.Id.ToString() + ".json").currentMoney;


                var embed = new EmbedBuilder() { Color = Colors.moneyCol };
                var fromField = new EmbedFieldBuilder() { Name = Context.User.Username + "'s new balance:", Value = "$" + finalbal1 };
                var toField = new EmbedFieldBuilder() { Name = user.Username + "'s new balance:", Value = "$" + finalbal2 };

                embed.Title = ("Bank Transfer");
                embed.Description = ("Successfully transferred money!");
                embed.AddField(fromField);
                embed.AddField(toField);

                await Context.Channel.SendMessageAsync("", false, embed);


            } else if (BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney < moneytotransfer)
            {
                var embed = new EmbedBuilder() { Color = Colors.moneyCol };


                embed.Title = ("Bank Transfer");
                embed.Description = ("You Cannot Transfer What You Do Not Have!");


                await Context.Channel.SendMessageAsync("", false, embed);

            } else if (moneytotransfer > BankConfig.Load("bank/" + Context.User.Id.ToString() + ".json").currentMoney)
            {
                var embed = new EmbedBuilder() { Color = Colors.moneyCol };


                embed.Title = ("Bank Transfer");
                embed.Description = ("You Cannot Transfer What You Do Not Have!");


                await Context.Channel.SendMessageAsync("", false, embed);
            }
            


        }


    }
}
