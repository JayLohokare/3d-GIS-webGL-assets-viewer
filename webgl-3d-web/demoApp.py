from flask import Flask, jsonify, request
from flask_cors import CORS, cross_origin

app = Flask(__name__)
cors = CORS(app, resources={r"/models": {"origins": "*"}})
cors = CORS(app, resources={r"/asset": {"origins": "*"}})
app.config['CORS_HEADERS'] = 'Content-Type'


@app.route('/', methods=['GET'])
def health():
    return "Alive"
    
    
@app.route('/models', methods=['GET'])
def getModels():
    model = [
        {
            "id": "bee08b85-4e6f-41de-8549-5c99bba4ec94",
            "name": "Tower #1",
            "latitude": 40.7722,
            "longitude": -76.246,
            "altitude": 0.0,
            "zoom": 18.0,
            "size": 0.01,
            "x_Rotation": 0.0,
            "y_Rotation": -90.0,
            "z_Rotation": 0.0,
            "urL_Model": "http://localhost:3000/models/tower.obj",
            "urL_Material": "http://localhost:3000/models/Substation.mtl",
            "assetId": "82a7a3df-9070-469e-baab-08d83aebdffd",
            "is_connected": True,
            "from_id": "4ca97bd8-ca6a-4b23-95ef-a4ce347dc4e6",
            "to_id": "ddb28413-9348-483a-9a47-657117068fbe"
        },
        {
            "id": "ddb28413-9348-483a-9a47-657117068fbe",
            "name": "Tower #3",
            "latitude": 40.7746,
            "longitude": -76.245412,
            "altitude": 0.0,
            "zoom": 18.0,
            "size": 0.01,
            "x_Rotation": 0.0,
            "y_Rotation": 0.0,
            "z_Rotation": 0.0,
            "urL_Model": "http://localhost:3000/models/tower.obj",
            "urL_Material": "http://localhost:3000/models/Substation.mtl",
            "assetId": "82a7a3df-9070-469e-baab-08d83aebdffd",
            "is_connected": True,
            "from_id": "bee08b85-4e6f-41de-8549-5c99bba4ec94",
            "to_id": None
        },
        {
            "id": "1f2216cb-12b0-4700-a65c-8e20d1ea1379",
            "name": "Liberty Island Electric pole #22",
            "latitude": 40.68920135498049,
            "longitude": -74.04450225830072,
            "altitude": 0.0,
            "zoom": 16.0,
            "size": 1.0,
            "x_Rotation": 0.0,
            "y_Rotation": -90.0,
            "z_Rotation": 0.0,
            "urL_Model": "http://localhost:3000/models/tower.obj",
            "urL_Material": "http://localhost:3000/models/Substation.mtl",
            "assetId": "82a7a3df-9070-469e-baab-08d83aebdffd",
            "is_connected": False,
            "from_id": None,
            "to_id": None
        },
        {
            "id": "4ca97bd8-ca6a-4b23-95ef-a4ce347dc4e6",
            "name": "Tower #2",
            "latitude": 40.77196,
            "longitude": -76.249,
            "altitude": 0.0,
            "zoom": 18.0,
            "size": 0.01,
            "x_Rotation": 0.0,
            "y_Rotation": -90.0,
            "z_Rotation": 0.0,
            "urL_Model": "http://localhost:3000/models/tower.obj",
            "urL_Material": "http://localhost:3000/models/Substation.mtl",
            "assetId": "82a7a3df-9070-469e-baab-08d83aebdffd",
            "is_connected": True, 
            "from_id": None,
            "to_id": "bee08b85-4e6f-41de-8549-5c99bba4ec94"
        },
        {
            "id": "4fea1f20-1e45-4874-9241-e82d34c09a24",
            "name": "Pole #1",
            "latitude": 40.777417,
            "longitude": -76.245412,
            "altitude": -40.0,
            "zoom": 15.0,
            "size": 1.2,
            "x_Rotation": 0.0,
            "y_Rotation": 0.0,
            "z_Rotation": 90.0,
            "urL_Model": "http://localhost:3000/models/pole.obj",
            "urL_Material": "http://localhost:3000/models/Substation.mtl",
            "assetId": "82a7a3df-9070-469e-baab-08d83aebdffd",
            "is_connected": False,
            "from_id": None,
            "to_id": None
        }
    ]
    response = jsonify(model)
    response.headers.add('Access-Control-Allow-Origin', '*')
    return response

@app.route('/asset', methods=['POST'])
def getModelDetails():
    data = request.json
    print(data)
    assetId = data['Message']['AssetId']
    
    responseMap = {
        "82a7a3df-9070-469e-baab-08d83aebdffd": {
            "message1" : "value",
            "message2" : "value",
            "message3" : "value",
            "message4" : "value",
            "message5" : "value",
            "message6" : "value",
            "message7" : "value",
            "message8" : "value",
            "message9" : "value",
            "message10" : "value",
            "message11" : "value",
            "message12" : "value"
        }
    }
    response = jsonify(responseMap[assetId])
    print(responseMap[assetId])
    print(assetId)
    print(response)
    response.headers.add('Access-Control-Allow-Origin', '*')
    return response 
    
    
if __name__ == '__main__':
    app.run(host='localhost', debug=True)
    
    


