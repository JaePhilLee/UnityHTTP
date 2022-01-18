# UnityHTTP

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
