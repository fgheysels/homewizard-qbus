EqoWeb:

Navigate to EQOWeb controller-ip:port  (192.168.1.14:8444)
Login using the username that can be found in QBus System Manager.


Via API:

## Login

```
HTTP POST 192.168.1.14:8444/default.aspx

x-www-form-urlencoded body:  key=strJSON value={"Type":1,"Value":{"Usr":"<username>","Psw":"<password>"}}
```

The response of the login call contains a JSON string which looks like this:

```json
{"Type":2,"Value":{"rsp":true,"id":"F554AB45"}}
```

The id (`F554AB45`) must be used in subsequent requests to EqoWeb.  This ID must be used in the `Cookie` header in subsequent requests:

```
Cookie: i=F554AB45
```