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
        public static readonly byte[] Owner = { 101, 198, 101, 159, 8, 177, 16, 196, 26, 207, 24, 21, 26, 164, 18, 56, 136, 55, 255, 30, 196, 181, 47, 131, 21, 10, 70, 133, 69, 143, 62, 197 };
        public static byte Decimals() => 8;
        private const ulong factor = 100000000; //decided by Decimals()
        private static ulong initialSupply = 1000000 * factor;
        

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;
        
        public static object Main(string method, params object[] args)
        {
            
            if (method == "deploy") return Deploy();
            
            if (method == "totalSupply") return Storage.Get(Storage.CurrentContext, "supply");

            if (method == "name") return Name();

            if (method == "symbol") return Symbol();

            if (method == "decimals") return Decimals();

            if (method == "balanceOf") return Storage.Get(Storage.CurrentContext, (byte[])args[0]);

            //Verify that the originator is honest.           
            if (!Runtime.CheckWitness((byte[])args[0])) return false;

            if (method == "transfer") return Transfer((byte[])args[0], (byte[])args[1], BytesToInt((byte[])args[2]));

            return false;
        }

        public static bool Deploy()
        {
            byte[] total_supply = Storage.Get(Storage.CurrentContext, "totalSupply");
            if (total_supply.Length != 0) return false;
            Storage.Put(Storage.CurrentContext, Owner, initialSupply);
            Storage.Put(Storage.CurrentContext, "totalSupply", initialSupply);
            Transferred(null, Owner, initialSupply);
            return true;
        }


        private static bool Transfer(byte[] originator, byte[] to, BigInteger amount)
        {
            //Get the account value of the source and destination accounts.
            var originatorValue = Storage.Get(Storage.CurrentContext, originator);
            var targetValue = Storage.Get(Storage.CurrentContext, to);


            BigInteger nOriginatorValue = BytesToInt(originatorValue) - amount;
            BigInteger nTargetValue = BytesToInt(targetValue) + amount;
            
            //If the transaction is valid, proceed.
            if (nOriginatorValue >= 0 &&  amount >= 0)
            {
                Storage.Put(Storage.CurrentContext, originator, IntToBytes(nOriginatorValue));
                Storage.Put(Storage.CurrentContext, to, IntToBytes(nTargetValue));
                Runtime.Notify("Transfer Successful", originator, to, amount, Blockchain.GetHeight());
                Transferred(originator, to, amount);
                return true;
            }
            return false;
        }


        private static byte[] IntToBytes(BigInteger value)
        {
            byte[] buffer = value.ToByteArray();
            return buffer;
        }


        private static BigInteger BytesToInt(byte[] array)
        {
            var buffer = new BigInteger(array);
            return buffer;
        }


    }
}
