// Copyright (c) 2016 Duality Blockchain Solutions
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace Blockchain.RPC
{
    public enum Chain {
        Dynamic,
        Sequence
    }

    //  Note: Do not alter the capitalization of the enum members as they are being cast as-is to the RPC server
    public enum SequenceRpcMethods {
        //== Decentralised DNS ==
        name_delete,        // <name>
        name_filter,        // [[[[[regexp] maxage=0] from=0] nb=0] stat]
        name_history,       // "name" ( fullhistory )
        name_list,          // [<name>]
        name_mempool,       // 
        name_new,           // <name> <value> <days> [toaddress] [valueAsFilepath]
        name_scan,          // [start-name] [max-returned] [max-value-length=-1]
        name_show,          // <name> [filepath]
        name_update,        // <name> <value> <days> [toaddress] [valueAsFilepath]

        //== Blockchain ==
        getbestblockhash,
        getblock,           // "hash" ( verbose )
        getblockchaininfo,
        getblockcount,
        getblockhash,       // index
        getchaintips,
        getdifficulty,
        getmempoolinfo,
        getrawmempool,      // ( verbose )
        gettxout,           // "txid" n ( includemempool )
        gettxoutsetinfo,    //
        verifychain,        // ( checklevel numblocks )

        //== Control ==
        getinfo,
        help, // ( "command" )
        stop,

        //== Generating ==
        getgenerate,
        gethashespersec,
        setgenerate, // generate ( genproclimit )

        //== Mining ==
        getblocktemplate, // ( "jsonrequestobject" )
        getlastpowblock, // [nHeight]
        getmininginfo,
        getnetworkhashps, // ( blocks height )
        getstakinginfo, //
        getsubsidy, // [nTarget]
        getwork, // ( "data" )
        prioritisetransaction, // <txid> <priority delta> <fee delta>
        submitblock, // "hexdata" ( "jsonparametersobject" )
        
        //== Network ==
        addnode, // "node" "add|remove|onetry"
        getaddednodeinfo, // dns ( "node" )
        getcheckpoint, //
        getconnectioncount, //
        getnettotals, //
        getnetworkinfo, //
        getpeerinfo, //
        ping, //

        //== Rawtransactions ==
        createrawtransaction, // [{"txid":"id","vout":n},...] {"address":amount,...}
        decoderawtransaction, // "hexstring"
        decodescript, // "hex"
        getrawtransaction, // "txid" ( verbose )
        sendrawtransaction, // "hexstring" ( allowhighfees )
        signrawtransaction, // "hexstring" ( [{"txid":"id","vout":n,"scriptPubKey":"hex","redeemScript":"hex"},...] ["privatekey1",...] sighashtype )

        //== Util ==
        addmultisigaddress, // nrequired["key",...] ( "account" )
        createmultisig, // nrequired["key",...]
        estimatefee, // nblocks
        estimatepriority, // nblocks
        validateaddress, // "silkaddress"
        verifymessage, // "silkaddress" "signature" "message"

        //== Wallet ==
        backupwallet, // "destination"
        dumpprivkey, // "silkaddress"
        dumpwallet, // "filename"
        encryptwallet, // "passphrase"
        getaccount, // "silkaddress"
        getaccountaddress, // "account"
        getaddressesbyaccount, // "account"
        getbalance, // ( "account" minconf includeWatchonly)
        getnewaddress, // ( "account" )
        getrawchangeaddress, //
        getreceivedbyaccount, // "account" ( minconf )
        getreceivedbyaddress, // "silkaddress" ( minconf )
        gettransaction, // "txid" ( includeWatchonly )
        getunconfirmedbalance, //
        getwalletinfo, //
        importaddress, // "address" ( "label" rescan )
        importprivkey, // "silkprivkey" ( "label" rescan )
        importwallet, // "filename"
        keypoolrefill, //(newsize )
        listaccounts, //(minconf includeWatchonly)
        listaddressgroupings, //
        listlockunspent, //
        listreceivedbyaccount, //(minconf includeempty includeWatchonly)
        listreceivedbyaddress, //(minconf includeempty includeWatchonly)
        listsinceblock, //( "blockhash" target-confirmations includeWatchonly)
        listtransactions, //( "account" count from includeWatchonly)
        listunspent, //(minconf maxconf  ["address",...] )
        lockunspent, // unlock[{"txid":"txid", "vout":n},...]
        makekeypair, //[prefix]
        move, // "fromaccount" "toaccount" amount(minconf "comment" )
        reservebalance, //[< reserve > [amount]]
        sendfrom, // "fromaccount" "tosilkaddress" amount(minconf "comment" "comment-to" )
        sendmany, // "fromaccount" {"address":amount,...} ( minconf "comment" )
        sendtoaddress, // "silkaddress" amount( "comment" "comment-to" )
        setaccount, // "silkaddress" "account"
        settxfee, // amount
        signmessage // "silkaddress" "message" 
    }

        //  Note: Do not alter the capitalization of the enum members as they are being cast as-is to the RPC server
    public enum DynamicRpcMethods {
            //TODO (Amir): Update RPC command list after Dynamic is released.
            //== Blockchain ==
            getbestblockhash,
            getblock,
            getblockchaininfo,
            getblockcount,
            getblockhash,
            getblockheader,
            getchaintips,
            getdifficulty,
            getmempoolinfo,
            getrawmempool,
            gettxout,
            gettxoutproof,
            gettxoutsetinfo,
            verifychain,
            verifytxoutproof,

            //== Control ==
            getinfo,
            help,
            stop,

            //== Generating ==
            generate,
            getgenerate,
            setgenerate,

            //== Mining ==
            getblocktemplate,
            getmininginfo,
            getnetworkhashps,
            prioritisetransaction,
            submitblock,

            //== Network ==
            addnode,
            clearbanned,
            disconnectnode,
            getaddednodeinfo,
            getconnectioncount,
            getnettotals,
            getnetworkinfo,
            getpeerinfo,
            listbanned,
            ping,
            setban,

            //== Rawtransactions ==
            createrawtransaction,
            decoderawtransaction,
            decodescript,
            fundrawtransaction,
            getrawtransaction,
            sendrawtransaction,
            signrawtransaction,
            sighashtype,

            //== Util ==
            createmultisig,
            estimatefee,
            estimatepriority,
            estimatesmartfee,
            estimatesmartpriority,
            validateaddress,
            verifymessage,

            //== Wallet ==
            abandontransaction,
            addmultisigaddress,
            backupwallet,
            dumpprivkey,
            dumpwallet,
            getaccount,
            getaccountaddress,
            getaddressesbyaccount,
            getbalance,
            getnewaddress,
            getrawchangeaddress,
            getreceivedbyaccount,
            getreceivedbyaddress,
            gettransaction,
            getunconfirmedbalance,
            getwalletinfo,
            importaddress,
            importprivkey,
            importpubkey,
            importwallet,
            keypoolrefill,
            listaccounts,
            listaddressgroupings,
            listlockunspent,
            listreceivedbyaccount,
            listreceivedbyaddress,
            listsinceblock,
            listtransactions,
            listunspent,
            lockunspent,
            move,
            sendfrom,
            sendmany,
            sendtoaddress,
            setaccount,
            settxfee,
            signmessage,
            walletlock,
            walletpassphrase,
            walletpassphrasechange
        }

    //  As of 2016-11-15: https://github.com/SilkNetwork/Silk-Core/blob/master/src/rpc/rpcprotocol.h
    //  Note: Do not alter enum members' capitalization
    //  Note: It's alright if the enum is not complete (for altcoins etc), the plain rpc error code number will be used instead in RpcInternalServerErrorException()
    public enum RpcErrorCode
    {
        //! Standard JSON-RPC 2.0 errors
        RPC_INVALID_REQUEST = -32600,
        RPC_METHOD_NOT_FOUND = -32601,
        RPC_INVALID_PARAMS = -32602,
        RPC_INTERNAL_ERROR = -32603,
        RPC_PARSE_ERROR = -32700,

        //! General application defined errors
        RPC_MISC_ERROR = -1,  //! std::exception thrown in command handling
        RPC_FORBIDDEN_BY_SAFE_MODE = -2,  //! Server is in safe mode, and command is not allowed in safe mode
        RPC_TYPE_ERROR = -3,  //! Unexpected type was passed as parameter
        RPC_INVALID_ADDRESS_OR_KEY = -5,  //! Invalid address or key
        RPC_OUT_OF_MEMORY = -7,  //! Ran out of memory during operation
        RPC_INVALID_PARAMETER = -8,  //! Invalid, missing or duplicate parameter
        RPC_DATABASE_ERROR = -20, //! Database error
        RPC_DESERIALIZATION_ERROR = -22, //! Error parsing or validating structure in raw format
        RPC_VERIFY_ERROR = -25, //! General error during transaction or block submission
        RPC_VERIFY_REJECTED = -26, //! Transaction or block was rejected by network rules
        RPC_VERIFY_ALREADY_IN_CHAIN = -27, //! Transaction already in chain
        RPC_IN_WARMUP = -28, //! Client still warming up

        //! Aliases for backward compatibility
        RPC_TRANSACTION_ERROR = RPC_VERIFY_ERROR,
        RPC_TRANSACTION_REJECTED = RPC_VERIFY_REJECTED,
        RPC_TRANSACTION_ALREADY_IN_CHAIN = RPC_VERIFY_ALREADY_IN_CHAIN,

        //! P2P client errors
        RPC_CLIENT_NOT_CONNECTED = -9,  //! silk is not connected
        RPC_CLIENT_IN_INITIAL_DOWNLOAD = -10, //! Still downloading initial blocks
        RPC_CLIENT_NODE_ALREADY_ADDED = -23, //! Node is already added
        RPC_CLIENT_NODE_NOT_ADDED = -24, //! Node has not been added before

        //! Wallet errors
        RPC_WALLET_ERROR = -4,  //! Unspecified problem with wallet (key not found etc.)
        RPC_WALLET_INSUFFICIENT_FUNDS = -6,  //! Not enough funds in wallet or account
        RPC_WALLET_INVALID_ACCOUNT_NAME = -11, //! Invalid account name
        RPC_WALLET_KEYPOOL_RAN_OUT = -12, //! Keypool ran out, call keypoolrefill first
        RPC_WALLET_UNLOCK_NEEDED = -13, //! Enter the wallet passphrase with walletpassphrase first
        RPC_WALLET_PASSPHRASE_INCORRECT = -14, //! The wallet passphrase entered was incorrect
        RPC_WALLET_WRONG_ENC_STATE = -15, //! Command given in wrong wallet encryption state (encrypting an encrypted wallet etc.)
        RPC_WALLET_ENCRYPTION_FAILED = -16, //! Failed to encrypt the wallet
        RPC_WALLET_ALREADY_UNLOCKED = -17, //! Wallet is already unlocked

        //! silk/ppcoin
        RPC_INSUFFICIENT_SEND_AMOUNT = -101,//! Transaction output is below minimum
    }

    public enum HTTPStatusCode
    {
        HTTP_OK = 200,
        HTTP_BAD_REQUEST = 400,
        HTTP_UNAUTHORIZED = 401,
        HTTP_FORBIDDEN = 403,
        HTTP_NOT_FOUND = 404,
        HTTP_INTERNAL_SERVER_ERROR = 500,
        HTTP_SERVICE_UNAVAILABLE = 503,
    }
}
