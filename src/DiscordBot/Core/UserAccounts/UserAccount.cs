﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core
{
    public class UserAccount
    {
        public ulong Id { get; set; }
        private int _currency;
        public int Currency
        {
            get => _currency;
            set { _currency = value; UserAccounts.SaveAccounts(); }
        }
        public bool IsCurrencyImaginary { get; set; }
        
        public void AddCurrency(int value)
        {
            Currency += value;
            UserAccounts.SaveAccounts();
        }
    }
}
