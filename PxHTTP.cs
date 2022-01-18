using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
# Update
    - 2022-01-18[jp] : 최초 제작일

# Example 1 : Internet Check
    PxHTTP.IsInternetConnected()

# Example 2 : API Call [POST, GET, PUT, DELETE]
    PxHTTP http = new PxHTTP(this, false);
    http.Call("http://localhost:3001/droids/0",  //URL 
                PxHTTP.APIType.GET,              //Method
                new Dictionary<string, string> { {"Content-Type", "application/json"} }, //Header
                null,                            //Body (JSON String)
                (value) => {                     //OnSuccessCallback
                    Debug.Log("OnSuccessCallback : " + value);
                },
                (errorMsg) => {                  //OnFailCallback
                    Debug.Log("OnFailCallback : " + errorMsg);
                });
*/


public class PxHTTP
{
    private MonoBehaviour   _context;
    private bool            _enableLog; //Log 판단 여부(Default: true). Error는 반드시 활성화.

    public enum APIType {
        POST,   //등록
        GET,    //조회
        PUT,    //수정
        DELETE  //삭제
        //UnityWebReuqest에 Patch는 따로 존재하지 않음.
    }

    public PxHTTP(MonoBehaviour context, bool enableLog = true) {
        this._context = context;
        this._enableLog = enableLog;
    }

    #region Public Methods
    public static bool IsInternetConnected() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            return false;
        } else {
            return true;
        }
    }

    public void Call(string url, APIType type, Dictionary<string, string> header, string body, System.Action<string> callbackOnSuccess, System.Action<string> callbackOnFail) {
        //인터넷 연결 체크
        if ( !IsInternetConnected() ) {
            callbackOnFail?.Invoke("API Call[" + type + "] \n\tURL: " + url + "\n\tERROR: Internet disconnected.");
            return;
        }

        _context.StartCoroutine( _Call(url, type, header, body, callbackOnSuccess, callbackOnFail) );
    }
    #endregion

    #region Private Methods
    private void PrintLog(string logStr) {
        if ( _enableLog )
            Debug.Log( logStr );
    }

    private IEnumerator _Call(string url, APIType type, Dictionary<string, string> header, string body, System.Action<string> callbackOnSuccess, System.Action<string> callbackOnFail) {
        string logStr = "PxHTTP " + type + "[" + url + "]" + "\n";

        UnityWebRequest request = null;

        if ( type == APIType.POST ) {
            request = UnityWebRequest.Post(url, body);
        } else if ( type == APIType.GET ) {
            request = UnityWebRequest.Get(url);
        } else if ( type == APIType.PUT ) {
            request = UnityWebRequest.Put(url, body);   
        } else if ( type == APIType.DELETE ) {
            request = UnityWebRequest.Delete(url);
        }

        if ( request != null )
        {
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            logStr += "\t - Header" + "\n";
            if ( header != null ) {
                foreach( KeyValuePair<string, string> item in header ) {
                    logStr += "\t\t > " + item.Key + " : " + item.Value + "\n";
                    request.SetRequestHeader( item.Key, item.Value );
                }
            }

            logStr += "\t - Body" + "\n";
            if ( body != null && body != "" ) {
                logStr += "\t\t > " + body + "\n";

                byte[] jsonBodyToBytes = new System.Text.UTF8Encoding().GetBytes( body );
                request.uploadHandler = new UploadHandlerRaw( jsonBodyToBytes );
            }

            yield return request.SendWebRequest();

            if ( request.isNetworkError || request.isHttpError ) {
                logStr += "\t - Result [Fail] : " + request.error + "\n";
                callbackOnFail?.Invoke( request.error );
            } else {
                logStr += "\t - Result [Success] : " + request.downloadHandler.text + "\n";
                callbackOnSuccess?.Invoke( request.downloadHandler.text );
            }
        } else {
            logStr += "\t - Result [Fail] : Unknown API Type \n";
            callbackOnFail?.Invoke( "Unknown API Type" );
        }

        PrintLog( logStr );
    }

    #endregion
}
