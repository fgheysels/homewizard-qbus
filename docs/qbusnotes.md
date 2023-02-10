# Introduction

## Prepare QBus configuration

- Login in your QBus installation via the QBus System Manager software.

- Create an output of type 'Verwarming/Koeling' and give it a name.  
  We'll use this output to light up a LED in an SWC switch in a certain color.  This LED will then indicate if our solar-panels are over-producing electricity or if we're importing more electricity from the grid then what we produce.

  To do this, assign this output to a button of a SWC switch.  (Drawback is that this button cannot be used for any other functionality).

- Create a 'Control List' (bedieningstabel).

  The first control-list must be of type `EQOWeb`.  
  `EQOWeb` is an API that allows us to interact with QBus devices.  The devices that are present in the `EQOWeb` control list, can be manipulated via the EQOWeb API.


## EQOWeb

An EQOWeb dashboard is available on port 8444 of the IP address of the QBus controller.
Navigate for instance to `192.168.1.14:8444` in a browser.

EQOWeb is also accessible via an API

### Login

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