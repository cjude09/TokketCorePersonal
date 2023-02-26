using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Services.ServicesDB
{
    public class DBConstant
    {
        #region constants
        public const int TOKS_PER_PARTITION = 1000000;
        #region Defaults
        /// <summary> Default Cosmos DB Database. </summary>
        public const string DefaultDB = "Tokket";

        /// <summary> Default Cosmos DB Container. </summary>
        public const string DefaultCNTR = "Knowledge";

        /// <summary> Default Cosmos DB User Database. </summary>
        public const string DefaultUserDB = "Tokket";

        /// <summary> Default Cosmos DB User Container. </summary>
        public const string DefaultUserCNTR = "Users";

        /// <summary> Default Cosmos DB Transactions Container. </summary>
        public const string DefaultTransactionsCNTR = "Transactions";
        #endregion
        #region Points and Coins
        public const int CoinsBasicTok = 2;
        public const int CoinsDetailedTok = 5;
        public const int CoinsMegaTok = 10;

        public const int CoinsAvatar = 10;

        public const long FOLLOW_POINTS = 5;
        public const long FOLLOW_COINS = 0;
        public const long REACTION_POINTS = 5;
        public const long REACTION_COINS = 0;
        public const long SET_POINTS = 50;
        public const long SET_COINS = 1;
        public const long REPLICATE_POINTS = 50;
        public const long REPLICATE_COINS = 1;
        public const long NONDETAILED_POINTS = 100;
        public const long NONDETAILED_COINS = 2;
        public const long DETAILED_POINTS = 200;
        public const long DETAILED_COINS = 4;
        #endregion
        #endregion
    }
}
