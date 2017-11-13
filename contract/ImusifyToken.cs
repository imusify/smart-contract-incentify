using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;


namespace contract
{
    public class ImusifyToken : SmartContract
    {
        public static string Name() => "Imusify";
        public static string Symbol() => "IMU";
        public static byte Decimals() => 8;
        public const ulong factor = 100000000;
        public const ulong totalsupply = 100000000 * factor;

        //Enable Transferred Action for NEP5 compliance as follows
        //
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
                    BigInteger balance = BalanceOf(account);
                    Runtime.Notify("imuBalance", account, balance);
                    return balance;
                }
                if (method == "levelOf")
                {
                    BigInteger level = LevelOf(account); ;
                    Runtime.Notify("imuLevel", account, level);
                    return level;
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
            byte[] balance = Storage.Get(Storage.CurrentContext, Key("balance", account));

            if (balance == null)
                return 0;

            return balance.AsBigInteger();
        }


        public static bool Deploy(byte[] account)
        {
            byte[] supply_check = Storage.Get(Storage.CurrentContext, "totalsupply");

            if (supply_check == null)
            {
                Storage.Put(Storage.CurrentContext, Key("balance", account), totalsupply);
                Storage.Put(Storage.CurrentContext, "totalsupply", totalsupply);
                Storage.Put(Storage.CurrentContext, "owner", account);
                return true;
            }
            return false;
        }


        private static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (amount >= 0)
            {
                BigInteger originValue = Storage.Get(Storage.CurrentContext, Key("balance", from)).AsBigInteger();
                BigInteger targetValue = Storage.Get(Storage.CurrentContext, Key("balance",   to)).AsBigInteger();

                BigInteger new_originValue = originValue - amount;
                BigInteger new_targetValue = targetValue + amount;

                if (new_originValue >= 0)
                {
                    Storage.Put(Storage.CurrentContext, Key("balance", from), new_originValue);
                    Storage.Put(Storage.CurrentContext, Key("balance", to)  , new_targetValue);

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


        private static BigInteger Reward(byte[] to)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, "owner");
            
            // if (Runtime.CheckWitness(owner)) // Enable this clause after CoZ contest

            BigInteger amount = RewardFunction(LevelUp(to));

            Transfer(owner, to, amount);

            return amount;
        }


        private static string Key(string prefix, byte[] bytes)
        {
            string hexString = "";

            foreach (char c in prefix) hexString += c;
            foreach (byte b in bytes ) hexString += b;

            return hexString;
        }


        private static BigInteger RewardFunction(BigInteger level)
        {
            BigInteger basic_reward = 100000000;

            BigInteger bonus_factor = 1;

            if (level > 1) bonus_factor = 2;
            if (level > 3) bonus_factor = 5;
            if (level > 9) bonus_factor = 20;

            return bonus_factor * basic_reward;
        }


        private static BigInteger LevelOf(byte[] account)
        {
            BigInteger nLevel = 0;

            byte[] level = Storage.Get(Storage.CurrentContext, Key("level", account));

            if (level != null)
                nLevel = level.AsBigInteger();

            return nLevel;
        }


        private static BigInteger LevelUp(byte[] account)
        {
            BigInteger new_level = LevelOf(account) + 1;
            
            Storage.Put(Storage.CurrentContext, Key("level", account), new_level);
            
            return new_level;
        }
    }
}
