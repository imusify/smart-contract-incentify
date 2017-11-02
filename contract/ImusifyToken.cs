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
        private const ulong factor = 100000000; //decided by Decimals() // why is this still there?
        private static BigInteger initialSupply = 100000000;


        public static readonly byte[] Owner = { 159, 243, 169, 254, 13, 229, 158, 123, 147, 6, 65, 141, 170, 124, 37, 124, 23, 231, 250, 6, 122, 98, 209, 238, 78, 48, 88, 145, 14, 145, 155, 214, 2 };


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

                    return true;
                }

                else if (method == "testverify") // tmp
                {
                    Runtime.Log("called testverify"); // tmp

                    if (VerifySignature(Owner, account))
                    {
                        Runtime.Notify("worked", account);
                        return true;
                    }
                    else // tmp
                    {
                        Runtime.Notify("didn't work", account);
                        return true;
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
                        Runtime.Log("NOT method == transfer && args.Length == 3"); // tmp
                        return false;
                    }
                }

                else
                    return false;  
            }

            else
                return false;
        }


        private static bool Transfer(byte[] origin, byte[] receiver, BigInteger amount)
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
                    Storage.Put(Storage.CurrentContext, origin,  IntToBytes(nOriginValue));
                    Storage.Put(Storage.CurrentContext, receiver, IntToBytes(nTargetValue));

                    Runtime.Notify("Transfer Successful", origin, receiver, amount, Blockchain.GetHeight());

                    //Transferred(origin, receiver, amount);

                    return true;
                }
            }

            return false;
        }


        public static bool Deploy()
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

            return true;
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