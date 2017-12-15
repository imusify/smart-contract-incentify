using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;
using System;
using System.ComponentModel;

namespace imusify
{
    public class ImusifyRewardSystem : SmartContract
    {
        public static string Name() => "Imusify";
        public static string Symbol() => "IMU";
        public static byte Decimals() => 8;
        public const ulong factor = 100000000;
        public const ulong totalsupply = 100000000 * factor;

        /* Enable Transferred Action for NEP5 compliance as follows */
        //[DisplayName("transfer")]
        //public static event Action<byte[], byte[], BigInteger> Transferred;

        public static object Main(string method, params object[] args)
        {
            if (method == "name")
                return Name();
            if (method == "symbol")
                return Symbol();
            if (method == "decimals")
                return Decimals();
            if (method == "totalsupply")
                return TotalSupply();
            if (args.Length > 0)
            {
                byte[] account = (byte[])args[0];

                if (method == "balanceOf")
                {
                    BigInteger balance = BalanceOf(account) + 1; // TODO: correct this +1 -1 hack
                    Runtime.Notify("imuBalance", account, balance - 1);
                    return balance - 1;
                }
                if (method == "levelOf")
                {
                    BigInteger level = LevelOf(account) + 1;  
                    Runtime.Notify("imuLevel", account, level - 1);
                    return level - 1;
                }
                if (method == "deploy")
                    return Deploy(account);
                if (method == "reward")
                    return Reward(account);
                if (method == "transfer")
                {
                    if (args.Length == 3)
                    {
                        byte[] to = (byte[])args[1];
                        BigInteger amount = (BigInteger)args[2];

                        //if Runtime.CheckWitness(account); // Enable this clause after CoZ contest
                        return Transfer(account, to, amount);
                    }
                }
            }
            return false;
        }


        private static BigInteger TotalSupply()
        {
            return Storage.Get(Storage.CurrentContext, "totalsupply").AsBigInteger();
        }


        private static BigInteger BalanceOf(byte[] account)
        {
            byte[] balance = Storage.Get(Storage.CurrentContext, Key("B", account));

            if (balance == null)
                return 0;

            return balance.AsBigInteger();
        }


        public static bool Deploy(byte[] account)
        {
            byte[] supplyCheck = Storage.Get(Storage.CurrentContext, "totalsupply");

            if (supplyCheck == null)
            {
                Storage.Put(Storage.CurrentContext, "owner", account);
                byte[] owner = Storage.Get(Storage.CurrentContext, "owner");
                Storage.Put(Storage.CurrentContext, Key("B", owner), totalsupply);
                Storage.Put(Storage.CurrentContext, "totalsupply", totalsupply);
                return true;
            }
            return false;
        }


        private static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (amount >= 0)
            {
                if (from == to)
                    return true;

                BigInteger originValue = Storage.Get(Storage.CurrentContext, Key("B", from)).AsBigInteger();
                BigInteger targetValue = Storage.Get(Storage.CurrentContext, Key("B", to)).AsBigInteger();

                BigInteger new_originValue = originValue - amount;
                BigInteger new_targetValue = targetValue + amount;

                if (originValue >= amount)
                {
                    Runtime.Log("Starting transfer...");
                    Storage.Put(Storage.CurrentContext, Key("B", from), new_originValue);
                    Storage.Put(Storage.CurrentContext, Key("B", to)  , new_targetValue);

                    Runtime.Notify("imuLevel"  , from, LevelOf(from));
                    Runtime.Notify("imuLevel"  , to  , LevelOf(to));
                    Runtime.Notify("imuBalance", from, new_originValue);
                    Runtime.Notify("imuBalance", to  , new_targetValue);
                    //Transferred(from, to, amount);

                    return true;
                }
            }

            return false;
        }


        private static string Key(string prefix, byte[] account)
        {
            return string.Concat(prefix, account.AsString());
        }


        private static BigInteger Reward(byte[] account)
        {
            /// Reward user according to his or her user level
            
            byte[] owner = Storage.Get(Storage.CurrentContext, "owner");
            
            // if (Runtime.CheckWitness(owner)) // Enable this clause after CoZ contest
            BigInteger amount = RewardFunction(LevelUp(account));

            Transfer(owner, account, amount);

            return amount;
        }


        private static BigInteger RewardFunction(BigInteger level)
        {
            /// Compute user reward depending on user level
            
            BigInteger basicReward = 100000000; 
            // TODO test if I can just use 'factor' here
            BigInteger bonusFactor = 1;

            /// Quick level up for demonstration purpose before official launch
            if (level > 1) bonusFactor = 2;
            if (level > 3) bonusFactor = 5;
            if (level > 9) bonusFactor = 20;

            return bonusFactor * basicReward;
        }


        private static BigInteger LevelOf(byte[] account)
        {
            BigInteger nLevel = 0;

            byte[] level = Storage.Get(Storage.CurrentContext, Key("L", account));

            if (level != null)
                nLevel = level.AsBigInteger();

            return nLevel;
        }


        private static BigInteger LevelUp(byte[] account)
        {
            BigInteger newLevel = LevelOf(account) + 1;
            
            Storage.Put(Storage.CurrentContext, Key("L", account), newLevel);
            
            return newLevel;
        }
    }
}
