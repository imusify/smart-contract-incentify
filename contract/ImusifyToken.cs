using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace contract
{
    public class ImusifyToken : SmartContract
    {
        public static string Name() => "Imusify";
        public static string Symbol() => "IMU";
        public static byte Decimals() => 8;
        private const ulong factor = 100000000; //decided by Decimals()
        private static BigInteger initialSupply = (BigInteger)100000000 * factor;


        public static readonly byte[] Owner = {2, 36, 228, 168, 201, 122, 97, 84, 88, 1, 96, 218, 66, 99, 122, 101, 163, 210, 255, 39, 160, 115, 42, 135, 68, 45, 19, 200, 44, 185, 158, 22, 146 };

        // We want this back for NEP5
        //
        //[DisplayName("transfer")]
        //public static event Action<byte[], byte[], BigInteger> Transferred;


        public static object Main(string method, params object[] args)
        {
            // todo: check if notify byte arrays are possible/readable
            // todo: delete storage

            if (method == "owner") // tmp clause
            {
                Runtime.Notify("Owner", Owner);

                return Owner;
            }
            else if (method == "deploy")
            {
                Runtime.Log("Called deploy"); // tmp
                return Deploy();
            }
            else if (method == "totalSupply")
            {
                Runtime.Log("Called total Supply"); // tmp
                byte[] totalsupply = Storage.Get(Storage.CurrentContext, "totalSupply");
                BigInteger totalsupply_ = BytesToInt(totalsupply);
                Runtime.Notify("account", totalsupply_); // tmp
                Runtime.Notify("account", totalsupply); // tmp

                return totalsupply;
            }
            else if (method == "name")
            {
                Runtime.Log("name"); // tmp
                return Name();
            }
            else if (method == "symbol")
            {
                Runtime.Log("symbol"); // tmp
                return Symbol();
            }
            else if (method == "decimals")
            {
                Runtime.Log("decimals"); // tmp
                return Decimals();
            }
            else if (args.Length > 0)
            {
                Runtime.Log("args.Length > 0"); // tmp

                byte[] account = (byte[])args[0];

                Runtime.Notify("account", account); // tmp

                if (method == "balanceOf")
                {
                    Runtime.Log("called balanceOf"); // tmp

                    byte[] balance = Storage.Get(Storage.CurrentContext, account);
                    BigInteger balance_ = BytesToInt(balance);
                    Runtime.Notify("account", balance_); // tmp
                    Runtime.Notify("account", balance); // tmp

                    return 0;
                }
                else if (method == "reward") // tmp
                {
                    Runtime.Log("reward"); // tmp
                    return Reward(account);
                }
                else if (method == "testverify1") // tmp
                {
                    Runtime.Log("called testverify1"); // tmp

                    if (VerifySignature(Owner, account))
                    {
                        Runtime.Notify("VerifySignature worked", account);
                        return 2;
                    }
                    else // tmp
                    {
                        Runtime.Notify("NOT VerifySignature", account);
                        return 3;
                    }
                }
                else if (method == "testverify2") // tmp
                {
                    Runtime.Log("called testverify2"); // tmp

                    bool isCaller = Runtime.CheckWitness(Owner);

                    if (isCaller)
                    {
                        Runtime.Notify("CheckWitness worked", account);
                        return 4;
                    }
                    else // tmp
                    {
                        Runtime.Notify("NOT isCaller", account);
                        return 5;
                    }
                }
                else if (method == "transfer" && args.Length == 3)
                {
                    Runtime.Log("called method == transfer WITH args.Length == 3"); // tmp

                    Runtime.Log("called transfer"); // tmp

                    bool isCaller = Runtime.CheckWitness(account);

                    Runtime.Notify("isCaller", isCaller); // tmp

                    if (isCaller)
                    {
                        byte[] receiver = (byte[])args[1];

                        BigInteger amount = BytesToInt((byte[])args[2]);

                        Runtime.Notify("amount", amount); // tmp

                        return Transfer(account, receiver, amount);
                    }
                    else
                    {
                        Runtime.Log("NOT isCaller"); // tmp
                        return 6;
                    }
                }
                else
                {
                    Runtime.Log("NOT method == transfer && args.Length == 3"); // tmp

                    return 7;
                }
            }
            else
            {
                Runtime.Notify("not a suitable method for args.Length > 0", method); // tmp

                return 8;
            }
        }

        private static int Reward(byte[] receiver)
        {
            Runtime.Log("called Reward"); // tmp

            bool isCaller = Runtime.CheckWitness(Owner);

            if (isCaller)
            {
                Runtime.Notify("CheckWitness Reward worked", receiver);

                int new_count = GetCount() + 1;
         
                BigInteger amount = RewardFunction(new_count);

                Storage.Put(Storage.CurrentContext, "count", IntToBytes(new_count));

                Runtime.Notify("amount", amount);

                Transfer(Owner, receiver, amount);

                Runtime.Notify("Transfer Reward worked", receiver);

                return 9;
            }
            else // tmp
            {
                Runtime.Notify("didn't work", receiver);

                return 10;
            }
        }

        private static BigInteger RewardFunction(int n)
        {
            int x = 3 * n;

            Runtime.Notify("RewardFunction x", x); // tmp

            BigInteger y = (BigInteger)x;

            return y;
        }

        private static int GetCount()
        {

            byte[] count_ = Storage.Get(Storage.CurrentContext, "count");

            BigInteger count = BytesToInt(count_);

            Runtime.Notify("count", count);

            return (int)count;
        }

        private static int Transfer(byte[] origin, byte[] receiver, BigInteger amount)
        {
            // NOTE: The ICO template tranfer fuction is more elaborate - take over the relevant if-clauses

            if (amount >= 0)
            {
                byte[] originValue = Storage.Get(Storage.CurrentContext, origin);
                byte[] targetValue = Storage.Get(Storage.CurrentContext, receiver);

                BigInteger nOriginValue = BytesToInt(originValue) - amount;
                BigInteger nTargetValue = BytesToInt(targetValue) + amount;

                Runtime.Notify("account", nOriginValue); // tmp
                Runtime.Notify("account", nTargetValue); // tmp

                if (nOriginValue >= 0)
                {
                    Storage.Put(Storage.CurrentContext, origin, IntToBytes(nOriginValue));
                    Storage.Put(Storage.CurrentContext, receiver, IntToBytes(nTargetValue));

                    Runtime.Notify("Transfer Successful", origin, receiver, amount, Blockchain.GetHeight());

                    //Transferred(origin, receiver, amount);

                    return 11;
                }
            }

            return 12;
        }


        public static int Deploy()
        {
            // NOTE: this can be deployed over and over again. To prevent this, do
            // byte[] total_supply = Storage.Get(Storage.CurrentContext, "totalSupply");
            // if (total_supply.Length != 0) return false;

            Storage.Put(Storage.CurrentContext, Owner, IntToBytes(initialSupply));
            Storage.Put(Storage.CurrentContext, "totalSupply", IntToBytes(initialSupply));

            Runtime.Notify("Deployed with", initialSupply); // tmp
            byte[] check1 = Storage.Get(Storage.CurrentContext, "totalSupply"); // tmp
            Runtime.Notify("Deployed with", BytesToInt(check1)); // tmp

            //Transferred(null, Owner, initialSupply);

            return 13;
        }


        private static byte[] IntToBytes(BigInteger value)
        {
            Runtime.Log("called IntToBytes"); // tmp

            return value.ToByteArray();
        }


        private static BigInteger BytesToInt(byte[] array)
        {
            Runtime.Log("called BytesToInt"); // tmp

            var buffer = new BigInteger(array);
            return buffer;
        }


    }
}