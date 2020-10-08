using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot
{
    public static class SecretKeys
    {
        public const string ApiUrlKey = "F4BC7E9794F402682BDD3705B4333569E995DB3D1D084F4BC7E9794F402682BDD3705B4333569E995DB3D1D084D5240BDD3705B4333569E995DB3D1D084D5240";

#if PRODUCTION
        public const string TimeLimitMasterKey = "OOF";
        public const string ProductMasterKey = "OOF v2";
        public const string ProductMasterBase = "UU";
        public const string ApiKey = "YOUR_KEY";
#else
        public const string TimeLimitMasterKey = "UNLIMITED POWEEEEEEEEEEEER";
        public const string ProductMasterKey = "nonProdMasterKey oksir";
        public const string ProductMasterBase = "ZZ";
        public const string ApiKey = "YOUR_KEY";
#endif

        public const string DevCertPass = "Eh, volevi! Guarda che faccia, non se lo aspettava!";
        public const string ProdCertPass = "Eh, volevi! Guarda che faccia, non se lo aspettava! v2";

        public const string ProdHost = "PROD_URL";
        public const string DevHost = "DEV_URL:8443";
        public const string BaseHost = "adeltax.com";
    }
}
