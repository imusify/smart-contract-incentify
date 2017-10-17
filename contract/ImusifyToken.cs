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
        public static readonly byte[] Owner = { 159, 243, 169, 254, 13, 229, 158, 123, 147, 6, 65, 141, 170, 124, 37, 124, 23, 231, 250, 6, 122, 98, 209, 238, 78, 48, 88, 145, 14, 145, 155, 214, 2 };
        public static byte Decimals() => 8;
        private const ulong factor = 100000000; //decided by Decimals()
        private static BigInteger initialSupply = 100000000;
        

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;
        
        public static object Main(string method, params object[] args)
        {
            
            if (method == "deploy") return Deploy();
            
            if (method == "totalSupply") return Storage.Get(Storage.CurrentContext, "totalSupply");

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
            Storage.Put(Storage.CurrentContext, Owner, IntToBytes(initialSupply));
            Storage.Put(Storage.CurrentContext, "totalSupply", IntToBytes(initialSupply));
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
