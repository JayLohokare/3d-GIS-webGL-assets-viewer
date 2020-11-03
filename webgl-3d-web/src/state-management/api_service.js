import URLService from "./url_service.js";
class APIService extends URLService {
  async getAssetsListAsync(mapViewPort, status, searchText, date) {

    try {
      let requstBody = {
        Viewport: mapViewPort,
        Status: status,
        Search: searchText,
        Date: date,
        StagedLayers: [0, 1, 2],
        NetworkLayers: [100, 105, 110, 900],
      };

      const response = await Promise.race([
        fetch(`${this.viewer3dURL}`, {
          method: "POST",
          headers: {
            "Access-Control-Allow-Origin": "*",
            Accept: "application/json",
            "Content-Type": "application/json",
          },
          body: JSON.stringify(requstBody),
        }),
        new Promise((_, reject) =>
          setTimeout(() => reject(new Error("timeout")), 30000)
        ),
      ]);

      const responseJson = await response.json();
      return responseJson;
    } catch (error) {
      console.log("Unable to fetch Assets");
      console.log(error);
    }
    return null;
  }

  async getFeatureLayersList(mapViewPort, isStaged) {
    try {
      let requstBody = {
        CreatedBy: "WebGL",
        CreatedBySystem: "3D",
        EventType: "3D_GET_FEATURE_SERVERS_GIS",
        Message: {
          Viewport: mapViewPort,
          IsStaged: isStaged,
        },
      };
      const response = await Promise.race([
        fetch(`${this.triggerURL}`, {
          method: "POST",
          headers: {
            "Access-Control-Allow-Origin": "*",
            Accept: "application/json",
            "Content-Type": "application/json",
          },
          body: JSON.stringify(requstBody),
        }),
        new Promise((_, reject) =>
          setTimeout(() => reject(new Error("timeout")), 20000)
        ),
      ]);

      const responseJson = await response.json();
      return responseJson;
    } catch (error) {
      console.log("Unable to fetch feature layers");
      console.log(error);
    }
    return null;
  }

  async getFeatureLayer(mapViewPort, layerId) {
    console.log(`Get assets in layers from: ${this.triggerURL}`);
    try {
      let requstBody = {
        CreatedBy: "WebGL",
        CreatedBySystem: "3D",
        EventType: "3D_GET_ASSETS_IN_VIEWPORT_GIS",
        Message: {
          Viewport: mapViewPort,
          layerId: layerId,
        },
      };

      const response = await Promise.race([
        fetch(`${this.triggerURL}`, {
          method: "POST",
          headers: {
            "Access-Control-Allow-Origin": "*",
            Accept: "application/json",
            "Content-Type": "application/json",
          },
          body: JSON.stringify(requstBody),
        }),
        new Promise((_, reject) =>
          setTimeout(() => reject(new Error("timeout")), 20000)
        ),
      ]);

      const responseJson = await response.json();
      return responseJson;
    } catch (error) {
      console.log("Unable to fetch assets in layer " + layerId);
      console.log(error);
    }
    return null;
  }

  async getConnectivtyDataAsync(
    viewData = [
      [-88.18, 41.75],
      [-88.18, 41.8],
      [-88.14, 41.8],
      [-88.14, 41.75],
    ]
  ) {
    let requstBody = {
      CreatedBy: "WebGL",
      CreatedBySystem: "3D",
      EventType: "3D_GET_ASSETS_IN_VIEWPORT_GIS",
      Message: {
        Viewport: viewData,
        MaxCount: -1,
        LayerId: 903,
        IsStaged: false,
      },
    };

    // console.log(`get connectivty data from: ${this.baseURLConnectivty}`);
    try {
      const response = await fetch(`${this.baseURLConnectivty}`, {
        method: "POST",
        headers: {
          "Access-Control-Allow-Origin": "*",
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requstBody),
      });

      const responseJson = await response.json();
      return responseJson; //TODO: need to protect this
    } catch (error) {
      console.log(error);
      return error;
    }
  }

