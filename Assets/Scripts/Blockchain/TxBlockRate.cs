﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-tx-block-rate
 */
public class TxBlockRate : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTxBlockRate";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTxBlockRate
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<string> paramsList;
    }

    void Start()
    {
        if (runAtStart)
            StartCoroutine(RunMethod());

        if (runForSeveralTimes)
            StartCoroutine(RunMethodCoroutine());
    }

    IEnumerator RunMethod()
    {
        GetTxBlockRate getTxBlockRate = new GetTxBlockRate
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getTxBlockRate.paramsList.Add("");

        string json = JsonUtility.ToJson(getTxBlockRate);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getTxBlockRateReq = new ZilRequest(METHOD, new object[] { 1 });
        yield return StartCoroutine(PostRequest<GetTxBlockRateResponse>(getTxBlockRateReq, (response, error) =>
        {
            if (response != null)
            {
                Debug.Log("BlockRate:" + response.result.ToString());
            }
            else if (error != null)
            {
                Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
            }
        }
            ));
    }

    IEnumerator RunMethodCoroutine()
    {
        //if run at start also enabled, then wait before first batch run
        if (runAtStart)
            yield return new WaitForSeconds(runDelay);

        for (int i = 1; i <= runTimes; i++)
        {
            yield return RunMethod();
            yield return new WaitForSeconds(runDelay);
        }
    }
}
