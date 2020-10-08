using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BuildChecker.Classes.Structures
{
    public class EsrpDecrypt
    {
        [DataMember(Name = "KeyData")]
        public string KeyData { get; set; }

        [DataMember(Name = "EncryptionBufferSize")]
        public long EncryptionBufferSize { get; set; }

        [DataMember(Name = "AlgorithmName")]
        public string AlgorithmName { get; set; }

        [DataMember(Name = "ChainingMode")]
        public string ChainingMode { get; set; }
    }
}