  async getStagedConnectivtyDataAsync() {
    let requstBody = {
      CreatedBy: "WebGL",
      CreatedBySystem: "3D",
      EventType: "3D_GET_ASSETS_IN_VIEWPORT_GIS",
      Message: {
        Viewport: [
          [-90.0, 40.0],
          [-90.0, 43.0],
          [-87.0, 43.0],
          [-87.0, 40.0],
        ],
        LayerId: 1,
        IsStaged: true,
      },
    };

    // console.log(`get staging connectivty data from: ${this.baseURLConnectivty}`);
    try {
      const response = await fetch(`${this.baseURLConnectivty}`, {
        method: "POST",
        headers: {
          "Access-Control-Allow-Origin": "*",
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requstBody),
      });

      const responseJson = await response.json();
      return responseJson; //TODO: need to protect this
    } catch (error) {
      console.log(error);
      return error;
    }
  }

  //TErminator 3 (Smaller tower)
  //Power transformer
  //Bus support stand -> Sub 34KV Bus Support Stand
  //40.777371,-76.245457
  async getAssetsListAsyncStatic() {
    const staticResponse = [

      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Substation",
        Equipement:  "Substation",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        Type: "Substation",
        geometry: {
          X: -88.171993,
          Y: 41.7826,
          Z: 0,
          RotationX: 0,
          RotationY: 23,
          RotationZ: 0,
          Scale: 0.025,
          ModelName: "SubControlHouse",
        },
      },

      {
        AssetId: "bee08b85-4e6f-41de-8549-23523",
        Description: "Substation Transformer #1",
        Equipement: "Substation Transformer #1",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        Type: "Transformer",
        geometry: {
          X: -76.245457,
          Y: 40.777371,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 0.0008,
          ModelName: "Power Transformer",
        },
      },
      {
        AssetId: "bee08b85-4e6f-41de-8549-435325",
        Description: "Substation Transformer #2",
        Equipement: "Substation Transformer #2",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        Type: "Transformer",
        geometry: {
          X: -76.245828,
          Y: 40.777475,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 0.0008,
          ModelName: "Power Transformer",
        },
      }, {
        AssetId: "bee08b85-4e6f-41de-8549-345412",
        Description: "Substation Breaker #1",
        Equipement:  "Substation Breaker #1",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        Type: "Breaker",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        geometry: {
          X: -76.245006,
          Y: 40.777678,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 0.4,
          ModelName: "12KVCircuitBreaker",
        },
      },
      {
        AssetId: "bee08b85-4e6f-41de-8549-340-8936",
        Description: "Substation Breaker #2",
        Equipement:  "Substation Breaker #2",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        Type: "Breaker",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        geometry: {
          X: -76.245366,
          Y: 40.777743,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 0.4,
          ModelName: "12KVCircuitBreaker",
        },
      },
      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Substation Breaker #3",
        Equipement:  "Substation Breaker #3",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        Type: "Breaker",
        geometry: {
          X: -76.245767, 
          Y: 40.777828,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 0.4,
          ModelName: "12KVCircuitBreaker",
        },
      },

      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24652343",
        Description: "Substation",
        Equipement:  "Substation",
        Type: "Substation",
        geometry: {
          X: -76.246198, 
          Y: 40.777523,
          Z: 0,
          RotationX: 0,
          RotationY: 70,
          RotationZ: 0,
          Scale: 0.03,
          ModelName: "SubControlHouse",
        },
      },
      
      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Transmission Pole #1",
        Equipement:  "Transmission Pole #1",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432476S800158",
        Type: "Pole",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        geometry: {
          X: -76.249013,
          Y: 40.773600,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 1,
          ModelName: "EL_POLE",
        },
      },

      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Transmission Pole #2",
        Equipement:  "Transmission Pole #2",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432476S2423158",
        Type: "Pole",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        geometry: {
          X: -76.247280,
          Y: 40.774276,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 1,
          ModelName: "EL_POLE",
        },
      },

      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Transmission Pole #3",
        Equipement:  "Transmission Pole #3",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Type: "Pole",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        Tag: "EP4324352800158",
        geometry: {
          X: -76.245391,
          Y: 40.774698,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 1,
          ModelName: "EL_POLE",
        },
      },
      {
        AssetId: "bee08b85-4e6f-41de-8549-340-24653",
        Description: "Transmission Pole #4",
        Equipement:  "Transmission Pole #4",
        LineDescription: "93 - Orwigsburg",
        LineTag: "MN93",
        Tag: "EP432423200158",
        Type: "Pole",
        TR_LINE_REGION_DESC: "Susquehanna",
        Owner_Description: "Utility Owned",
        geometry: {
          X: -76.245477,
          Y: 40.776226,
          Z: 0,
          RotationX: 0,
          RotationY: 0,
          RotationZ: 0,
          Scale: 1,
          ModelName: "EL_POLE",
        },
      },

      //AUD New assets list
      {"Status":"new","Equipement":"EL_FUSE","Type":null,"Description":"","AssetId":"f77facd5-bf0a-4519-ba67-145d2bb7a915","geometry":{"X":-88.16715464793565,"Y":41.78365212448795,"Z":8.0,"Scale":0.029999999329447746,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":14,"AssetType":221,"LayerId":0,"ModelName":"EL_FUSE"}},
      {"Status":"Approved","Equipement":"High Voltage Transformer - AC Three Phase Power","Type":"High Voltage Transformer ","Description":"High Voltage Transformer - AC Three Phase Power","AssetId":"6dbeaafa-a1af-4ab5-b4a1-1dca99f14900","geometry":{"X":-88.1717995563818,"Y":41.782577501471536,"Z":0.0,"Scale":0.0003000000142492354,"RotationX":0.0,"RotationY":90.0,"RotationZ":0.0,"AssetGroup":11,"AssetType":202,"LayerId":100,"ModelName":"Power Transformer"}},
      {"Status":"new","Equipement":"EL_SERVICE_POINT","Type":null,"Description":"","AssetId":"c61c7b80-c8bf-4b0c-9dae-325cd81817c3","geometry":{"X":-88.16697999837643,"Y":41.78282212566463,"Z":0.0,"Scale":0.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":36,"AssetType":702,"LayerId":0,"ModelName":null}},
      {"Status":"new","Equipement":"EL_RISER","Type":null,"Description":"","AssetId":"763f5866-95ba-4efb-8277-54c0e235c51d","geometry":{"X":-88.16715464793565,"Y":41.78365212448795,"Z":6.0,"Scale":0.019999999552965164,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":9,"AssetType":103,"LayerId":0,"ModelName":"EL_RISER"}},
      {"Status":"new","Equipement":"EL_TRANSFORMER","Type":null,"Description":"","AssetId":"150f2ada-a26a-4e87-8712-65411596bdae","geometry":{"X":-88.16676397511016,"Y":41.78303098650768,"Z":0.0,"Scale":0.05000000074505806,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":38,"AssetType":335,"LayerId":0,"ModelName":"Pad Mounted Transformer"}},
      {"Status":"Approved","Equipement":"High Voltage Switch - AC Disconnect","Type":"High Voltage Switch ","Description":"High Voltage Switch - AC Disconnect","AssetId":"55e74e25-5cde-42f7-91e6-6e0100f38790","geometry":{"X":-88.17177188313349,"Y":41.782467670496885,"Z":0.0,"Scale":0.20000000298023224,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":10,"AssetType":182,"LayerId":100,"ModelName":"12KVCircuitBreaker"}},
      {"Status":"new","Equipement":"EL_POLE","Type":null,"Description":"","AssetId":"4a1a6090-2925-4da0-a088-7dbda8527ed7","geometry":{"X":-88.16723637306694,"Y":41.78409623642401,"Z":0.0,"Scale":1.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":121,"AssetType":324,"LayerId":0,"ModelName":"EL_POLE"}},
      {"Status":"new","Equipement":"EL_PAD","Type":null,"Description":"","AssetId":"fde09737-47c2-4e1b-b871-82d56d841d32","geometry":{"X":-88.16676397511016,"Y":41.78303098650768,"Z":0.0,"Scale":0.05000000074505806,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":106,"AssetType":201,"LayerId":0,"ModelName":"Default PAD"}},
      {"Status":"Approved","Equipement":"High Voltage Switch - AC Circuit Breaker","Type":"High Voltage Switch ","Description":"High Voltage Switch - AC Circuit Breaker","AssetId":"03f5f2c4-9d87-4300-b52d-876f6721b2b5","geometry":{"X":-88.1717841096892,"Y":41.782484937744336,"Z":0.0,"Scale":0.20000000298023224,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":10,"AssetType":181,"LayerId":100,"ModelName":"12KVCircuitBreaker"}},
      {"Status":"Approved","Equipement":"High Voltage Transformer - AC Three Phase Power","Type":"High Voltage Transformer ","Description":"High Voltage Transformer - AC Three Phase Power","AssetId":"d41ef658-588a-4eee-b360-9f94f1fda1ed","geometry":{"X":-88.17186589068018,"Y":41.78255410424948,"Z":0.0,"Scale":0.0003000000142492354,"RotationX":0.0,"RotationY":90.0,"RotationZ":0.0,"AssetGroup":11,"AssetType":202,"LayerId":100,"ModelName":"Power Transformer"}},
      {"Status":"new","Equipement":"EL_SERVICE_POINT","Type":null,"Description":"","AssetId":"c2e49170-e2ff-4099-be16-b60f7148a9ab","geometry":{"X":-88.16762976239784,"Y":41.78263698635166,"Z":0.0,"Scale":0.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":36,"AssetType":702,"LayerId":0,"ModelName":null}},
      {"Status":"Approved","Equipement":"High Voltage Switch - AC Circuit Breaker","Type":"High Voltage Switch ","Description":"High Voltage Switch - AC Circuit Breaker","AssetId":"ff1f655d-3a75-4a4c-a8d6-bf4fd64b1c71","geometry":{"X":-88.17179426342898,"Y":41.782569851748555,"Z":0.0,"Scale":0.20000000298023224,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":10,"AssetType":181,"LayerId":100,"ModelName":"12KVCircuitBreaker"}},
      {"Status":"new","Equipement":"EL_POLE","Type":null,"Description":"","AssetId":"86dce38e-def4-4b5f-bf77-c9a20542f8bd","geometry":{"X":-88.16846354361029,"Y":41.78373360953357,"Z":0.0,"Scale":1.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":121,"AssetType":324,"LayerId":0,"ModelName":"EL_POLE"}},
      {"Status":"new","Equipement":"EL_POLE","Type":null,"Description":"","AssetId":"e54bddce-b53c-4fb5-95f9-da35befb46bd","geometry":{"X":-88.16783115929465,"Y":41.78391394560283,"Z":0.0,"Scale":1.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":121,"AssetType":324,"LayerId":0,"ModelName":"EL_POLE"}},
      {"Status":"new","Equipement":"EL_POLE","Type":null,"Description":"","AssetId":"45c25c08-a4bb-4794-8c72-e61a08e2bfa5","geometry":{"X":-88.16715464793565,"Y":41.78365212448795,"Z":0.0,"Scale":1.0,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":121,"AssetType":324,"LayerId":0,"ModelName":"EL_POLE"}},
      {"Status":"Approved","Equipement":"High Voltage Switch - AC Circuit Breaker","Type":"High Voltage Switch ","Description":"High Voltage Switch - AC Circuit Breaker","AssetId":"6e2833bd-6196-4743-9a70-efeb3036515c","geometry":{"X":-88.17186305338019,"Y":41.782545786310095,"Z":0.0,"Scale":0.20000000298023224,"RotationX":0.0,"RotationY":0.0,"RotationZ":0.0,"AssetGroup":10,"AssetType":181,"LayerId":100,"ModelName":"12KVCircuitBreaker"}},

    ];
    return staticResponse;
  }
}

export let apiService = new APIService();
